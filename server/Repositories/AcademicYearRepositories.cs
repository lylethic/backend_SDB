using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using server.Data;
using server.Dtos;
using server.IService;
using System.Text;

namespace server.Repositories
{
  public class AcademicYearRepositories : IAcademicYear
  {
    readonly SoDauBaiContext _context;
    public AcademicYearRepositories(SoDauBaiContext context) { this._context = context; }

    public async Task<Data_Response<AcademicYearDto>> CreateAcademicYear(AcademicYearDto model)
    {
      try
      {
        var find = "SELECT * FROM AcademicYear WHERE academicYearId = @id";

        var academicYear = await _context.AcademicYears
          .FromSqlRaw(find, new SqlParameter("@id", model.AcademicYearId))
          .FirstOrDefaultAsync();

        if (academicYear is not null)
        {
          return new Data_Response<AcademicYearDto>(409, "AcademicYear already exists");
        }

        var sqlInsert = @"INSERT INTO AcademicYear (displayAcademicYear_Name, YearStart, YearEnd, Description) 
                          VALUES (@displayAcademicYear_Name, @YearStart ,@YearEnd, @Description);
                          SELECT CAST(SCOPE_IDENTITY() as int);"
        ;

        var yearStart = new DateTime(model.YearStart.Year, model.YearStart.Month, model.YearStart.Day);
        var yearEnd = new DateTime(model.YearEnd.Year, model.YearEnd.Month, model.YearEnd.Day);


        var insert = await _context.Database.ExecuteSqlRawAsync(sqlInsert,
          new SqlParameter("@AcademicYearId", model.AcademicYearId),
          new SqlParameter("@displayAcademicYear_Name", model.DisplayAcademicYearName),
          new SqlParameter("@YearStart", yearStart),
          new SqlParameter("@YearEnd", yearEnd),
          new SqlParameter("@Description", model.Description)
        );

        var result = new AcademicYearDto
        {
          AcademicYearId = insert,
          DisplayAcademicYearName = model.DisplayAcademicYearName,
          YearStart = model.YearStart,
          YearEnd = model.YearEnd,
          Description = model.Description,
        };

        return new Data_Response<AcademicYearDto>(200, result);
      }
      catch (Exception ex)
      {
        return new Data_Response<AcademicYearDto>(200, $"Server error: {ex.Message}");
      }
    }

    public async Task<Data_Response<AcademicYearDto>> DeleteAcademicYear(int id)
    {
      try
      {
        var find = "SELECT * FROM AcademicYear WHERE AcademicYearId = @id";
        var academicYear = await _context.AcademicYears
          .FromSqlRaw(find, new SqlParameter("@id", id))
          .FirstOrDefaultAsync();

        if (academicYear is null)
        {
          return new Data_Response<AcademicYearDto>(404, "AcademicYear not found");
        }

        var deleteQuery = "DELETE FROM AcademicYear WHERE AcademicYearId = @id";
        await _context.Database.ExecuteSqlRawAsync(deleteQuery, new SqlParameter("@id", id));
        return new Data_Response<AcademicYearDto>(200, "Deleted");
      }
      catch (Exception ex)
      {
        return new Data_Response<AcademicYearDto>(500, $"Server error: {ex.Message}");
      }
    }

    public async Task<Data_Response<AcademicYearDto>> GetAcademicYear(int id)
    {
      try
      {
        var find = "SELECT * FROM AcademicYear WHERE academicYearId = @id";
        var academicYear = await _context.AcademicYears
          .FromSqlRaw(find, new SqlParameter("@id", id))
          .FirstOrDefaultAsync();

        if (academicYear is null)
        {
          return new Data_Response<AcademicYearDto>(404, "AcademicYear not found");
        }


        var result = new AcademicYearDto
        {
          AcademicYearId = id,
          DisplayAcademicYearName = academicYear.DisplayAcademicYearName,
          YearStart = academicYear.YearStart,
          YearEnd = academicYear.YearEnd,
          Description = academicYear.Description,
        };

        return new Data_Response<AcademicYearDto>(200, result);
      }
      catch (Exception ex)
      {
        return new Data_Response<AcademicYearDto>(500, $"Server error: {ex.Message}");
      }
    }

    public async Task<List<AcademicYearDto>> GetAcademicYears()
    {
      try
      {
        var query = "SELECT * FROM AcademicYear";
        var academicYear = await _context.AcademicYears.FromSqlRaw(query).ToListAsync();

        if (academicYear is null)
        {
          throw new Exception("No content");
        }

        var result = academicYear.Select(x => new AcademicYearDto
        {
          AcademicYearId = x.AcademicYearId,
          DisplayAcademicYearName = x.DisplayAcademicYearName,
          YearStart = x.YearStart,
          YearEnd = x.YearEnd,
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

    public async Task<Data_Response<AcademicYearDto>> UpdateAcademicYear(int id, AcademicYearDto model)
    {
      try
      {
        // Check if exists in the database
        var findQuery = "SELECT * FROM AcademicYear WHERE academicYearId = @id";
        var existingAca = await _context.AcademicYears
            .FromSqlRaw(findQuery, new SqlParameter("@id", id))
            .FirstOrDefaultAsync();

        if (existingAca == null)
        {
          return new Data_Response<AcademicYearDto>(404, "AcademicYear not found");
        }

        // Build update query dynamically based on non-null fields
        var queryBuilder = new StringBuilder("UPDATE AcademicYear SET ");
        var parameters = new List<SqlParameter>();

        if (!string.IsNullOrEmpty(model.DisplayAcademicYearName))
        {
          queryBuilder.Append("DisplayAcademicYear_Name = @DisplayAcademicYear_Name, ");
          parameters.Add(new SqlParameter("@DisplayAcademicYear_Name", model.DisplayAcademicYearName));
        }

        queryBuilder.Append("YearStart = @YearStart, ");
        parameters.Add(new SqlParameter("@YearStart", model.YearStart));

        queryBuilder.Append("YearEnd = @YearEnd, ");
        parameters.Add(new SqlParameter("@YearEnd", model.YearEnd));

        if (!string.IsNullOrEmpty(model.Description))
        {
          queryBuilder.Append("Description = @Description, ");
          parameters.Add(new SqlParameter("@Description", model.Description));
        }
        // Remove trailing comma from the query if necessary
        if (queryBuilder[queryBuilder.Length - 2] == ',')
        {
          queryBuilder.Length -= 2;
        }

        queryBuilder.Append(" WHERE academicYearId = @id");
        parameters.Add(new SqlParameter("@id", id));

        // Execute the update query
        var updateQuery = queryBuilder.ToString();
        await _context.Database.ExecuteSqlRawAsync(updateQuery, parameters.ToArray());

        return new Data_Response<AcademicYearDto>(200, "AcademicYear updated successfully");
      }
      catch (Exception ex)
      {
        return new Data_Response<AcademicYearDto>(500, $"Server Error: {ex.Message}");
      }
    }
  }
}
