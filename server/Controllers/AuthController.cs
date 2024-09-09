﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using server.Data;
using server.Dtos;
using server.IService;
using server.Models;

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
    [HttpPost, Route("Login")]
    public async Task<IActionResult> Login(AuthDto model)
    {
      var result = await _authRepo.Login(model);
      if (!result.IsSuccess)
      {
        return Unauthorized();
      }

      return Ok(new
      {
        success = result.IsSuccess,
        message = result.Message,
        accessToken = result.AccessToken,
        refreshToken = result.RefreshToken
      });
    }

    // POST: api/Register
    [HttpPost, Route("Register")]
    public async Task<IActionResult> Register(RegisterDto model)
    {
      var result = await _authRepo.Register(model);
      if (!result.IsSuccess)
      {
        return Unauthorized();
      }

      return Ok(new
      {
        success = result.IsSuccess,
        message = result.Message,
        accessToken = result.AccessToken,
        refreshToken = result.RefreshToken
      });
    }

    //[HttpPost, Route("RefreshToken")]
    //public async Task<ActionResult<ResponseDto>> RefreshToken(TokenApiDto tokenApiModel)
    //{
    //  var result = await _tokenService.RefreshToken(tokenApiModel);
    //  if (!result.IsSuccess)
    //  {
    //    return Forbid();
    //  }
    //  return Ok(result);
    //}
  }
}
