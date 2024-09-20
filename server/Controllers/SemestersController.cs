using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using server.Data;
using server.Dtos;
using server.IService;
using server.Models;

namespace server.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class SemestersController : ControllerBase
  {
    private readonly ISemester _semester;

    public SemestersController(ISemester semester)
    {
      this._semester = semester;
    }

    // GET: api/AcademicYears1
    [HttpGet]
    public async Task<IActionResult> GetSemesters()
    {
      try
      {
        var academicYears = await _semester.GetSemesters();
        if (academicYears == null)
        {
          return NotFound(); // 404
        }
        return Ok(academicYears); // 200
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
        return StatusCode(500, "Server error"); // 500
      }
    }

    // GET: api/AcademicYears1/5
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
      var semester = await _semester.GetSemester(id);

      if (semester.StatusCode != 200)
      {
        return BadRequest(semester);
      }

      return Ok(semester);
    }

    // PUT: api/AcademicYears1/5
    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, SemesterDto model)
    {
      var semester = await _semester.UpdateSemester(id, model);

      if (semester.StatusCode != 200)
      {
        return BadRequest(semester);
      }

      return Ok(semester);
    }

    // POST: api/AcademicYears1
    [HttpPost]
    public async Task<IActionResult> Post(SemesterDto model)
    {
      var semester = await _semester.CreateSemester(model);

      if (semester.StatusCode != 200)
      {
        return BadRequest(semester);
      }

      return Ok(semester);
    }

    // DELETE: api/AcademicYears1/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
      var semester = await _semester.DeleteSemester(id);

      if (semester.StatusCode != 200)
      {
        return BadRequest(semester);
      }

      return Ok(semester);
    }
  }
}
