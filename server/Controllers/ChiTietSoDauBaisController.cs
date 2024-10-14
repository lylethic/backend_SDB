using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using server.Dtos;
using server.IService;

namespace server.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  [Authorize]
  public class ChiTietSoDauBaisController : ControllerBase
  {
    readonly IChiTietSoDauBai _detail;

    public ChiTietSoDauBaisController(IChiTietSoDauBai detail)
    {
      this._detail = detail;
    }

    // GET: api/<ChiTietSoDauBaisController>
    [HttpGet]
    public async Task<IActionResult> GetAllRecords(int pageNumber = 1, int pageSize = 50)
    {
      try
      {
        var result = await _detail.GetChiTietSoDauBais(pageNumber, pageSize);

        if (result is null)
        {
          return NotFound("Khong tim thay ket qua");
        }

        return Ok(result);
      }
      catch (Exception ex)
      {
        return StatusCode(500, $"Error: {ex.Message}");
      }
    }

    // GET api/<ChiTietSoDauBaisController>/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetByIdChiTietSoDauBai(int id)
    {
      try
      {
        var result = await _detail.GetChiTietSoDauBai(id);
        if (result is null)
        {
          return BadRequest(result);
        }

        return Ok(result);
      }
      catch (Exception ex)
      {
        return StatusCode(500, $"Error: {ex.Message}");
      }
    }

    // GET:: 
    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    /* {
      "statusCode": 200,
      "message": "",
      "data": {
        "chiTietSoDauBaiId": 2,
        "weekId": 1,
        "weekName": "Tuần 1",
        "status": true,
        "xepLoaiId": 1,
        "tenXepLoai": "A",
        "soDiem": 10
      }
    }
   } */
    [HttpGet("GetChiTiet_Week")]
    public async Task<IActionResult> GetChiTiet_Week(int id)
    {
      var result = await _detail.GetChiTiet_Week_XepLoai(id);

      if (result is null)
      {
        return BadRequest(result);
      }

      return Ok(result);
    }

    /// <summary>
    /// Connect 4 table: Chitietsodaubai + BiaSodaubai + Class + Teacher
    /// <param name="id">Id cua chiTietSoDauBai</param>
    /// <returns>
    /// {
    //  "statusCode": 200,
    //  "message": "",
    //  "data": {
    //    "chiTietSoDauBaiId": 1,
    //    "biaSoDauBaiId": 1,
    //    "classId": 1,
    //    "schoolId": 1,
    //    "academicyearId": 0,
    //    "className": "Lớp 10A1",
    //    "teacherId": 29,
    //    "teacherFullName": "Phùng Minh Tú"
    //  }
    //}
    /// </returns>
    /// </summary>
    [HttpGet("getChiTiet_Bia_Class_Teacher")]
    public async Task<IActionResult> GetChiTiet_Bia_Class_Teacher(int id)
    {
      var result = await _detail.GetChiTiet_Bia_Class_Teacher(id);
      if (result is null)
      {
        return BadRequest(result);
      }

      return Ok(result);
    }

    /// <summary>
    /// Get Chi Tiet by schoolId, weekId, biaId, classId
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpGet("GetChiTietBySchool")]
    public async Task<IActionResult> GetChiTietBySchool([FromQuery] int schoolId, [FromQuery] int weekId, [FromQuery] int biaId, [FromQuery] int classId, [FromQuery] int pageNumber, [FromQuery] int pageSize)
    {
      var result = await _detail.GetChiTietBySchool(schoolId, weekId, biaId, classId, pageNumber, pageSize);

      if (result is null)
      {
        return BadRequest(result);
      }

      return Ok(result);
    }

    // POST api/<ChiTietSoDauBaisController>
    /*
    {
      "chiTietSoDauBaiId": 0,
      "biaSoDauBaiId": 1,
      "semesterId": 1,
      "weekId": 1,
      "subjectId": 13,
      "classificationId": 1,
      "daysOfTheWeek": "Thứ 2",
      "thoiGian": "2024-10-07T03:50:30.779Z",
      "buoiHoc": "Sáng",
      "tietHoc": 1,
      "lessonContent": "Đạo hàm",
      "attend": 40,
      "noteComment": "Tốt",
      "createdBy": 29,
      "createdAt": "2024-10-07T03:50:30.779Z",
      "updatedAt": "2024-10-07T03:50:30.779Z"
    }
     */

    [HttpPost("create")]
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
    public async Task<IActionResult> UpdateChiTietSoDauBai(int id, ChiTietSoDauBaiDto model)
    {
      try
      {
        var result = await _detail.UpdateChiTietSoDauBai(id, model);

        if (result is null)
        {
          return BadRequest(result);
        }

        return Ok(result);
      }
      catch (Exception ex)
      {
        return StatusCode(500, $"Error: {ex.Message}");
      }
    }

    // DELETE api/<ChiTietSoDauBaisController>/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteChiTietSoDauBai(int id)
    {
      try
      {
        var result = await _detail.DeleteChiTietSoDauBai(id);
        if (result is null)
        {
          return BadRequest(result);
        }

        return Ok(result);
      }
      catch (Exception ex)
      {
        return StatusCode(500, $"Error: {ex.Message}");
      }
    }

    [Authorize(Policy = "SuperAdminAndAdmin")]
    [HttpDelete("bulkdelete")]
    public async Task<IActionResult> BulkDelete(List<int> ids)
    {
      try
      {
        var result = await _detail.BulkDelete(ids);
        if (result is null)
        {
          return BadRequest(result);
        }

        return Ok(result);
      }
      catch (Exception ex)
      {
        return StatusCode(500, $"Error: {ex.Message}");
      }
    }

    [Authorize(Policy = "SuperAdminAndAdmin")]
    [HttpPost("upload")]
    public async Task<IActionResult> ImportExcelFile(IFormFile file)
    {
      var result = await _detail.ImportExcel(file);

      if (result.Contains("Successfully"))
      {
        return Ok(result);
      }

      return BadRequest(result);
    }

  }
}
