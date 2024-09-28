using ExcelDataReader;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using server.Data;
using server.Dtos;
using server.IService;
using System.Text;

namespace server.Repositories
{
  public class BiaSoDauBaiRepositories : IBiaSoDauBai
  {
    readonly SoDauBaiContext _context;

    public BiaSoDauBaiRepositories(SoDauBaiContext context) { this._context = context; }

    public async Task<Data_Response<BiaSoDauBaiDto>> CreateBiaSoDauBai(BiaSoDauBaiDto model)
    {
      using (var transaction = await _context.Database.BeginTransactionAsync())
      {
        try
        {
          var find = "SELECT * FROM BiaSoDauBai WHERE BiaSoDauBaiId = @id";
          var sodaubai = await _context.BiaSoDauBais
              .FromSqlRaw(find, new SqlParameter("@id", model.BiaSoDauBaiId))
              .FirstOrDefaultAsync();

          if (sodaubai is not null)
          {
            return new Data_Response<BiaSoDauBaiDto>(409, "Sodaubai already exists");
          }

          model.DateCreated = DateOnly.FromDateTime(DateTime.Now);
          model.DateUpdated = null;

          var queryInsert = @"INSERT INTO BiaSoDauBai (schoolId, academicYearId, classId, status, dateCreated, dateUpdated)
                                VALUES (@schoolId, @academicYearId, @classId, @status, @dateCreated, @dateUpdated);
                                SELECT CAST(SCOPE_IDENTITY() AS INT);";

          var insert = await _context.Database.ExecuteSqlRawAsync(queryInsert,
              new SqlParameter("@schoolId", model.SchoolId),
              new SqlParameter("@academicYearId", model.AcademicyearId),
              new SqlParameter("@classId", model.ClassId),
              new SqlParameter("@status", model.Status),
              new SqlParameter("@dateCreated", model.DateCreated),
              new SqlParameter("@dateUpdated", DBNull.Value)
          );

          // Commit the transaction after the insert succeeds
          await transaction.CommitAsync();

          var result = new BiaSoDauBaiDto
          {
            SchoolId = model.SchoolId,
            AcademicyearId = model.AcademicyearId,
            ClassId = model.ClassId,
            Status = model.Status,
            DateCreated = model.DateCreated,
            DateUpdated = model.DateUpdated,
          };

          return new Data_Response<BiaSoDauBaiDto>(200, result);
        }
        catch (Exception ex)
        {
          // Rollback the transaction if any exception occurs
          await transaction.RollbackAsync();
          return new Data_Response<BiaSoDauBaiDto>(500, $"Server Error: {ex.Message}");
        }
      }
    }

    public async Task<Data_Response<BiaSoDauBaiDto>> GetBiaSoDauBai(int id)
    {
      try
      {
        var query = @"SELECT * from BiaSoDauBai WHERE BiaSoDauBaiId = @id";

        // Fetch 
        var sodaubai = await _context.BiaSoDauBais
            .FromSqlRaw(query, new SqlParameter("@id", id))
            .FirstOrDefaultAsync();

        if (sodaubai is null)
        {
          return new Data_Response<BiaSoDauBaiDto>(404, "Not found");
        }

        // Map the result to the StudentDto
        var result = new BiaSoDauBaiDto
        {
          BiaSoDauBaiId = id,
          SchoolId = sodaubai.SchoolId,
          AcademicyearId = sodaubai.AcademicyearId,
          ClassId = sodaubai.ClassId,
          Status = sodaubai.Status,
          DateCreated = sodaubai.DateCreated,
          DateUpdated = sodaubai.DateUpdated,
        };

        return new Data_Response<BiaSoDauBaiDto>(200, result);
      }
      catch (Exception ex)
      {
        return new Data_Response<BiaSoDauBaiDto>(500, $"Server error: {ex.Message}");
      }
    }

    public async Task<List<BiaSoDauBaiDto>> GetBiaSoDauBais(int pageNumber, int pageSize)
    {
      try
      {
        var skip = (pageNumber - 1) * pageSize;

        var roles = await _context.BiaSoDauBais
            .Skip(skip)     // Skip the first (pageNumber - 1) * pageSize records
            .Take(pageSize) // Take pageSize records
            .ToListAsync();

        var result = roles.Select(x => new BiaSoDauBaiDto
        {
          BiaSoDauBaiId = x.BiaSoDauBaiId,
          SchoolId = x.SchoolId,
          AcademicyearId = x.AcademicyearId,
          ClassId = x.ClassId,
          Status = x.Status,
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

    public async Task<Data_Response<BiaSoDauBaiDto>> UpdateBiaSoDauBai(int id, BiaSoDauBaiDto model)
    {
      using (var transaction = await _context.Database.BeginTransactionAsync())
      {
        try
        {
          var find = "SELECT * FROM BiaSoDauBai WHERE BiaSoDauBaiId = @id";
          var exists = await _context.BiaSoDauBais
              .FromSqlRaw(find, new SqlParameter("@id", id))
              .FirstOrDefaultAsync();

          if (exists == null)
          {
            return new Data_Response<BiaSoDauBaiDto>(404, "BiaSoDauBaiId not found");
          }

          var queryBuilder = new StringBuilder("UPDATE BiaSoDauBai SET ");
          var parameters = new List<SqlParameter>();

          if (model.SchoolId != 0)
          {
            queryBuilder.Append("SchoolId = @SchoolId, ");
            parameters.Add(new SqlParameter("@SchoolId", model.SchoolId));
          }

          if (model.AcademicyearId != 0)
          {
            queryBuilder.Append("AcademicyearId = @AcademicyearId, ");
            parameters.Add(new SqlParameter("@AcademicyearId", model.AcademicyearId));
          }

          if (model.ClassId != 0)
          {
            queryBuilder.Append("ClassId = @ClassId, ");
            parameters.Add(new SqlParameter("@ClassId", model.ClassId));
          }

          queryBuilder.Append("Status = @Status, ");
          parameters.Add(new SqlParameter("@Status", model.Status));

          // Always update the dateUpdated field to current date
          var currentDate = DateOnly.FromDateTime(DateTime.Now);
          queryBuilder.Append("DateUpdated = @DateUpdated, ");
          parameters.Add(new SqlParameter("@DateUpdated", currentDate));

          // Remove the last comma and space
          if (queryBuilder.Length > 0)
          {
            queryBuilder.Length -= 2;
          }

          queryBuilder.Append(" WHERE BiaSoDauBaiId = @id");
          parameters.Add(new SqlParameter("@id", id));

          var updateQuery = queryBuilder.ToString();
          await _context.Database.ExecuteSqlRawAsync(updateQuery, parameters.ToArray());

          // Commit the transaction
          await transaction.CommitAsync();

          return new Data_Response<BiaSoDauBaiDto>(200, "Updated successfully");
        }
        catch (Exception ex)
        {
          // Rollback the transaction in case of an error
          await transaction.RollbackAsync();

          return new Data_Response<BiaSoDauBaiDto>(500, $"Server Error: {ex.Message}");
        }
      }
    }

    public async Task<Data_Response<BiaSoDauBaiDto>> DeleteBiaSoDauBai(int id)
    {
      try
      {
        var find = "SELECT * FROM BiaSoDauBai WHERE BiaSoDauBaiId = @id";
        var sodaubai = await _context.BiaSoDauBais
          .FromSqlRaw(find, new SqlParameter("@id", id))
          .FirstOrDefaultAsync();

        if (sodaubai is null)
        {
          return new Data_Response<BiaSoDauBaiDto>(404, "Student not found");
        }

        var deleteQuery = "DELETE FROM BiaSoDauBai WHERE BiaSoDauBaiId = @id";
        await _context.Database.ExecuteSqlRawAsync(deleteQuery, new SqlParameter("@id", id));
        return new Data_Response<BiaSoDauBaiDto>(200, "Deleted");
      }
      catch (Exception ex)
      {
        return new Data_Response<BiaSoDauBaiDto>(500, $"Server error: {ex.Message}");
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

        var deleteQuery = $"DELETE FROM BiaSoDaiBai WHERE BiaSoDaiBaiId IN ({idList})";

        var delete = await _context.Database.ExecuteSqlRawAsync(deleteQuery);

        if (delete == 0)
        {
          return new Data_Response<string>(404, "No BiaSoDaiBaiId found to delete");
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

    public async Task<string> ImportClassExcel(IFormFile file)
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

          var filePath = Path.Combine(uploadsFolder, file.Name);
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

                  var myBiaSoDauBai = new Models.BiaSoDauBai
                  {
                    SchoolId = Convert.ToInt32(reader.GetValue(1)),
                    AcademicyearId = Convert.ToInt32(reader.GetValue(2)),
                    ClassId = Convert.ToInt32(reader.GetValue(3)),
                    Status = Convert.ToBoolean(reader.GetValue(4)),
                    DateCreated = reader.IsDBNull(5) ? null : DateOnly.FromDateTime(Convert.ToDateTime(reader.GetValue(5))),
                    DateUpdated = reader.IsDBNull(6) ? null : DateOnly.FromDateTime(Convert.ToDateTime(reader.GetValue(6))),
                  };


                  await _context.BiaSoDauBais.AddAsync(myBiaSoDauBai);
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
  }
}
