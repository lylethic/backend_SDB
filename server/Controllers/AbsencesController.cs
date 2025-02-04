using Microsoft.AspNetCore.Mvc;

namespace server.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class AbsencesController : ControllerBase
  {
    // GET: api/<AbsencesController>
    [HttpGet]
    public IEnumerable<string> Get()
    {
      return new string[] { "value1", "value2" };
    }

    // GET api/<AbsencesController>/5
    [HttpGet("{id}")]
    public string Get(int id)
    {
      return "value";
    }

    // POST api/<AbsencesController>
    [HttpPost]
    public void Post([FromBody] string value)
    {
    }

    // PUT api/<AbsencesController>/5
    [HttpPut("{id}")]
    public void Put(int id, [FromBody] string value)
    {
    }

    // DELETE api/<AbsencesController>/5
    [HttpDelete("{id}")]
    public void Delete(int id)
    {
    }
  }
}
