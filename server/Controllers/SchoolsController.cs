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
  public class SchoolsController : ControllerBase
  {
    private readonly ISchool _school;
    public SchoolsController(ISchool school)
    {
      this._school = school;
    }

    // GET: api/<SchoolsController>
    [HttpGet, Route("school/{id}")]
    public async Task<IActionResult> GetSchoolById(int id)
    {
      var result = await _school.GetSchool(id);

      if (result.StatusCode != 200)
      {
        return BadRequest(result);
      }
      return Ok(result);
    }

    // GET api/<SchoolsController>/5
    [HttpGet, Route("schools")]
    public async Task<IActionResult> GetSchools()
    {
      try
      {
        var schools = await _school.GetSchools();
        if (schools == null)
        {
          return NotFound();
        }

        return Ok(schools);
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
        return StatusCode(500, "Server error"); // 500
      }
    }

    // POST api/<SchoolsController>
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

    // PUT api/<SchoolsController>/5
    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, SchoolDto model)
    {
      var result = await _school.UpdateSchool(id, model);
      if (result.StatusCode != 200)
      {
        return BadRequest(result);
      }
      return Ok(result);
    }

    // DELETE api/<SchoolsController>/5
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
  }
}
