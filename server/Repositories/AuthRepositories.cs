using AutoMapper;
using Azure.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NuGet.Protocol;
using server.Data;
using server.Dtos;
using server.IService;
using server.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Security.Cryptography;

namespace server.Repositories
{
  public class AuthRepositories : IAuth
  {
    private readonly SoDauBaiContext _context;
    private readonly IConfiguration _config;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ITokenService _tokenService;

    public AuthRepositories(SoDauBaiContext context, IConfiguration config, IHttpContextAccessor httpContextAccessor, ITokenService tokenService)
    {
      _context = context;
      _config = config;
      _httpContextAccessor = httpContextAccessor;
      _tokenService = tokenService;
    }

    public async Task<ResponseDto> Login(AuthDto model)
    {
      if (model is null)
      {
        return new ResponseDto
        {
          IsSuccess = false,
          Message = "Invalid client request",
        };
      }

      var user = _context.Accounts.FirstOrDefault(u =>
        (u.Email == model.Email) && (u.Password == model.Password));


      if (user is null)
      {
        return new ResponseDto
        {
          IsSuccess = false,
          Message = "Unauthorized",
        };
      }

      var claims = new List<Claim>()
      {
        new Claim(ClaimTypes.Email, model.Email),
      };

      var accessToken = _tokenService.GenerateAccessToken(claims);
      var refreshToken = _tokenService.GenerateRefreshToken();
      _tokenService.SetJWTCookie(accessToken);

      // Save token into table TokenStored
      var result = new TokenStored
      {
        AccountId = user.AccountId,
        TokenString = accessToken,
      };

      _context.TokenStoreds.AddAsync(result);
      await _context.SaveChangesAsync();

      return new ResponseDto
      {
        IsSuccess = true,
        Message = "Login successful",
        AccessToken = accessToken,
        RefreshToken = refreshToken
      };
    }

    public async Task<ResponseDto> Register(RegisterDto model)
    {
      if (model == null)
      {
        return new ResponseDto
        {
          IsSuccess = false,
          Message = "Invalid registration request",
        };
      }

      // Check if user already exists
      var existingUser = await _context.Accounts.FirstOrDefaultAsync(u => u.Email == model.Email);
      if (existingUser != null)
      {
        return new ResponseDto
        {
          IsSuccess = false,
          Message = "Email already registered",
        };
      }

      // Hash the password
      var passwordHasher = new PasswordHasher<Account>();
      var user = new Account
      {
        Email = model.Email,
        Password = passwordHasher.HashPassword(null, model.Password),
      };

      // Save user to the database
      _context.Accounts.Add(user);
      await _context.SaveChangesAsync();

      // Optionally, log the user in after registering (generate tokens)
      var claims = new List<Claim>
      {
        new Claim(ClaimTypes.Email, user.Email),
      };

      var accessToken = _tokenService.GenerateAccessToken(claims);
      var refreshToken = _tokenService.GenerateRefreshToken();
      _tokenService.SetJWTCookie(accessToken);

      // Save token into table TokenStored
      var tokenResult = new TokenStored
      {
        AccountId = user.AccountId,
        TokenString = accessToken,
      };

      await _context.TokenStoreds.AddAsync(tokenResult);
      await _context.SaveChangesAsync();

      return new ResponseDto
      {
        IsSuccess = true,
        Message = "Registration successful",
        AccessToken = accessToken,
        RefreshToken = refreshToken
      };
    }

    public void GenerateHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
      using (var hash = new HMACSHA512())
      {
        passwordHash = hash.ComputeHash(Encoding.UTF8.GetBytes(password));
        passwordSalt = hash.Key;
      }
    }

    public bool ValidateHash(string password, byte[] passwordhash, byte[] passwordsalt)
    {
      using (var hash = new HMACSHA512(passwordhash))
      {
        var newPassHash = hash.ComputeHash(Encoding.UTF8.GetBytes(password));
        for (int i = 0; i < newPassHash.Length; i++)
        {
          if (newPassHash[i] != passwordhash[i])
            return false;
        }
        return true;
      }
    }
  }
}
