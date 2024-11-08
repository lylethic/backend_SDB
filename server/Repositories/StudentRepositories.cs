﻿using ExcelDataReader;
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

    public async Task<ResponseData<StudentDto>> CreateStudent(StudentDto model)
    {
      try
      {
        var find = "SELECT * FROM STUDENT WHERE StudentId = @id";

        var student = await _context.Students
          .FromSqlRaw(find, new SqlParameter("@id", model.StudentId))
          .FirstOrDefaultAsync();

        if (student is not null)
        {
          return new ResponseData<StudentDto>(409, "Student already exists");
        }

        var sqlInsert = @"INSERT INTO STUDENT (ClassId, GradeId, AccountId, Fullname, Status, Description, DateCreated, DateUpdated) 
                          VALUES (@ClassId, @GradeId, @AccountId, @Fullname, @Status, @Description, @DateCreated, @DateUpdated);
                          SELECT CAST(SCOPE_IDENTITY() as int);";

        var currentdate = DateTime.UtcNow;

        var insert = await _context.Database.ExecuteSqlRawAsync(sqlInsert,
          new SqlParameter("@ClassId", model.ClassId),
          new SqlParameter("@GradeId", model.GradeId),
          new SqlParameter("@AccountId", model.AccountId),
          new SqlParameter("@Fullname", model.Fullname),
          new SqlParameter("@Status", model.Status),
          new SqlParameter("@Description", model.Description),
          new SqlParameter("@DateCreated", currentdate),
          new SqlParameter("@DateUpdated", DBNull.Value)
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
          DateCreated = model.DateCreated,
          DateUpdated = model.DateUpdated
        };

        return new ResponseData<StudentDto>(200, result);
      }
      catch (Exception ex)
      {
        return new ResponseData<StudentDto>(200, $"Server error: {ex.Message}");
      }
    }

    public async Task<ResponseData<StudentDto>> DeleteStudent(int id)
    {
      try
      {
        var find = "SELECT * FROM STUDENT WHERE StudentId = @id";
        var student = await _context.Students
          .FromSqlRaw(find, new SqlParameter("@id", id))
          .FirstOrDefaultAsync();

        if (student is null)
        {
          return new ResponseData<StudentDto>(404, "Student not found");
        }

        var deleteQuery = "DELETE FROM Student WHERE StudentId = @id";
        await _context.Database.ExecuteSqlRawAsync(deleteQuery, new SqlParameter("@id", id));
        return new ResponseData<StudentDto>(200, "Deleted");
      }
      catch (Exception ex)
      {
        return new ResponseData<StudentDto>(500, $"Server error: {ex.Message}");
      }
    }

    public async Task<ResponseData<StudentDto>> GetStudent(int id)
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
          return new ResponseData<StudentDto>(404, "Student not found");
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

        return new ResponseData<StudentDto>(200, result);
      }
      catch (Exception ex)
      {
        return new ResponseData<StudentDto>(500, $"Server error: {ex.Message}");
      }
    }

    public async Task<List<StudentDto>> GetStudents(int pageNumber, int pageSize)
    {
      try
      {
        var skip = (pageNumber - 1) * pageSize;
        var query = @"SELECT * FROM Student ORDER BY FULLNAME OFFSET @skip ROWS FETCH NEXT @pageSize ROWS ONLY;";
        var students = await _context.Students.FromSqlRaw(query,
              new SqlParameter("@skip", skip),
              new SqlParameter("@pageSize", pageSize)
          ).ToListAsync();

        var result = students.Select(x => new StudentDto
        {
          StudentId = x.StudentId,
          ClassId = x.ClassId,
          GradeId = x.GradeId,
          AccountId = x.AccountId,
          Fullname = x.Fullname,
          Status = x.Status,
          Description = x.Description,
          DateCreated = x.DateCreated,
          DateUpdated = x.DateUpdated,
        }).ToList();

        return result;
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
        throw new Exception($"Error: {ex.Message}");
      }
    }

    public async Task<ResponseData<StudentDto>> UpdateStudent(int id, StudentDto model)
    {
      try
      {
        var find = "SELECT * FROM Student WHERE StudentId = @id";
        var exists = await _context.Students
          .FromSqlRaw(find, new SqlParameter("@id", id))
          .FirstOrDefaultAsync();

        if (exists is null)
        {
          return new ResponseData<StudentDto>(404, "Student not found");
        }

        bool hasChanges = false;

        var parameters = new List<SqlParameter>();
        var queryBuilder = new StringBuilder("UPDATE Student SET ");

        if (model.ClassId != 0 && model.ClassId != exists.ClassId)
        {
          queryBuilder.Append("ClassId = @ClassId, ");
          parameters.Add(new SqlParameter("@ClassId", model.ClassId));
          hasChanges = true;
        }

        if (model.GradeId != 0 && model.GradeId != exists.GradeId)
        {
          queryBuilder.Append("GradeId = @GradeId, ");
          parameters.Add(new SqlParameter("@GradeId", model.GradeId));
          hasChanges = true;
        }

        if (model.AccountId != 0 && model.AccountId != exists.AccountId)
        {
          queryBuilder.Append("accountId = @accountId, ");
          parameters.Add(new SqlParameter("@accountId", model.AccountId));
          hasChanges = true;
        }

        if (!string.IsNullOrEmpty(model.Fullname) && model.Fullname != exists.Fullname)
        {
          queryBuilder.Append("Fullname = @Fullname, ");
          parameters.Add(new SqlParameter("@Fullname", model.Fullname));
          hasChanges = true;
        }

        if (model.Status != exists.Status && model.Status != exists.Status)
        {
          queryBuilder.Append("Status = @Status, ");
          parameters.Add(new SqlParameter("@Status", model.Status));
          hasChanges = true;
        }

        if (model.Description != exists.Description && model.Description != exists.Description)
        {
          queryBuilder.Append("Description = @Description, ");
          parameters.Add(new SqlParameter("@Description", model.Description));
          hasChanges = true;
        }

        if (model.DateCreated.HasValue)
        {
          queryBuilder.Append("DateCreated = @DateCreated, ");
          parameters.Add(new SqlParameter("@DateCreated", model.DateCreated.Value));
        }

        if (model.DateUpdated != exists.DateUpdated)
        {
          queryBuilder.Append("DateUpdated = @DateUpdated, ");
          parameters.Add(new SqlParameter("@DateUpdated", model.DateUpdated));
          hasChanges = true;
        }

        if (hasChanges)
        {
          if (queryBuilder.Length > 0)
          {
            queryBuilder.Length -= 2;
          }

          queryBuilder.Append(" WHERE StudentId = @id");
          parameters.Add(new SqlParameter("@id", id));

          var updateQuery = queryBuilder.ToString();
          await _context.Database.ExecuteSqlRawAsync(updateQuery, parameters.ToArray());

          return new ResponseData<StudentDto>(200, "Updated");
        }
        else
        {
          return new ResponseData<StudentDto>(200, "No changes detected");
        }
      }
      catch (Exception ex)
      {
        return new ResponseData<StudentDto>(500, $"Server Error: {ex.Message}");
      }
    }

    public async Task<string> ImportExcel(IFormFile file)
    {
      try
      {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        if (file is not null && file.Length > 0)
        {
          var uploadsFolder = $"{Directory.GetCurrentDirectory()}\\Uploads";

          if (!Directory.Exists(uploadsFolder))
          {
            Directory.CreateDirectory(uploadsFolder);
          }

          var filePath = Path.Combine(uploadsFolder, file.FileName);
          using (var stream = new FileStream(filePath, FileMode.Create))
          {
            await file.CopyToAsync(stream);
          }

          using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
          {
            using var reader = ExcelReaderFactory.CreateReader(stream);

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
                // Check if there are no more rows or empty rows
                if (reader.GetValue(1) == null && reader.GetValue(2) == null && reader.GetValue(3) == null && reader.GetValue(4) == null && reader.GetValue(5) == null && reader.GetValue(6) == null)
                {
                  // Stop processing when an empty row is encountered
                  break;
                }

                var myStudent = new Models.Student
                {
                  ClassId = Convert.ToInt32(reader.GetValue(1)),
                  GradeId = Convert.ToInt32(reader.GetValue(2)),
                  AccountId = Convert.ToInt32(reader.GetValue(3)),
                  Fullname = reader.GetValue(4).ToString() ?? "Undefined",
                  Status = Convert.ToBoolean(reader.GetValue(5)),
                  Description = reader.GetValue(6)?.ToString() ?? $"{DateTime.UtcNow}",
                  DateCreated = DateTime.UtcNow,
                  DateUpdated = null
                };

                await _context.Students.AddAsync(myStudent);
                await _context.SaveChangesAsync();
              }
            } while (reader.NextResult());
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

    public async Task<ResponseData<string>> BulkDelete(List<int> ids)
    {
      await using var transaction = await _context.Database.BeginTransactionAsync();

      try
      {
        if (ids is null || ids.Count == 0)
        {
          return new ResponseData<string>(400, "No IDs provided.");
        }

        var idList = string.Join(",", ids);

        var deleteQuery = $"DELETE FROM STUDENT WHERE StudentId IN ({idList})";

        var delete = await _context.Database.ExecuteSqlRawAsync(deleteQuery);

        if (delete == 0)
        {
          return new ResponseData<string>(404, "No students found to delete");
        }

        await transaction.CommitAsync();

        return new ResponseData<string>(200, "Deleted succesfully");
      }
      catch (Exception ex)
      {
        await transaction.RollbackAsync();
        return new ResponseData<string>(500, $"Server error: {ex.Message}");
      }
    }
  }
}
