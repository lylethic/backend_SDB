using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using server.Dtos;
using server.IService;

namespace server.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  [Authorize]
  public class WeeksController : ControllerBase
  {
    private readonly IWeek _week;

    public WeeksController(IWeek week)
    {
      this._week = week;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllWeek(int pageNumber = 1, int pageSize = 50)
    {
      try
      {
        var result = await _week.GetWeeks(pageNumber, pageSize);
        if (result is null)
        {
          return NotFound();
        }

        return Ok(result);
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
        return StatusCode(500, "Server error"); // 500
      }
    }

    // GET api/<WeeksController>/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
      var subject = await _week.GetWeek(id);

      if (subject.StatusCode != 200)
      {
        return BadRequest(subject);
      }

      return Ok(subject);
    }

    [HttpGet, Route("Get7DaysInWeek")]
    public async Task<IActionResult> Get7DaysInWeek(int selectedWeekId)
    {
      try
      {
        var result = await _week.Get7DaysInWeek(selectedWeekId);
        return Ok(result);
      }
      catch (Exception ex)
      {
        return BadRequest($"Có lỗi: {ex.Message}");
      }
    }

    // POST api/<WeeksController>
    /* Mau
    {
      "weekId": 0,
      "semesterId": 1,
      "weekName": "Tuần 101",
      "weekStart": "2024-10-21",
      "weekEnd": "2024-10-27",
      "status": true
    }
    */
    [HttpPost]
    public async Task<IActionResult> CreateWeek(WeekDto model)
    {
      var subject = await _week.CreateWeek(model);

      if (subject.StatusCode != 200)
      {
        return BadRequest(subject);
      }

      return Ok(subject);
    }

    // PUT api/<WeeksController>/5
    [Authorize(Policy = "SuperAdminAndAdmin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateWeek(int id, WeekDto model)
    {
      var subject = await _week.UpdateWeek(id, model);

      if (subject.StatusCode != 200)
      {
        return BadRequest(subject);
      }

      return Ok(subject);
    }

    // DELETE api/<WeeksController>/5
    [Authorize(Policy = "SuperAdminAndAdmin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteWeek(int id)
    {
      var subject = await _week.DeleteWeek(id);

      if (subject.StatusCode != 200)
      {
        return BadRequest(subject);
      }

      return Ok(subject);
    }

    [Authorize(Policy = "SuperAdminAndAdmin")]
    [HttpDelete("bulk-delete")]
    public async Task<IActionResult> BulkDelete([FromBody] List<int> ids)
    {
      var subject = await _week.BulkDelete(ids);

      if (subject.StatusCode != 200)
      {
        return BadRequest(subject);
      }

      return Ok(subject);
    }

    [Authorize(Policy = "SuperAdminAndAdmin")]
    [HttpPost("upload")]
    public async Task<IActionResult> ImportExcelFile(IFormFile file)
    {
      try
      {
        var result = await _week.ImportExcelFile(file);

        if (result.Contains("Successfully"))
        {
          return Ok(result);
        }

        return BadRequest(result);
      }
      catch (DbUpdateException dbEx)
      {
        throw new Exception($"Error: {dbEx.Message}");
      }
    }

  }
}

