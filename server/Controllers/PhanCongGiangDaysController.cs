using Microsoft.AspNetCore.Mvc;
using server.Dtos;
using server.IService;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace server.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class PhanCongGiangDaysController : ControllerBase
  {
    private readonly IPC_GiangDay_BiaSDB _func;

    public PhanCongGiangDaysController(IPC_GiangDay_BiaSDB func)
    {
      this._func = func;
    }

    // GET: api/<PhanCongGiangDaysController>
    [HttpGet]
    public async Task<IActionResult> GetAll(int pageNumber = 1, int pageSize = 50)
    {
      try
      {
        var result = await _func.GetPC_GiangDay_BiaSDBs(pageNumber, pageSize);
        if (result == null)
        {
          return NotFound();
        }

        return Ok(result);
      }
      catch (Exception ex)
      {

        throw new Exception($"Server error: {ex.Message}");
      }
    }

    // GET api/<PhanCongGiangDaysController>/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
      var result = await _func.GetPC_GiangDay_BiaSDB(id);

      if (result.StatusCode != 200)
      {
        return BadRequest(result.StatusCode);
      }

      return Ok(result);
    }

    // POST api/<PhanCongGiangDaysController>
    [HttpPost]
    public async Task<IActionResult> Create(PC_GiangDay_BiaSDBDto model)
    {
      var result = await _func.CreatePC_GiangDay_BiaSDB(model);

      if (result.StatusCode != 200)
      {
        return BadRequest(result.StatusCode);
      }

      return Ok(result);
    }

    // PUT api/<PhanCongGiangDaysController>/5
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, PC_GiangDay_BiaSDBDto model)
    {
      var result = await _func.UpdatePC_GiangDay_BiaSDB(id, model);

      if (result.StatusCode != 200)
      {
        return BadRequest(result.StatusCode);
      }

      return Ok(result);
    }

    // DELETE api/<PhanCongGiangDaysController>/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
      var result = await _func.DeletePC_GiangDay_BiaSDB(id);

      if (result.StatusCode != 200)
      {
        return BadRequest(result.StatusCode);
      }

      return Ok(result);
    }

    [HttpDelete("BulkDelete")]
    public async Task<IActionResult> BulkDelete(List<int> ids)
    {
      var result = await _func.BulkDelete(ids);

      if (result.StatusCode != 200)
      {
        return BadRequest(result.StatusCode);
      }

      return Ok(result);
    }

    [HttpPost("upload")]
    public async Task<IActionResult> ImportExcel(IFormFile file)
    {
      try
      {
        var result = await _func.ImportExcelFile(file);

        if (result.Contains("Successfully"))
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
