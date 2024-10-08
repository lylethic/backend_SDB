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
      try
      {
        var result = await _pc.GetPC_ChuNhiems(pageNumber, pageSize);

        if (result is null)
        {
          return NotFound("Not found");
        }

        return Ok(result);
      }
      catch (Exception ex)
      {
        return StatusCode(500, $"Server error: {ex.Message}");
      }
    }

    // GET api/<PCChuNhiemsController>/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetByIdPC_ChuNhiem(int id)
    {
      var result = await _pc.GetPC_ChuNhiem(id);

      if (result is null)
      {
        return BadRequest(result?.Message);
      }

      return Ok(result);
    }

    // POST api/<PCChuNhiemsController>
    [HttpPost]
    public async Task<IActionResult> CreatePC_ChuNhiem(PC_ChuNhiemDto model)
    {
      var result = await _pc.CreatePC_ChuNhiem(model);

      if (result is null)
      {
        return BadRequest(result?.Message);
      }

      return Ok(result);
    }

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
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePC_ChuNhiem(int id, PC_ChuNhiemDto model)
    {
      var result = await _pc.UpdatePC_ChuNhiem(id, model);

      if (result is null)
      {
        return BadRequest(result?.Message);
      }

      return Ok(result);
    }

    // DELETE api/<PCChuNhiemsController>/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePC_ChuNhiem(int id)
    {
      var result = await _pc.DeletePC_ChuNhiem(id);

      if (result is null)
      {
        return BadRequest(result?.Message);
      }

      return Ok(result);
    }

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
