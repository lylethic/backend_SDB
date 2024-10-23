using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using server.Dtos;
using server.IService;
using server.Types;

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
        return StatusCode(result.StatusCode, new
        {
          message = result.Message,
          errors = result.Errors,
          statusCode = result.StatusCode,
        });
      }

      return Ok(new
      {
        isSuccess = result.IsSuccess,
        statusCode = result.StatusCode,
        message = result.Message,
        data = result.Data
      });
    }

    // POST: api/Register
    [HttpPost, Route("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register(RegisterDto model)
    {
      var result = await _authRepo.Register(model);
      if (!result.IsSuccess)
      {
        return StatusCode(422, result);
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
        return StatusCode(422, result);
      }

      return Ok(result);
    }

    [HttpPost, Route("logout"), Authorize]
    public async Task<IActionResult> Logout()
    {
      var result = await _authRepo.Logout();
      if (!result.IsSuccess)
      {
        return StatusCode(422, new
        {
          message = result.Message,
        });
      }

      return Ok(new
      {
        message = result.Message,
      });
    }
  }
}
