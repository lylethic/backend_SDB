using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using server.Dtos;
using server.IService;

namespace server.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  [Authorize]
  public class TeachersController : ControllerBase
  {
    private readonly ITeacher _teacherRepo;

    public TeachersController(ITeacher teacherRepo)
    {
      this._teacherRepo = teacherRepo;
    }

    // GET: api/Teachers
    [HttpGet]
    public async Task<IActionResult> GetTeachers([FromQuery] QueryObject? queryObject)
    {
      var teachers = await _teacherRepo.GetTeachers(queryObject);

      if (teachers.StatusCode == 404)
      {
        return NotFound(new
        {
          statusCode = teachers.StatusCode,
          message = teachers.Message,
        });
      }

      if (teachers.StatusCode == 200)
      {
        return Ok(new
        {
          statusCode = teachers.StatusCode,
          message = teachers.Message,
          data = teachers.TeacherListDetails
        });
      }

      return StatusCode(500, new
      {
        statusCode = teachers.StatusCode,
        message = teachers.Message,
      });
    }

    [HttpGet, Route("get-teachers-by-school")]
    public async Task<IActionResult> GetTeachersBySchool([FromQuery] QueryObject? queryObject, [FromQuery] int schoolId)
    {
      var teachers = await _teacherRepo.GetTeachersBySchool(queryObject, schoolId);

      if (teachers.StatusCode == 400)
      {
        return BadRequest(new
        {
          statusCode = teachers.StatusCode,
          message = teachers.Message,
        });
      }

      if (teachers.StatusCode == 404)
      {
        return NotFound(new
        {
          statusCode = teachers.StatusCode,
          message = teachers.Message,
        });
      }

      if (teachers.StatusCode == 200)
      {
        return Ok(new
        {
          statusCode = teachers.StatusCode,
          message = teachers.Message,
          data = teachers.TeacherListDetails
        });
      }

      return StatusCode(500, new
      {
        statusCode = teachers.StatusCode,
        message = teachers.Message,
      });
    }

    [HttpGet, Route("teachers-by-school")]
    public async Task<IActionResult> GetTeachersBySchool([FromQuery] int schoolId)
    {
      var teachers = await _teacherRepo.GetTeachersBySchool(schoolId);

      if (teachers.StatusCode == 404)
      {
        return NotFound(new
        {
          statusCode = teachers.StatusCode,
          message = teachers.Message,
        });
      }

      if (teachers.StatusCode == 400)
      {
        return BadRequest(new
        {
          statusCode = teachers.StatusCode,
          message = teachers.Message,
        });
      }

      if (teachers.StatusCode == 200)
      {
        return Ok(new
        {
          statusCode = teachers.StatusCode,
          message = teachers.Message,
          data = teachers.TeacherListDetails
        });
      }

      return StatusCode(500, new
      {
        statusCode = teachers.StatusCode,
        message = teachers.Message,
      });
    }

    // GET: api/Teachers/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetTeacher(int id)
    {
      var teacher = await _teacherRepo.GetTeacher(id);

      if (teacher.StatusCode == 404)
      {
        return NotFound(new
        {
          statusCode = teacher.StatusCode,
          message = teacher.Message,
        });
      }

      if (teacher.StatusCode == 200)
      {
        return Ok(new
        {
          statusCode = teacher.StatusCode,
          message = teacher.Message,
          data = teacher.TeacherDetail
        });
      }

      return StatusCode(500, new
      {
        statusCode = teacher.StatusCode,
        message = teacher.Message,
      });
    }

    [HttpGet("teacher-to-update/{id}")]
    public async Task<IActionResult> GetTeacherToUpdate(int id)
    {
      var teacher = await _teacherRepo.GetTeacherToUpdate(id);

      if (teacher.StatusCode == 404)
      {
        return NotFound(new
        {
          statusCode = teacher.StatusCode,
          message = teacher.Message,
        });
      }

      if (teacher.StatusCode == 200)
      {
        return Ok(new
        {
          statusCode = teacher.StatusCode,
          message = teacher.Message,
          data = teacher.Data
        });
      }

      return StatusCode(500, new
      {
        statusCode = teacher.StatusCode,
        message = teacher.Message,
      });
    }


    // amount of teacher by schoolId is optional
    [HttpGet("count-amount-of-teachers")]
    public async Task<IActionResult> GetCountAmountOfTeachers(int? id = null)
    {
      var result = await _teacherRepo.GetCountTeachersBySchool(id);
      return Ok(result);
    }

    // PUT: api/Teachers/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTeacher(int id, TeacherDto model)
    {
      var teacher = await _teacherRepo.UpdateTeacher(id, model);

      if (teacher.StatusCode == 404)
      {
        return NotFound(new
        {
          statusCode = teacher.StatusCode,
          message = teacher.Message,
        });
      }

      if (teacher.StatusCode == 200)
      {
        return Ok(new
        {
          statusCode = teacher.StatusCode,
          message = teacher.Message,
        });
      }

      return StatusCode(500, new
      {
        statusCode = teacher.StatusCode,
        message = teacher.Message,
      });
    }

    // POST: api/Teachers
    [Authorize(Policy = "SuperAdminAndAdmin")]
    [HttpPost]
    public async Task<IActionResult> PostTeacher(TeacherDto model)
    {
      var result = await _teacherRepo.CreateTeacher(model);

      if (result.StatusCode == 409)
      {
        return StatusCode(409, new
        {
          statusCode = result.StatusCode,
          message = result.Message,
        });
      }

      if (result.StatusCode == 200)
      {
        return Ok(new
        {
          statusCode = result.StatusCode,
          message = result.Message,
          data = result.Data
        });
      }

      return StatusCode(500, new
      {
        statusCode = result.StatusCode,
        message = result.Message,
      });
    }

    // DELETE: api/Teachers/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTeacher(int id)
    {
      var result = await _teacherRepo.DeleteTeacher(id);

      if (result.StatusCode == 404)
      {
        return NotFound(new
        {
          statusCode = result.StatusCode,
          message = result.Message,
        });
      }

      if (result.StatusCode == 200)
      {
        return Ok(new
        {
          statusCode = result.StatusCode,
          message = result.Message,
        });
      }

      return StatusCode(500, new
      {
        statusCode = result.StatusCode,
        message = result.Message,
      });
    }

    [Authorize(Policy = "SuperAdminAndAdmin")]
    [HttpDelete("bulkdelete")]
    public async Task<IActionResult> BulkDelete(List<int> ids)
    {
      var teacher = await _teacherRepo.BulkDelete(ids);

      if (teacher.StatusCode != 200)
      {
        return BadRequest(teacher);
      }

      return Ok(teacher);
    }

    [Authorize(Policy = "SuperAdminAndAdmin")]
    [HttpPost, Route("upload")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> ImportExcelFile(IFormFile file)
    {
      var result = await _teacherRepo.ImportExcelFile(file);
      if (result.StatusCode == 200)
      {
        return Ok(new
        {
          status = 200,
          message = result.Message
        });
      }

      if (result.StatusCode == 400)
        return StatusCode(500, new
        {
          statusCode = result.StatusCode,
          message = result.Message,
        });

      return StatusCode(500, new
      {
        statusCode = result.StatusCode,
        message = result.Message,
      });
    }

    [Authorize(Policy = "SuperAdminAndAdmin")]
    [HttpGet("search")]
    public async Task<IActionResult> SearchTeacher([FromQuery] QueryObjects? queryObject)
    {

      queryObject ??= new QueryObjects();
      if (queryObject.PageNumber < 1 || queryObject.PageSize < 1)
      {
        return BadRequest("Page number and page size must be positive integers.");
      }
      var results = await _teacherRepo.SearchTeacher(queryObject);

      if (results.StatusCode == 404)
      {
        return NotFound(new
        {
          statusCode = results.StatusCode,
          message = results.Message,
        });
      }
      if (results.StatusCode == 200)
        return Ok(new
        {
          statusCode = results.StatusCode,
          message = results.Message,
          totalResults = results.TotalCount,
          data = results.TeacherListDetails
        });

      return StatusCode(500, new
      {
        statusCode = results.StatusCode,
        message = results.Message,
      });
    }

  }
}
