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
    public async Task<IActionResult> Get(int id)
    {
      var result = await _rollCall.RollCall(id);
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
        message = result.Message
      });
    }

    // POST api/<RollCallsController>
    /*
      {
        "rollCall": {
          "callRollId": 0,
          "classId": 21,
          "weekId": 4,
          "dayOfTheWeek": "thứ 2",
          "numberOfAttendants": 39,
          "dateAt": "2025-02-10",
          "dateCreated": "2025-02-04T07:53:34.839Z",
          "dateUpdated": null
        },
        "absences": [
          {
            "callRollId": 0,
            "studentId": 156,
            "isExecute": true,
            "description": "Sốt xuất huyết"
          }
        ]
      }
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
    public async Task<IActionResult> Put(int id, [FromBody] RollCallDto model)
    {
      var result = await _rollCall.Update(id, model);
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

    // DELETE api/<RollCallsController>/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
      var result = await _rollCall.Delete(id);
      if (result.StatusCode == 200)
      {
        return Ok(new
        {
          message = result.Message,
        });
      }
      return StatusCode(result.StatusCode, new
      {
        message = result.Message,
      });
    }

    [HttpDelete("bulk-delete/{id}")]
    public async Task<IActionResult> BulkDelete(List<int> ids)
    {
      var result = await _rollCall.BulkDelete(ids);
      if (result.StatusCode == 200)
      {
        return Ok(new
        {
          message = result.Message,
        });
      }
      return StatusCode(result.StatusCode, new
      {
        message = result.Message,
      });
    }
  }

  public class RollCallCreateRequest
  {
    public RollCallDto RollCall { get; set; }
    public List<RollCallDetailDto> Absences { get; set; }
  }
}

