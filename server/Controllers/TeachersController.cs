using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using server.Models;
using server.Dtos;
using server.IService;


namespace server.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class TeachersController : ControllerBase
  {
    private readonly ITeacher _teacherRepo;

    public TeachersController(ITeacher teacherRepo)
    {
      this._teacherRepo = teacherRepo;
    }

    // GET: api/Teachers
    [HttpGet]
    public async Task<IActionResult> GetTeachers()
    {
      try
      {
        var teachers = await _teacherRepo.GetTeachers();
        if (teachers == null)
        {
          return NotFound(); // 404
        }
        return Ok(teachers); // 200
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
        return StatusCode(500, "Server error"); // 500
      }
    }

    // GET: api/Teachers/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetTeacher(int id)
    {
      var teacher = await _teacherRepo.GetTeacher(id);

      if (teacher.StatusCode != 200)
      {
        return BadRequest(teacher);
      }

      return Ok(teacher);
    }

    // PUT: api/Teachers/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutTeacher(int id, TeacherDto model)
    {
      var teacher = await _teacherRepo.UpdateTeacher(id, model);

      if (teacher.StatusCode != 200)
      {
        return BadRequest(teacher);
      }

      return Ok(teacher);
    }

    // POST: api/Teachers
    [HttpPost]
    public async Task<IActionResult> PostTeacher(TeacherDto model)
    {
      var teacher = await _teacherRepo.CreateTeacher(model);

      if (teacher.StatusCode != 200)
      {
        return BadRequest(teacher);
      }

      return Ok(teacher);
    }

    // DELETE: api/Teachers/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTeacher(int id)
    {
      var teacher = await _teacherRepo.DeleteTeacher(id);

      if (teacher.StatusCode != 200)
      {
        return BadRequest(teacher);
      }

      return Ok(teacher);
    }
  }
}
