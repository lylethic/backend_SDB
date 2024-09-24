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

        var sqlInsert = @"INSERT INTO Grade (academicYearId, gradeName, description)
                          VALUES (@academicYearId, @gradeName, @description);
                          SELECT CAST(SCOPE_IDENTITY() as int);";

        var insert = await _context.Database.ExecuteSqlRawAsync(sqlInsert,
          new SqlParameter("@academicYearId", model.AcademicYearId),
          new SqlParameter("@gradeName", model.GradeName),
          new SqlParameter("@description", model.Description)
          );

        var result = new GradeDto
        {
          AcademicYearId = model.AcademicYearId,
          GradeName = model.GradeName,
          Description = model.Description,
        };

        return new Data_Response<GradeDto>(200, result);
      }
      catch (Exception ex)
      {
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

    public async Task<Data_Response<GradeDto>> GetGrade(int id)
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

        var result = new GradeDto
        {
          GradeId = id,
          AcademicYearId = grade.AcademicYearId,
          GradeName = grade.GradeName,
          Description = grade.Description,
        };

        return new Data_Response<GradeDto>(200, result);

      }
      catch (Exception ex)
      {
        return new Data_Response<GradeDto>(500, "Server error");
      }
    }

    public async Task<List<GradeDto>> GetGrades()
    {
      try
      {
        var find = "SELECT * FROM GRADE";

        var grade = await _context.Grades
          .FromSqlRaw(find).ToListAsync();

        if (grade is null)
        {
          throw new Exception("Empty");
        }

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
        throw;
      }
    }

    public async Task<Data_Response<GradeDto>> UpdateGrade(int id, GradeDto model)
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

        var queryBuilder = new StringBuilder("UPDATE Grade SET ");
        var parameters = new List<SqlParameter>();

        if (model.AcademicYearId != 0 || !string.IsNullOrEmpty(model.GradeName))
        {
          queryBuilder.Append("AcademicYearId = @AcademicYearId, ");
          parameters.Add(new SqlParameter("@AcademicYearId", model.AcademicYearId));

          queryBuilder.Append("GradeName = @GradeName, ");
          parameters.Add(new SqlParameter("@GradeName", model.GradeName));

          queryBuilder.Append("Description = @Description, ");
          parameters.Add(new SqlParameter("@Description", model.Description));
        }

        if (queryBuilder[queryBuilder.Length - 2] == ',')
        {
          queryBuilder.Length -= 2;
        }

        queryBuilder.Append(" WHERE GradeId = @id");
        parameters.Add(new SqlParameter("@id", id));

        var updateQuery = queryBuilder.ToString();
        await _context.Database.ExecuteSqlRawAsync(updateQuery, parameters.ToArray());

        return new Data_Response<GradeDto>(200, "Updated");
      }
      catch (Exception ex)
      {
        return new Data_Response<GradeDto>(500, $"Server error: {ex.Message}");
      }
    }
  }
}
