﻿using Microsoft.AspNetCore.Authorization;
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
    public async Task<IActionResult> GetAllWeek(int pageNumber, int pageSize)
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

    // POST api/<WeeksController>
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

    // PUT api/<WeeksController>/5
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
  }
}
