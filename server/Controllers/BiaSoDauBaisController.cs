using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using server.Dtos;
using server.IService;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace server.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  [Authorize]
  public class BiaSoDauBaisController : ControllerBase
  {
    readonly IBiaSoDauBai _biaSodaubai;

    public BiaSoDauBaisController(IBiaSoDauBai biasodaubai)
    {
      this._biaSodaubai = biasodaubai;
    }

    // GET: api/<BiaSoDauBaisController>
    [HttpGet("count-bia-so-dau-bai")]
    public async Task<IActionResult> CountBiaSoDauBaiAsync()
    {
      var result = await _biaSodaubai.CountBiaSoDauBaiAsync();
      return Ok(result);
    }

    // GET: api/<BiaSoDauBaisController>
    [HttpGet("count-bia-so-dau-bai-active")]
    public async Task<IActionResult> CountBiaSoDauBaiActiveAsync()
    {
      var result = await _biaSodaubai.CountBiaSoDauBaiActiveAsync();
      return Ok(result);
    }

    // GET: api/<BiaSoDauBaisController>
    // GetBiaSoDauBais_Active for user
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] QueryObject? queryObject)
    {
      var result = await _biaSodaubai.GetBiaSoDauBais_Active(queryObject);
      if (result.StatusCode == 200)
      {
        return Ok(new
        {
          message = result.Message,
          data = result.ListBiaSoDauBaiRes
        });
      }

      if (result.StatusCode == 404)
      {
        return NotFound(new
        {
          message = result.Message,
        });
      }

      return StatusCode(500, new
      {
        message = result.Message
      });
    }

    // Status true & false for Admin
    [HttpGet, Route("get-all-bia-so")]
    public async Task<IActionResult> GetAllBiaSo([FromQuery] QueryObject? queryObject)
    {
      var result = await _biaSodaubai.GetBiaSoDauBais(queryObject);
      if (result.StatusCode == 200)
      {
        return Ok(new
        {
          status = 200,
          message = result.Message,
          data = result.ListBiaSoDauBaiRes
        });
      }

      if (result.StatusCode == 404)
      {
        return NotFound(new
        {
          status = 404,
          message = result.Message,
        });
      }

      return StatusCode(500, new
      {
        status = 500,
        message = result.Message
      });
    }

    // GET api/<BiaSoDauBaisController>/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
      var result = await _biaSodaubai.GetBiaSoDauBai(id);

      if (result.StatusCode == 200)
      {
        return Ok(new
        {
          message = result.Message,
          data = result.BiaSoDauBaiDto
        });
      }

      if (result.StatusCode == 404)
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

    [HttpGet("get-by-school")]
    public async Task<IActionResult> GetBiaSoDauBaisBySchool([FromQuery] QueryObject? queryObject, int schoolId)
    {
      var result = await _biaSodaubai.GetBiaSoDauBaisBySchool_Active(queryObject, schoolId);
      if (result.StatusCode == 200)
      {
        return Ok(new
        {
          status = 200,
          message = result.Message,
          data = result.ListBiaSoDauBaiRes
        });
      }

      if (result.StatusCode == 404)
      {
        return NotFound(new
        {
          status = 404,
          message = result.Message,
        });
      }

      if (result.StatusCode == 400)
      {
        return BadRequest(new
        {
          status = 400,
          message = result.Message,
        });
      }

      return StatusCode(500, new
      {
        status = 500,
        message = result.Message,
      });
    }

    // status true&false for admin
    [HttpGet("get-all-bia-so-by-school")]
    public async Task<IActionResult> GetAllBiaSoDauBaisBySchool([FromQuery] QueryObject? queryObject, int schoolId)
    {
      var result = await _biaSodaubai.GetBiaSoDauBaisBySchool(queryObject, schoolId);
      if (result.StatusCode == 200)
      {
        return Ok(new
        {
          message = result.Message,
          data = result.ListBiaSoDauBaiRes
        });
      }

      if (result.StatusCode == 404)
      {
        return NotFound(new
        {
          message = result.Message,
        });
      }

      return StatusCode(500, new
      {
        message = result.Message,
      });
    }

    // POST api/<BiaSoDauBaisController>
    [Authorize(Policy = "SuperAdminAndAdmin")]
    [HttpPost]
    public async Task<IActionResult> Create(BiaSoDauBaiDto model)
    {
      var result = await _biaSodaubai.CreateBiaSoDauBai(model);

      if (result.StatusCode == 200)
      {
        return Ok(new
        {
          message = result.Message,
          data = result.BiaSoDauBaiDto
        });
      }

      if (result.StatusCode == 409)
      {
        return StatusCode(409, new
        {
          message = result.Message,
        });
      }

      if (result.StatusCode == 404)
      {
        return NotFound(new
        {
          message = result.Message,
        });
      }

      return StatusCode(500, new
      {
        message = result.Message,
      });
    }

    // PUT api/<BiaSoDauBaisController>/5
    [Authorize(Policy = "SuperAdminAndAdmin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, BiaSoDauBaiDto model)
    {
      var result = await _biaSodaubai.UpdateBiaSoDauBai(id, model);

      if (result.StatusCode == 200)
      {
        return Ok(new
        {
          message = result.Message,
          data = result.BiaSoDauBaiDto
        });
      }

      if (result.StatusCode == 404)
      {
        return NotFound(new
        {
          message = result.Message,
        });
      }

      return StatusCode(500, new
      {
        message = result.Message,
      });
    }

    // DELETE api/<BiaSoDauBaisController>/5
    [Authorize(Policy = "SuperAdminAndAdmin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
      var result = await _biaSodaubai.DeleteBiaSoDauBai(id);

      if (result.StatusCode == 200)
      {
        return Ok(new
        {
          message = result.Message,
        });
      }

      if (result.StatusCode == 404)
      {
        return NotFound(new
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
    [HttpDelete("bulkdelete")]
    public async Task<IActionResult> BulkDelete(List<int> ids)
    {
      var result = await _biaSodaubai.BulkDelete(ids);

      if (result.StatusCode == 200)
      {
        return Ok(new
        {
          message = result.Message,
        });
      }

      if (result.StatusCode == 400)
      {
        return BadRequest(new
        {
          message = result.Message,
        });
      }

      if (result.StatusCode == 404)
      {
        return NotFound(new
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
    [HttpPost("upload")]
    public async Task<IActionResult> ImportExcel(IFormFile file)
    {
      var result = await _biaSodaubai.ImportExcel(file);
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
        return Ok(new
        {
          statusCode = result.StatusCode,
          message = result.Message,
        });
      }

      return StatusCode(500, new
      {
        message = result.Message,
      });

    }

    //GET /api/BiaSoDauBais/Search? schoolName = ABC & schoolId = 1 & classId = 2
    /* Search keywords: Schoolname, SchoolId, classId */
    [HttpGet("search")]
    public async Task<IActionResult> SearchBiaSoDauBais(SearchBiaSoDauBaiObject? searchObject)
    {
      searchObject ??= new SearchBiaSoDauBaiObject();
      var result = await _biaSodaubai.SearchBiaSoDauBais(searchObject);

      if (result.StatusCode == 200)
      {
        return Ok(new
        {
          message = result.Message,
          data = result.ListBiaSoDauBaiDto
        });
      }

      if (result.StatusCode == 400)
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
  }
}
