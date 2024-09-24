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

    public async Task<List<SchoolDto>> GetSchools()
    {
      try
      {
        var query = "SELECT * FROM SCHOOL";

        var schoolList = await _context.Schools.FromSqlRaw(query).ToListAsync();

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
        var schoolId_Exists = await _context.Schools
          .FromSqlRaw(findSchool, new SqlParameter("@id", id))
          .FirstOrDefaultAsync();

        if (schoolId_Exists is null)
        {
          return new Data_Response<SchoolDto>(404, "School not found");
        }

        var queryBuilder = new StringBuilder("UPDATE SCHOOL SET ");
        var parameters = new List<SqlParameter>();

        if (model.ProvinceId != 0 || model.DistrictId != 0 || !string.IsNullOrEmpty(model.NameSchool)
          || !string.IsNullOrEmpty(model.Description) || !string.IsNullOrEmpty(model.Address)
          || !string.IsNullOrEmpty(model.PhoneNumber) || model.SchoolType)
        {
          queryBuilder.Append("provinceId = @provinceId, ");
          parameters.Add(new SqlParameter("@provinceId", model.ProvinceId));

          queryBuilder.Append("districtId = @districtId, ");
          parameters.Add(new SqlParameter("@districtId", model.DistrictId));

          queryBuilder.Append("description = @description, ");
          parameters.Add(new SqlParameter("@Description", model.Description));

          queryBuilder.Append("nameSchcool = @nameSchcool, ");
          parameters.Add(new SqlParameter("@nameSchcool", model.NameSchool));

          queryBuilder.Append("address = @address, ");
          parameters.Add(new SqlParameter("@address", model.Address));

          queryBuilder.Append("phoneNumber = @phoneNumber, ");
          parameters.Add(new SqlParameter("@phoneNumber", model.PhoneNumber));

          queryBuilder.Append("schoolType = @schoolType, ");
          parameters.Add(new SqlParameter("@schoolType", model.SchoolType));
        }

        // Remove the last comma and space
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
      catch (Exception ex)
      {
        return new Data_Response<SchoolDto>(500, $"Server Error: {ex.Message}");
      }
    }
  }
}
