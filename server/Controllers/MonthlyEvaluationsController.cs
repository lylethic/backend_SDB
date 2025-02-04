using Microsoft.AspNetCore.Mvc;

namespace server.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class MonthlyEvaluationsController : ControllerBase
  {
    // GET: api/<MonthlyEvaluationsController>
    [HttpGet]
    public IEnumerable<string> Get()
    {
      return new string[] { "value1", "value2" };
    }

    // GET api/<MonthlyEvaluationsController>/5
    [HttpGet("{id}")]
    public string Get(int id)
    {
      return "value";
    }

    // POST api/<MonthlyEvaluationsController>
    [HttpPost]
    public void Post([FromBody] string value)
    {
    }

    // PUT api/<MonthlyEvaluationsController>/5
    [HttpPut("{id}")]
    public void Put(int id, [FromBody] string value)
    {
    }

    // DELETE api/<MonthlyEvaluationsController>/5
    [HttpDelete("{id}")]
    public void Delete(int id)
    {
    }
  }
}
