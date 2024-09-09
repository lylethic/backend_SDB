using Microsoft.AspNetCore.Mvc;
using server.Dtos;
using System.Collections;
using System.Security.Claims;

namespace server.IService
{
  public interface ITokenService
  {
    string GenerateAccessToken(IEnumerable<Claim> claims);
    string GenerateRefreshToken();
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    Task<ResponseDto> RefreshToken([FromBody] TokenApiDto model);

    void SetJWTCookie(string token);
  }
}
