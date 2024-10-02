using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    public async Task<IActionResult> GetStudents(int pageNumber = 1, int pageSize = 50)
    {
      try
      {
        var roles = await _studentRepo.GetStudents(pageNumber, pageSize);
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

    [HttpDelete("bulkdelete")]
    public async Task<IActionResult> BulkDelete(List<int> ids)
    {
      var result = await _studentRepo.BulkDelete(ids);
      if (result.StatusCode != 200)
      {
        return BadRequest(result);
      }

      return Ok(result);
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadExcelFile(IFormFile file)
    {
      try
      {
        var result = await _studentRepo.ImportExcel(file);

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