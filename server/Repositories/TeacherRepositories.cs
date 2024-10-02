using ExcelDataReader;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using server.Data;
using server.Dtos;
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

        var sqlInsert = @"INSERT INTO Teacher (AccountId, SchoolId, Fullname, DateOfBirth, Gender, Address, Status, DateCreate, DateUpdate) 
                          VALUES (@AccountId, @SchoolId, @Fullname, @DateOfBirth, @Gender, @Address, @Status, @DateCreate, @DateUpdate);
                          SELECT CAST(SCOPE_IDENTITY() as int);";

        var currentdate = DateTime.Now;

        var insert = await _context.Database.ExecuteSqlRawAsync(sqlInsert,
          new SqlParameter("@AccountId", model.AccountId),
          new SqlParameter("@SchoolId", model.SchoolId),
          new SqlParameter("@Fullname", model.Fullname),
          new SqlParameter("@DateOfBirth", model.DateOfBirth.ToString("dd/MM/yyyy")),
          new SqlParameter("@Gender", model.Gender),
          new SqlParameter("@Address", model.Address),
          new SqlParameter("@Status", model.Status),
          new SqlParameter("@DateCreate", currentdate),
          new SqlParameter("@DateUpdate", DBNull.Value)
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
          DateCreate = model.DateCreate,
          DateUpdate = model.DateUpdate
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
          DateCreate = teacher.DateCreate,
          DateUpdate = teacher.DateUpdate
        };

        return new Data_Response<TeacherDto>(200, result);
      }
      catch (Exception ex)
      {
        return new Data_Response<TeacherDto>(500, $"Server error: {ex.Message}");
      }
    }

    public async Task<List<TeacherDto>> GetTeachers(int pageNumber, int pageSize)
    {
      try
      {
        var skip = (pageNumber - 1) * pageSize;

        var query = @"SELECT * FROM Teacher
                      ORDER BY fullname
                      OFFSET @skip ROWS
                      FETCH NEXT @pageSize ROWS ONLY";

        var teachers = await _context.Teachers
          .FromSqlRaw(query,
          new SqlParameter("@skip", skip),
          new SqlParameter("@pageSize", pageSize)
          ).ToListAsync() ?? throw new Exception("Empty");

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
        throw new Exception($"Server error: {ex.Message}");
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
          DateCreate = x.DateCreate,
          DateUpdate = x.DateUpdate
        }).ToList();

        return result;
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
        throw new Exception($"Error: {ex.Message}");
      }
    }

    public async Task<Data_Response<TeacherDto>> UpdateTeacher(int id, TeacherDto model)
    {
      using var transaction = await _context.Database.BeginTransactionAsync();
      try
      {
        // Check if the teacher exists in the database
        var findQuery = "SELECT * FROM Teacher WHERE TeacherId = @id";
        var existingTeacher = await _context.Teachers
            .FromSqlRaw(findQuery, new SqlParameter("@id", id))
            .FirstOrDefaultAsync();

        if (existingTeacher is null)
        {
          return new Data_Response<TeacherDto>(404, "Teacher not found");
        }

        bool hasChanges = false;

        // Build update query dynamically based on non-null fields
        var parameters = new List<SqlParameter>();
        var queryBuilder = new StringBuilder("UPDATE Teacher SET ");

        if (!string.IsNullOrEmpty(model.Fullname) && model.Fullname != existingTeacher.Fullname)
        {
          queryBuilder.Append("Fullname = @Fullname, ");
          parameters.Add(new SqlParameter("@Fullname", model.Fullname));
          hasChanges = true;
        }

        if (model.AccountId != 0 && model.AccountId != existingTeacher.AccountId)
        {
          queryBuilder.Append("AccountId = @AccountId, ");
          parameters.Add(new SqlParameter("@AccountId", model.AccountId));
          hasChanges = true;
        }

        if (model.SchoolId != 0 && model.SchoolId != existingTeacher.SchoolId)
        {
          queryBuilder.Append("SchoolId = @SchoolId, ");
          parameters.Add(new SqlParameter("@SchoolId", model.SchoolId));
          hasChanges = true;
        }

        if (model.DateOfBirth != default && model.DateOfBirth != existingTeacher.DateOfBirth)
        {
          queryBuilder.Append("DateOfBirth = @DateOfBirth, ");
          parameters.Add(new SqlParameter("@DateOfBirth", model.DateOfBirth));
          hasChanges = true;
        }

        if (model.Gender != default && model.Gender != existingTeacher.Gender)
        {
          queryBuilder.Append("Gender = @Gender, ");
          parameters.Add(new SqlParameter("@Gender", model.Gender));
          hasChanges = true;
        }

        if (!string.IsNullOrEmpty(model.Address) && model.Address != existingTeacher.Address)
        {
          queryBuilder.Append("Address = @Address, ");
          parameters.Add(new SqlParameter("@Address", model.Address));
          hasChanges = true;
        }

        if (model.Status != existingTeacher.Status)
        {
          queryBuilder.Append("Status = @Status ");
          parameters.Add(new SqlParameter("@Status", model.Status));
          hasChanges = true;
        }

        if (model.DateCreate.HasValue)
        {
          queryBuilder.Append("DateCreate = @DateCreate, ");
          parameters.Add(new SqlParameter("@DateCreate", model.DateCreate.Value));
        }

        if (model.DateUpdate != existingTeacher.DateUpdate)
        {
          queryBuilder.Append("DateUpdate = @DateUpdate, ");
          parameters.Add(new SqlParameter("@DateUpdate", model.DateUpdate));
          hasChanges = true;
        }

        if (hasChanges)
        {
          if (queryBuilder[^2] == ',')
          {
            queryBuilder.Length -= 2;
          }

          queryBuilder.Append(" WHERE TeacherId = @id");
          parameters.Add(new SqlParameter("@id", id));

          // Execute the update query
          var updateQuery = queryBuilder.ToString();
          await _context.Database.ExecuteSqlRawAsync(updateQuery, [.. parameters]);

          // Commit the transaction
          await transaction.CommitAsync();
          return new Data_Response<TeacherDto>(200, "Teacher updated successfully");
        }
        else
        {
          return new Data_Response<TeacherDto>(200, "No changes detected");
        }
      }
      catch (Exception ex)
      {
        await transaction.RollbackAsync();

        return new Data_Response<TeacherDto>(500, $"Server Error: {ex.Message}");
      }
    }

    public async Task<string> ImportExcelFile(IFormFile file)
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

                  var myTeachers = new Models.Teacher
                  {
                    AccountId = Convert.ToInt16(reader.GetValue(1)),
                    SchoolId = Convert.ToInt16(reader.GetValue(2)),
                    Fullname = reader.GetValue(3).ToString()?.Trim() ?? "Fullname",
                    DateOfBirth = Convert.ToDateTime(reader.GetValue(4)),
                    Gender = Convert.ToBoolean(reader.GetValue(5)),
                    Address = reader.GetValue(6).ToString()?.Trim() ?? "address",
                    Status = Convert.ToBoolean(reader.GetValue(7)),
                    DateCreate = DateTime.Now,
                    DateUpdate = null
                  };

                  await _context.Teachers.AddAsync(myTeachers);
                  await _context.SaveChangesAsync();
                }
              } while (reader.NextResult());
            }
          }

          return "Successfully inserted";
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

        var deleteQuery = $"DELETE FROM Teacher WHERE TeacherId IN ({idList})";

        var delete = await _context.Database.ExecuteSqlRawAsync(deleteQuery);

        if (delete == 0)
        {
          return new Data_Response<string>(404, "No TeacherId found to delete");
        }

        await transaction.CommitAsync();

        return new Data_Response<string>(200, "Deleted");
      }
      catch (Exception ex)
      {
        await transaction.RollbackAsync();
        return new Data_Response<string>(500, $"Server error: {ex.Message}");
      }
    }
  }
}
