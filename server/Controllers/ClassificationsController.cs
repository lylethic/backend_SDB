using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using server.Dtos;
using server.IService;

namespace server.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  [Authorize]
  public class ClassificationsController : ControllerBase
  {
    private readonly IClassify _classify;

    public ClassificationsController(IClassify classify)
    {
      this._classify = classify;
    }

    // GET: api/<ClassificationsController>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 50)
    {
      try
      {
        var result = await _classify.GetClassifys(pageNumber, pageSize);
        if (result is null)
        {
          return NotFound(); // 404
        }

        return Ok(result);
      }
      catch (Exception ex)
      {
        throw new Exception($"Error: {ex.Message}");
      }
    }

    // GET api/<ClassificationsController>/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
      var result = await _classify.GetClassify(id);

      if (result.StatusCode != 200)
      {
        return BadRequest();
      }

      return Ok(result);
    }

    // POST api/<ClassificationsController>
    [Authorize(Policy = "SuperAdminAndAdmin")]
    [HttpPost]
    public async Task<IActionResult> Create(ClassifyDto model)
    {
      var result = await _classify.CreateClassify(model);

      if (result.StatusCode != 200)
      {
        return BadRequest();
      }

      return Ok(result);
    }

    // PUT api/<ClassificationsController>/5
    [Authorize(Policy = "SuperAdminAndAdmin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, ClassifyDto model)
    {
      var result = await _classify.UpdateClassify(id, model);

      if (result.StatusCode != 200)
      {
        return BadRequest(result.Message);
      }

      return Ok(result);
    }

    // DELETE api/<ClassificationsController>/5
    [Authorize(Policy = "SuperAdminAndAdmin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
      var result = await _classify.DeleteClassify(id);

      if (result.StatusCode != 200)
      {
        return BadRequest();
      }

      return Ok(result);
    }

    [Authorize(Policy = "SuperAdminAndAdmin")]
    [HttpDelete("bulkdelete")]
    public async Task<IActionResult> BulkDelete(List<int> ids)
    {
      var result = await _classify.BulkDelete(ids);

      if (result.StatusCode != 200)
      {
        return BadRequest();
      }

      return Ok(result);
    }

    [Authorize(Policy = "SuperAdminAndAdmin")]
    [HttpPost("upload")]
    public async Task<IActionResult> UploadExcelFile(IFormFile file)
    {
      try
      {
        var result = await _classify.ImportExcel(file);

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
