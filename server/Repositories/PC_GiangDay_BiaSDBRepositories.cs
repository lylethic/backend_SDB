using ExcelDataReader;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using server.Data;
using server.Dtos;
using server.IService;
using server.Types.PhanCongGDBia;
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

    public async Task<PhanCongGiangDayBiaResType> GetPC_GiangDay_BiaSDBs(QueryObject queryObject)
    {
      try
      {
        queryObject ??= new QueryObject();
        var skip = (queryObject.PageNumber - 1) * queryObject.PageSize;

        // Show nhung GV nao day lop nao   
        var query = @"SELECT pc.phanCongGiangDayId, 
                      pc.biaSoDauBaiId, 
                      pc.teacherId, 
                      pc.status, 
                      pc.dateCreated, 
                      pc.dateUpdated, 
                      t.fullname,
                      c.className,
                      c.classId
                      FROM PhanCongGiangDay as pc
                      LEFT JOIN TEACHER AS T 
                      ON pc.teacherId = T.teacherId
                      LEFT JOIN CLASS AS C ON t.teacherId = c.teacherId
                      ORDER BY BIASODAUBAIID 
                      OFFSET @skip ROWS
                      FETCH NEXT @pageSize ROWS ONLY";

        var phancongSBD = await _context.PhanCongGiangDays
          .FromSqlRaw(query,
                      new SqlParameter("@skip", skip),
                      new SqlParameter("@pageSize", queryObject.PageSize)
          ).Select(static x => new
          {
            x.BiaSoDauBaiId,
            x.PhanCongGiangDayId,
            x.TeacherId,
            x.Status,
            x.DateCreated,
            x.DateUpdated,
            teacherName = x.Teacher.Fullname,
            classId = x.Teacher.Classes.First().ClassId,
            className = x.Teacher.Classes.First().ClassName,
          })
          .ToListAsync() ?? throw new Exception("Empty");

        if (!phancongSBD.Any())
        {
          return new PhanCongGiangDayBiaResType(400, "No data found");
        }

        var result = phancongSBD.Select(x => new MapData
        {
          PhanCongGiangDayId = x.PhanCongGiangDayId,
          TeacherId = x.TeacherId,
          BiaSoDauBaiId = x.BiaSoDauBaiId,
          Status = x.Status,
          DateCreated = x.DateCreated,
          DateUpdated = x.DateUpdated,
          ClassId = x.classId,
          ClassName = x.className,
          Fullname = x.teacherName
        }).ToList();

        return new PhanCongGiangDayBiaResType(200, "Thành công", result);
      }
      catch (Exception ex)
      {
        return new PhanCongGiangDayBiaResType(500, $"Server error: {ex.Message}");
      }
    }

    public async Task<PhanCongGiangDayBiaResType> BulkDelete(List<int> ids)
    {
      await using var transaction = await _context.Database.BeginTransactionAsync();
      try
      {
        if (ids == null || ids.Count == 0)
        {
          return new PhanCongGiangDayBiaResType(400, "No IDs provided");
        }


        // Create a comma-separated list of IDs for the SQL query
        var idList = string.Join(",", ids);

        // Prepare the delete query with parameterized input
        var deleteQuery = $"DELETE FROM PhanCongGiangDay WHERE PhanCongGiangDayId IN ({idList})";

        // Execute
        var affectedRows = await _context.Database.ExecuteSqlRawAsync(deleteQuery);

        if (affectedRows == 0)
        {
          return new PhanCongGiangDayBiaResType(404, "No ids found to delete");
        }

        await transaction.CommitAsync();

        return new PhanCongGiangDayBiaResType(200, "Deleted Thành côngy");
      }
      catch (Exception ex)
      {
        await transaction.RollbackAsync();
        return new PhanCongGiangDayBiaResType(500, $"Server error: {ex.Message}");
      }
    }

    public async Task<PhanCongGiangDayBiaResType> CreatePC_GiangDay_BiaSDB(PC_GiangDay_BiaSDBDto model)
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
          return new PhanCongGiangDayBiaResType(404, "Teacher Not found");
        }

        //check PC_GiangDay_BiaSDB
        var find = "SELECT * FROM PhanCongGiangDay WHERE PhanCongGiangDayId = @id";

        var getClass = await _context.PhanCongGiangDays
          .FromSqlRaw(find, new SqlParameter("@id", model.PhanCongGiangDayId))
          .FirstOrDefaultAsync();

        if (getClass is not null)
        {
          return new PhanCongGiangDayBiaResType(409, "PC_GiangDay_BiaSDB already exists");
        }

        var sqlInsert = @"INSERT INTO PhanCongGiangDay (TeacherId, biaSoDauBaiId, Status, DateCreated, DateUpdated)
                          VALUES (@TeacherId, @Status, @DateCreated, @DateUpdated);
                          SELECT CAST(SCOPE_IDENTITY() as int);";

        var insert = await _context.Database.ExecuteSqlRawAsync(sqlInsert,
          new SqlParameter("@TeacherId", model.TeacherId),
          new SqlParameter("@biaSoDauBaiId", model.BiaSoDauBaiId),
          new SqlParameter("@Status", model.Status),
          new SqlParameter("@DateCreated", DateTime.UtcNow),
          new SqlParameter("@DateUpdated", DBNull.Value)
          );

        var result = new PC_GiangDay_BiaSDBDto
        {
          PhanCongGiangDayId = insert,
          TeacherId = model.TeacherId,
          BiaSoDauBaiId = model.BiaSoDauBaiId,
          Status = model.Status,
        };

        return new PhanCongGiangDayBiaResType(200, "Thành công", result);
      }
      catch (Exception ex)
      {
        return new PhanCongGiangDayBiaResType(500, $"Server error: {ex.Message}");
      }
    }

    public async Task<PhanCongGiangDayBiaResType> DeletePC_GiangDay_BiaSDB(int id)
    {
      try
      {
        var find = "SELECT * FROM PhanCongGiangDay WHERE PhanCongGiangDayId = @id";
        var getClass = await _context.PhanCongGiangDays
          .FromSqlRaw(find, new SqlParameter("@id", id))
          .FirstOrDefaultAsync();

        if (getClass is null)
        {
          return new PhanCongGiangDayBiaResType(404, "Not found");
        }

        var deleteQuery = "DELETE FROM PhanCongGiangDay WHERE PhanCongGiangDayId = @id";

        await _context.Database.ExecuteSqlRawAsync(deleteQuery, new SqlParameter("@id", id));

        return new PhanCongGiangDayBiaResType(200, "Deleted");
      }
      catch (Exception ex)
      {
        return new PhanCongGiangDayBiaResType(500, $"Server error: {ex.Message}");
      }
    }

    public async Task<PhanCongGiangDayBiaResType> GetPC_GiangDay_BiaSDB(int id)
    {
      try
      {
        var find = "SELECT * FROM PhanCongGiangDay WHERE phanCongGiangDayId  = @id";
        var phancongSDB = await _context.PhanCongGiangDays
          .FromSqlRaw(find, new SqlParameter("@id", id))
          .FirstOrDefaultAsync();

        if (phancongSDB is null)
        {
          return new PhanCongGiangDayBiaResType(404, "Not found");
        }

        var result = new PC_GiangDay_BiaSDBDto
        {
          PhanCongGiangDayId = id,
          TeacherId = phancongSDB.TeacherId,
          BiaSoDauBaiId = phancongSDB.BiaSoDauBaiId,
          Status = phancongSDB.Status,
        };

        return new PhanCongGiangDayBiaResType(200, "Thành công", result);
      }
      catch (Exception ex)
      {
        return new PhanCongGiangDayBiaResType(500, $"Server Error: {ex.Message}");
      }
    }

    public async Task<PhanCongGiangDayBiaResType> ImportExcelFile(IFormFile file)
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
                    DateCreated = DateTime.UtcNow,
                    DateUpdated = null
                  };

                  await _context.PhanCongGiangDays.AddAsync(myPhanCongGiangDay);
                  await _context.SaveChangesAsync();
                }
              } while (reader.NextResult());
            }
          }

          return new PhanCongGiangDayBiaResType(200, "Tải lên thành công.");
        }

        return new PhanCongGiangDayBiaResType(400, "No file uploaded");
      }
      catch (Exception ex)
      {
        throw new Exception($"Error while uploading file: {ex.Message}");
      }
    }

    public async Task<PhanCongGiangDayBiaResType> UpdatePC_GiangDay_BiaSDB(int id, PC_GiangDay_BiaSDBDto model)
    {
      try
      {
        var find = "SELECT * FROM PhanCongGiangDay WHERE PhanCongGiangDayId = @id";

        var existingPhanCongGiangDay = await _context.PhanCongGiangDays
          .FromSqlRaw(find, new SqlParameter("@id", id))
          .FirstOrDefaultAsync();

        if (existingPhanCongGiangDay is null)
        {
          return new PhanCongGiangDayBiaResType(404, "Not found");
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

        if (model.BiaSoDauBaiId != 0 && model.BiaSoDauBaiId != existingPhanCongGiangDay.BiaSoDauBaiId)
        {
          queryBuilder.Append("biaSoDauBaiId = @biaSoDauBaiId, ");
          parameters.Add(new SqlParameter("@biaSoDauBaiId", model.BiaSoDauBaiId));
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

          return new PhanCongGiangDayBiaResType(200, "Updated");
        }
        else
        {
          return new PhanCongGiangDayBiaResType(200, "No changes detected");
        }
      }
      catch (Exception ex)
      {
        return new PhanCongGiangDayBiaResType(500, $"Server error: {ex.Message}");
      }
    }
  }
}
