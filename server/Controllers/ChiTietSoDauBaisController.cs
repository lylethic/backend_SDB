using Microsoft.AspNetCore.Mvc;
using server.Dtos;
using server.IService;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace server.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  //[Authorize("Admin, Teacher")]
  public class ChiTietSoDauBaisController : ControllerBase
  {
    readonly IChiTietSoDauBai _detail;

    public ChiTietSoDauBaisController(IChiTietSoDauBai detail)
    {
      this._detail = detail;
    }

    // GET: api/<ChiTietSoDauBaisController>
    [HttpGet]
    public async Task<IActionResult> GetAllRecords()
    {
      return null;
    }

    // GET api/<ChiTietSoDauBaisController>/5
    [HttpGet("{id}")]
    public string Get(int id)
    {
      return "value";
    }

    // POST api/<ChiTietSoDauBaisController>
    //    {
    //  "chiTietSoDauBaiId": 0,
    //  "biaSoDauBaiId": 1,
    //  "semesterId": 1,
    //  "weekId": 1,
    //  "subjectId": 13,
    //  "classificationId": 1,
    //  "daysOfTheWeek": "Thứ 2",
    //  "thoiGian": "2024-10-07T03:50:30.779Z",
    //  "buoiHoc": "Sáng",
    //  "tietHoc": 1,
    //  "lessonContent": "Đạo hàm",
    //  "attend": 40,
    //  "noteComment": "Tốt",
    //  "createdBy": 29,
    //  "createdAt": "2024-10-07T03:50:30.779Z",
    //  "updatedAt": "2024-10-07T03:50:30.779Z"
    //}
    [HttpPost]
    public async Task<IActionResult> CreateChiTietSoDauBai(ChiTietSoDauBaiDto model)
    {
      var result = await _detail.CreateChiTietSoDauBai(model);

      if (result is null)
      {
        return BadRequest(result);

      }
      return Ok(result);
    }

    // PUT api/<ChiTietSoDauBaisController>/5
    [HttpPut("{id}")]
    public void Put(int id, [FromBody] string value)
    {
    }

    // DELETE api/<ChiTietSoDauBaisController>/5
    [HttpDelete("{id}")]
    public void Delete(int id)
    {
    }
  }
}
