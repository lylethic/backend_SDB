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
    [Authorize(Policy = "SuperAdminAndAdmin")]

    [HttpGet]
    public async Task<IActionResult> GetSchools(int pageNumber = 1, int pageSize = 50)
    {
      try
      {
        var schools = await _school.GetSchools(pageNumber, pageSize);

        if (schools is null)
        {
          return NotFound();
        }

        return Ok(schools);
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
  }
}
