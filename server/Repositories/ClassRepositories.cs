using ExcelDataReader;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using server.Data;
using server.Dtos;
using server.IService;
using System.Text;

namespace server.Repositories
{
  public class ClassRepositories : IClass
  {
    private readonly SoDauBaiContext _context;

    public ClassRepositories(SoDauBaiContext context)
    {
      this._context = context;
    }

    public async Task<Data_Response<ClassDto>> CreateClass(ClassDto model)
    {
      try
      {
        // check grade
        var findGrade = "SELECT * FROM Grade WHERE gradeId = @id";
        var grade = await _context.Grades
          .FromSqlRaw(findGrade, new SqlParameter("@id", model.GradeId))
          .FirstOrDefaultAsync();

        if (grade is null)
        {
          return new Data_Response<ClassDto>(404, "Grade Not found");
        }

        // check teacher
        var findTeacher = "SELECT * FROM Teacher WHERE teacherId = @id";
        var teacherExists = await _context.Teachers
          .FromSqlRaw(findTeacher, new SqlParameter("@id", model.TeacherId))
          .FirstOrDefaultAsync();

        if (teacherExists is null)
        {
          return new Data_Response<ClassDto>(404, "Teacher Not found");
        }

        // check aca
        var findAcademic = "SELECT * FROM AcademicYear WHERE academicYearId = @id";
        var acaExists = await _context.AcademicYears
          .FromSqlRaw(findAcademic, new SqlParameter("@id", model.AcademicYearId))
          .FirstOrDefaultAsync();

        if (acaExists is null)
        {
          return new Data_Response<ClassDto>(404, "AcademicYear Not found");
        }

        // check school
        var findSchool = "SELECT * FROM School WHERE SchoolId = @id";
        var schoolExists = await _context.Schools
          .FromSqlRaw(findSchool, new SqlParameter("@id", model.SchoolId))
          .FirstOrDefaultAsync();

        if (schoolExists is null)
        {
          return new Data_Response<ClassDto>(404, "School Not found");
        }

        //check class
        var find = "SELECT * FROM Class WHERE classId = @id";

        var getClass = await _context.Classes
          .FromSqlRaw(find, new SqlParameter("@id", model.ClassId))
          .FirstOrDefaultAsync();

        if (getClass is not null)
        {
          return new Data_Response<ClassDto>(409, "Grade already exists");
        }

        var sqlInsert = @"INSERT INTO Class (gradeId, teacherId, academicYearId, schoolId, className, status, description)
                          VALUES (@gradeId, @teacherId, @academicYearId, @schoolId, @className, @status, @description);
                          SELECT CAST(SCOPE_IDENTITY() as int);";

        var insert = await _context.Database.ExecuteSqlRawAsync(sqlInsert,
          new SqlParameter("@gradeId", model.GradeId),
          new SqlParameter("@teacherId", model.TeacherId),
          new SqlParameter("@academicYearId", model.AcademicYearId),
          new SqlParameter("@schoolId", model.SchoolId),
          new SqlParameter("@className", model.ClassName),
          new SqlParameter("@status", model.Status),
          new SqlParameter("@description", model.Description)
          );

        var result = new ClassDto
        {
          ClassId = insert,
          GradeId = model.GradeId,
          TeacherId = model.TeacherId,
          AcademicYearId = model.AcademicYearId,
          SchoolId = model.SchoolId,
          ClassName = model.ClassName,
          Status = model.Status,
          Description = model.Description,
        };

        return new Data_Response<ClassDto>(200, result);
      }
      catch (Exception ex)
      {
        return new Data_Response<ClassDto>(500, $"Server error: {ex.Message}");
      }
    }

    public async Task<Data_Response<ClassDto>> DeleteClass(int id)
    {
      try
      {
        var find = "SELECT * FROM CLASS WHERE CLASSID = @id";
        var getClass = await _context.Classes.FromSqlRaw(find, new SqlParameter("@id", id)).FirstOrDefaultAsync();

        if (getClass is null)
        {
          return new Data_Response<ClassDto>(404, "Not found");
        }

        var deleteQuery = "DELETE FROM CLASS WHERE ClassId = @id";
        await _context.Database.ExecuteSqlRawAsync(deleteQuery, new SqlParameter("@id", id));

        return new Data_Response<ClassDto>(200, "Deleted");
      }
      catch (Exception ex)
      {
        return new Data_Response<ClassDto>(500, $"Server error: {ex.Message}");
      }
    }

    public async Task<Data_Response<ClassDto>> GetClass(int id)
    {
      try
      {
        var find = "SELECT * FROM CLASS WHERE ClassId = @id";
        var getClass = await _context.Classes
          .FromSqlRaw(find, new SqlParameter("@id", id))
          .FirstOrDefaultAsync();

        if (getClass is null)
        {
          return new Data_Response<ClassDto>(404, "Not found");
        }

        var result = new ClassDto
        {
          ClassId = id,
          GradeId = getClass.GradeId,
          TeacherId = getClass.TeacherId,
          AcademicYearId = id,
          SchoolId = getClass.SchoolId,
          ClassName = getClass.ClassName,
          Status = getClass.Status,
          Description = getClass.Description,
        };

        return new Data_Response<ClassDto>(200, result);
      }
      catch (Exception ex)
      {
        return new Data_Response<ClassDto>(500, $"Server error: {ex.Message}");
      }
    }

    public async Task<List<ClassDto>> GetClasses()
    {
      try
      {
        var find = "SELECT * FROM Class";
        var classes = await _context.Classes
          .FromSqlRaw(find).ToListAsync();

        if (classes is null)
        {
          throw new Exception("Empty");
        }

        var result = classes.Select(x => new ClassDto
        {
          ClassId = x.ClassId,
          GradeId = x.GradeId,
          TeacherId = x.TeacherId,
          AcademicYearId = x.AcademicYearId,
          SchoolId = x.SchoolId,
          ClassName = x.ClassName,
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

    public async Task<Data_Response<ClassDto>> UpdateClass(int id, ClassDto model)
    {
      try
      {
        var find = "SELECT * FROM Class WHERE ClassId = @id";

        var getClass = await _context.Classes
          .FromSqlRaw(find, new SqlParameter("@id", id))
          .FirstOrDefaultAsync();

        if (getClass is null)
        {
          return new Data_Response<ClassDto>(404, "Not found");
        }

        var queryBuilder = new StringBuilder("UPDATE Class SET ");
        var parameters = new List<SqlParameter>();

        if (model.GradeId != 0 || model.TeacherId != 0 || model.AcademicYearId != 0 || model.SchoolId != 0 || !string.IsNullOrEmpty(model.ClassName))
        {
          queryBuilder.Append("GradeId = @GradeId, ");
          parameters.Add(new SqlParameter("@GradeId", model.GradeId));

          queryBuilder.Append("TeacherId = @TeacherId, ");
          parameters.Add(new SqlParameter("@TeacherId", model.TeacherId));

          queryBuilder.Append("AcademicYearId = @AcademicYearId, ");
          parameters.Add(new SqlParameter("@AcademicYearId", model.AcademicYearId));

          queryBuilder.Append("SchoolId = @SchoolId, ");
          parameters.Add(new SqlParameter("@SchoolId", model.SchoolId));

          queryBuilder.Append("ClassName = @ClassName, ");
          parameters.Add(new SqlParameter("@ClassName", model.ClassName));

          queryBuilder.Append("Status = @Status, ");
          parameters.Add(new SqlParameter("@Status", model.Status));

          queryBuilder.Append("Description = @Description, ");
          parameters.Add(new SqlParameter("@Description", model.Description));
        }

        if (queryBuilder[queryBuilder.Length - 2] == ',')
        {
          queryBuilder.Length -= 2;
        }

        queryBuilder.Append(" WHERE ClassId = @id");
        parameters.Add(new SqlParameter("@id", id));

        var updateQuery = queryBuilder.ToString();
        await _context.Database.ExecuteSqlRawAsync(updateQuery, parameters.ToArray());

        return new Data_Response<ClassDto>(200, "Updated");
      }
      catch (Exception ex)
      {
        return new Data_Response<ClassDto>(500, $"Server error: {ex.Message}");
      }
    }

    public async Task<string> ImportExcel(IFormFile file)
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

          var filePath = Path.Combine(uploadsFolder, file.FileName);

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

                  var myClass = new Models.Class
                  {
                    GradeId = Convert.ToInt32(reader.GetValue(1)),
                    TeacherId = Convert.ToInt32(reader.GetValue(2)),
                    AcademicYearId = Convert.ToInt32(reader.GetValue(3)),
                    SchoolId = Convert.ToInt32(reader.GetValue(4)),
                    ClassName = reader.GetValue(5)?.ToString() ?? "Unknown",
                    Status = Convert.ToBoolean(reader.GetValue(6)),
                    Description = reader.GetValue(7)?.ToString()
                  };

                  await _context.Classes.AddAsync(myClass);
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
        if (ids == null || !ids.Any())
        {
          return new Data_Response<string>(400, "No IDs provided");
        }


        // Create a comma-separated list of IDs for the SQL query
        var idList = string.Join(",", ids);

        // Prepare the delete query with parameterized input
        var deleteQuery = $"DELETE FROM CLASS WHERE ClassId IN ({idList})";

        // Execute
        var affectedRows = await _context.Database.ExecuteSqlRawAsync(deleteQuery);

        if (affectedRows == 0)
        {
          return new Data_Response<string>(404, "No classes found to delete");
        }

        await transaction.CommitAsync();

        return new Data_Response<string>(200, "Deleted successfully");
      }
      catch (Exception ex)
      {
        await transaction.RollbackAsync();
        return new Data_Response<string>(500, $"Server error: {ex.Message}");
      }
    }

  }
}
