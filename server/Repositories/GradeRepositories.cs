using ExcelDataReader;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using server.Data;
using server.Dtos;
using server.IService;
using System.Text;

namespace server.Repositories
{
  public class GradeRepositories : IGrade
  {
    readonly SoDauBaiContext _context;

    public GradeRepositories(SoDauBaiContext context)
    {
      this._context = context;
    }

    public async Task<Data_Response<GradeDto>> CreateGrade(GradeDto model)
    {
      try
      {
        // check academic
        var findAcademic = "SELECT * FROM AcademicYear WHERE AcademicYearId = @id";
        var acaExists = await _context.AcademicYears
          .FromSqlRaw(findAcademic, new SqlParameter("@id", model.AcademicYearId))
          .FirstOrDefaultAsync();

        if (acaExists is null)
        {
          return new Data_Response<GradeDto>(404, "Academic-year not found");
        }

        //check grade
        var find = "SELECT * FROM Grade WHERE GradeId = @id";
        var grade = await _context.Grades
          .FromSqlRaw(find, new SqlParameter("@id", model.GradeId))
          .FirstOrDefaultAsync();

        if (grade is not null)
        {
          return new Data_Response<GradeDto>(409, "Grade already exists");
        }

        var sqlInsert = @"INSERT INTO Grade (academicYearId, gradeName, description, dateCreated, dateUpdated)
                          VALUES (@academicYearId, @gradeName, @description, @dateCreated, @dateUpdated);
                          SELECT CAST(SCOPE_IDENTITY() as int);";

        var insert = await _context.Database.ExecuteSqlRawAsync(sqlInsert,
          new SqlParameter("@academicYearId", model.AcademicYearId),
          new SqlParameter("@gradeName", model.GradeName),
          new SqlParameter("@description", model.Description),
          new SqlParameter("@dateCreated", model.DateCreated),
          new SqlParameter("@dateUpdated", model.DateUpdated)
          );

        var result = new GradeDto
        {
          AcademicYearId = model.AcademicYearId,
          GradeName = model.GradeName,
          Description = model.Description,
          DateCreated = model.DateCreated,
          DateUpdated = model.DateUpdated,
        };

        return new Data_Response<GradeDto>(200, result);
      }
      catch (Exception ex)
      {
        return new Data_Response<GradeDto>(500, $"Server error: {ex.Message}");
      }
    }

    public async Task<Data_Response<GradeDto>> GetGrade(int id)
    {
      try
      {
        var find = @"SELECT g.gradeId, g.gradeName, 
                            a.academicYearId, a.displayAcademicYear_Name, a.yearStart, a.yearEnd
                     FROM GRADE as g inner join AcademicYear as a on g.academicYearId = a.academicYearId
                     WHERE GradeId = @id";

        var grade = await _context.Grades
          .FromSqlRaw(find, new SqlParameter("@id", id))
          .Select(static x => new
          {
            x.GradeId,
            x.GradeName,
            AcademicYearId = x.AcademicYear.AcademicYearId,
            displayName = x.AcademicYear.DisplayAcademicYearName,
            yearStart = x.AcademicYear.YearStart,
            yearEnd = x.AcademicYear.YearEnd,
          })
          .FirstOrDefaultAsync();

        if (grade is null)
        {
          return new Data_Response<GradeDto>(404, "Grade not found");
        }

        var result = new GradeDto
        {
          GradeId = id,
          GradeName = grade.GradeName,
          AcademicYear = new AcademicYearDto
          {
            AcademicYearId = grade.AcademicYearId,
            DisplayAcademicYearName = grade.displayName,
            YearStart = grade.yearStart,
            YearEnd = grade.yearEnd,
          }
        };

        return new Data_Response<GradeDto>(200, result);

      }
      catch (Exception ex)
      {
        return new Data_Response<GradeDto>(500, $"Server error: {ex.Message}");
      }
    }

    public async Task<List<GradeDto>> GetGrades(int pageNumber, int pageSize)
    {
      try
      {
        var skip = (pageNumber - 1) * pageSize;

        var find = @"SELECT * FROM GRADE 
                      ORDER BY GRADEID
                      OFFSET @skip ROWS
                      FETCH  NEXT @pageSize ROWS ONLY;";

        var grade = await _context.Grades
          .FromSqlRaw(find,
          new SqlParameter("@skip", skip),
          new SqlParameter("@pageSize", pageSize)
          ).ToListAsync() ?? throw new Exception("Empty");

        var result = grade.Select(x => new GradeDto
        {
          GradeId = x.GradeId,
          AcademicYearId = x.AcademicYearId,
          GradeName = x.GradeName,
          Description = x.Description,
        }).ToList();

        return result;
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
        throw new Exception($"Server Error: {ex.Message}");
      }
    }

    public async Task<Data_Response<GradeDto>> UpdateGrade(int id, GradeDto model)
    {
      using var transaction = await _context.Database.BeginTransactionAsync();

      try
      {
        var find = "SELECT * FROM Grade WHERE GradeId = @id";

        var existingGrade = await _context.Grades
          .FromSqlRaw(find, new SqlParameter("@id", id))
          .FirstOrDefaultAsync();

        if (existingGrade is null)
        {
          return new Data_Response<GradeDto>(404, "Grade not found");
        }

        bool hasChanges = false;

        var queryBuilder = new StringBuilder("UPDATE Grade SET ");
        var parameters = new List<SqlParameter>();

        if (model.AcademicYearId != 0 && model.AcademicYearId != existingGrade.AcademicYearId)
        {
          queryBuilder.Append("AcademicYearId = @AcademicYearId, ");
          parameters.Add(new SqlParameter("@AcademicYearId", model.AcademicYearId));
          hasChanges = true;
        }

        if (!string.IsNullOrEmpty(model.GradeName) && model.GradeName != existingGrade.GradeName)
        {
          queryBuilder.Append("GradeName = @GradeName, ");
          parameters.Add(new SqlParameter("@GradeName", model.GradeName));
          hasChanges |= true;
        }
        if (model.Description != existingGrade.Description)
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

        if (model.DateUpdated != existingGrade.DateUpdated)
        {
          queryBuilder.Append("DateUpdated = @DateUpdated, ");
          parameters.Add(new SqlParameter("@DateUpdated", DateTime.UtcNow));
          hasChanges = true;
        }

        if (hasChanges)
        {
          if (queryBuilder[queryBuilder.Length - 2] == ',')
          {
            queryBuilder.Length -= 2;
          }

          queryBuilder.Append(" WHERE GradeId = @id");
          parameters.Add(new SqlParameter("@id", id));

          var updateQuery = queryBuilder.ToString();
          await _context.Database.ExecuteSqlRawAsync(updateQuery, [.. parameters]);

          await transaction.CommitAsync();
          return new Data_Response<GradeDto>(200, "Updated");
        }
        else
        {
          return new Data_Response<GradeDto>(200, "No changes detected");
        }

      }
      catch (Exception ex)
      {
        await transaction.RollbackAsync();

        return new Data_Response<GradeDto>(500, $"Server error: {ex.Message}");
      }
    }

    public async Task<Data_Response<GradeDto>> DeleteGrade(int id)
    {
      try
      {
        var find = "SELECT * FROM Grade WHERE GradeId = @id";

        var grade = await _context.Grades
          .FromSqlRaw(find, new SqlParameter("@id", id))
          .FirstOrDefaultAsync();

        if (grade is null)
        {
          return new Data_Response<GradeDto>(404, "Grade not found");
        }

        var deleteQuery = "DELETE FROM Grade WHERE GradeId = @id";
        await _context.Database.ExecuteSqlRawAsync(deleteQuery, new SqlParameter("@id", id));

        return new Data_Response<GradeDto>(200, "Deleted");
      }
      catch (Exception ex)
      {
        return new Data_Response<GradeDto>(500, $"Server error: {ex.Message}");
      }
    }

    public async Task<Data_Response<string>> BulkDelete(List<int> ids)
    {
      await using var transaction = await _context.Database.BeginTransactionAsync();
      try
      {
        if (ids == null || ids.Count == 0)
        {
          return new Data_Response<string>(400, "No IDs provided");
        }

        // Create a comma-separated list of IDs for the SQL query
        var idList = string.Join(",", ids);

        // Prepare the delete query with parameterized input
        var deleteQuery = $"DELETE FROM Grade WHERE GradeId IN ({idList})";

        // Execute
        var affectedRows = await _context.Database.ExecuteSqlRawAsync(deleteQuery);

        if (affectedRows == 0)
        {
          return new Data_Response<string>(404, "No ids found to delete");
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
                if (reader.GetValue(1) == null && reader.GetValue(2) == null && reader.GetValue(3) == null)
                {
                  // Stop processing when an empty row is encountered
                  break;
                }

                var myGrades = new Models.Grade
                {
                  AcademicYearId = Convert.ToInt16(reader.GetValue(1)),
                  GradeName = reader.GetValue(2).ToString()?.Trim() ?? "Khoi lop",
                  Description = reader.GetValue(3).ToString()?.Trim() ?? "Mo Ta",
                  DateCreated = DateTime.UtcNow,
                  DateUpdated = null
                };

                await _context.Grades.AddAsync(myGrades);
                await _context.SaveChangesAsync();
              }
            } while (reader.NextResult());
          }

          return "Successfully inserted.";
        }

        return "No file uploaded";
      }
      catch (Exception ex)
      {
        throw new Exception($"Error while uploading file: {ex.Message}");
      }
    }
  }
}
