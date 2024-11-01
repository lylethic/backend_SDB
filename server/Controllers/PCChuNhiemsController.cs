using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using server.Dtos;
using server.IService;

namespace server.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  [Authorize]
  public class PCChuNhiemsController : ControllerBase
  {
    private readonly IPC_ChuNhiem _pc;

    public PCChuNhiemsController(IPC_ChuNhiem pc)
    {
      this._pc = pc;
    }

    // GET: api/<PCChuNhiemsController>
    [HttpGet]
    public async Task<IActionResult> GetAll_PCChuNhiem(int pageNumber = 1, int pageSize = 50)
    {
      var result = await _pc.GetPC_ChuNhiems(pageNumber, pageSize);

      if (result.StatusCode == 200)
      {
        return Ok(new
        {
          message = result.Message,
          data = result.Datas
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

    // GET api/<PCChuNhiemsController>/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetByIdPC_ChuNhiem(int id)
    {
      var result = await _pc.GetPC_ChuNhiem(id);

      if (result.StatusCode == 200)
      {
        return Ok(new
        {
          message = result.Message,
          data = result.Data
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

    [HttpGet, Route("thongtinlopphancong/{id}")]
    public async Task<IActionResult> GetThongtinlopphancong(int id)
    {
      var result = await _pc.Get_ChuNhiem_Teacher_Class(id);

      if (result.StatusCode == 200)
      {
        return Ok(new
        {
          message = result.Message,
          data = result.Data
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

    // POST api/<PCChuNhiemsController>
    [Authorize(Policy = "SuperAdminAndAdmin")]
    [HttpPost]
    public async Task<IActionResult> CreatePC_ChuNhiem(PC_ChuNhiemDto model)
    {
      var result = await _pc.CreatePC_ChuNhiem(model);

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
          data = result.PC_ChuNhiemDto
        });
      }

      if (result.StatusCode == 409)
      {
        return BadRequest(new
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
    [HttpPost("upload")]
    public async Task<IActionResult> ImportExcelFile(IFormFile file)
    {
      try
      {
        var result = await _pc.ImportExcelFile(file);

        if (result.Contains("Successfully"))
        {
          return Ok(result);
        }

        return BadRequest();
      }
      catch (Exception ex)
      {
        return StatusCode(500, $"Server error: {ex.Message}");
      }
    }


    // PUT api/<PCChuNhiemsController>/5
    [Authorize(Policy = "SuperAdminAndAdmin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePC_ChuNhiem(int id, PC_ChuNhiemDto model)
    {
      var result = await _pc.UpdatePC_ChuNhiem(id, model);

      if (result.StatusCode == 404)
      {
        return NotFound(result);
      }

      if (result.StatusCode == 200)
      {
        return Ok(result);
      }

      return StatusCode(500, result);
    }

    // DELETE api/<PCChuNhiemsController>/5
    [Authorize(Policy = "SuperAdminAndAdmin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePC_ChuNhiem(int id)
    {
      var result = await _pc.DeletePC_ChuNhiem(id);

      if (result.StatusCode == 404)
      {
        return NotFound(result);
      }

      if (result.StatusCode == 200)
      {
        return Ok(result);
      }

      return StatusCode(500, result);
    }

    [Authorize(Policy = "SuperAdminAndAdmin")]
    [HttpDelete("bulk-delete")]
    public async Task<IActionResult> BulkDelete(List<int> ids)
    {
      var result = await _pc.BulkDelete(ids);

      if (result is null)
      {
        return BadRequest(result?.Message);
      }

      return Ok(result);
    }
  }
}
