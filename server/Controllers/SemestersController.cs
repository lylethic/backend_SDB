using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using server.Dtos;
using server.IService;

namespace server.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  [Authorize]
  public class SemestersController : ControllerBase
  {
    private readonly ISemester _semester;

    public SemestersController(ISemester semester)
    {
      this._semester = semester;
    }

    // GET: api/AcademicYears1
    [HttpGet]
    public async Task<IActionResult> GetSemesters(int pageNumber = 1, int pageSize = 50)
    {
      try
      {
        var academicYears = await _semester.GetSemesters(pageNumber, pageSize);
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
    [Authorize(Policy = "SuperAdminAndAdmin")]
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
    [Authorize(Policy = "SuperAdminAndAdmin")]
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
    [Authorize(Policy = "SuperAdminAndAdmin")]
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

    [Authorize(Policy = "SuperAdminAndAdmin")]
    [HttpDelete("bulkdelete")]
    public async Task<IActionResult> BulkDelete(List<int> ids)
    {
      var semester = await _semester.BulkDelete(ids);

      if (semester.StatusCode != 200)
      {
        return BadRequest(semester);
      }

      return Ok(semester);
    }

    [Authorize(Policy = "SuperAdminAndAdmin")]
    [HttpPost("upload")]
    public async Task<IActionResult> ImportExcelFile(IFormFile file)
    {
      try
      {
        var result = await _semester.ImportExcelFile(file);
        if (result.Contains("Successfully"))
        {
          return Ok(result);
        }

        return BadRequest(result);
      }
      catch (Exception ex)
      {
        throw new Exception($"Error: {ex.Message}");
      }
    }
  }
}
