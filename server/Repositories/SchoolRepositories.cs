using ExcelDataReader;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using server.Dtos;
using server.IService;
using System.Text;

namespace server.Repositories
{
  public class SchoolRepositories : ISchool
  {
    private readonly Data.SoDauBaiContext _context;

    public SchoolRepositories(Data.SoDauBaiContext context)
    {
      this._context = context;
    }

    public async Task<Data_Response<SchoolDto>> CreateSchool(SchoolDto model)
    {
      try
      {
        var findSchool = "SELECT * FROM School WHERE SchoolId = @id";
        var shoolExists = await _context.Schools
          .FromSqlRaw(findSchool, new SqlParameter("@id", model.SchoolId))
          .FirstOrDefaultAsync();

        if (shoolExists is not null)
        {
          return new Data_Response<SchoolDto>(409, "School already exists");
        }

        var queryInsert = @"INSERT INTO SCHOOL (provinceId, districtId, nameSchcool, address, phoneNumber, schoolType, description) 
                            VALUES (@provinceId, @districtId, @nameSchcool, @address, @phoneNumber, @schoolType, @description);
                            SELECT CAST(SCOPE_IDENTITY() as int);";

        var schoolInsert = await _context.Database.ExecuteSqlRawAsync(queryInsert,
          new SqlParameter("@provinceId", model.ProvinceId),
          new SqlParameter("@districtId", model.DistrictId),
          new SqlParameter("@nameSchcool", model.NameSchool),
          new SqlParameter("@address", model.Address),
          new SqlParameter("@phoneNumber", model.PhoneNumber),
          new SqlParameter("@schoolType", model.SchoolType),
          new SqlParameter("@description", model.Description));

        var result = new SchoolDto
        {
          SchoolId = schoolInsert,
          ProvinceId = model.ProvinceId,
          DistrictId = model.DistrictId,
          NameSchool = model.NameSchool,
          Address = model.Address,
          PhoneNumber = model.PhoneNumber,
          SchoolType = model.SchoolType,
          Description = model.Description
        };

        return new Data_Response<SchoolDto>(200, result);
      }
      catch (Exception ex)
      {
        return new Data_Response<SchoolDto>(200, $"Server error: {ex.Message}");
      }
    }

    public async Task<Data_Response<SchoolDto>> DeleteSchool(int id)
    {
      try
      {
        // Find
        string find = "SELECT * FROM School WHERE SchoolId = @id";
        var schoolExists = await _context.Schools
          .FromSqlRaw(find, new SqlParameter("@id", id))
          .FirstOrDefaultAsync();

        // Check null??
        if (schoolExists is null)
        {
          return new Data_Response<SchoolDto>(404, "Account not found!");
        }

        // Delete query
        var deleteQuery = "DELETE FROM School WHERE SchoolId = @id";
        await _context.Database
          .ExecuteSqlRawAsync(deleteQuery, new SqlParameter("@id", id));

        return new Data_Response<SchoolDto>(200, "Deleted");
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
        return new Data_Response<SchoolDto>(500, $"Error deleting: {ex.Message}");
      }
    }

    public async Task<Data_Response<SchoolDto>> GetSchool(int id)
    {
      try
      {
        var query = "SELECT * FROM SCHOOL WHERE SchoolId = @id";

        var school = await _context.Schools
            .FromSqlRaw(query, new SqlParameter("@id", id))
            .FirstOrDefaultAsync();

        if (school is null)
        {
          return new Data_Response<SchoolDto>(404, "School not founnd");
        }

        var result = new SchoolDto
        {
          SchoolId = school.SchoolId,
          ProvinceId = school.ProvinceId,
          DistrictId = school.DistrictId,
          NameSchool = school.NameSchcool,
          PhoneNumber = school.PhoneNumber,
          Address = school.Address,
          SchoolType = school.SchoolType,
          Description = school.Description
        };

        return new Data_Response<SchoolDto>(200, result);
      }
      catch (Exception ex)
      {
        return new Data_Response<SchoolDto>(500, $"Server error: {ex.Message}");
      }
    }

    public async Task<List<SchoolDto>> GetSchools(int pageNumber, int pageSize)
    {
      try
      {
        var skip = (pageNumber - 1) * pageSize;

        var query = @"SELECT * FROM SCHOOL 
                      ORDER BY schoolId 
                      OFFSET @skip ROWS
                      FETCH NEXT @pageSize ROWS ONLY";

        var schoolList = await _context.Schools
          .FromSqlRaw(query,
          new SqlParameter("@skip", skip),
          new SqlParameter("@pageSize", pageSize)
          ).ToListAsync();

        var result = schoolList.Select(x => new SchoolDto
        {
          SchoolId = x.SchoolId,
          ProvinceId = x.ProvinceId,
          DistrictId = x.DistrictId,
          NameSchool = x.NameSchcool,
          PhoneNumber = x.PhoneNumber,
          Address = x.Address,
          SchoolType = x.SchoolType,
          Description = x.Description
        }).ToList();

        return result;
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
        throw;
      }
    }

    public async Task<Data_Response<SchoolDto>> UpdateSchool(int id, SchoolDto model)
    {
      try
      {
        var findSchool = "SELECT * FROM SCHOOL WHERE SchoolId = @id";

        var existingSchool = await _context.Schools
          .FromSqlRaw(findSchool, new SqlParameter("@id", id))
          .FirstOrDefaultAsync();

        if (existingSchool is null)
        {
          return new Data_Response<SchoolDto>(404, "School not found");
        }

        bool hasChanges = false;

        var parameters = new List<SqlParameter>();
        var queryBuilder = new StringBuilder("UPDATE SCHOOL SET ");

        if (model.ProvinceId != 0 && model.ProvinceId != existingSchool.ProvinceId)
        {
          queryBuilder.Append("provinceId = @provinceId, ");
          parameters.Add(new SqlParameter("@provinceId", model.ProvinceId));
          hasChanges = true;
        }

        if (model.ProvinceId != 0 && model.DistrictId != existingSchool.DistrictId)
        {
          queryBuilder.Append("districtId = @districtId, ");
          parameters.Add(new SqlParameter("@districtId", model.DistrictId));
          hasChanges = true;
        }

        if (!string.IsNullOrEmpty(model.NameSchool) && model.NameSchool != existingSchool.NameSchcool)
        {
          queryBuilder.Append("description = @description, ");
          parameters.Add(new SqlParameter("@Description", model.Description));
          hasChanges = true;
        }

        if (!string.IsNullOrEmpty(model.NameSchool) && model.NameSchool != existingSchool.NameSchcool)
        {
          queryBuilder.Append("nameSchcool = @nameSchcool, ");
          parameters.Add(new SqlParameter("@nameSchcool", model.NameSchool));
          hasChanges = true;
        }

        if (!string.IsNullOrEmpty(model.Address) && model.Address != existingSchool.Address)
        {
          queryBuilder.Append("address = @address, ");
          parameters.Add(new SqlParameter("@address", model.Address));
          hasChanges = true;
        }

        if (!string.IsNullOrEmpty(model.PhoneNumber) && model.PhoneNumber != existingSchool.PhoneNumber)
        {

          queryBuilder.Append("phoneNumber = @phoneNumber, ");
          parameters.Add(new SqlParameter("@phoneNumber", model.PhoneNumber));
        }

        if (model.SchoolType != existingSchool.SchoolType)
        {
          queryBuilder.Append("schoolType = @schoolType, ");
          parameters.Add(new SqlParameter("@schoolType", model.SchoolType));
          hasChanges = true;
        }

        if (hasChanges)
        {
          if (queryBuilder.Length > 0)
          {
            queryBuilder.Length -= 2;
          }

          queryBuilder.Append(" WHERE SchoolId = @id");
          parameters.Add(new SqlParameter("@id", id));

          var updateQuery = queryBuilder.ToString();
          await _context.Database.ExecuteSqlRawAsync(updateQuery, parameters.ToArray());

          return new Data_Response<SchoolDto>(200, "Updated");
        }
        else
        {
          return new Data_Response<SchoolDto>(200, "No changes detected");
        }
      }
      catch (Exception ex)
      {
        return new Data_Response<SchoolDto>(500, $"Server Error: {ex.Message}");
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

                var mySchool = new Models.School
                {
                  ProvinceId = Convert.ToByte(reader.GetValue(1)),
                  DistrictId = Convert.ToByte(reader.GetValue(2)),
                  NameSchcool = reader.GetValue(3).ToString()?.Trim() ?? "Ten truong hoc?",
                  Address = reader.GetValue(4).ToString()?.Trim() ?? "Dia chi truong hoc?",
                  PhoneNumber = reader.GetValue(5).ToString()?.Trim() ?? "So dien thoai truong hoc?",
                  SchoolType = Convert.ToBoolean(reader.GetValue(6)),
                  Description = reader.GetValue(7).ToString()?.Trim() ?? "Mo ta cho truong hoc",
                };

                await _context.Schools.AddAsync(mySchool);
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

        var deleteQuery = $"DELETE FROM SCHOOL WHERE SCHOOLID IN ({idList})";

        var delete = await _context.Database.ExecuteSqlRawAsync(deleteQuery);

        if (delete == 0)
        {
          return new Data_Response<string>(404, "No SCHOOLID found to delete");
        }

        await transaction.CommitAsync();

        return new Data_Response<string>(200, "Deleted");
      }
      catch (Exception ex)
      {
        await transaction.RollbackAsync();
        return new Data_Response<string>(500, $"Server error: {ex.Message}");
      }
    }
  }
}
