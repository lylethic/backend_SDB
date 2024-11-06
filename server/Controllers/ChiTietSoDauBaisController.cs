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
      var result = await _detail.GetChiTietSoDauBais(pageNumber, pageSize);
      if (result.StatusCode == 200)
      {
        return Ok(new
        {
          message = result.Message,
          data = result.ListSoDauBaiDto
        });
      }

      return StatusCode(500, new
      {
        message = result.Message,
      });

    }

    [HttpGet, Route("chi-tiet")]
    public async Task<IActionResult> GetChiTietSoDauBaisByWeek(int pageNumber, int pageSize, int weekId)
    {
      var result = await _detail.GetChiTietSoDauBaisByWeek(pageNumber, pageSize, weekId);
      if (result.StatusCode == 200)
      {
        return Ok(new
        {
          message = result.Message,
          data = result.ListChiTietBody
        });
      }

      if (result.StatusCode == 404)
      {
        return NotFound(new
        {
          message = result.Message,
        });
      }

      return StatusCode(500, new
      {
        message = result.Message,
      });

    }


    // GET api/<ChiTietSoDauBaisController>/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetByIdChiTietSoDauBai(int id)
    {
      var result = await _detail.GetChiTietSoDauBai(id);
      if (result.StatusCode == 200)
      {
        return Ok(new
        {
          message = result.Message,
          data = result.SoDauBaiDto
        });
      }

      if (result.StatusCode == 404)
      {
        return BadRequest(new
        {
          message = result.Message,
        });
      }

      return StatusCode(500, new
      {
        message = result.Message,
      });
    }

    // GET:: 
    /// <summary>
    /// Get theo tuan de lay xep loai
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
    [HttpGet("get-chi-tiet-week")]
    public async Task<IActionResult> GetChiTiet_Week(int id)
    {
      var result = await _detail.GetChiTiet_Week_XepLoai(id);

      if (result.StatusCode == 200)
      {
        return Ok(new
        {
          message = result.Message,
          data = result.ListChiTiet_WeekResData
        });
      }

      if (result.StatusCode == 404)
      {
        return BadRequest(new
        {
          message = result.Message,
        });
      }

      return StatusCode(500, new
      {
        message = result.Message,
      });
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
    [HttpGet("get-chi-tiet-bia-class-teacher")]
    public async Task<IActionResult> GetChiTiet_Bia_Class_Teacher(int id)
    {
      var result = await _detail.GetChiTiet_Bia_Class_Teacher(id);
      if (result.StatusCode == 200)
      {
        return Ok(new
        {
          message = result.Message,
          data = result.ChiTietAndBiaSoDauBaiRes
        });
      }

      if (result.StatusCode == 404)
      {
        return BadRequest(new
        {
          message = result.Message,
        });
      }

      return StatusCode(500, new
      {
        message = result.Message,
      });

    }

    /// <summary>
    /// Get Chi Tiet by schoolId, weekId, biaId, classId 
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpGet("get-chi-tiet-by-school")]
    public async Task<IActionResult> GetChiTietBySchool([FromQuery] int schoolId, [FromQuery] int weekId, [FromQuery] int biaId, [FromQuery] int classId, [FromQuery] int pageNumber, [FromQuery] int pageSize)
    {
      var result = await _detail.GetChiTietBySchool(schoolId, weekId, biaId, classId, pageNumber, pageSize);

      if (result.StatusCode == 200)
      {
        return Ok(new
        {
          message = result.Message,
          data = result.ListChiTietSDBResData
        });
      }

      if (result.StatusCode == 404)
      {
        return BadRequest(new
        {
          message = result.Message,
        });
      }

      return StatusCode(500, new
      {
        message = result.Message,
      });
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

      if (result.StatusCode == 200)
      {
        return Ok(new
        {
          message = result.Message,
          data = result.SoDauBaiDto
        });
      }

      if (result.StatusCode == 404)
      {
        return NotFound(new
        {
          message = result.Message,
        });
      }

      return StatusCode(500, new
      {
        message = result.Message,
      });
    }

    // PUT api/<ChiTietSoDauBaisController>/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateChiTietSoDauBai(int id, ChiTietSoDauBaiDto model)
    {
      var result = await _detail.UpdateChiTietSoDauBai(id, model);

      if (result.StatusCode == 200)
      {
        return Ok(new
        {
          message = result.Message,
        });
      }

      if (result.StatusCode == 404)
      {
        return NotFound(new
        {
          message = result.Message,
        });
      }

      return StatusCode(500, new
      {
        message = result.Message,
      });
    }

    // DELETE api/<ChiTietSoDauBaisController>/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteChiTietSoDauBai(int id)
    {
      var result = await _detail.DeleteChiTietSoDauBai(id);

      if (result.StatusCode == 200)
      {
        return Ok(new
        {
          message = result.Message,
        });
      }

      if (result.StatusCode == 404)
      {
        return NotFound(new
        {
          message = result.Message,
        });
      }

      return StatusCode(500, new
      {
        message = result.Message,
      });
    }

    [Authorize(Policy = "SuperAdminAndAdmin")]
    [HttpDelete("bulkdelete")]
    public async Task<IActionResult> BulkDelete(List<int> ids)
    {
      var result = await _detail.BulkDelete(ids);

      if (result.StatusCode == 200)
      {
        return Ok(new
        {
          message = result.Message,
        });
      }

      if (result.StatusCode == 400)
      {
        return BadRequest(new
        {
          message = result.Message,
        });
      }

      if (result.StatusCode == 404)
      {
        return NotFound(new
        {
          message = result.Message,
        });
      }

      return StatusCode(500, new
      {
        message = result.Message,
      });
    }

    [Authorize(Policy = "SuperAdminAndAdmin")]
    [HttpPost("upload")]
    public async Task<IActionResult> ImportExcelFile(IFormFile file)
    {
      var result = await _detail.ImportExcel(file);

      if (result.StatusCode == 200)
      {
        return Ok(new
        {
          message = result.Message,
        });
      }

      if (result.StatusCode == 400)
      {
        return BadRequest(new
        {
          message = result.Message,
        });
      }

      return StatusCode(500, new
      {
        message = result.Message,
      });
    }

    [Authorize(Policy = "SuperAdminAndAdmin")]
    [HttpPost("export")]
    public async Task<IActionResult> ExportChiTietSoDauBaiToExcel(int weekId, int classId)
    {
      var exportFolder = Path.Combine(Directory.GetCurrentDirectory(), "Exports");

      // Ensure the directory exists
      if (!Directory.Exists(exportFolder))
      {
        Directory.CreateDirectory(exportFolder);
      }

      var filePath = Path.Combine(exportFolder, $"ChiTietSoDauBaiTuan${weekId}.xlsx");

      var result = await _detail.ExportChiTietSoDauBaiToExcel(weekId, classId, filePath);

      if (result.StatusCode != 200)
      {
        return BadRequest(result);
      }

      // Return file for download after successful export
      var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
      var fileName = $"ChiTietSoDauBaiTuan${weekId}.xlsx";

      return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
    }
  }
}
