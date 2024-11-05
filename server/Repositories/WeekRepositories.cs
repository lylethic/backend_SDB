using ExcelDataReader;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using server.Data;
using server.Dtos;
using server.IService;
using server.Models;
using server.Types.Week;
using System.Globalization;
using System.Text;

namespace server.Repositories
{
  public class WeekRepositories : IWeek
  {
    private readonly SoDauBaiContext _context;
    private readonly ILogger<WeekRepositories> _logger;

    public WeekRepositories(SoDauBaiContext context, ILogger<WeekRepositories> logger)
    {
      this._context = context ?? throw new ArgumentNullException(nameof(context));
      this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
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

        var deleteQuery = $"DELETE FROM Week WHERE WeekId IN ({idList})";

        var delete = await _context.Database.ExecuteSqlRawAsync(deleteQuery);

        if (delete == 0)
        {
          return new ResponseData<string>(404, "No WeekId found to delete");
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

    public async Task<ResponseData<WeekDto>> CreateWeek(WeekDto model)
    {
      try
      {
        var find = "SELECT * FROM Week WHERE WeekId = @id";

        var existingWeek = await _context.Weeks
          .FromSqlRaw(find, new SqlParameter("@id", model.WeekId))
          .FirstOrDefaultAsync();

        if (existingWeek is not null)
        {
          return new ResponseData<WeekDto>(409, "Week already exists");
        }

        var findSemester = "SELECT * FROM Semester WHERE SemesterId = @id";

        var semester = await _context.Semesters
          .FromSqlRaw(findSemester, new SqlParameter("@id", model.SemesterId))
          .FirstOrDefaultAsync();

        if (semester is null)
        {
          return new ResponseData<WeekDto>(409, "Semester not found");
        }

        var sqlInsert = @"INSERT INTO Week (SemesterId, WeekName, WeekStart, WeekEnd, Status) 
                          VALUES (@SemesterId, @WeekName, @WeekStart, @WeekEnd, @Status);
                          SELECT CAST(SCOPE_IDENTITY() as int);";

        var WeekStart = new DateTime(model.WeekStart.Year, model.WeekStart.Month, model.WeekStart.Day);
        var WeekEnd = new DateTime(model.WeekEnd.Year, model.WeekEnd.Month, model.WeekEnd.Day);

        var insert = await _context.Database.ExecuteSqlRawAsync(sqlInsert,
          new SqlParameter("@SemesterId", model.SemesterId),
          new SqlParameter("@WeekName", model.WeekName),
          new SqlParameter("@WeekStart", WeekStart),
          new SqlParameter("@WeekEnd", WeekEnd),
          new SqlParameter("@status", model.Status)
        );

        var result = new WeekDto
        {
          WeekId = insert,
          SemesterId = model.SemesterId,
          WeekName = model.WeekName,
          WeekStart = model.WeekStart,
          WeekEnd = model.WeekEnd,
          Status = model.Status,
        };

        return new ResponseData<WeekDto>(200, result);
      }
      catch (Exception ex)
      {
        return new ResponseData<WeekDto>(200, $"Server error: {ex.Message}");
      }
    }

    public async Task<ResponseData<WeekDto>> DeleteWeek(int id)
    {
      try
      {
        var findSemester = "SELECT * FROM Semester WHERE SemesterId = @id";

        var semester = await _context.Semesters
          .FromSqlRaw(findSemester, new SqlParameter("@id", id))
          .FirstOrDefaultAsync();

        if (semester is null)
        {
          return new ResponseData<WeekDto>(404, "Semester not found");
        }

        var deleteQuery = "DELETE FROM Week WHERE WeekId = @id";

        await _context.Database.ExecuteSqlRawAsync(deleteQuery, new SqlParameter("@id", id));

        return new ResponseData<WeekDto>(200, "A Week Deleted");
      }
      catch (Exception ex)
      {
        return new ResponseData<WeekDto>(500, $"Server error: {ex.Message}");
      }
    }

    public async Task<ResponseData<WeekData>> GetWeek(int id)
    {
      try
      {
        var find = @"SELECT *
                      FROM WEEK as W INNER JOIN Semester as A ON W.semesterId = A.semesterId 
                      WHERE W.weekId = @id;";

        var week = await _context.Weeks
          .FromSqlRaw(find, new SqlParameter("@id", id))
          .Select(static x => new Week
          {
            WeekId = x.WeekId,
            WeekName = x.WeekName,
            WeekStart = x.WeekStart,
            WeekEnd = x.WeekEnd,
            Status = x.Status,
            SemesterId = x.SemesterId,
            Semester = new Semester
            {
              SemesterName = x.Semester.SemesterName,
              DateStart = x.Semester.DateStart,
              DateEnd = x.Semester.DateEnd,
            }
          })
          .FirstOrDefaultAsync();

        if (week is null)
        {
          return new ResponseData<WeekData>(404, "Week not found");
        }

        if (!week.Status)
        {
          return new ResponseData<WeekData>(400, "Week is inactive.This week are locked.");
        }
        else
        {
          var result = new WeekData
          {
            WeekId = id,
            WeekName = week.WeekName,
            WeekStart = week.WeekStart,
            WeekEnd = week.WeekEnd,
            Status = week.Status,
            SemesterId = week.SemesterId,
            SemesterName = week.Semester.SemesterName,
            DateStart = week.Semester.DateStart,
            DateEnd = week.Semester.DateEnd
          };

          return new ResponseData<WeekData>(200, result);
        }
      }
      catch (Exception ex)
      {
        return new ResponseData<WeekData>(500, $"Server error: {ex.Message}");
      }
    }

    public async Task<List<WeekDto>> GetWeeks(int pageNumber, int pageSize)
    {
      try
      {
        var skip = (pageNumber - 1) * pageSize;

        var query = @"SELECT * FROM Week
                      ORDER BY WeekId
                      OFFSET @skip ROWS
                      FETCH NEXT @pageSize ROWS ONLY";

        var weeks = await _context.Weeks
          .FromSqlRaw(query,
          new SqlParameter("@skip", skip),
          new SqlParameter("@pageSize", pageSize)
          ).ToListAsync() ?? throw new Exception("Empty");

        var result = weeks.Select(x => new WeekDto
        {
          WeekId = x.WeekId,
          SemesterId = x.SemesterId,
          WeekName = x.WeekName,
          WeekStart = x.WeekStart,
          WeekEnd = x.WeekEnd,
          Status = x.Status
        }).ToList();

        return result;
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
        throw new Exception($"Server error: {ex.Message}");
      }
    }

    public async Task<List<WeekDto>> GetWeeksBySemester(int pageNumber, int pageSize, int semesterId)
    {
      try
      {
        var skip = (pageNumber - 1) * pageSize;

        var query = @"SELECT * FROM Week
                      WHERE SEMESTERID = @semesterId
                      ORDER BY WeekId
                      OFFSET @skip ROWS
                      FETCH NEXT @pageSize ROWS ONLY";

        var weeks = await _context.Weeks
          .FromSqlRaw(query,
          new SqlParameter("@skip", skip),
          new SqlParameter("@pageSize", pageSize),
          new SqlParameter("@semesterId", semesterId)
          ).ToListAsync() ?? throw new Exception("Empty");

        var result = weeks.Select(x => new WeekDto
        {
          WeekId = x.WeekId,
          SemesterId = x.SemesterId,
          WeekName = x.WeekName,
          WeekStart = x.WeekStart,
          WeekEnd = x.WeekEnd,
        }).ToList();

        return result;
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
        throw new Exception($"Server error: {ex.Message}");
      }
    }

    public async Task<string> ImportExcelFile(IFormFile file)
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

                // Log each value before processing
                Console.WriteLine($"SemesterId: {reader.GetValue(1)}");
                Console.WriteLine($"WeekName: {reader.GetValue(2)}");
                Console.WriteLine($"WeekStart: {reader.GetValue(3)}");
                Console.WriteLine($"WeekEnd: {reader.GetValue(4)}");
                Console.WriteLine($"Status: {reader.GetValue(5)}");

                // Check if there are no more rows or empty rows
                if (reader.GetValue(1) == null && reader.GetValue(2) == null && reader.GetValue(3) == null && reader.GetValue(4) == null && reader.GetValue(5) == null)
                {
                  // Stop processing when an empty row is encountered
                  break;
                }

                var myWeek = new Models.Week
                {
                  SemesterId = reader.GetValue(1) != null ? Convert.ToInt16(reader.GetValue(1)) : 0,  // Provide default value or handle appropriately
                  WeekName = reader.GetValue(2)?.ToString() ?? "null",
                  WeekStart = reader.GetValue(3) != null ? DateOnly.FromDateTime((DateTime)reader.GetValue(3)) : DateOnly.MinValue,  // Handle potential null
                  WeekEnd = reader.GetValue(4) != null ? DateOnly.FromDateTime((DateTime)reader.GetValue(4)) : DateOnly.MinValue,    // Handle potential null
                  Status = reader.GetValue(5) != null && Convert.ToBoolean(reader.GetValue(5))  // Handle potential null
                };


                await _context.Weeks.AddAsync(myWeek);
                await _context.SaveChangesAsync();
              }
            } while (reader.NextResult());
          }

          return "Successfully inserted";
        }
        return "No file uploaded";
      }
      catch (DbUpdateException dbEx)
      {
        throw new Exception($"Error while uploading file: {dbEx.Message}");
      }
    }

    public async Task<ResponseData<WeekDto>> UpdateWeek(int id, WeekDto model)
    {
      try
      {
        var findWeek = "SELECT * FROM WEEK WHERE WEEKID = @id";

        var existingWeek = await _context.Weeks
          .FromSqlRaw(findWeek, new SqlParameter("@id", id))
          .FirstOrDefaultAsync();

        if (existingWeek is null)
        {
          return new ResponseData<WeekDto>(404, "Week not found");
        }

        bool hasChanges = false;

        var parameters = new List<SqlParameter>();
        var queryBuilder = new StringBuilder("UPDATE Week SET ");

        if (model.SemesterId != 0 && model.SemesterId != existingWeek.SemesterId)
        {
          queryBuilder.Append("SemesterId = @SemesterId, ");
          parameters.Add(new SqlParameter("@SemesterId", model.SemesterId));
          hasChanges = true;
        }

        if (!string.IsNullOrEmpty(model.WeekName) && model.WeekName != existingWeek.WeekName)
        {
          queryBuilder.Append("WeekName = @WeekName, ");
          parameters.Add(new SqlParameter("@WeekName", model.WeekName));
          hasChanges = true;
        }

        if (model.WeekStart != existingWeek.WeekStart)
        {
          queryBuilder.Append("WeekStart = @WeekStart, ");
          parameters.Add(new SqlParameter("@WeekStart", model.WeekStart));
          hasChanges = true;
        }

        if (model.WeekEnd != existingWeek.WeekEnd)
        {
          queryBuilder.Append("WeekEnd = @WeekEnd, ");
          parameters.Add(new SqlParameter("@WeekEnd", model.WeekEnd));
          hasChanges = true;
        }

        if (hasChanges)
        {
          if (queryBuilder[queryBuilder.Length - 2] == ',')
            queryBuilder.Length -= 2;

          queryBuilder.Append(" WHERE WeekId = @id");
          parameters.Add(new SqlParameter("@id", id));

          // Execute the update query
          var updateQuery = queryBuilder.ToString();
          await _context.Database.ExecuteSqlRawAsync(updateQuery, [.. parameters]);
          return new ResponseData<WeekDto>(200, "Updated");

        }
        else
        {
          return new ResponseData<WeekDto>(200, "No changes detected");
        }
      }
      catch (Exception ex)
      {
        return new ResponseData<WeekDto>(500, $"Server Error: {ex.Message}");
      }
    }

    public async Task<WeekResType> Get7DaysInWeek(int selectedWeekId)
    {
      var selectedWeek = await _context.Weeks.FirstOrDefaultAsync(x => x.WeekId == selectedWeekId);
      if (selectedWeek == null)
      {
        return new WeekResType("Không tìm thấy");
      }

      var daysInWeek = new List<SevenDaysInWeek>();
      var currentDate = selectedWeek.WeekStart;

      for (int i = 0; i < 7; i++)
      {
        // Tính toán ngày
        daysInWeek.Add(new SevenDaysInWeek
        {
          Day = currentDate.ToString("dddd", new CultureInfo("vi-VN")),
          Date = currentDate.ToString("dd/MM/yyyy") // Định dạng ngày
        });

        currentDate = currentDate.AddDays(1); // Thêm một ngày
      }

      return new WeekResType("Thành công", daysInWeek);
    }
  }
}
