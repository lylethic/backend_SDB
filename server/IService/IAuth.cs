using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using server.Dtos;
using System.IdentityModel.Tokens.Jwt;

namespace server.IService
{
  public interface IAuth
  {
    public Task<ResponseDto> Login(AuthDto model);
    public Task<ResponseDto> Register(RegisterDto model);
    void GenerateHash(string password, out byte[] passwordHash, out byte[] passwordSalt);
    Boolean ValidateHash(string password, byte[] passwordhash, byte[] passwordsalt);
      //public Task<ResponseDto> RefreshToken(RefreshRequest request);
  }
}
