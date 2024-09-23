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
  public class GradesController : ControllerBase
  {
    private readonly IGrade _grade;

    public GradesController(IGrade grade)
    {
      this._grade = grade;
    }

    // GET: api/<GradesController>
    [HttpGet]
    public async Task<IActionResult> GetAll_Grade()
    {
      try
      {
        var result = await _grade.GetGrades();
        if (result is null)
        {
          return NotFound();
        }

        return Ok(result);
      }
      catch (Exception ex)
      {
        return StatusCode(500, $"Server error: {ex.Message}");
      }
    }

    // GET api/<GradesController>/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById_Grade(int id)
    {
      var result = await _grade.GetGrade(id);

      if (result.StatusCode != 200)
      {
        return BadRequest(result);
      }

      return Ok(result);
    }

    // POST api/<GradesController>
    [HttpPost]
    public async Task<IActionResult> CreateGrade(GradeDto model)
    {
      var result = await _grade.CreateGrade(model);

      if (result.StatusCode != 200)
      {
        return BadRequest(result);
      }

      return Ok(result);
    }

    // PUT api/<GradesController>/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateGrade(int id, GradeDto model)
    {
      var result = await _grade.UpdateGrade(id, model);

      if (result.StatusCode != 200)
      {
        return BadRequest(result);
      }

      return Ok(result);
    }

    // DELETE api/<GradesController>/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteGrade(int id)
    {
      var result = await _grade.DeleteGrade(id);

      if (result.StatusCode != 200)
      {
        return BadRequest(result);
      }

      return Ok(result);
    }
  }
}
