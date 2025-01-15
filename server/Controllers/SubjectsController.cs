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
  public class SubjectsController : ControllerBase
  {
    private readonly ISubject _subjectRepo;

    public SubjectsController(ISubject subjectRepo)
    {
      this._subjectRepo = subjectRepo;
    }

    // GET: api/<SubjectsController>
    [HttpGet]
    public async Task<IActionResult> GetAllSubject([FromQuery] QueryObject? query)
    {
      var result = await _subjectRepo.GetSubjects(query);
      if (result.StatusCode == 200)
      {
        return Ok(new
        {
          status = result.StatusCode,
          data = result.Data
        });
      }

      return StatusCode(500, result);
    }

    // GET api/<SubjectsController>/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
      var result = await _subjectRepo.GetSubject(id);

      if (result.StatusCode == 200)
      {
        return Ok(new
        {
          status = result.StatusCode,
          data = result.Data
        });
      }

      if (result.StatusCode == 404)
      {
        return NotFound(result);
      }

      return StatusCode(500, result);
    }

    // POST api/<SubjectsController>
    [Authorize(Policy = "SuperAdminAndAdmin")]
    [HttpPost]
    public async Task<IActionResult> CreateSubject(SubjectDto model)
    {
      var subject = await _subjectRepo.CreateSubject(model);

      if (subject.StatusCode == 200)
      {
        return Ok(subject);
      }

      return StatusCode(500, subject);
    }

    // PUT api/<SubjectsController>/5
    [Authorize(Policy = "SuperAdminAndAdmin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateSubject(int id, SubjectDto model)
    {
      var result = await _subjectRepo.UpdateSubject(id, model);

      if (result.StatusCode == 200)
      {
        return Ok(result);
      }

      if (result.StatusCode == 404)
      {
        return NotFound(result);
      }
      return StatusCode(500, result);
    }

    // DELETE api/<SubjectsController>/5
    [Authorize(Policy = "SuperAdminAndAdmin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
      var subject = await _subjectRepo.DeleteSubject(id);

      if (subject.StatusCode == 200)
      {
        return Ok(subject);
      }

      return StatusCode(500, subject);
    }

    [Authorize(Policy = "SuperAdminAndAdmin")]
    [HttpDelete("bulkdelete")]
    public async Task<IActionResult> BulkDelete(List<int> ids)
    {
      var result = await _subjectRepo.BulkDelete(ids);

      if (result.StatusCode == 200)
      {
        return Ok(result);
      }

      if (result.StatusCode == 400)
      {
        return BadRequest(result);
      }

      if (result.StatusCode == 404)
      {
        return NotFound(result);
      }
      return StatusCode(500, result);
    }

    [Authorize(Policy = "SuperAdminAndAdmin")]
    [HttpPost("upload")]
    public async Task<IActionResult> ImportExcelFile(IFormFile file)
    {
      var result = await _subjectRepo.ImportExcelFile(file);

      if (result.StatusCode == 200)
      {
        return Ok(result);
      }

      return StatusCode(500, result);
    }
  }
}
