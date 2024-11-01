using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using server.Dtos;
using server.IService;

namespace server.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  [Authorize]
  public class ClassesController : ControllerBase
  {
    private readonly IClass _func;

    public ClassesController(IClass func)
    {
      this._func = func;
    }

    // GET: api/<ClassesController>
    [HttpGet]
    public async Task<IActionResult> GetAllClasses([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 50)
    {
      try
      {
        var result = await _func.GetClasses(pageNumber, pageSize);
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

    [HttpGet, Route("GetClassesBySchool")]
    public async Task<IActionResult> GetClassesBySchool([FromQuery] int pageNumber, [FromQuery] int pageSize, int schoolId)
    {
      try
      {
        var result = await _func.GetClassesBySchool(pageNumber, pageSize, schoolId);
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

    // GET api/<ClassesController>/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
      var result = await _func.GetClass(id);

      if (result.StatusCode != 200)
      {
        return BadRequest(result);
      }

      return Ok(result);
    }

    // POST api/<ClassesController>
    [HttpPost]
    [Authorize(Policy = "SuperAdminAndAdmin")]
    public async Task<IActionResult> CreateClass(ClassDto model)
    {
      var result = await _func.CreateClass(model);

      if (result.StatusCode != 200)
      {
        return BadRequest(result);
      }

      return Ok(result);
    }

    // PUT api/<ClassesController>/5
    [Authorize(Policy = "SuperAdminAndAdmin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateClass(int id, ClassDto model)
    {
      var result = await _func.UpdateClass(id, model);

      if (result.StatusCode != 200)
      {
        return BadRequest(result);
      }

      return Ok(result);
    }

    // DELETE api/<ClassesController>/5
    [Authorize(Policy = "SuperAdminAndAdmin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteClass(int id)
    {
      var result = await _func.DeleteClass(id);

      if (result.StatusCode != 200)
      {
        return BadRequest(result);
      }

      return Ok(result);
    }

    [Authorize(Policy = "SuperAdminAndAdmin")]
    [HttpPost("upload")]
    public async Task<IActionResult> UploadExcelFile(IFormFile file)
    {
      try
      {
        var result = await _func.ImportExcel(file);

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

    [Authorize(Policy = "SuperAdminAndAdmin")]
    [HttpDelete("bulkdelete")]
    public async Task<IActionResult> BulkDelete(List<int> ids)
    {
      var result = await _func.BulkDelete(ids);

      if (result.StatusCode != 200)
      {
        return BadRequest(result);
      }

      return Ok(result);
    }
  }
}
