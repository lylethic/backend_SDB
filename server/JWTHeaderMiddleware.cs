using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace server
{
  public class JWTHeaderMiddleware
  {
    private readonly RequestDelegate _next;
    private readonly IConfiguration _config;

    public JWTHeaderMiddleware(RequestDelegate next, IConfiguration config)
    {
      _next = next;
      _config = config;
    }

    public async Task Invoke(HttpContext context)
    {
      var cookieName = "jwtCookie";
      var jwtToken = context.Request.Cookies[cookieName];

      if (!string.IsNullOrEmpty(jwtToken))
      {
        // Validate the JWT before appending it to the header
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_config["JwtSettings:SecretKey"]!);

        try
        {
          // Token validation parameters
          var validationParameters = new TokenValidationParameters
          {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = _config["JwtSettings:Issuer"],
            ValidAudience = _config["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ClockSkew = TimeSpan.Zero // Optional: To prevent token expiration buffer
          };

          // Validate the token
          var principal = tokenHandler.ValidateToken(jwtToken, validationParameters, out var validatedToken);

          // Check if the validated token is a valid JWT
          if (validatedToken is JwtSecurityToken jwtSecurityToken &&
              jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
          {
            // If token is valid, append the Authorization header
            if (!context.Request.Headers.ContainsKey("Authorization"))
            {
              context.Request.Headers.Append("Authorization", "Bearer " + jwtToken);
            }
          }
        }
        catch (SecurityTokenException ex)
        {
          // Token is invalid, so do nothing or handle the invalid token scenario (e.g., log the error)
          throw new Exception(ex.Message);
        }
      }

      await _next(context);
    }
  }

}
