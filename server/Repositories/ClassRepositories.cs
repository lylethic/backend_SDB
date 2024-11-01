using ExcelDataReader;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using server.Data;
using server.Dtos;
using server.IService;
using server.Models;
using server.Types;
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

    public async Task<ResponseData<ClassDto>> CreateClass(ClassDto model)
    {
      try
      {
        // check grade
        var findGrade = @"SELECT * FROM Grade WHERE gradeId = @id";
        var grade = await _context.Grades
          .FromSqlRaw(findGrade, new SqlParameter("@id", model.GradeId))
          .FirstOrDefaultAsync();

        if (grade is null)
        {
          return new ResponseData<ClassDto>(404, "Grade Not found");
        }

        // check teacher
        var findTeacher = "SELECT * FROM Teacher WHERE teacherId = @id";
        var teacherExists = await _context.Teachers
          .FromSqlRaw(findTeacher, new SqlParameter("@id", model.TeacherId))
          .FirstOrDefaultAsync();

        if (teacherExists is null)
        {
          return new ResponseData<ClassDto>(404, "Teacher Not found");
        }

        // check aca
        var findAcademic = "SELECT * FROM AcademicYear WHERE academicYearId = @id";
        var acaExists = await _context.AcademicYears
          .FromSqlRaw(findAcademic, new SqlParameter("@id", model.AcademicYearId))
          .FirstOrDefaultAsync();

        if (acaExists is null)
        {
          return new ResponseData<ClassDto>(404, "AcademicYear Not found");
        }

        // check school
        var findSchool = "SELECT * FROM School WHERE SchoolId = @id";
        var schoolExists = await _context.Schools
          .FromSqlRaw(findSchool, new SqlParameter("@id", model.SchoolId))
          .FirstOrDefaultAsync();

        if (schoolExists is null)
        {
          return new ResponseData<ClassDto>(404, "School Not found");
        }

        //check class
        var find = "SELECT * FROM Class WHERE classId = @id";

        var getClass = await _context.Classes
          .FromSqlRaw(find, new SqlParameter("@id", model.ClassId))
          .FirstOrDefaultAsync();

        if (getClass is not null)
        {
          return new ResponseData<ClassDto>(409, "Grade already exists");
        }

        var sqlInsert = @"INSERT INTO Class (gradeId, teacherId, academicYearId, schoolId, className, status, description, dateCreated, dateUpdated)
                          VALUES (@gradeId, @teacherId, @academicYearId, @schoolId, @className, @status, @description, @dateCreated, @dateUpdated);
                          SELECT CAST(SCOPE_IDENTITY() as int);";

        var currentDate = DateTime.UtcNow;

        var insert = await _context.Database.ExecuteSqlRawAsync(sqlInsert,
          new SqlParameter("@gradeId", model.GradeId),
          new SqlParameter("@teacherId", model.TeacherId),
          new SqlParameter("@academicYearId", model.AcademicYearId),
          new SqlParameter("@schoolId", model.SchoolId),
          new SqlParameter("@className", model.ClassName),
          new SqlParameter("@status", model.Status),
          new SqlParameter("@description", model.Description),
          new SqlParameter("@dateCreated", model.DateCreated),
          new SqlParameter("@dateUpdated", DBNull.Value)
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
          DateCreated = model.DateCreated,
          DateUpdated = model.DateUpdated,
        };

        return new ResponseData<ClassDto>(200, result);
      }
      catch (Exception ex)
      {
        return new ResponseData<ClassDto>(500, $"Server error: {ex.Message}");
      }
    }

    public async Task<ResponseData<ClassDto>> DeleteClass(int id)
    {
      try
      {
        var find = "SELECT * FROM CLASS WHERE CLASSID = @id";
        var getClass = await _context.Classes.FromSqlRaw(find, new SqlParameter("@id", id)).FirstOrDefaultAsync();

        if (getClass is null)
        {
          return new ResponseData<ClassDto>(404, "Not found");
        }

        var deleteQuery = "DELETE FROM CLASS WHERE ClassId = @id";
        await _context.Database.ExecuteSqlRawAsync(deleteQuery, new SqlParameter("@id", id));

        return new ResponseData<ClassDto>(200, "Deleted");
      }
      catch (Exception ex)
      {
        return new ResponseData<ClassDto>(500, $"Server error: {ex.Message}");
      }
    }

    public async Task<ResponseData<ClassDto>> GetClass(int id)
    {
      try
      {
        var find = "SELECT * FROM CLASS WHERE ClassId = @id";
        var getClass = await _context.Classes
          .FromSqlRaw(find, new SqlParameter("@id", id))
          .FirstOrDefaultAsync();

        if (getClass is null)
        {
          return new ResponseData<ClassDto>(404, "Not found");
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
          DateCreated = getClass.DateCreated,
          DateUpdated = getClass.DateUpdated,
        };

        return new ResponseData<ClassDto>(200, result);
      }
      catch (Exception ex)
      {
        return new ResponseData<ClassDto>(500, $"Server error: {ex.Message}");
      }
    }

    public async Task<List<ClassDto>> GetClasses(int pageNumber, int pageSize)
    {
      try
      {
        var skip = (pageNumber - 1) * pageSize;

        var find = @"SELECT * FROM Class 
                      ORDER BY CLASSID 
                      OFFSET @skip ROWS 
                      FETCH NEXT @pageSize ROWS ONLY";

        var classes = await _context.Classes
          .FromSqlRaw(find,
          new SqlParameter("@skip", skip),
          new SqlParameter("@pageSize", pageSize)
          ).ToListAsync() ?? throw new Exception("Empty");

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
          DateCreated = x.DateCreated,
          DateUpdated = x.DateUpdated,
        }).ToList();

        return result;
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
        throw;
      }
    }

    public async Task<List<ClassDto>> GetClassesBySchool(int pageNumber, int pageSize, int schoolId)
    {
      try
      {
        var skip = (pageNumber - 1) * pageSize;

        var find = @"SELECT * FROM Class 
                      WHERE SCHOOLID = @schoolId
                      ORDER BY CLASSID 
                      OFFSET @skip ROWS 
                      FETCH NEXT @pageSize ROWS ONLY";

        var classes = await _context.Classes
          .FromSqlRaw(find,
          new SqlParameter("@schoolId", schoolId),
          new SqlParameter("@skip", skip),
          new SqlParameter("@pageSize", pageSize)
          ).ToListAsync() ?? throw new Exception("Empty");

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
          DateCreated = x.DateCreated,
          DateUpdated = x.DateUpdated,
        }).ToList();

        return result;
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
        throw new Exception($"Server error: {ex.Message}");
      }
    }

    public async Task<ResponseData<ClassDto>> UpdateClass(int id, ClassDto model)
    {
      try
      {
        var find = "SELECT * FROM Class WHERE ClassId = @id";
        var getClass = await _context.Classes
            .FromSqlRaw(find, new SqlParameter("@id", id))
            .FirstOrDefaultAsync();

        if (getClass is null)
        {
          return new ResponseData<ClassDto>(404, "Class not found");
        }

        bool hasChanges = false;
        var parameters = new List<SqlParameter>();
        var queryBuilder = new StringBuilder("UPDATE Class SET ");

        // Conditional checks for each field
        if (model.GradeId != 0 && model.GradeId != getClass.GradeId)
        {
          queryBuilder.Append("GradeId = @GradeId, ");
          parameters.Add(new SqlParameter("@GradeId", model.GradeId));
          hasChanges = true;
        }

        if (model.TeacherId != 0 && model.TeacherId != getClass.TeacherId)
        {
          queryBuilder.Append("TeacherId = @TeacherId, ");
          parameters.Add(new SqlParameter("@TeacherId", model.TeacherId));
          hasChanges = true;
        }

        if (model.AcademicYearId != 0 && model.AcademicYearId != getClass.AcademicYearId)
        {
          queryBuilder.Append("AcademicYearId = @AcademicYearId, ");
          parameters.Add(new SqlParameter("@AcademicYearId", model.AcademicYearId));
          hasChanges = true;
        }

        if (model.SchoolId != 0 && model.SchoolId != getClass.SchoolId)
        {
          queryBuilder.Append("SchoolId = @SchoolId, ");
          parameters.Add(new SqlParameter("@SchoolId", model.SchoolId));
          hasChanges = true;
        }

        if (!string.IsNullOrEmpty(model.ClassName) && model.ClassName != getClass.ClassName)
        {
          queryBuilder.Append("ClassName = @ClassName, ");
          parameters.Add(new SqlParameter("@ClassName", model.ClassName));
          hasChanges = true;
        }

        if (!string.IsNullOrEmpty(model.Description) && model.Description != getClass.Description)
        {
          queryBuilder.Append("Description = @Description, ");
          parameters.Add(new SqlParameter("@Description", model.Description));
          hasChanges = true;
        }

        if (model.Status != getClass.Status)
        {
          queryBuilder.Append("Status = @Status, ");
          parameters.Add(new SqlParameter("@Status", model.Status));
          hasChanges = true;
        }

        if (hasChanges)
        {
          // Always update DateUpdated
          queryBuilder.Append("DateUpdated = @DateUpdated ");
          parameters.Add(new SqlParameter("@DateUpdated", DateTime.UtcNow));

          // Remove the last comma and space if present
          if (queryBuilder[queryBuilder.Length - 2] == ',')
          {
            queryBuilder.Length -= 2;
          }

          // Add WHERE clause
          queryBuilder.Append(" WHERE ClassId = @id");
          parameters.Add(new SqlParameter("@id", id));

          // Execute the update query
          var updateQuery = queryBuilder.ToString();
          await _context.Database.ExecuteSqlRawAsync(updateQuery, [.. parameters]);

          return new ResponseData<ClassDto>(200, "Class updated successfully");
        }
        else
        {
          return new ResponseData<ClassDto>(200, "No changes detected");
        }
      }
      catch (Exception ex)
      {
        return new ResponseData<ClassDto>(500, $"Server error: {ex.Message}");
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

          using (var stream = System.IO.File.Open(filePath, FileMode.Open, FileAccess.Read))
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
                if (reader.GetValue(1) == null && reader.GetValue(2) == null && reader.GetValue(3) == null
                  && reader.GetValue(4) == null && reader.GetValue(5) == null && reader.GetValue(6) == null
                  && reader.GetValue(7) == null)
                {
                  // Stop processing when an empty row is encountered
                  break;
                }

                var myClass = new Models.Class
                {
                  GradeId = Convert.ToInt32(reader.GetValue(1)),
                  TeacherId = Convert.ToInt32(reader.GetValue(2)),
                  AcademicYearId = Convert.ToInt32(reader.GetValue(3)),
                  SchoolId = Convert.ToInt32(reader.GetValue(4)),
                  ClassName = reader.GetValue(5)?.ToString() ?? "Unknown",
                  Status = Convert.ToBoolean(reader.GetValue(6)),
                  Description = reader.GetValue(7)?.ToString(),
                  DateCreated = DateTime.UtcNow,
                  DateUpdated = null
                };

                await _context.Classes.AddAsync(myClass);
                await _context.SaveChangesAsync();
              }
            } while (reader.NextResult());
          }
          return "Successfully inserted!";
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
        if (ids == null || !ids.Any())
        {
          return new ResponseData<string>(400, "No IDs provided");
        }


        // Create a comma-separated list of IDs for the SQL query
        var idList = string.Join(",", ids);

        // Prepare the delete query with parameterized input
        var deleteQuery = $"DELETE FROM CLASS WHERE ClassId IN ({idList})";

        // Execute
        var affectedRows = await _context.Database.ExecuteSqlRawAsync(deleteQuery);

        if (affectedRows == 0)
        {
          return new ResponseData<string>(404, "No classes found to delete");
        }

        await transaction.CommitAsync();

        return new ResponseData<string>(200, "Deleted successfully");
      }
      catch (Exception ex)
      {
        await transaction.RollbackAsync();
        return new ResponseData<string>(500, $"Server error: {ex.Message}");
      }
    }
  }
}
