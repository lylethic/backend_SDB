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

    public async Task<List<PC_GiangDay_BiaSDBDto>> GetPC_GiangDay_BiaSDBs(int pageNumber, int pageSize)
    {
      try
      {
        var skip = (pageNumber - 1) * pageSize;

        var fetch = @"SELECT * FROM PhanCongGiangDay
                      ORDER BY BIASODAUBAIID 
                      OFFSET @skip ROWS
                      FETCH NEXT @pageSize ROWS ONLY";

        var phancongSBD = await _context.PhanCongGiangDays
          .FromSqlRaw(fetch,
                      new SqlParameter("@skip", skip),
                      new SqlParameter("@pageSize", pageSize)
          ).ToListAsync() ?? throw new Exception("Empty");

        var result = phancongSBD.Select(x => new PC_GiangDay_BiaSDBDto
        {
          PhanCongGiangDayId = x.PhanCongGiangDayId,
          TeacherId = x.TeacherId,
          biaSoDauBaiId = x.BiaSoDauBaiId,
          Status = x.Status,
          DateCreated = x.DateCreated,
          DateUpdated = x.DateUpdated
        }).ToList();

        return result;
      }
      catch (Exception ex)
      {
        throw new Exception($"Server error: {ex.Message}");
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

        var sqlInsert = @"INSERT INTO PhanCongGiangDay (TeacherId, biaSoDauBaiId, Status, DateCreated, DateUpdated)
                          VALUES (@TeacherId, @Status, @DateCreated, @DateUpdated);
                          SELECT CAST(SCOPE_IDENTITY() as int);";

        var insert = await _context.Database.ExecuteSqlRawAsync(sqlInsert,
          new SqlParameter("@TeacherId", model.TeacherId),
          new SqlParameter("@biaSoDauBaiId", model.biaSoDauBaiId),
          new SqlParameter("@Status", model.Status),
          new SqlParameter("@DateCreated", DateTime.Now),
          new SqlParameter("@DateUpdated", DBNull.Value)
          );

        var result = new PC_GiangDay_BiaSDBDto
        {
          PhanCongGiangDayId = insert,
          TeacherId = model.TeacherId,
          biaSoDauBaiId = model.biaSoDauBaiId,
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
          biaSoDauBaiId = phancongSDB.BiaSoDauBaiId,
          Status = phancongSDB.Status,
        };

        return new Data_Response<PC_GiangDay_BiaSDBDto>(200, result);
      }
      catch (Exception ex)
      {
        return new Data_Response<PC_GiangDay_BiaSDBDto>(500, $"Server Error: {ex.Message}");
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

                  // Check if there are no more rows or empty rows
                  if (reader.GetValue(1) == null && reader.GetValue(2) == null && reader.GetValue(3) == null)
                  {
                    // Stop processing when an empty row is encountered
                    break;
                  }

                  var myPhanCongGiangDay = new Models.PhanCongGiangDay
                  {
                    TeacherId = Convert.ToInt32(reader.GetValue(1)),
                    BiaSoDauBaiId = Convert.ToInt32(reader.GetValue(2)),
                    Status = Convert.ToBoolean(reader.GetValue(3)),
                    DateCreated = DateTime.Now,
                    DateUpdated = null
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

        var existingPhanCongGiangDay = await _context.PhanCongGiangDays
          .FromSqlRaw(find, new SqlParameter("@id", id))
          .FirstOrDefaultAsync();

        if (existingPhanCongGiangDay is null)
        {
          return new Data_Response<PC_GiangDay_BiaSDBDto>(404, "Not found");
        }

        bool hasChanges = false;

        var parameters = new List<SqlParameter>();
        var queryBuilder = new StringBuilder("UPDATE Class SET ");

        if (model.TeacherId != 0 && model.TeacherId != existingPhanCongGiangDay.TeacherId)
        {
          queryBuilder.Append("TeacherId = @TeacherId, ");
          parameters.Add(new SqlParameter("@TeacherId", model.TeacherId));
          hasChanges = true;
        }

        if (model.biaSoDauBaiId != 0 && model.biaSoDauBaiId != existingPhanCongGiangDay.BiaSoDauBaiId)
        {
          queryBuilder.Append("biaSoDauBaiId = @biaSoDauBaiId, ");
          parameters.Add(new SqlParameter("@biaSoDauBaiId", model.biaSoDauBaiId));
          hasChanges = true;
        }

        if (model.Status != existingPhanCongGiangDay.Status)
        {
          queryBuilder.Append("Status = @Status, ");
          parameters.Add(new SqlParameter("@Status", model.Status));
          hasChanges = true;
        }

        if (hasChanges)
        {

          if (queryBuilder[queryBuilder.Length - 2] == ',')
          {
            queryBuilder.Length -= 2;
          }

          queryBuilder.Append(" WHERE PhanCongGiangDayId = @id");
          parameters.Add(new SqlParameter("@id", id));

          var updateQuery = queryBuilder.ToString();
          await _context.Database.ExecuteSqlRawAsync(updateQuery, [.. parameters]);

          return new Data_Response<PC_GiangDay_BiaSDBDto>(200, "Updated");
        }
        else
        {
          return new Data_Response<PC_GiangDay_BiaSDBDto>(200, "No changes detected");
        }
      }
      catch (Exception ex)
      {
        return new Data_Response<PC_GiangDay_BiaSDBDto>(500, $"Server error: {ex.Message}");
      }
    }
  }
}
