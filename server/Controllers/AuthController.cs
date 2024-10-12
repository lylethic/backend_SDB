using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using server.Dtos;
using server.IService;

namespace server.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class AuthController : ControllerBase
  {
    private readonly IAuth _authRepo;
    private readonly ITokenService _tokenService;

    public AuthController(IAuth authRepo, ITokenService tokenRepo)
    {
      _authRepo = authRepo;
      _tokenService = tokenRepo;
    }

    // POST: api/Auth
    [HttpPost, Route("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(AuthDto model)
    {
      var result = await _authRepo.Login(model);
      if (!result.IsSuccess)
      {
        return Unauthorized(result.Message);
      }

      return Ok(result);
    }

    // POST: api/Register
    [HttpPost, Route("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register(RegisterDto model)
    {
      var result = await _authRepo.Register(model);
      if (!result.IsSuccess)
      {
        return Unauthorized(result.Message);
      }

      return Ok(new
      {
        success = result.IsSuccess,
        message = result.Message,
        accessToken = result.AccessToken,
      });
    }

    [HttpPost, Route("refresh")]
    [AllowAnonymous]
    public async Task<ActionResult<ResponseDto>> RefreshToken()
    {
      var result = await _tokenService.RefreshToken();
      if (!result.IsSuccess)
      {
        return BadRequest(new { Message = "Có lỗi: " + result.Message });
      }

      return Ok(result);
    }

    [HttpPost, Route("logout"), Authorize]
    public async Task<ActionResult<ResponseDto>> Logout()
    {
      var result = await _authRepo.Logout();
      if (!result.IsSuccess)
      {
        return BadRequest(new { Message = "Có lỗi: " + result.Message });
      }

      return Ok(result);
    }
  }
}
