using server.Dtos;
using System.Security.Claims;

namespace server.IService
{
  public interface ITokenService
  {
    string GenerateAccessToken(IEnumerable<Claim> claims);

    string GenerateRefreshToken();

    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);

    Task<ResponseDto> RefreshToken(TokenApiDto model);

    void SetJWTCookie(string token);
  }
}
