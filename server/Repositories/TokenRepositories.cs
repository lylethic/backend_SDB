using Microsoft.IdentityModel.Tokens;
using server.Dtos;
using server.IService;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace server.Repositories
{
  public class TokenRepositories : ITokenService
  {
    private readonly Data.SoDauBaiContext _context;
    private readonly IConfiguration _config;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TokenRepositories(Data.SoDauBaiContext context, IConfiguration config, IHttpContextAccessor httpContextAccessor)
    {
      this._context = context ?? throw new ArgumentException(nameof(context));
      this._config = config ?? throw new ArgumentException(nameof(config));
      this._httpContextAccessor = httpContextAccessor ?? throw new ArgumentException(nameof(httpContextAccessor));
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
        expires: DateTime.UtcNow.AddDays(3)
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
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSettings:SecretKey"]!)),
        ValidateLifetime = false
      };

      var tokenHandler = new JwtSecurityTokenHandler();

      SecurityToken securityToken;

      var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
      var jwtSecurityToken = securityToken as JwtSecurityToken;

      if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
      {
        throw new SecurityTokenException("Invalid token");
      }

      return principal;
    }

    public async Task<ResponseDto> RefreshToken()
    {
      // Retrieve the access token from the cookie
      var accessToken = _httpContextAccessor.HttpContext?.Request.Cookies["jwtCookie"];
      if (string.IsNullOrEmpty(accessToken))
      {
        return new ResponseDto(false, "Invalid client request. Access token not found in cookie.");
      }

      try
      {
        // Validate AccessToken => Sap het han chua
        var principal = GetPrincipalFromExpiredToken(accessToken);
        var accountId = principal.FindFirst("AccountId")?.Value;

        var tokenStored = _context.Sessions
                         .Where(id => id.AccountId == Convert.ToInt16(accountId))
                         .OrderByDescending(s => s.ExpiresAt)
                         .FirstOrDefault();


        if (tokenStored is null)
        {
          return new ResponseDto(false, "Session not found");

        }
        // Check if the stored refresh token is still valid (not expired)
        if (tokenStored.ExpiresAt <= DateTime.UtcNow)
        {
          return new ResponseDto(false, "Refresh token has expired");
        }

        // Generate new access and refresh tokens
        var newAccessToken = GenerateAccessToken(principal.Claims);
        var newRefreshToken = GenerateRefreshToken();

        // Update refresh token in the database
        tokenStored.Token = newRefreshToken;
        tokenStored.ExpiresAt = DateTime.UtcNow.AddDays(1);
        await _context.SaveChangesAsync();

        // Luu vao cookies (Server-side-ren)
        SetJWTCookie(newAccessToken);
        SetRefreshTokenCookie(newRefreshToken);

        return new ResponseDto(true, "Token refreshed successfully", newAccessToken);
      }
      catch (Exception ex)
      {
        return new ResponseDto(false, ex.Message);
      }
    }

    public void SetJWTCookie(string token)
    {
      var cookieOptions = new CookieOptions
      {
        HttpOnly = true,
        Secure = true,
        SameSite = SameSiteMode.Strict, // Prevent CSRF attacks
        Expires = DateTime.UtcNow.AddDays(3),
      };
      try
      {
        _httpContextAccessor.HttpContext?.Response.Cookies.Append("jwtCookie", token, cookieOptions);
      }
      catch (Exception ex)
      {
        throw new Exception($"Failed to set JWT cookie: ${ex.Message}");
      }
    }

    public void SetRefreshTokenCookie(string refreshToken)
    {
      var cookieOptions = new CookieOptions
      {
        HttpOnly = true,
        Secure = true,
        SameSite = SameSiteMode.Strict,
        Expires = DateTime.UtcNow.AddDays(5),
      };
      try
      {
        _httpContextAccessor.HttpContext?.Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
      }
      catch (Exception ex)
      {
        throw new Exception($"Failed to set refresh token cookie: {ex.Message}");
      }
    }

    public void ClearJWTCookie()
    {
      var cookieOptions = new CookieOptions
      {
        HttpOnly = true,
        Secure = true,
        SameSite = SameSiteMode.Strict,
        Expires = DateTime.UtcNow.AddDays(-1) // Set expiration date to the past
      };
      try
      {
        _httpContextAccessor.HttpContext?.Response.Cookies.Append("jwtCookie", "", cookieOptions);
      }
      catch (Exception ex)
      {
        throw new Exception($"Failed to clear JWT cookie: {ex.Message}");
      }
    }

    public void ClearRefreshTokenCookie()
    {
      var cookieOptions = new CookieOptions
      {
        HttpOnly = true,
        Secure = true,
        SameSite = SameSiteMode.Strict,
        Expires = DateTime.UtcNow.AddDays(-1) // Set expiration date to the past
      };
      try
      {
        _httpContextAccessor.HttpContext?.Response.Cookies.Append("refreshToken", "", cookieOptions);
      }
      catch (Exception ex)
      {
        throw new Exception($"Failed to clear refresh token cookie: {ex.Message}");
      }
    }
  }
}
