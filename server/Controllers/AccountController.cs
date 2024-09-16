using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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
  [Authorize]
  public class AccountController : ControllerBase
  {
    private readonly IAccount _acc;

    public AccountController(IAccount acc)
    {
      _acc = acc;
    }

    // GET: api/Auth
    [HttpGet, Route("accounts")]
    public async Task<IActionResult> GetAccounts()
    {
      try
      {
        var accounts = await _acc.GetAccounts();
        if (accounts == null)
        {
          return NotFound(); // 404
        }
        return Ok(accounts); // 200
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
        return StatusCode(500, "Server error"); // 500
      }
    }

    // GET: api/Auth/5
    [HttpGet, Route("account/{id}")]
    public async Task<IActionResult> GetAccount(int id)
    {
      var account = await _acc.GetAccount(id);

      if (account.StatusCode != 200)
      {
        return BadRequest(account);
      }
      return Ok(account);
    }

    // PUT: api/Auth/5
    [HttpPut, Route("edit/{id}")]
    public async Task<IActionResult> PutAccount(int id, AccountDto account)
    {
      var result = await _acc.UpdateAccount(id, account);
      if (result.StatusCode != 200)
      {
        return BadRequest(result);
      }
      return Ok(result);
    }

    // POST: api/Auth
    [HttpPost, Route("account")]
    public async Task<IActionResult> CreateAccount(RegisterDto account)
    {
      var result = await _acc.AddAccount(account);
      if (result.StatusCode != 200)
      {
        return BadRequest(result);
      }
      return Ok(result);
    }

    // DELETE: api/Auth/5
    [HttpDelete, Route("delete/{id}")]
    public async Task<IActionResult> DeleteAccount(int id)
    {
      var result = await _acc.DeleteAccount(id);
      if (result.StatusCode != 200)
      {
        return BadRequest(result);
      }

      return Ok(result);
    }
  }
}
