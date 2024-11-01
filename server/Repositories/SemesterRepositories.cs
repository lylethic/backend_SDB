using ExcelDataReader;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using server.Data;
using server.Dtos;
using server.Helpers;
using server.IService;
using server.Models;
using server.Types;
using System.Text;

namespace server.Repositories
{
    public class SemesterRepositories : ISemester
  {
    readonly SoDauBaiContext _context;

    public SemesterRepositories(SoDauBaiContext context) { this._context = context; }

    public async Task<ResponseData<SemesterDto>> CreateSemester(SemesterDto model)
    {
      try
      {
        var find = "SELECT * FROM Semester WHERE SemesterId = @id";

        var academicYear = await _context.Semesters
          .FromSqlRaw(find, new SqlParameter("@id", model.SemesterId))
          .FirstOrDefaultAsync();

        if (academicYear is not null)
        {
          return new ResponseData<SemesterDto>(409, "Semester already exists");
        }

        var sqlInsert = @"INSERT INTO Semester (academicYearId, semesterName, dateStart, dateEnd, description) 
                          VALUES (@academicYearId, @semesterName, @dateStart, @dateEnd, @description);
                          SELECT CAST(SCOPE_IDENTITY() as int);"
        ;

        // DateOnly expects a specific format (ISO 8601, e.g., yyyy-MM-dd) to be correctly parsed during JSON deserialization. 
        var dateStart = new DateTime(model.DateStart.Year, model.DateStart.Month, model.DateStart.Day);
        var dateEnd = new DateTime(model.DateEnd.Year, model.DateEnd.Month, model.DateEnd.Day);

        var insert = await _context.Database.ExecuteSqlRawAsync(sqlInsert,
          new SqlParameter("@academicYearId", model.AcademicYearId),
          new SqlParameter("@semesterName", model.SemesterName),
          new SqlParameter("@dateStart", dateStart),
          new SqlParameter("@dateEnd", dateEnd),
          new SqlParameter("@description", model.Description)
        );

        var result = new SemesterDto
        {
          SemesterId = insert,
          AcademicYearId = model.AcademicYearId,
          SemesterName = model.SemesterName,
          DateStart = model.DateStart,
          DateEnd = model.DateEnd,
          Description = model.Description
        };

        return new ResponseData<SemesterDto>(200, result);
      }
      catch (Exception ex)
      {
        return new ResponseData<SemesterDto>(200, $"Server error: {ex.Message}");
      }
    }

    public async Task<ResponseData<SemesterDto>> DeleteSemester(int id)
    {
      try
      {
        var find = "SELECT * FROM Semester WHERE SemesterId = @id";
        var semester = await _context.Semesters
          .FromSqlRaw(find, new SqlParameter("@id", id))
          .FirstOrDefaultAsync();

        if (semester is null)
        {
          return new ResponseData<SemesterDto>(404, "Semester not found");
        }

        var deleteQuery = "DELETE FROM Semester WHERE SemesterId = @id";
        await _context.Database.ExecuteSqlRawAsync(deleteQuery, new SqlParameter("@id", id));

        return new ResponseData<SemesterDto>(200, "Deleted");
      }
      catch (Exception ex)
      {
        return new ResponseData<SemesterDto>(500, $"Server error: {ex.Message}");
      }
    }

    public async Task<ResponseData<SemesterResType>> GetSemester(int id)
    {
      try
      {
        var find = @"SELECT s.semesterId, 
				                    s.semesterName, 
				                    s.dateStart, 
				                    s.dateEnd,
				                    a.academicYearId, a.displayAcademicYear_Name, 
				                    a.yearStart, 
				                    a.yearEnd
                    FROM Semester s INNER JOIN
                    AcademicYear A ON S.academicYearId = A.academicYearId
                    WHERE s.SemesterId = @id";

        var semester = await _context.Semesters
          .FromSqlRaw(find, new SqlParameter("@id", id))
          .Select(static x => new Semester
          {
            SemesterId = x.SemesterId,
            SemesterName = x.SemesterName,
            DateStart = x.DateStart,
            DateEnd = x.DateEnd,
            AcademicYearId = x.AcademicYearId,
            AcademicYear = new AcademicYear
            {
              DisplayAcademicYearName = x.AcademicYear.DisplayAcademicYearName,
              YearStart = x.AcademicYear.YearStart,
              YearEnd = x.AcademicYear.YearEnd,
            }
          })
          .FirstOrDefaultAsync();

        if (semester is null)
        {
          return new ResponseData<SemesterResType>(404, "Semester not found");
        }

        var result = new SemesterResType
        {
          SemesterId = id,
          SemesterName = semester.SemesterName,
          DateStart = semester.DateStart,
          DateEnd = semester.DateEnd,
          AcademicYearId = semester.AcademicYearId,
          DisplayAcademicYearName = semester.AcademicYear.DisplayAcademicYearName,
          YearStart = semester.AcademicYear.YearStart,
          YearEnd = semester.AcademicYear.YearEnd
        };

        return new ResponseData<SemesterResType>(200, result);
      }
      catch (Exception ex)
      {
        return new ResponseData<SemesterResType>(500, $"Server error: {ex.Message}");
      }
    }

    public async Task<List<SemesterDto>> GetSemesters(int pageNumber, int pageSize)
    {
      try
      {
        var skip = (pageNumber - 1) * pageSize;

        var query = @"SELECT * FROM Semester 
                      ORDER BY SEMESTERNAME
                      OFFSET @skip ROWS
                      FETCH NEXT @pageSize ROWS ONLY;";

        var semester = await _context.Semesters.FromSqlRaw(query,
              new SqlParameter("@skip", skip),
              new SqlParameter("@pageSize", pageSize))
          .ToListAsync();

        var result = semester.Select(x => new SemesterDto
        {
          SemesterId = x.SemesterId,
          SemesterName = x.SemesterName,
          AcademicYearId = x.AcademicYearId,
          DateStart = x.DateStart,
          DateEnd = x.DateEnd,
          Description = x.Description
        }).ToList();

        return result;
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
        throw new Exception($"Error: {ex.Message}");
      }
    }

    public async Task<ResponseData<SemesterDto>> UpdateSemester(int id, SemesterDto model)
    {
      try
      {
        // Check if the teacher exists in the database
        var findQuery = "SELECT * FROM Semester WHERE semesterId = @id";
        var existingSemester = await _context.Semesters
            .FromSqlRaw(findQuery, new SqlParameter("@id", id))
            .FirstOrDefaultAsync();

        if (existingSemester is null)
        {
          return new ResponseData<SemesterDto>(404, "Semester not found");
        }

        bool hasChanges = false;

        var parameters = new List<SqlParameter>();
        var queryBuilder = new StringBuilder("UPDATE Semester SET ");

        if (model.AcademicYearId != 0 && model.AcademicYearId != existingSemester.AcademicYearId)
        {
          queryBuilder.Append("AcademicYearId = @AcademicYearId, ");
          parameters.Add(new SqlParameter("@AcademicYearId", model.AcademicYearId));
          hasChanges = true;
        }

        if (!string.IsNullOrEmpty(model.SemesterName) && model.SemesterName != existingSemester.SemesterName)
        {
          queryBuilder.Append("SemesterName = @SemesterName, ");
          parameters.Add(new SqlParameter("@SemesterName", model.SemesterName));
          hasChanges = true;
        }

        if (model.DateStart != existingSemester.DateStart)
        {
          queryBuilder.Append("DateStart = @DateStart, ");
          parameters.Add(new SqlParameter("@DateStart", model.DateStart));
          hasChanges = true;
        }

        if (model.DateEnd != existingSemester.DateEnd)
        {
          queryBuilder.Append("DateEnd = @DateEnd, ");
          parameters.Add(new SqlParameter("@DateEnd", model.DateEnd));
          hasChanges = true;
        }

        if (!string.IsNullOrEmpty(model.Description) && model.Description != existingSemester.Description)
        {
          queryBuilder.Append("Description = @Description, ");
          parameters.Add(new SqlParameter("@Description", model.Description));
          hasChanges = true;
        }

        if (hasChanges)
        {
          if (queryBuilder[queryBuilder.Length - 2] == ',')
          {
            queryBuilder.Length -= 2;
          }

          queryBuilder.Append(" WHERE semesterId = @id");
          parameters.Add(new SqlParameter("@id", id));

          // Execute the update query
          var updateQuery = queryBuilder.ToString();
          await _context.Database.ExecuteSqlRawAsync(updateQuery, parameters.ToArray());

          return new ResponseData<SemesterDto>(200, "Semester updated successfully");
        }
        else
        {
          return new ResponseData<SemesterDto>(200, "No changes detected");
        }
      }
      catch (Exception ex)
      {
        return new ResponseData<SemesterDto>(500, $"Server Error: {ex.Message}");
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
                if (reader.GetValue(1) == null && reader.GetValue(2) == null && reader.GetValue(3) == null && reader.GetValue(4) == null && reader.GetValue(5) == null)
                {
                  // Stop processing when an empty row is encountered
                  break;
                }

                var mySemester = new Models.Semester
                {
                  AcademicYearId = Convert.ToInt16(reader.GetValue(1)),
                  SemesterName = reader.GetValue(2).ToString() ?? "Semester name",
                  DateStart = ExcelHelper.ConvertExcelDateToDateOnly(reader.GetValue(3))
                  ?? DateOnly.FromDateTime(DateTime.UtcNow),
                  DateEnd = ExcelHelper.ConvertExcelDateToDateOnly(reader.GetValue(4))
                  ?? DateOnly.FromDateTime(DateTime.UtcNow),
                  Description = reader.GetValue(5).ToString()
                };

                await _context.Semesters.AddAsync(mySemester);
                await _context.SaveChangesAsync();
              }
            } while (reader.NextResult());
          }

          return "Successfully.";
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

        var deleteQuery = $"DELETE FROM SEMESTER WHERE SEMESTERID IN ({idList})";

        var delete = await _context.Database.ExecuteSqlRawAsync(deleteQuery);

        if (delete == 0)
        {
          return new ResponseData<string>(404, "No Semester found to delete");
        }

        await transaction.CommitAsync();

        return new ResponseData<string>(200, "Deleted");
      }
      catch (Exception ex)
      {
        await transaction.RollbackAsync();
        return new ResponseData<string>(500, $"Server error: {ex.Message}");
      }
    }
  }
}
