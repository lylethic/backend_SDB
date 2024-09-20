using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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
  [Authorize]
  public class StudentsController : ControllerBase
  {
    private readonly IStudent _studentRepo;

    public StudentsController(IStudent studentRepo)
    {
      _studentRepo = studentRepo;
    }

    // GET: api/Students
    [HttpGet]
    public async Task<IActionResult> GetStudents()
    {
      try
      {
        var roles = await _studentRepo.GetStudents();
        if (roles == null)
        {
          return NotFound(); // 404
        }
        return Ok(roles); // 200
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
        return StatusCode(500, "Server error"); // 500
      }
    }

    // GET: api/Students/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetStudent(int id)
    {
      var result = await _studentRepo.GetStudent(id);
      if (result.StatusCode != 200)
      {
        return BadRequest(result);
      }

      return Ok(result);
    }

    // PUT: api/Students/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutStudent(int id, StudentDto model)
    {
      var result = await _studentRepo.UpdateStudent(id, model);
      if (result.StatusCode != 200)
      {
        return BadRequest(result);
      }

      return Ok(result);
    }

    // POST: api/Students
    [HttpPost]
    public async Task<ActionResult<Student>> PostStudent(StudentDto model)
    {
      var result = await _studentRepo.CreateStudent(model);
      if (result.StatusCode != 200)
      {
        return BadRequest(result);
      }

      return Ok(result);
    }

    // DELETE: api/Students/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteStudent(int id)
    {
      var result = await _studentRepo.DeleteStudent(id);
      if (result.StatusCode != 200)
      {
        return BadRequest(result);
      }

      return Ok(result);
    }
  }
}