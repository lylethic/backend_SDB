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

    [HttpGet("count-number-of-accounts")]
    public async Task<IActionResult> GetCountAccounts()
    {
      var result = await _acc.GetCountAccounts();
      return Ok(result);
    }

    [HttpGet("count-number-of-accounts-by-school")]
    public async Task<IActionResult> GetCountAccountsBySchool(int id)
    {
      var result = await _acc.GetCountAccountsBySchool(id);
      return Ok(result);
    }

    // GET: api/Auth
    [HttpGet]
    public async Task<IActionResult> GetAccounts(int pageNumber = 1, int pageSize = 50)
    {
      try
      {
        var accounts = await _acc.GetAccounts(pageNumber, pageSize);
        if (accounts.IsSuccess == false)
        {
          return NotFound(new
          {
            isSuccess = false,
            statusCode = 404,
            message = "No results found"
          });
        }
        return Ok(new
        {
          statusCode = accounts.StatusCode,
          message = accounts.Message,
          data = accounts.Data,
        }); // 200
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
        return StatusCode(500, new
        {
          isSuccess = false,
          statusCode = 500,
          message = $"An error occurred while retrieving accounts: {ex.Message}"
        });
      }
    }

    [HttpGet, Route("GetAccountsByRole")]
    public async Task<IActionResult> GetAccountsByRole(int pageNumber, int pageSize, int roleId)
    {
      try
      {
        var accounts = await _acc.GetAccountsByRole(pageNumber, pageSize, roleId);
        if (accounts == null || accounts.Count == 0)
        {
          return NotFound("No results"); // 404
        }
        return Ok(accounts); // 200
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
        return StatusCode(500, "Server error"); // 500
      }
    }

    [HttpGet, Route("GetAccountsBySchool")]
    public async Task<IActionResult> GetAccountsBySchool(int pageNumber, int pageSize, int schoolId)
    {
      try
      {
        var accounts = await _acc.GetAccountsBySchoolId(pageNumber, pageSize, schoolId);
        if (accounts.IsSuccess == false)
        {
          return NotFound(new
          {
            isSuccess = false,
            statusCode = 404,
            message = "No results found"
          });
        }
        return Ok(new
        {
          statusCode = accounts.StatusCode,
          message = accounts.Message,
          data = accounts.Data,
        }); // 200
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
        return StatusCode(500, new
        {
          isSuccess = false,
          statusCode = 500,
          message = $"An error occurred while retrieving accounts: {ex.Message}"
        });
      }
    }

    // GET: api/Auth/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetAccountById(int id)
    {
      var account = await _acc.GetAccount(id);

      if (account.StatusCode == 200)
      {
        return Ok(new
        {
          message = account.Message,
          data = account.Data
        });
      }

      if (account.StatusCode == 404)
      {
        return NotFound(new
        {
          message = account.Message,
          data = account.Data,
        });
      }
      return BadRequest(account);
    }

    [HttpGet, Route("detail/{id}")]
    public async Task<IActionResult> GetAccountByIdToEdit(int id)
    {
      var account = await _acc.GetAccountById(id);

      if (account.StatusCode == 200)
      {
        return Ok(new
        {
          message = account.Message,
          data = account.Data
        });
      }

      if (account.StatusCode == 404)
      {
        return NotFound(new
        {
          message = account.Message,
          data = account.Data,
        });
      }
      return BadRequest(account);
    }

    [HttpGet("search")]
    public async Task<IActionResult> RelativeSearchAccounts([FromQuery] string? TeacherName, [FromQuery] int? schoolId, [FromQuery] int? roleId, [FromQuery] int pageNumber, [FromQuery] int pageSize)
    {
      try
      {
        if (pageNumber < 1 || pageSize < 1)
        {
          return BadRequest("Page number and page size must be positive integers.");
        }

        var results = await _acc.RelativeSearchAccounts(TeacherName, schoolId, roleId, pageNumber, pageSize);

        if (results == null || !results.Any())
        {
          return NotFound("No accounts found matching the criteria."); // 404 if no results
        }

        return Ok(results); // return 200 with the search results
      }
      catch (Exception ex)
      {
        // return a 500 Internal Server Error in case of any exceptions
        return StatusCode(500, $"Internal server error: {ex.Message}");
      }
    }

    // PUT: api/Auth/5
    [Authorize(Policy = "SuperAdminAndAdmin")]
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
    [Authorize(Policy = "SuperAdminAndAdmin")]
    [HttpPost]
    public async Task<IActionResult> CreateAccount(RegisterDto account)
    {
      var result = await _acc.AddAccount(account);

      if (result.StatusCode == 422)
      {
        return StatusCode(422, new
        {
          message = result.Message,
          errors = result.Errors,
          statusCode = result.StatusCode,

        });
      }

      if (result.StatusCode == 409)
      {
        return StatusCode(409, new
        {
          message = result.Message,
          errors = result.Errors,
          statusCode = result.StatusCode,
        });
      }

      if (result.StatusCode == 200)
      {
        return Ok(new
        {
          statusCode = result.StatusCode,
          message = result.Message,
          data = result.AccountAddResType
        });
      }

      return StatusCode(500, new
      {
        statusCode = result.StatusCode,
        message = result.Message,
      });
    }

    // DELETE: api/Auth/5
    [Authorize(Policy = "SuperAdminAndAdmin")]
    [HttpDelete, Route("{id}")]
    public async Task<IActionResult> DeleteAccount(int id)
    {
      var result = await _acc.DeleteAccount(id);

      if (result.StatusCode == 404)
      {
        return NotFound(new
        {
          message = result.Message,
        });
      }

      if (result.StatusCode == 200)
      {
        return Ok(new
        {
          message = result.Message,
        });
      }

      return StatusCode(500, new
      {
        message = result.Message,
      });
    }

    [Authorize(Policy = "SuperAdminAndAdmin")]
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

    [Authorize(Policy = "SuperAdminAndAdmin")]
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
