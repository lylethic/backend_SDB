using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using server.Dtos;
using server.IService;

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
    [HttpGet]
    public async Task<IActionResult> GetAccounts(int pageNumber = 1, int pageSize = 50)
    {
      try
      {
        var accounts = await _acc.GetAccounts(pageNumber, pageSize);
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

    [HttpGet, Route("GetAccountsByRole")]
    public async Task<IActionResult> GetAccountsByRole(int pageNumber, int pageSize, int roleId)
    {
      try
      {
        var accounts = await _acc.GetAccountsByRole(pageNumber, pageSize, roleId);
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
    [HttpGet, Route("{id}")]
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
    [HttpPut, Route("{id}")]
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
    [HttpPost]
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
    [HttpDelete, Route("{id}")]
    public async Task<IActionResult> DeleteAccount(int id)
    {
      var result = await _acc.DeleteAccount(id);
      if (result.StatusCode != 200)
      {
        return BadRequest(result);
      }

      return Ok(result);
    }

    [HttpDelete, Route("bulkdelete")]
    public async Task<IActionResult> BulkDelete(List<int> ids)
    {
      var result = await _acc.BulkDelete(ids);
      if (result.StatusCode != 200)
      {
        return BadRequest(result);
      }

      return Ok(result);
    }

    [HttpPost, Route("upload")]
    public async Task<IActionResult> ImportExcel(IFormFile file)
    {
      try
      {
        var result = await _acc.ImportExcel(file);
        if (!result.Contains("Successfully"))
        {
          return BadRequest(result);
        }

        return Ok(result);
      }
      catch (Exception ex)
      {
        return StatusCode(500, $"Server Error: {ex.Message}");
      }

    }
  }
}
