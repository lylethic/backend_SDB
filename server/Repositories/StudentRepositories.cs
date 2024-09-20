using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using server.Data;
using server.Dtos;
using server.IService;
using server.Models;
using System.Collections.Generic;
using System.Text;


namespace server.Repositories
{
  public class StudentRepositories : IStudent
  {
    private readonly Data.SoDauBaiContext _context;

    public StudentRepositories(SoDauBaiContext context) { this._context = context; }

    public async Task<Data_Response<StudentDto>> CreateStudent(StudentDto model)
    {
      try
      {
        var find = "SELECT * FROM STUDENT WHERE StudentId = @id";

        var student = await _context.Students
          .FromSqlRaw(find, new SqlParameter("@id", model.StudentId))
          .FirstOrDefaultAsync();

        if (student is not null)
        {
          return new Data_Response<StudentDto>(409, "Student already exists");
        }

        var sqlInsert = @"INSERT INTO STUDENT (ClassId, GradeId, AccountId, RoleId, ShoolId, Fullname, Status, Description) 
                          VALUES (@ClassId, @GradeId, @AccountId, @RoleId, @ShoolId, @Fullname, @Status, @Description);
                          SELECT CAST(SCOPE_IDENTITY() as int);";

        var insert = await _context.Database.ExecuteSqlRawAsync(sqlInsert,
          new SqlParameter("@ClassId", model.ClassId),
          new SqlParameter("@GradeId", model.GradeId),
          new SqlParameter("@AccountId", model.AccountId),
          new SqlParameter("@RoleId", model.RoleId),
          new SqlParameter("@ShoolId", model.SchoolId),
          new SqlParameter("@Fullname", model.Fullname),
          new SqlParameter("@Status", model.Status),
          new SqlParameter("@Description", model.Description)
        );

        var result = new StudentDto
        {
          StudentId = insert,
          ClassId = model.ClassId,
          GradeId = model.GradeId,
          AccountId = model.AccountId,
          RoleId = model.RoleId,
          SchoolId = model.SchoolId,
          Fullname = model.Fullname,
          Status = model.Status,
          Description = model.Description,
        };

        return new Data_Response<StudentDto>(200, result);
      }
      catch (Exception ex)
      {
        return new Data_Response<StudentDto>(200, $"Server error: {ex.Message}");
      }
    }

    public async Task<Data_Response<StudentDto>> DeleteStudent(int id)
    {
      try
      {
        var findRole = "SELECT * FROM STUDENT WHERE StudentId = @id";
        var role = await _context.Students
          .FromSqlRaw(findRole, new SqlParameter("@id", id))
          .FirstOrDefaultAsync();

        if (role is null)
        {
          return new Data_Response<StudentDto>(404, "Student not found");
        }

        var deleteQuery = "DELETE FROM Student WHERE StudentId = @id";
        await _context.Database.ExecuteSqlRawAsync(deleteQuery, new SqlParameter("@id", id));
        return new Data_Response<StudentDto>(200, "Deleted");
      }
      catch (Exception ex)
      {
        return new Data_Response<StudentDto>(500, $"Server error: {ex.Message}");
      }
    }

    public async Task<Data_Response<StudentDto>> GetStudent(int id)
    {
      try
      {
        var query = "SELECT * FROM Student WHERE StudentId = @id";
        var student = await _context.Students
          .FromSqlRaw(query, new SqlParameter("@id", id))
          .FirstOrDefaultAsync();

        if (student is null)
        {
          return new Data_Response<StudentDto>(404, "Student not found");
        }

        var result = new StudentDto
        {
          StudentId = id,
          ClassId = student.ClassId,
          GradeId = student.GradeId,
          AccountId = student.AccountId,
          RoleId = student.RoleId,
          SchoolId = student.ShoolId,
          Fullname = student.Fullname,
          Status = student.Status,
          Description = student.Description,
        };

        return new Data_Response<StudentDto>(200, result);
      }
      catch (Exception ex)
      {
        return new Data_Response<StudentDto>(500, $"Server error: {ex.Message}");
      }
    }

    public async Task<List<StudentDto>> GetStudents()
    {
      try
      {
        var query = "SELECT * FROM Student";
        var roles = await _context.Students.FromSqlRaw(query).ToListAsync();

        var result = roles.Select(x => new StudentDto
        {
          StudentId = x.StudentId,
          ClassId = x.ClassId,
          GradeId = x.GradeId,
          AccountId = x.AccountId,
          RoleId = x.RoleId,
          SchoolId = x.ShoolId,
          Fullname = x.Fullname,
          Status = x.Status,
          Description = x.Description,
        }).ToList();

        return result;
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
        throw;
      }
    }

    public async Task<Data_Response<StudentDto>> UpdateStudent(int id, StudentDto model)
    {
      try
      {
        var find = "SELECT * FROM Student WHERE StudentId = @id";
        var exists = await _context.Students
          .FromSqlRaw(find, new SqlParameter("@id", model.StudentId))
          .FirstOrDefaultAsync();

        if (exists is null)
        {
          return new Data_Response<StudentDto>(404, "Role not found");
        }

        var queryBuilder = new StringBuilder("UPDATE Student SET ");
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

        if (model.StudentId != 0)
        {
          queryBuilder.Append("AccountId = @AccountId, ");
          parameters.Add(new SqlParameter("@AccountId", model.AccountId));
        }

        if (String.IsNullOrEmpty(model.Description))
        {
          queryBuilder.Append("Description = @Description, ");
          parameters.Add(new SqlParameter("@Description", model.Description));
        }

        queryBuilder.Append("Status = @Status, ");
        parameters.Add(new SqlParameter("@Status", model.Status));


        // Remove the last comma and space
        if (queryBuilder.Length > 0)
        {
          queryBuilder.Length -= 2;
        }

        queryBuilder.Append(" WHERE StudentId = @id");
        parameters.Add(new SqlParameter("@id", id));

        var updateQuery = queryBuilder.ToString();
        await _context.Database.ExecuteSqlRawAsync(updateQuery, parameters.ToArray());

        return new Data_Response<StudentDto>(200, "Updated");
      }
      catch (Exception ex)
      {
        return new Data_Response<StudentDto>(500, $"Server Error: {ex.Message}");
      }
    }
  }
}
