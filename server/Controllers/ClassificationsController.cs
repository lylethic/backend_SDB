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
    public async Task<IActionResult> GetAll([FromQuery] QueryObject? queryObject)
    {
      var result = await _classify.GetClassifys(queryObject);
      if (result.StatusCode == 200)
      {
        return Ok(new
        {
          statusCode = result.StatusCode,
          data = result.Data
        });
      }

      return StatusCode(500, result);
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
      var result = await _classify.ImportExcel(file);

      if (result.StatusCode == 200)
      {
        return Ok(result);
      }
      if (result.StatusCode == 400)
      {
        return BadRequest(result);
      }

      return StatusCode(500, result);
    }
  }
}
