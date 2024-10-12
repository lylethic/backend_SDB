using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using server.Dtos;
using server.IService;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace server.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  [Authorize]
  public class BiaSoDauBaisController : ControllerBase
  {
    readonly IBiaSoDauBai _biaSodaubai;

    public BiaSoDauBaisController(IBiaSoDauBai biasodaubai) { this._biaSodaubai = biasodaubai; }

    // GET: api/<BiaSoDauBaisController>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 50)
    {
      try
      {
        var result = await _biaSodaubai.GetBiaSoDauBais(pageNumber, pageSize);
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

    // GET api/<BiaSoDauBaisController>/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
      var result = await _biaSodaubai.GetBiaSoDauBai(id);

      if (result.StatusCode != 200)
      {
        return BadRequest();
      }

      return Ok(result.Data);
    }

    [HttpGet("getbyschool")]
    public async Task<IActionResult> GetBiaSoDauBaisBySchool(int pageNumber, int pageSize, int schoolId)
    {
      try
      {
        var result = await _biaSodaubai.GetBiaSoDauBaisBySchoolId(pageNumber, pageSize, schoolId);
        if (result is null || !result.Any())
        {
          return NotFound("Khong co ket qua"); // 404
        }

        return Ok(result);
      }
      catch (Exception ex)
      {
        throw new Exception($"Error: {ex.Message}");
      }
    }

    // POST api/<BiaSoDauBaisController>
    [Authorize(Policy = "SuperAdminAndAdmin")]
    [HttpPost]
    public async Task<IActionResult> Create(BiaSoDauBaiDto model)
    {
      var result = await _biaSodaubai.CreateBiaSoDauBai(model);

      if (result.StatusCode != 200)
      {
        return BadRequest(result.Message);
      }

      return Ok(result.Data);
    }

    // PUT api/<BiaSoDauBaisController>/5
    [Authorize(Policy = "SuperAdminAndAdmin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, BiaSoDauBaiDto model)
    {
      var result = await _biaSodaubai.UpdateBiaSoDauBai(id, model);

      if (result.StatusCode != 200)
      {
        return BadRequest(result.Message);
      }

      return Ok(result);
    }

    // DELETE api/<BiaSoDauBaisController>/5
    [Authorize(Policy = "SuperAdminAndAdmin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
      var result = await _biaSodaubai.DeleteBiaSoDauBai(id);

      if (result.StatusCode != 200)
      {
        return BadRequest(result.Message);
      }

      return Ok(result.Data);
    }

    [Authorize(Policy = "SuperAdminAndAdmin")]
    [HttpDelete("bulkdelete")]
    public async Task<IActionResult> BulkDelete(List<int> ids)
    {
      var result = await _biaSodaubai.BulkDelete(ids);

      if (result.StatusCode != 200)
      {
        return BadRequest(result.Message);
      }

      return Ok(result.Data);
    }

    [Authorize(Policy = "SuperAdminAndAdmin")]
    [HttpPost("upload")]
    public async Task<IActionResult> ImportExcel(IFormFile file)
    {
      try
      {
        var result = await _biaSodaubai.ImportExcel(file);

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

    //GET /api/BiaSoDauBais/Search? schoolName = ABC & schoolId = 1 & classId = 2
    /* Search keywords: Schoolname, SchoolId, classId */
    [HttpGet("Search")]
    public async Task<IActionResult> SearchBiaSoDauBais(int? schoolId, int? classId)
    {
      var result = await _biaSodaubai.SearchBiaSoDauBais(schoolId, classId);

      if (result == null || !result.Any())
      {
        return NotFound("No results found.");
      }

      return Ok(result);
    }
  }
}
