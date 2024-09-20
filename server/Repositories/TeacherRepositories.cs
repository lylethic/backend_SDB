using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using server.Data;
using server.Dtos;
using server.IService;
using server.Models;
using System.Text;

namespace server.Repositories
{
  public class TeacherRepositories : IService.ITeacher
  {
    private readonly SoDauBaiContext _context;

    public TeacherRepositories(SoDauBaiContext context)
    {
      this._context = context;
    }

    public async Task<Data_Response<TeacherDto>> CreateTeacher(TeacherDto model)
    {
      try
      {
        var findTeacher = "SELECT * FROM Teacher WHERE teacherId = @id";

        var taecher = await _context.Teachers
          .FromSqlRaw(findTeacher, new SqlParameter("@id", model.TeacherId))
          .FirstOrDefaultAsync();

        if (taecher is not null)
        {
          return new Data_Response<TeacherDto>(409, "Teacher already exists");
        }

        var sqlInsert = @"INSERT INTO Teacher (AccountId, SchoolId, Fullname, DateOfBirth, Gender, Address, Status) 
                          VALUES (@AccountId, @SchoolId, @Fullname ,@DateOfBirth, @Gender, @Address, @Status);
                          SELECT CAST(SCOPE_IDENTITY() as int);";

        var insert = await _context.Database.ExecuteSqlRawAsync(sqlInsert,
          new SqlParameter("@AccountId", model.AccountId),
          new SqlParameter("@SchoolId", model.SchoolId),
          new SqlParameter("@Fullname", model.Fullname),
          new SqlParameter("@DateOfBirth", model.DateOfBirth),
          new SqlParameter("@Gender", model.Gender),
          new SqlParameter("@Address", model.Address),
          new SqlParameter("@Status", model.Status)
        );

        var result = new TeacherDto
        {
          TeacherId = insert,
          AccountId = model.AccountId,
          SchoolId = model.SchoolId,
          Fullname = model.Fullname,
          DateOfBirth = model.DateOfBirth,
          Gender = model.Gender,
          Address = model.Address,
          Status = model.Status,
        };

        return new Data_Response<TeacherDto>(200, result);
      }
      catch (Exception ex)
      {
        return new Data_Response<TeacherDto>(200, $"Server error: {ex.Message}");
      }
    }

    public async Task<Data_Response<TeacherDto>> DeleteTeacher(int id)
    {
      try
      {
        var findTeacher = "SELECT * FROM Teacher WHERE TeacherId = @id";
        var teacher = await _context.Teachers
          .FromSqlRaw(findTeacher, new SqlParameter("@id", id))
          .FirstOrDefaultAsync();

        if (teacher is null)
        {
          return new Data_Response<TeacherDto>(404, "Teacher not found");
        }

        var deleteQuery = "DELETE FROM Teacher WHERE TeacherId = @id";
        await _context.Database.ExecuteSqlRawAsync(deleteQuery, new SqlParameter("@id", id));
        return new Data_Response<TeacherDto>(200, "Deleted");
      }
      catch (Exception ex)
      {
        return new Data_Response<TeacherDto>(500, $"Server error: {ex.Message}");
      }
    }

    public async Task<Data_Response<TeacherDto>> GetTeacher(int id)
    {
      try
      {
        var findTeacher = "SELECT * FROM Teacher WHERE TeacherId = @id";
        var teacher = await _context.Teachers
          .FromSqlRaw(findTeacher, new SqlParameter("@id", id))
          .FirstOrDefaultAsync();

        if (teacher is null)
        {
          return new Data_Response<TeacherDto>(404, "Teacher not found");
        }


        var result = new TeacherDto
        {
          TeacherId = id,
          AccountId = teacher.AccountId,
          SchoolId = teacher.SchoolId,
          Fullname = teacher.Fullname,
          DateOfBirth = teacher.DateOfBirth,
          Gender = teacher.Gender,
          Address = teacher.Address,
          Status = teacher.Status,
        };

        return new Data_Response<TeacherDto>(200, result);
      }
      catch (Exception ex)
      {
        return new Data_Response<TeacherDto>(500, $"Server error: {ex.Message}");
      }
    }

    public async Task<List<TeacherDto>> GetTeachers()
    {
      try
      {
        var query = "SELECT * FROM Teacher";
        var teachers = await _context.Teachers.FromSqlRaw(query).ToListAsync();

        if (teachers is null)
        {
          throw new Exception("No content");
        }

        var result = teachers.Select(x => new TeacherDto
        {
          TeacherId = x.TeacherId,
          AccountId = x.AccountId,
          SchoolId = x.SchoolId,
          Fullname = x.Fullname,
          DateOfBirth = x.DateOfBirth,
          Gender = x.Gender,
          Address = x.Address,
          Status = x.Status,
        }).ToList();

        return result;
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
        throw;
      }
    }

    public async Task<Data_Response<TeacherDto>> UpdateTeacher(int id, TeacherDto model)
    {
      try
      {
        // Check if the teacher exists in the database
        var findQuery = "SELECT * FROM Teacher WHERE TeacherId = @id";
        var existingTeacher = await _context.Teachers
            .FromSqlRaw(findQuery, new SqlParameter("@id", id))
            .FirstOrDefaultAsync();

        if (existingTeacher == null)
        {
          return new Data_Response<TeacherDto>(404, "Teacher not found");
        }

        // Build update query dynamically based on non-null fields
        var queryBuilder = new StringBuilder("UPDATE Teacher SET ");
        var parameters = new List<SqlParameter>();

        if (!string.IsNullOrEmpty(model.Fullname))
        {
          queryBuilder.Append("Fullname = @Fullname, ");
          parameters.Add(new SqlParameter("@Fullname", model.Fullname));
        }

        if (model.AccountId != 0)
        {
          queryBuilder.Append("AccountId = @AccountId, ");
          parameters.Add(new SqlParameter("@AccountId", model.AccountId));
        }

        if (model.SchoolId != 0)
        {
          queryBuilder.Append("SchoolId = @SchoolId, ");
          parameters.Add(new SqlParameter("@SchoolId", model.SchoolId));
        }

        if (model.DateOfBirth != default)
        {
          queryBuilder.Append("DateOfBirth = @DateOfBirth, ");
          parameters.Add(new SqlParameter("@DateOfBirth", model.DateOfBirth));
        }

        if (model.Gender != default)
        {
          queryBuilder.Append("Gender = @Gender, ");
          parameters.Add(new SqlParameter("@Gender", model.Gender));
        }

        if (!string.IsNullOrEmpty(model.Address))
        {
          queryBuilder.Append("Address = @Address, ");
          parameters.Add(new SqlParameter("@Address", model.Address));
        }

        queryBuilder.Append("Status = @Status ");
        parameters.Add(new SqlParameter("@Status", model.Status));

        // Remove trailing comma from the query if necessary
        if (queryBuilder[queryBuilder.Length - 2] == ',')
        {
          queryBuilder.Length -= 2;
        }

        queryBuilder.Append(" WHERE TeacherId = @id");
        parameters.Add(new SqlParameter("@id", id));

        // Execute the update query
        var updateQuery = queryBuilder.ToString();
        await _context.Database.ExecuteSqlRawAsync(updateQuery, parameters.ToArray());

        return new Data_Response<TeacherDto>(200, "Teacher updated successfully");
      }
      catch (Exception ex)
      {
        return new Data_Response<TeacherDto>(500, $"Server Error: {ex.Message}");
      }
    }

  }
}
