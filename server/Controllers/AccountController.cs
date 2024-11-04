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

      var result = await _acc.GetAccounts(pageNumber, pageSize);
      if (result.StatusCode == 404)
      {
        return NotFound(new
        {
          statusCode = 404,
          message = result.Message
        });
      }
      if (result.StatusCode == 200)
      {

        return Ok(new
        {
          statusCode = result.StatusCode,
          message = result.Message,
          data = result.Data,
        });
      }
      return StatusCode(500, new
      {
        statusCode = result.StatusCode,
        message = result.Message,
      });
    }

    [HttpGet, Route("get-accounts-by-role")]
    public async Task<IActionResult> GetAccountsByRole(int pageNumber, int pageSize, int roleId)
    {
      var result = await _acc.GetAccountsByRole(pageNumber, pageSize, roleId);
      if (result.StatusCode == 404)
      {
        return NotFound(new
        {
          statusCode = result.StatusCode,
          message = result.Message
        });
      }

      if (result.StatusCode == 200)
      {
        return Ok(new
        {
          statusCode = result.StatusCode,
          message = result.Message,
          data = result.AccountDto,
        });
      }

      return StatusCode(500, new
      {
        statusCode = result.StatusCode,
        message = result.Message
      });
    }

    [HttpGet, Route("get-accounts-by-school")]
    public async Task<IActionResult> GetAccountsBySchool(int pageNumber, int pageSize, int schoolId)
    {
      var result = await _acc.GetAccountsBySchoolId(pageNumber, pageSize, schoolId);
      if (result.StatusCode == 404)
      {
        return NotFound(new
        {
          statusCode = result.StatusCode,
          message = result.Message
        });
      }

      if (result.StatusCode == 200)
        return Ok(new
        {
          statusCode = result.StatusCode,
          message = result.Message,
          data = result.Data,
        });

      return StatusCode(500, new
      {
        statusCode = result.StatusCode,
        message = result.Message,
      });
    }

    // GET: api/Auth/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetAccountById(int id)
    {
      var result = await _acc.GetAccount(id);

      if (result.StatusCode == 200)
      {
        return Ok(new
        {
          statusCode = result.StatusCode,
          message = result.Message,
          data = result.AccountResData
        });
      }

      if (result.StatusCode == 404)
      {
        return NotFound(new
        {
          statusCode = result.StatusCode,
          message = result.Message,
        });
      }
      return StatusCode(500, new
      {
        statusCode = result.StatusCode,
        message = result.Message,
      });
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
          data = account.AccountData
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
      if (pageNumber < 1 || pageSize < 1)
      {
        return BadRequest("Page number and page size must be positive integers.");
      }

      var results = await _acc.RelativeSearchAccounts(TeacherName, schoolId, roleId, pageNumber, pageSize);

      if (results.StatusCode == 404)
      {
        return NotFound(new
        {
          statusCode = results.StatusCode,
          message = results.Message,
        });
      }
      if (results.StatusCode == 200)
        return Ok(new
        {
          statusCode = results.StatusCode,
          message = results.Message,
          data = results.AccountsResData
        });

      return StatusCode(500, new
      {
        statusCode = results.StatusCode,
        message = results.Message,
      });
    }

    // PUT: api/Auth/5
    [Authorize(Policy = "SuperAdminAndAdmin")]
    [HttpPut, Route("{id}")]
    public async Task<IActionResult> UpdateAccount(int id, AccountBody model)
    {
      var result = await _acc.UpdateAccount(id, model);
      if (result.StatusCode == 200)
      {
        return Ok(new
        {
          statusCode = result.StatusCode,
          message = result.Message,
        });
      }

      if (result.StatusCode == 404)
      {
        return NotFound(new
        {
          statusCode = result.StatusCode,
          message = result.Message,
        });
      }

      return StatusCode(500, new
      {
        statusCode = result.StatusCode,
        message = result.Message,
      });
    }

    // POST: api/Auth
    [Authorize(Policy = "SuperAdminAndAdmin")]
    [HttpPost]
    public async Task<IActionResult> CreateAccount(RegisterDto account)
    {
      var result = await _acc.CreateAccount(account);

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
          statusCode = result.StatusCode,
          message = result.Message,
        });
      }

      if (result.StatusCode == 200)
      {
        return Ok(new
        {
          statusCode = result.StatusCode,
          message = result.Message,
        });
      }

      return StatusCode(500, new
      {
        statusCode = result.StatusCode,
        message = result.Message,
      });
    }

    [Authorize(Policy = "SuperAdminAndAdmin")]
    [HttpDelete, Route("bulkdelete")]
    public async Task<IActionResult> BulkDelete(List<int> ids)
    {
      var result = await _acc.BulkDelete(ids);
      if (result.StatusCode == 200)
      {
        return Ok(new
        {
          statusCode = result.StatusCode,
          message = result.Message,
        });
      }

      if (result.StatusCode == 400)
      {
        return BadRequest(new
        {
          statusCode = result.StatusCode,
          message = result.Message,
        });
      }

      if (result.StatusCode == 404)
      {
        return NotFound(new
        {
          statusCode = result.StatusCode,
          message = result.Message,
        });
      }

      return StatusCode(500, new
      {
        statusCode = result.StatusCode,
        message = result.Message,
      });
    }

    [Authorize(Policy = "SuperAdminAndAdmin")]
    [HttpPost, Route("upload")]
    public async Task<IActionResult> ImportExcel(IFormFile file)
    {
      var result = await _acc.ImportExcel(file);

      if (result.StatusCode == 400)
      {
        return BadRequest(new
        {
          statusCode = result.StatusCode,
          message = result.Message,
        });
      }

      return Ok(new
      {
        statusCode = result.StatusCode,
        message = result.Message,
      });
    }
  }
}
