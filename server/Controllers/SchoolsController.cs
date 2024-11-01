using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using server.Dtos;
using server.IService;

namespace server.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  [Authorize]
  public class SchoolsController : ControllerBase
  {
    private readonly ISchool _school;
    public SchoolsController(ISchool school)
    {
      this._school = school;
    }

    [HttpGet]
    public async Task<IActionResult> GetSchools(int pageNumber = 1, int pageSize = 50)
    {
      try
      {
        var result = await _school.GetSchools(pageNumber, pageSize);

        if (result.StatusCode == 200)
        {
          return StatusCode(200, new
          {
            statusCode = result.StatusCode,
            message = result.Message,
            data = result.SchoolData
          });
        }

        return StatusCode(500, result);
      }
      catch (Exception ex)
      {
        return StatusCode(500, $"Server error: {ex.Message}");
      }
    }

    [HttpGet("get-schools-no-pagination")]
    public async Task<IActionResult> GetSchoolsNoPagination()
    {
      try
      {
        var result = await _school.GetSchoolsNoPagnination();

        if (result.StatusCode == 200)
        {
          return StatusCode(200, new
          {
            statusCode = result.StatusCode,
            message = result.Message,
            data = result.SchoolData
          });
        }

        return StatusCode(500, result);
      }
      catch (Exception ex)
      {
        return StatusCode(500, $"Server error: {ex.Message}");
      }
    }

    [HttpGet, Route("{id}")]
    public async Task<IActionResult> GetSchoolById(int id)
    {
      var result = await _school.GetSchool(id);

      if (result.StatusCode != 200)
      {
        return BadRequest(result);
      }
      return Ok(result);
    }

    [HttpGet, Route("get-name-of-school/{id}")]
    public async Task<IActionResult> GetNameOfSchoolById(int id)
    {
      try
      {
        var result = await _school.GetNameOfSchool(id);
        return StatusCode(200, new { nameSchool = result });
      }
      catch (Exception ex)
      {
        return StatusCode(500, ex.Message);
      }
    }

    [Authorize(Policy = "SuperAdmin")]
    [HttpPost]
    public async Task<IActionResult> CreateSchool(SchoolDto model)
    {
      var result = await _school.CreateSchool(model);
      if (result.StatusCode != 200)
      {
        return BadRequest(result);
      }
      return Ok(result);
    }

    [Authorize(Policy = "SuperAdmin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, SchoolDto model)
    {
      var result = await _school.UpdateSchool(id, model);
      if (result.StatusCode != 200)
      {
        return BadRequest(result);
      }
      return Ok(result);
    }

    [Authorize(Policy = "SuperAdmin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
      var result = await _school.DeleteSchool(id);
      if (result.StatusCode != 200)
      {
        return BadRequest(result);
      }
      return Ok(result);
    }

    [Authorize(Policy = "SuperAdmin")]
    [HttpDelete("bulkdelete")]
    public async Task<IActionResult> BulKDelete(List<int> ids)
    {
      var result = await _school.BulkDelete(ids);

      if (result.StatusCode != 200)
      {
        return BadRequest(result);
      }

      return Ok(result);
    }

    [Authorize(Policy = "SuperAdmin")]
    [HttpPost("upload")]
    public async Task<IActionResult> ImportExcelFile(IFormFile file)
    {
      try
      {
        var result = await _school.ImportExcelFile(file);

        if (result.Contains("Successfully"))
        {
          return Ok(result);
        }

        return BadRequest(result);
      }
      catch (Exception ex)
      {
        throw new Exception($"Server Error: {ex.Message}");
      }
    }

    [Authorize(Policy = "SuperAdmin")]
    [HttpPost("export")]
    public async Task<IActionResult> ExportSchools([FromBody] List<int> ids)
    {
      var exportFolder = Path.Combine(Directory.GetCurrentDirectory(), "Exports");

      // Ensure the directory exists
      if (!Directory.Exists(exportFolder))
      {
        Directory.CreateDirectory(exportFolder);
      }

      var filePath = Path.Combine(exportFolder, "Schools.xlsx");

      var result = await _school.ExportSchoolsExcel(ids, filePath);

      if (result.StatusCode != 200)
      {
        return BadRequest(result);
      }

      // Return file for download after successful export
      var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
      var fileName = "Schools.xlsx";

      return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
    }
  }
}
