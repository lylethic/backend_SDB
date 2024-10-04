using Microsoft.EntityFrameworkCore;
using server.Dtos;
using server.IService;
using server.Models;
using server.Types;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace server.Repositories
{
  public class AuthRepositories : IAuth
  {
    private readonly Data.SoDauBaiContext _context;
    private readonly IConfiguration _config;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ITokenService _tokenService;

    public AuthRepositories(Data.SoDauBaiContext context, IConfiguration config, IHttpContextAccessor httpContextAccessor, ITokenService tokenService)
    {
      _context = context;
      _config = config;
      _httpContextAccessor = httpContextAccessor;
      _tokenService = tokenService;
    }

    public async Task<LoginResType> Login(AuthDto model)
    {
      if (model is null)
      {
        return new LoginResType(false, "Invalid client request");
      }

      var user = await _context.Accounts.FirstOrDefaultAsync(u => u.Email == model.Email);

      if (user is null)
      {
        return new LoginResType(false, "Invalid email");
      }

      // Validate password
      bool isPasswordValid = ValidateHash(model.Password!, user.MatKhau, user.PasswordSalt);
      if (!isPasswordValid)
      {
        return new LoginResType(false, "Invalid password");
      }

      // Generate JWT tokens
      // Information in JWT
      var claims = new List<Claim>()
      {
        new Claim(ClaimTypes.Email, model.Email),
      };

      var accessToken = _tokenService.GenerateAccessToken(claims);
      var refreshToken = _tokenService.GenerateRefreshToken();

      _tokenService.SetJWTCookie(accessToken);
      _tokenService.SetRefreshTokenCookie(refreshToken);

      var session = new Session
      {
        AccountId = user.AccountId,
        Token = refreshToken,
        ExpiresAt = DateTime.Now.AddDays(3),
        CreatedAt = DateTime.Now,
      };

      await _context.Sessions.AddAsync(session);
      await _context.SaveChangesAsync();

      return new LoginResType
      {
        IsSuccess = true,
        Message = "Login successful",
        AccessToken = accessToken,
        RefreshToken = refreshToken,
        Email = user.Email,
        AccountId = user.AccountId,
        RoleId = user.RoleId,
        SchoolId = user.SchoolId,
      };
    }

    public async Task<ResponseDto> Logout()
    {
      try
      {
        // Get email in Claims
        var userStored = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Email)?.Value;

        if (string.IsNullOrEmpty(userStored))
        {
          return new ResponseDto(true, "User not found in the current session");
        }

        var user = await _context.Accounts.FirstOrDefaultAsync(u => u.Email == userStored);

        if (user == null)
        {
          return new ResponseDto(true, "User not found");
        }

        // Remove refresh token in DB
        var session = await _context.Sessions
            .FirstOrDefaultAsync(s => s.AccountId == user.AccountId);

        if (session != null)
        {
          _context.Sessions.Remove(session);
          await _context.SaveChangesAsync();
        }

        // Clear cookies
        _tokenService.ClearJWTCookie();
        _tokenService.ClearRefreshTokenCookie();

        return new ResponseDto(true, "Logout successful");
      }
      catch (Exception ex)
      {
        // Log the exception if necessary
        return new ResponseDto(false, "An error occurred while logging out: " + ex.Message);
      }
    }

    public async Task<ResponseDto> Register(RegisterDto model)
    {
      // request empty
      if (model == null)
      {
        return new ResponseDto
        {
          IsSuccess = false,
          Message = "Invalid registration request",
        };
      }

      // Check if user already exists
      var existingUser = await _context.Accounts
        .FirstOrDefaultAsync(u => u.Email == model.Email);

      if (existingUser != null)
      {
        return new ResponseDto
        {
          IsSuccess = false,
          Message = "Email already registered",
        };
      }

      // Hash the password
      byte[] passwordHash, passwordSalt;
      GenerateHash(model.Password, out passwordHash, out passwordSalt);

      var user = new Account
      {
        RoleId = model.RoleId,
        SchoolId = model.SchoolId,
        Email = model.Email,
        MatKhau = passwordHash,
        PasswordSalt = passwordSalt
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
      var tokenResult = new Session
      {
        AccountId = user.AccountId,
        Token = refreshToken,
        ExpiresAt = DateTime.Now.AddDays(2),
        CreatedAt = DateTime.Now,
      };

      await _context.Sessions.AddAsync(tokenResult);
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
      using (var hash = new HMACSHA512(passwordsalt))
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
