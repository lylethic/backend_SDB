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
    public async Task<IActionResult> GetTeachers(int pageNumber = 1, int pageSize = 50)
    {
      var teachers = await _teacherRepo.GetTeachers(pageNumber, pageSize);

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
          data = teachers.Datas
        });
      }

      return StatusCode(500, new
      {
        statusCode = teachers.StatusCode,
        message = teachers.Message,
      });
    }

    [HttpGet, Route("get-teachers-by-school")]
    public async Task<IActionResult> GetTeachersBySchool(int pageNumber, int pageSize, int schoolId)
    {
      var teachers = await _teacherRepo.GetTeachersBySchool(pageNumber, pageSize, schoolId);

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
          data = teachers.Datas
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
          data = teacher.Datas
        });
      }

      return StatusCode(500, new
      {
        statusCode = teacher.StatusCode,
        message = teacher.Message,
      });
    }

    [HttpGet, Route("count-amount-of-teachers/{id}")]
    public async Task<IActionResult> GetCountAmountOfTeachers(int id)
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
          data = teacher.Datas
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
          data = result.Datas
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
          data = result.Datas
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
    public async Task<IActionResult> ImportExcelFile(IFormFile file)
    {
      try
      {
        var result = await _teacherRepo.ImportExcelFile(file);
        if (result.Contains("Successfully"))
        {
          return Ok(result);
        }

        return BadRequest(result);
      }
      catch (Exception ex)
      {
        throw new Exception($"Error: {ex.Message}");
      }
    }
  }
}
