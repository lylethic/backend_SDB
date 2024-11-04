using ExcelDataReader;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using server.Data;
using server.Dtos;
using server.IService;
using server.Types;
using System.Text;

namespace server.Repositories
{
  public class BiaSoDauBaiRepositories : IBiaSoDauBai
  {
    private readonly SoDauBaiContext _context;

    public BiaSoDauBaiRepositories(SoDauBaiContext context)
    {
      this._context = context;
    }

    public async Task<BiaSoDauBaiResType> CreateBiaSoDauBai(BiaSoDauBaiDto model)
    {
      using var transaction = await _context.Database.BeginTransactionAsync();

      try
      {
        var find = "SELECT * FROM BiaSoDauBai WHERE BiaSoDauBaiId = @id";
        var sodaubai = await _context.BiaSoDauBais
            .FromSqlRaw(find, new SqlParameter("@id", model.BiaSoDauBaiId))
            .FirstOrDefaultAsync();

        if (sodaubai is not null)
        {
          return new BiaSoDauBaiResType(409, "Sodaubai already exists");
        }

        // Check if schoolId exists
        var schoolExists = await _context.Schools
            .AnyAsync(c => c.SchoolId == model.SchoolId);

        if (!schoolExists)
        {
          return new BiaSoDauBaiResType(404, "SchoolId not found");
        }

        // Check if academicYearId exists
        var academicYearExists = await _context.AcademicYears
            .AnyAsync(c => c.AcademicYearId == model.AcademicyearId);

        if (!academicYearExists)
        {
          return new BiaSoDauBaiResType(404, "academicYearId not found");
        }

        // Check if classId exists
        var classExists = await _context.Classes
            .AnyAsync(c => c.ClassId == model.ClassId);

        if (!classExists)
        {
          return new BiaSoDauBaiResType(404, "ClassId not found");
        }

        model.DateCreated = DateTime.UtcNow;
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

        return new BiaSoDauBaiResType(200, "Thành công", result);
      }
      catch (Exception ex)
      {
        await transaction.RollbackAsync();
        return new BiaSoDauBaiResType(500, $"Server Error: {ex.Message}");
      }
    }

    public async Task<BiaSoDauBaiResType> GetBiaSoDauBai(int id)
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
          return new BiaSoDauBaiResType(404, "Not found");
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

        return new BiaSoDauBaiResType(200, "Thành công", result);
      }
      catch (Exception ex)
      {
        return new BiaSoDauBaiResType(500, $"Server error: {ex.Message}");
      }
    }

    public async Task<BiaSoDauBaiResType> GetBiaSoDauBais(int pageNumber, int pageSize)
    {
      try
      {
        var skip = (pageNumber - 1) * pageSize;

        var biaSoDauBai = await _context.BiaSoDauBais
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync();

        var result = biaSoDauBai.Select(x => new BiaSoDauBaiDto
        {
          BiaSoDauBaiId = x.BiaSoDauBaiId,
          SchoolId = x.SchoolId,
          AcademicyearId = x.AcademicyearId,
          ClassId = x.ClassId,
          Status = x.Status,
          DateCreated = x.DateCreated,
          DateUpdated = x.DateUpdated,
        }).ToList();

        return new BiaSoDauBaiResType(200, "Thành công", result);
      }
      catch (Exception ex)
      {
        return new BiaSoDauBaiResType(500, $"Server error: {ex.Message}");
      }
    }

    public async Task<BiaSoDauBaiResType> GetBiaSoDauBaisBySchoolId(int pageNumber, int pageSize, int schoolId)
    {
      try
      {
        var skip = (pageNumber - 1) * pageSize;

        var query = _context.BiaSoDauBais
            .AsNoTracking() // Optional: Improves performance for read-only queries
            .Where(x => x.SchoolId == schoolId)
            .OrderBy(x => x.DateCreated); // Ensure consistent ordering

        var biaSoDauBai = await query
            .Skip(skip)     // Skip the first (pageNumber - 1) * pageSize records
            .Take(pageSize) // Take pageSize records
            .ToListAsync();

        var result = biaSoDauBai.Select(x => new BiaSoDauBaiDto
        {
          BiaSoDauBaiId = x.BiaSoDauBaiId,
          SchoolId = x.SchoolId,
          AcademicyearId = x.AcademicyearId,
          ClassId = x.ClassId,
          Status = x.Status,
          DateCreated = x.DateCreated,
          DateUpdated = x.DateUpdated,
        }).ToList();

        return new BiaSoDauBaiResType(200, "Thành công", result);
      }
      catch (Exception ex)
      {
        return new BiaSoDauBaiResType(500, $"Server error: {ex.Message}");
      }
    }

    public async Task<BiaSoDauBaiResType> UpdateBiaSoDauBai(int id, BiaSoDauBaiDto model)
    {
      using var transaction = await _context.Database.BeginTransactionAsync();
      try
      {
        var find = "SELECT * FROM BiaSoDauBai WHERE BiaSoDauBaiId = @id";
        var existingBiaSoDaiBai = await _context.BiaSoDauBais
            .FromSqlRaw(find, new SqlParameter("@id", id))
            .FirstOrDefaultAsync();

        if (existingBiaSoDaiBai == null)
        {
          return new BiaSoDauBaiResType(404, "BiaSoDauBaiId not found");
        }

        bool hasChanges = false;

        // Compare if difference
        var parameters = new List<SqlParameter>();
        var queryBuilder = new StringBuilder("UPDATE BiaSoDauBai SET ");

        if (model.SchoolId != 0 && model.SchoolId != existingBiaSoDaiBai.SchoolId)
        {
          queryBuilder.Append("SchoolId = @SchoolId, ");
          parameters.Add(new SqlParameter("@SchoolId", model.SchoolId));
          hasChanges = true;
        }

        if (model.AcademicyearId != 0 && model.AcademicyearId != existingBiaSoDaiBai.AcademicyearId)
        {
          queryBuilder.Append("AcademicyearId = @AcademicyearId, ");
          parameters.Add(new SqlParameter("@AcademicyearId", model.AcademicyearId));
          hasChanges = true;
        }

        if (model.ClassId != 0 && model.ClassId != existingBiaSoDaiBai.ClassId)
        {
          queryBuilder.Append("ClassId = @ClassId, ");
          parameters.Add(new SqlParameter("@ClassId", model.ClassId));
          hasChanges = true;
        }

        if (model.Status != existingBiaSoDaiBai.Status)
        {
          queryBuilder.Append("Status = @Status, ");
          parameters.Add(new SqlParameter("@Status", model.Status));
          hasChanges = true;
        }

        if (model.DateCreated.HasValue)
        {
          queryBuilder.Append("DateCreated = @DateCreated, ");
          parameters.Add(new SqlParameter("@DateCreated", model.DateCreated.Value));
        }

        var currentDate = DateTime.UtcNow;
        if (currentDate != existingBiaSoDaiBai.DateUpdated)
        {
          queryBuilder.Append("DateUpdated = @DateUpdated, ");
          parameters.Add(new SqlParameter("@DateUpdated", currentDate));
          hasChanges = true;
        }

        // Remove the last comma and space
        if (hasChanges)
        {
          queryBuilder.Length -= 2;
          queryBuilder.Append(" WHERE BiaSoDauBaiId = @id");
          parameters.Add(new SqlParameter("@id", id));

          var updateQuery = queryBuilder.ToString();
          await _context.Database.ExecuteSqlRawAsync(updateQuery, [.. parameters]);

          // Commit the transaction
          await transaction.CommitAsync();

          return new BiaSoDauBaiResType(200, "Updated successfully");
        }
        else
        {
          return new BiaSoDauBaiResType(200, "No changes detected");
        }
      }
      catch (Exception ex)
      {
        // Rollback the transaction in case of an error
        await transaction.RollbackAsync();

        return new BiaSoDauBaiResType(500, $"Server Error: {ex.Message}");
      }
    }

    public async Task<BiaSoDauBaiResType> DeleteBiaSoDauBai(int id)
    {
      try
      {
        var find = "SELECT * FROM BiaSoDauBai WHERE BiaSoDauBaiId = @id";
        var sodaubai = await _context.BiaSoDauBais
          .FromSqlRaw(find, new SqlParameter("@id", id))
          .FirstOrDefaultAsync();

        if (sodaubai is null)
        {
          return new BiaSoDauBaiResType(404, "Student not found");
        }

        var deleteQuery = "DELETE FROM BiaSoDauBai WHERE BiaSoDauBaiId = @id";
        await _context.Database.ExecuteSqlRawAsync(deleteQuery, new SqlParameter("@id", id));
        return new BiaSoDauBaiResType(200, "Deleted");
      }
      catch (Exception ex)
      {
        return new BiaSoDauBaiResType(500, $"Server error: {ex.Message}");
      }
    }

    public async Task<BiaSoDauBaiResType> BulkDelete(List<int> ids)
    {
      await using var transaction = await _context.Database.BeginTransactionAsync();

      try
      {
        if (ids is null || ids.Count == 0)
        {
          return new BiaSoDauBaiResType(400, "No IDs provided.");
        }

        var idList = string.Join(",", ids);

        var deleteQuery = $"DELETE FROM BiaSoDauBai WHERE BiaSoDauBaiId IN ({idList})";

        var delete = await _context.Database.ExecuteSqlRawAsync(deleteQuery);

        if (delete == 0)
        {
          return new BiaSoDauBaiResType(404, "No BiaSoDauBaiId found to delete");
        }

        await transaction.CommitAsync();

        return new BiaSoDauBaiResType(200, "Deleted succesfully");
      }
      catch (Exception ex)
      {
        await transaction.RollbackAsync();
        return new BiaSoDauBaiResType(500, $"Server error: {ex.Message}");
      }
    }

    public async Task<BiaSoDauBaiResType> ImportExcel(IFormFile file)
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
                  if (reader.GetValue(1) == null && reader.GetValue(2) == null && reader.GetValue(3) == null && reader.GetValue(4) == null)
                  {
                    // Stop processing when an empty row is encountered
                    break;
                  }

                  var myBiaSoDauBai = new Models.BiaSoDauBai
                  {
                    SchoolId = Convert.ToInt32(reader.GetValue(1)),
                    AcademicyearId = Convert.ToInt32(reader.GetValue(2)),
                    ClassId = Convert.ToInt32(reader.GetValue(3)),
                    Status = Convert.ToBoolean(reader.GetValue(4)),
                    DateCreated = DateTime.UtcNow,
                    DateUpdated = null
                  };


                  await _context.BiaSoDauBais.AddAsync(myBiaSoDauBai);
                  await _context.SaveChangesAsync();
                }
              } while (reader.NextResult());
            }
          }

          return new BiaSoDauBaiResType(200, "Successfully inserted");
        }
        return new BiaSoDauBaiResType(400, "No file uploaded");

      }
      catch (Exception ex)
      {
        return new BiaSoDauBaiResType(500, $"Error while uploading file: {ex.Message}");
      }
    }

    public async Task<BiaSoDauBaiResType> SearchBiaSoDauBais(int? schoolId = null, int? classId = null)
    {
      var query = _context.BiaSoDauBais
          .AsNoTracking()
          .Include(b => b.School)  // Assuming School navigation property exists
          .AsQueryable();

      if (schoolId.HasValue)
      {
        query = query.Where(x => x.SchoolId == schoolId.Value);
      }

      if (classId.HasValue)
      {
        query = query.Where(x => x.ClassId == classId.Value);
      }

      // Execute the query and map the results to DTO
      var biaSoDauBai = await query
          .OrderBy(x => x.DateCreated)
          .ToListAsync();

      // Check if any data was found
      if (!biaSoDauBai.Any())
      {
        return new BiaSoDauBaiResType(400, "Không tìm thấy kết quả");
      }

      var result = biaSoDauBai.Select(x => new BiaSoDauBaiDto
      {
        BiaSoDauBaiId = x.BiaSoDauBaiId,
        SchoolId = x.SchoolId,
        AcademicyearId = x.AcademicyearId,
        ClassId = x.ClassId,
        Status = x.Status,
        DateCreated = x.DateCreated,
        DateUpdated = x.DateUpdated,
      }).ToList();

      return new BiaSoDauBaiResType(200, "Có kết quả", result);
    }
  }
}
