using ExcelDataReader;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using server.Data;
using server.Dtos;
using server.IService;
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

        var sqlInsert = @"INSERT INTO STUDENT (ClassId, GradeId, AccountId, Fullname, Status, Description) 
                          VALUES (@ClassId, @GradeId, @AccountId, @Fullname, @Status, @Description);
                          SELECT CAST(SCOPE_IDENTITY() as int);";

        var insert = await _context.Database.ExecuteSqlRawAsync(sqlInsert,
          new SqlParameter("@ClassId", model.ClassId),
          new SqlParameter("@GradeId", model.GradeId),
          new SqlParameter("@AccountId", model.AccountId),
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
        var find = "SELECT * FROM STUDENT WHERE StudentId = @id";
        var student = await _context.Students
          .FromSqlRaw(find, new SqlParameter("@id", id))
          .FirstOrDefaultAsync();

        if (student is null)
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
        // Correct SQL query with proper aliases
        var query = @"
            SELECT s.StudentId, s.ClassId, s.GradeId, s.AccountId, s.Fullname, s.Status, s.Description,
                   a.AccountId as A_AccountId, a.RoleId, a.SchoolId, a.Email
            FROM Student s INNER JOIN Account a ON s.AccountId = a.AccountId
            WHERE s.StudentId = @id";

        // Fetch the data using FromSqlRaw
        var student = await _context.Students
            .FromSqlRaw(query, new SqlParameter("@id", id))
            .Select(static s => new
            {
              s.StudentId,
              s.ClassId,
              s.GradeId,
              s.Account.AccountId,
              s.Fullname,
              s.Status,
              s.Description,
              AccountAccountId = s.Account.AccountId,
              RoleId = s.Account.RoleId,
              SchoolId = s.Account.SchoolId,
              Email = s.Account.Email
            })
            .FirstOrDefaultAsync();

        if (student is null)
        {
          return new Data_Response<StudentDto>(404, "Student not found");
        }

        // Map the result to the StudentDto
        var result = new StudentDto
        {
          StudentId = id,
          ClassId = student.ClassId,
          GradeId = student.GradeId,
          AccountId = student.AccountId,
          Fullname = student.Fullname,
          Status = student.Status,
          Description = student.Description,
          Account = new AccountDto
          {
            AccountId = student.AccountAccountId,
            RoleId = student.RoleId,
            SchoolId = student.SchoolId,
            Email = student.Email
          }
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
        var students = await _context.Students.FromSqlRaw(query).ToListAsync();

        var result = students.Select(x => new StudentDto
        {
          StudentId = x.StudentId,
          ClassId = x.ClassId,
          GradeId = x.GradeId,
          AccountId = x.AccountId,
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
          .FromSqlRaw(find, new SqlParameter("@id", id))
          .FirstOrDefaultAsync();

        if (exists is null)
        {
          return new Data_Response<StudentDto>(404, "Student not found");
        }

        var queryBuilder = new StringBuilder("UPDATE Student SET ");
        var parameters = new List<SqlParameter>();

        if (model.ClassId != 0)
        {
          queryBuilder.Append("ClassId = @ClassId, ");
          parameters.Add(new SqlParameter("@ClassId", model.ClassId));
        }

        if (model.GradeId != 0)
        {
          queryBuilder.Append("GradeId = @GradeId, ");
          parameters.Add(new SqlParameter("@GradeId", model.GradeId));
        }

        if (model.AccountId != 0)
        {
          queryBuilder.Append("accountId = @accountId, ");
          parameters.Add(new SqlParameter("@accountId", model.AccountId));
        }

        if (!string.IsNullOrEmpty(model.Fullname))
        {
          queryBuilder.Append("Fullname = @Fullname, ");
          parameters.Add(new SqlParameter("@Fullname", model.Fullname));
        }

        queryBuilder.Append("Status = @Status, ");
        parameters.Add(new SqlParameter("@Status", model.Status));

        queryBuilder.Append("Description = @Description, ");
        parameters.Add(new SqlParameter("@Description", model.Description));


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

    public async Task<string> ImportClassExcel(IFormFile file)
    {
      try
      {
        System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

        if (file is not null && file.Length > 0)
        {
          var uploadsFolder = $"{Directory.GetCurrentDirectory()}\\Uploads";

          if (!Directory.Exists(uploadsFolder))
          {
            Directory.CreateDirectory(uploadsFolder);
          }

          var filePath = Path.Combine(uploadsFolder, file.Name);
          using (var stream = new FileStream(filePath, FileMode.Create))
          {
            await file.CopyToAsync(stream);
          }

          using (var stream = System.IO.File.Open(filePath, FileMode.Open, FileAccess.Read))
          {
            using (var reader = ExcelReaderFactory.CreateReader(stream))
            {
              bool isHeaderSkipped = false;

              do
              {
                while (reader.Read())
                {
                  if (!isHeaderSkipped)
                  {
                    isHeaderSkipped = true;
                    continue;
                  }

                  var myStudent = new Models.Student
                  {
                    ClassId = Convert.ToInt32(reader.GetValue(1)),
                    GradeId = Convert.ToInt32(reader.GetValue(2)),
                    AccountId = Convert.ToInt32(reader.GetValue(3)),
                    Fullname = reader.GetValue(4).ToString() ?? "Undefined",
                    Status = Convert.ToBoolean(reader.GetValue(5)),
                    Description = reader.GetValue(6)?.ToString() ?? $"{DateTime.UtcNow}"
                  };

                  await _context.Students.AddAsync(myStudent);
                  await _context.SaveChangesAsync();
                }
              } while (reader.NextResult());
            }
          }

          return "Successfully inserted all classes.";
        }
        return "No file uploaded";

      }
      catch (Exception ex)
      {
        throw new Exception($"Error while uploading file: {ex.Message}");
      }
    }

    public async Task<Data_Response<string>> BulkDelete(List<int> ids)
    {
      await using var transaction = await _context.Database.BeginTransactionAsync();

      try
      {
        if (ids is null || ids.Count == 0)
        {
          return new Data_Response<string>(400, "No IDs provided.");
        }

        var idList = string.Join(",", ids);

        var deleteQuery = $"DELETE FROM STUDENT WHERE StudentId IN ({idList})";

        var delete = await _context.Database.ExecuteSqlRawAsync(deleteQuery);

        if (delete == 0)
        {
          return new Data_Response<string>(404, "No students found to delete");
        }

        await transaction.CommitAsync();

        return new Data_Response<string>(200, "Deleted succesfully");
      }
      catch (Exception ex)
      {
        await transaction.RollbackAsync();
        return new Data_Response<string>(500, $"Server error: {ex.Message}");
      }
    }
  }
}
