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
    public async Task<IActionResult> GetAllSubject()
    {
      try
      {
        var result = await _subjectRepo.GetSubjects();
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

    // GET api/<SubjectsController>/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
      var subject = await _subjectRepo.GetSubject(id);

      if (subject.StatusCode != 200)
      {
        return BadRequest(subject);
      }

      return Ok(subject);
    }

    // POST api/<SubjectsController>
    [HttpPost]
    public async Task<IActionResult> Post(SubjectDto model)
    {
      var subject = await _subjectRepo.CreateSubject(model);

      if (subject.StatusCode != 200)
      {
        return BadRequest(subject);
      }

      return Ok(subject);
    }

    // PUT api/<SubjectsController>/5
    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, SubjectDto model)
    {
      var subject = await _subjectRepo.UpdateSubject(id, model);

      if (subject.StatusCode != 200)
      {
        return BadRequest(subject);
      }

      return Ok(subject);
    }

    // DELETE api/<SubjectsController>/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
      var subject = await _subjectRepo.DeleteSubject(id);

      if (subject.StatusCode != 200)
      {
        return BadRequest(subject);
      }

      return Ok(subject);
    }
  }
}
