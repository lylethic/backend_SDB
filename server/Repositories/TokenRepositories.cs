
using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using server.Data;
using server.Dtos;
using server.IService;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Security.Cryptography;

namespace server.Repositories
{
  public class TokenRepositories : ITokenService
  {
    private readonly SoDauBaiContext _context;
    private readonly IConfiguration _config;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TokenRepositories(SoDauBaiContext context, IConfiguration config, IHttpContextAccessor httpContextAccessor)
    {
      this._context = context;
      this._config = config;
      this._httpContextAccessor = httpContextAccessor;
    }

    public string GenerateAccessToken(IEnumerable<Claim> claims)
    {
      var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSettings:SecretKey"]!));

      var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

      // Create a JWT
      var tokeOptions = new JwtSecurityToken(
        issuer: _config["JwtSettings:Issuer"],
        audience: _config["JwtSettings:Audience"],
        claims: claims,
        signingCredentials: signinCredentials,
        expires: DateTime.UtcNow.AddHours(3)
        );

      var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);

      return tokenString;
    }

    public string GenerateRefreshToken()
    {
      var randomNumber = new byte[32];
      using (var rng = RandomNumberGenerator.Create())
      {
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
      }
    }

    public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
      var tokenValidationParameters = new TokenValidationParameters
      {
        ValidateAudience = false,
        ValidateIssuer = false,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSettings:SecreKey"]!)),
        ValidateLifetime = false
      };

      var tokenHandler = new JwtSecurityTokenHandler();

      SecurityToken securityToken;

      var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
      var jwtSecurityToken = securityToken as JwtSecurityToken;

      if (jwtSecurityToken != null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
      {
        throw new SecurityTokenException("Invaid token");
      }

      return principal;
    }

    public async Task<ResponseDto> RefreshToken([FromBody] TokenApiDto model)
    {

      if (string.IsNullOrEmpty(model.RefreshToken))
      {
        return new ResponseDto
        {
          IsSuccess = false,
          Message = "Refresh token is required",
        };
      }


      var storedToken = _context.TokenStoreds.FirstOrDefault(t => t.TokenString == model.AccessToken);

      try
      {
        var principal = GetPrincipalFromExpiredToken(model.RefreshToken);

        if (principal is null)
        {
          return new ResponseDto
          {
            IsSuccess = false,
            Message = "Invalid refresh token",
          };
        }

        var userEmail = principal.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userEmail))
        {
          return new ResponseDto
          {
            IsSuccess = false,
            Message = "User not found",
          };
        }

        var tokenStored = _context.TokenStoreds.FirstOrDefault(x => x.TokenString == model.RefreshToken);

        if (tokenStored is null)
        {
          _context.TokenStoreds.Remove(tokenStored);
          await _context.SaveChangesAsync();
        }

        var claims = new List<Claim>()
        {
          new Claim(ClaimTypes.NameIdentifier, userEmail),
          new Claim(ClaimTypes.Role, "Admin")
        };

        var newAccessToken = GenerateAccessToken(claims);

        return new ResponseDto { IsSuccess = true, Message = "Success", AccessToken = newAccessToken };
      }
      catch (Exception ex)
      {
        return new ResponseDto
        {
          IsSuccess = false,
          Message = ex.Message,
        };
      }
    }


    public void SetJWTCookie(string token)
    {
      var cookieOptions = new CookieOptions
      {
        HttpOnly = true,
        Expires = DateTime.UtcNow.AddHours(3),
      };
      _httpContextAccessor.HttpContext.Response.Cookies.Append("jwtCookie", token, cookieOptions);
    }
  }
}
