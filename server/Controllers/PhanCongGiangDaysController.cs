using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using server.Dtos;
using server.IService;

namespace server.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  [Authorize]
  public class PhanCongGiangDaysController : ControllerBase
  {
    private readonly IPC_GiangDay_BiaSDB _pc;

    public PhanCongGiangDaysController(IPC_GiangDay_BiaSDB pc)
    {
      this._pc = pc;
    }

    // GET: api/<PhanCongGiangDaysController>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] QueryObject queryObject)
    {
      var result = await _pc.GetPC_GiangDay_BiaSDBs(queryObject);
      if (result.StatusCode == 400)
      {
        return BadRequest(new
        {
          status = 400,
          message = result.Message
        });
      }

      if (result.StatusCode == 200)
      {
        return Ok(new
        {
          status = 200,
          message = result.Message,
          data = result.ListMapData
        });
      }

      return StatusCode(500, new
      {
        status = 500,
        message = result.Message
      });
    }

    // GET api/<PhanCongGiangDaysController>/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
      var result = await _pc.GetPC_GiangDay_BiaSDB(id);

      if (result.StatusCode != 200)
      {
        return BadRequest(result.StatusCode);
      }

      return Ok(result);
    }

    // POST api/<PhanCongGiangDaysController>
    [Authorize(Policy = "AdmiSuperAdminAndAdminn")]
    [HttpPost]
    public async Task<IActionResult> Create(PC_GiangDay_BiaSDBDto model)
    {
      var result = await _pc.CreatePC_GiangDay_BiaSDB(model);

      if (result.StatusCode != 200)
      {
        return BadRequest(result.StatusCode);
      }

      return Ok(result);
    }

    // PUT api/<PhanCongGiangDaysController>/5
    [Authorize(Policy = "SuperAdminAndAdmin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, PC_GiangDay_BiaSDBDto model)
    {
      var result = await _pc.UpdatePC_GiangDay_BiaSDB(id, model);

      if (result.StatusCode != 200)
      {
        return BadRequest(result.StatusCode);
      }

      return Ok(result);
    }

    // DELETE api/<PhanCongGiangDaysController>/5
    [Authorize(Policy = "SuperAdminAndAdmin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
      var result = await _pc.DeletePC_GiangDay_BiaSDB(id);

      if (result.StatusCode != 200)
      {
        return BadRequest(result.StatusCode);
      }

      return Ok(result);
    }

    [Authorize(Policy = "SuperAdminAndAdmin")]
    [HttpDelete("BulkDelete")]
    public async Task<IActionResult> BulkDelete(List<int> ids)
    {
      var result = await _pc.BulkDelete(ids);

      if (result.StatusCode != 200)
      {
        return BadRequest(result.StatusCode);
      }

      return Ok(result);
    }

    [Authorize(Policy = "AdSuperAdminAndAdminmin")]
    [HttpPost("upload")]
    public async Task<IActionResult> ImportExcel(IFormFile file)
    {
      try
      {
        var result = await _pc.ImportExcelFile(file);

        if (result.StatusCode == 200)
        {
          return Ok(result);
        }

        return BadRequest(result);
      }
      catch (Exception ex)
      {
        return StatusCode(500, $"Server Error: {ex.Message}");
      }
    }
  }
}
