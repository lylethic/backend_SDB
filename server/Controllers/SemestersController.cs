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
      var academicYears = await _semester.GetSemesters(pageNumber, pageSize);
      if (academicYears.StatusCode == 200)
      {
        return Ok(new
        {
          data = academicYears.Data
        });
      }
      return StatusCode(500, academicYears); // 200

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
      var result = await _semester.ImportExcelFile(file);
      if (result.StatusCode == 200)
      {
        return Ok(result);
      }

      return StatusCode(500, result);
    }
  }
}
