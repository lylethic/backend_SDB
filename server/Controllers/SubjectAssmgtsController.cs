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
  public class SubjectAssmgtsController : ControllerBase
  {
    private readonly ISubject_Assgm _subject_Assgm;

    public SubjectAssmgtsController(ISubject_Assgm subject_Assgm)
    {
      this._subject_Assgm = subject_Assgm;
    }

    // GET: api/<SubjectAssmgtsController>
    [HttpGet]
    public async Task<IActionResult> GetAll_SubjectAssignment()
    {
      try
      {
        var result = await _subject_Assgm.GetSubjectAssgms();
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

    // GET api/<SubjectAssmgtsController>/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById_SubjectAssignment(int id)
    {
      var result = await _subject_Assgm.GetSubjectAssgm(id);
      if (result.StatusCode != 200)
      {
        return BadRequest(result);
      }
      return Ok(result);
    }

    // POST api/<SubjectAssmgtsController>
    [HttpPost]
    public async Task<IActionResult> CreateSubjectAssignment(SubjectAssgmDto model)
    {
      var result = await _subject_Assgm.CreateSubjectAssgm(model);
      if (result.StatusCode != 200)
      {
        return BadRequest(result);
      }
      return Ok(result);
    }

    // PUT api/<SubjectAssmgtsController>/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateSubjectAssignment(int id, SubjectAssgmDto model)
    {
      var result = await _subject_Assgm.UpdateSubjectAssgm(id, model);
      if (result.StatusCode != 200)
      {
        return BadRequest(result);
      }
      return Ok(result);
    }

    // DELETE api/<SubjectAssmgtsController>/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSubjectAssignment(int id)
    {
      var result = await _subject_Assgm.DeleteSubjectAssgm(id);
      if (result.StatusCode != 200)
      {
        return BadRequest(result);
      }
      return Ok(result);
    }
  }
}
