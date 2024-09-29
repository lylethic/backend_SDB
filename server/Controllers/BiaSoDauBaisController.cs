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

    // POST api/<BiaSoDauBaisController>
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
  }
}
