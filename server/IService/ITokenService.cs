using server.Dtos;
using System.Security.Claims;

namespace server.IService
{
  public interface ITokenService
  {
    string GenerateAccessToken(IEnumerable<Claim> claims);

    string GenerateRefreshToken();

    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);

    Task<ResponseDto> RefreshToken();

    void SetJWTCookie(string token);

    void SetRefreshTokenCookie(string refreshToken);

    void ClearJWTCookie();

    void ClearRefreshTokenCookie();
  }
}
