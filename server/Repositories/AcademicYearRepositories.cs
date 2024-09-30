using ExcelDataReader;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using server.Data;
using server.Dtos;
using server.Helpers;
using server.IService;
using System.Text;

namespace server.Repositories
{
  public class AcademicYearRepositories : IAcademicYear
  {
    private readonly SoDauBaiContext _context;

    public AcademicYearRepositories(SoDauBaiContext context)
    {
      this._context = context;
    }

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

    public async Task<List<AcademicYearDto>> GetAcademicYears(int pageNumber, int pageSize)
    {
      try
      {
        var skip = (pageNumber - 1) * pageSize;
        var query = @"SELECT * FROM AcademicYear 
                      ORDER BY ACADEMICYEARID
                      OFFSET @skip ROWS
                      FETCH NEXT @pageSize ROWS ONLY";

        var academicYear = await _context.AcademicYears
          .FromSqlRaw(query,
          new SqlParameter("@skip", skip),
          new SqlParameter("@pageSize", pageSize)
          ).ToListAsync() ?? throw new Exception("Empty");

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
        throw new Exception($"Server error: {ex.Message}");
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

                  var myAcademicYear = new Models.AcademicYear
                  {
                    DisplayAcademicYearName = reader.GetValue(1).ToString() ?? "null",
                    YearStart = ExcelHelper.ConvertExcelDateToDateOnly(reader.GetValue(2))
                    ?? DateOnly.FromDateTime(DateTime.Now),
                    YearEnd = ExcelHelper.ConvertExcelDateToDateOnly(reader.GetValue(3))
                    ?? DateOnly.FromDateTime(DateTime.Now),
                    Description = reader.GetValue(4).ToString() ?? "null"
                  };

                  await _context.AcademicYears.AddAsync(myAcademicYear);
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

        var deleteQuery = $"DELETE FROM AcademicYear WHERE AcademicYearId IN ({idList})";

        var delete = await _context.Database.ExecuteSqlRawAsync(deleteQuery);

        if (delete == 0)
        {
          return new Data_Response<string>(404, "No AcademicYearId found to delete");
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
