using server.Dtos;
using server.Types;

namespace server.IService
{
  public interface IAuth
  {
    Task<LoginResType> Login(AuthDto model);
    Task<LogoutResType> Logout();
    Task<ResponseDto> Register(RegisterDto model);
    void GenerateHash(string password, out byte[] passwordHash, out byte[] passwordSalt);
    Boolean ValidateHash(string password, byte[] passwordhash, byte[] passwordsalt);
  }
}
