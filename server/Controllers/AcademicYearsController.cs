using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using server.Dtos;
using server.IService;

namespace server.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  [Authorize]
  public class AcademicYearsController : ControllerBase
  {

    private readonly IAcademicYear _acaYearRepo;

    public AcademicYearsController(IAcademicYear acaYearRepo)
    {
      this._acaYearRepo = acaYearRepo;
    }

    // GET: api/AcademicYears
    [HttpGet]
    public async Task<IActionResult> GetAcademicYears(int pageNumber = 1, int pageSize = 50)
    {
      try
      {
        var academicYears = await _acaYearRepo.GetAcademicYears(pageNumber, pageSize);
        if (academicYears == null)
        {
          return NotFound(); // 404
        }
        return Ok(academicYears); // 200
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
        return StatusCode(500, $"Server error: {ex.Message}"); // 500
      }
    }

    // GET: api/AcademicYears/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetAcademicYear(int id)
    {
      var academicYears = await _acaYearRepo.GetAcademicYear(id);

      if (academicYears.StatusCode != 200)
      {
        return BadRequest(academicYears);
      }

      return Ok(academicYears);
    }

    // PUT: api/AcademicYears/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutAcademicYear(int id, AcademicYearDto model)
    {
      var academicYears = await _acaYearRepo.UpdateAcademicYear(id, model);

      if (academicYears.StatusCode != 200)
      {
        return BadRequest(academicYears);
      }

      return Ok(academicYears);
    }

    // POST: api/AcademicYears
    [HttpPost]
    public async Task<IActionResult> PostAcademicYear(AcademicYearDto model)
    {
      var academicYears = await _acaYearRepo.CreateAcademicYear(model);

      if (academicYears.StatusCode != 200)
      {
        return BadRequest(academicYears);
      }

      return Ok(academicYears);
    }

    // DELETE: api/AcademicYears/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAcademicYear(int id)
    {
      var academicYears = await _acaYearRepo.DeleteAcademicYear(id);

      if (academicYears.StatusCode != 200)
      {
        return BadRequest(academicYears);
      }

      return Ok(academicYears);
    }

    [HttpDelete("bulkdelete")]
    public async Task<IActionResult> BulkDelete(List<int> ids)
    {
      var academicYears = await _acaYearRepo.BulkDelete(ids);

      if (academicYears.StatusCode != 200)
      {
        return BadRequest();
      }

      return Ok(academicYears);
    }

    [HttpPost("upload")]
    public async Task<IActionResult> ImportExcel(IFormFile file)
    {
      try
      {
        var result = await _acaYearRepo.ImportExcel(file);

        if (result.Contains("Successfully"))
        {
          return Ok(result);
        }

        return BadRequest(result);
      }
      catch (Exception ex)
      {

        throw new Exception($"Failed: {ex.Message}");
      }
    }
  }
}
