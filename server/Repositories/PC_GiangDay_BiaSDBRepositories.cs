using ExcelDataReader;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using server.Data;
using server.Dtos;
using server.IService;
using System.Text;

namespace server.Repositories
{
  public class PC_GiangDay_BiaSDBRepositories : IPC_GiangDay_BiaSDB
  {
    private readonly SoDauBaiContext _context;

    public PC_GiangDay_BiaSDBRepositories(SoDauBaiContext context)
    {
      this._context = context;
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
        var deleteQuery = $"DELETE FROM PhanCongGiangDay WHERE PhanCongGiangDayId IN ({idList})";

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

    public async Task<Data_Response<PC_GiangDay_BiaSDBDto>> CreatePC_GiangDay_BiaSDB(PC_GiangDay_BiaSDBDto model)
    {
      try
      {
        // check teacher
        var findTeacher = "SELECT * FROM Teacher WHERE teacherId = @id";
        var teacherExists = await _context.Teachers
          .FromSqlRaw(findTeacher, new SqlParameter("@id", model.TeacherId))
          .FirstOrDefaultAsync();

        if (teacherExists is null)
        {
          return new Data_Response<PC_GiangDay_BiaSDBDto>(404, "Teacher Not found");
        }

        //check PC_GiangDay_BiaSDB
        var find = "SELECT * FROM PhanCongGiangDay WHERE PhanCongGiangDayId = @id";

        var getClass = await _context.PhanCongGiangDays
          .FromSqlRaw(find, new SqlParameter("@id", model.PhanCongGiangDayId))
          .FirstOrDefaultAsync();

        if (getClass is not null)
        {
          return new Data_Response<PC_GiangDay_BiaSDBDto>(409, "PC_GiangDay_BiaSDB already exists");
        }

        var sqlInsert = @"INSERT INTO PhanCongGiangDay (TeacherId, Status)
                          VALUES (@TeacherId, @Status);
                          SELECT CAST(SCOPE_IDENTITY() as int);";

        var insert = await _context.Database.ExecuteSqlRawAsync(sqlInsert,
          new SqlParameter("@TeacherId", model.TeacherId),
          new SqlParameter("@Status", model.Status)
          );

        var result = new PC_GiangDay_BiaSDBDto
        {
          PhanCongGiangDayId = insert,
          TeacherId = model.TeacherId,
          Status = model.Status,
        };

        return new Data_Response<PC_GiangDay_BiaSDBDto>(200, result);
      }
      catch (Exception ex)
      {
        return new Data_Response<PC_GiangDay_BiaSDBDto>(500, $"Server error: {ex.Message}");
      }
    }

    public async Task<Data_Response<PC_GiangDay_BiaSDBDto>> DeletePC_GiangDay_BiaSDB(int id)
    {
      try
      {
        var find = "SELECT * FROM PhanCongGiangDay WHERE PhanCongGiangDayId = @id";
        var getClass = await _context.PhanCongGiangDays
          .FromSqlRaw(find, new SqlParameter("@id", id))
          .FirstOrDefaultAsync();

        if (getClass is null)
        {
          return new Data_Response<PC_GiangDay_BiaSDBDto>(404, "Not found");
        }

        var deleteQuery = "DELETE FROM PhanCongGiangDay WHERE PhanCongGiangDayId = @id";

        await _context.Database.ExecuteSqlRawAsync(deleteQuery, new SqlParameter("@id", id));

        return new Data_Response<PC_GiangDay_BiaSDBDto>(200, "Deleted");
      }
      catch (Exception ex)
      {
        return new Data_Response<PC_GiangDay_BiaSDBDto>(500, $"Server error: {ex.Message}");
      }
    }

    public async Task<Data_Response<PC_GiangDay_BiaSDBDto>> GetPC_GiangDay_BiaSDB(int id)
    {
      try
      {
        var find = "SELECT * FROM PhanCongGiangDay WHERE phanCongGiangDayId  = @id";
        var phancongSDB = await _context.PhanCongGiangDays
          .FromSqlRaw(find, new SqlParameter("@id", id))
          .FirstOrDefaultAsync();

        if (phancongSDB is null)
        {
          return new Data_Response<PC_GiangDay_BiaSDBDto>(404, "Not found");
        }

        var result = new PC_GiangDay_BiaSDBDto
        {
          PhanCongGiangDayId = id,
          TeacherId = phancongSDB.TeacherId,
          Status = phancongSDB.Status,
        };

        return new Data_Response<PC_GiangDay_BiaSDBDto>(200, result);
      }
      catch (Exception ex)
      {
        return new Data_Response<PC_GiangDay_BiaSDBDto>(500, $"Server Error: {ex.Message}");
      }
    }

    public async Task<List<PC_GiangDay_BiaSDBDto>> GetPC_GiangDay_BiaSDBs()
    {
      try
      {
        var find = "SELECT * FROM PhanCongGiangDay";
        var phancongSBD = await _context.PhanCongGiangDays
          .FromSqlRaw(find)
          .ToListAsync();

        if (phancongSBD is null)
        {
          throw new Exception("Empty");
        }

        var result = phancongSBD.Select(x => new PC_GiangDay_BiaSDBDto
        {
          PhanCongGiangDayId = x.PhanCongGiangDayId,
          TeacherId = x.TeacherId,
          Status = x.Status,
        }).ToList();

        return result;
      }
      catch (Exception ex)
      {
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

                  var myPhanCongGiangDay = new Models.PhanCongGiangDay
                  {
                    TeacherId = Convert.ToInt32(reader.GetValue(1)),
                    Status = Convert.ToBoolean(reader.GetValue(2)),
                  };

                  await _context.PhanCongGiangDays.AddAsync(myPhanCongGiangDay);
                  await _context.SaveChangesAsync();
                }
              } while (reader.NextResult());
            }
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

    public async Task<Data_Response<PC_GiangDay_BiaSDBDto>> UpdatePC_GiangDay_BiaSDB(int id, PC_GiangDay_BiaSDBDto model)
    {
      try
      {
        var find = "SELECT * FROM PhanCongGiangDay WHERE PhanCongGiangDayId = @id";

        var getPhanCongGiangDay = await _context.PhanCongGiangDays
          .FromSqlRaw(find, new SqlParameter("@id", id))
          .FirstOrDefaultAsync();

        if (getPhanCongGiangDay is null)
        {
          return new Data_Response<PC_GiangDay_BiaSDBDto>(404, "Not found");
        }

        var queryBuilder = new StringBuilder("UPDATE Class SET ");
        var parameters = new List<SqlParameter>();

        if (model.TeacherId != 0)
        {
          queryBuilder.Append("TeacherId = @TeacherId, ");
          parameters.Add(new SqlParameter("@TeacherId", model.TeacherId));

          queryBuilder.Append("Status = @Status, ");
          parameters.Add(new SqlParameter("@Status", model.Status));
        }

        if (queryBuilder[queryBuilder.Length - 2] == ',')
        {
          queryBuilder.Length -= 2;
        }

        queryBuilder.Append(" WHERE PhanCongGiangDayId = @id");
        parameters.Add(new SqlParameter("@id", id));

        var updateQuery = queryBuilder.ToString();
        await _context.Database.ExecuteSqlRawAsync(updateQuery, parameters.ToArray());

        return new Data_Response<PC_GiangDay_BiaSDBDto>(200, "Updated");
      }
      catch (Exception ex)
      {
        return new Data_Response<PC_GiangDay_BiaSDBDto>(500, $"Server error: {ex.Message}");
      }
    }
  }
}
