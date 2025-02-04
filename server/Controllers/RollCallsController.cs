using Microsoft.AspNetCore.Mvc;
using server.Dtos;
using server.IService;

namespace server.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class RollCallsController : ControllerBase
  {
    private readonly IRollCall _rollCall;

    public RollCallsController(IRollCall rollCall)
    {
      this._rollCall = rollCall;
    }

    // GET: api/<RollCallsController>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
      var result = await _rollCall.RollCalls();
      if (result.StatusCode == 200)
      {
        return Ok(new
        {
          message = result.Message,
          data = result.ListRollCallRes
        });
      }
      return StatusCode(result.StatusCode, new
      {
        message = result.Message,
        data = result.ListRollCallRes
      });
    }

    // GET api/<RollCallsController>/5
    [HttpGet("{id}")]
    public string Get(int id)
    {
      return "value";
    }

    // POST api/<RollCallsController>
    /*
     {
      "rollCall": {
        "callRollId": 1,
        "classId": 21,
        "weekId": 4,
        "dayOfTheWeek": "Monday",
        "dateAt": "2025-02-04",
        "numberOfAttendants": 38
      },
      "absences": [
        {
          "studentId": 156,
          "description": "Sick"
        },
        {
          "studentId": 157,
          "description": "Late"
        }
      ]
     */
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] RollCallCreateRequest request)
    {
      {
        var result = await _rollCall.Create(request.RollCall, request.Absences);
        if (result.StatusCode == 200)
        {
          return Ok(new
          {
            message = result.Message,
            data = result.RollCall
          });
        }

        return StatusCode(result.StatusCode, new
        {
          message = result.Message,
          data = result.RollCall
        });
      }
    }

    // PUT api/<RollCallsController>/5
    [HttpPut("{id}")]
    public void Put(int id, [FromBody] string value)
    {

    }

    // DELETE api/<RollCallsController>/5
    [HttpDelete("{id}")]
    public void Delete(int id)
    {
    }
  }
  public class RollCallCreateRequest
  {
    public RollCallDto RollCall { get; set; }
    public List<AbsenceDto> Absences { get; set; }
  }
}
