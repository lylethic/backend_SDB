using ClosedXML.Excel;
using ExcelDataReader;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using server.Dtos;
using server.IService;
using server.Types;
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

    public async Task<SchoolResType> CreateSchool(SchoolDto model)
    {
      try
      {
        var findSchool = "SELECT * FROM School WHERE SchoolId = @id";
        var shoolExists = await _context.Schools
          .FromSqlRaw(findSchool, new SqlParameter("@id", model.SchoolId))
          .FirstOrDefaultAsync();

        if (shoolExists is not null)
        {
          return new SchoolResType
          {
            StatusCode = 409,
            Message = "Lỗi xảy ra khi xác thực dữ liệu...",
            Errors =
            [
              new("NameSchool", "Trường học đã tồn tại")
            ]
          };
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

        return new SchoolResType(200, "Thành công", result);
      }
      catch (Exception ex)
      {
        return new SchoolResType(500, $"Server error: {ex.Message}");
      }
    }

    public async Task<SchoolResType> DeleteSchool(int id)
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
          return new SchoolResType
          {
            StatusCode = 404,
            Message = "Lỗi xảy ra khi xác thực dữ liệu...",
            Errors =
            [
              new("SchoolId", "Không tìm thấy Trường học")
            ]
          }; ;
        }

        // Delete query
        var deleteQuery = "DELETE FROM School WHERE SchoolId = @id";
        await _context.Database
          .ExecuteSqlRawAsync(deleteQuery, new SqlParameter("@id", id));

        return new SchoolResType(200, "Xóa thành công");
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
        return new SchoolResType(500, $"Server error deleting: {ex.Message}");
      }
    }

    public async Task<SchoolResType> GetSchool(int id)
    {
      try
      {
        var query = "SELECT * FROM SCHOOL WHERE SchoolId = @id";

        var school = await _context.Schools
            .FromSqlRaw(query, new SqlParameter("@id", id))
            .FirstOrDefaultAsync();

        if (school is null)
        {
          return new SchoolResType
          {
            StatusCode = 404,
            Message = "Lỗi xảy ra khi xác thực dữ liệu...",
            Errors =
            [
              new ("SchoolId", "Không tìm thấy")
            ]
          };
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

        return new SchoolResType(200, "Thành công", result);
      }
      catch (Exception ex)
      {
        return new SchoolResType(500, $"Server error: {ex.Message}");
      }
    }

    public async Task<SchoolResType> GetSchools(int pageNumber, int pageSize)
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

        return new SchoolResType(200, "Thành công", result);
      }
      catch (Exception ex)
      {
        return new SchoolResType(500, $"Có lỗi: {ex.Message}");
      }
    }

    public async Task<SchoolResType> UpdateSchool(int id, SchoolDto model)
    {
      try
      {
        var findSchool = "SELECT * FROM SCHOOL WHERE SchoolId = @id";

        var existingSchool = await _context.Schools
          .FromSqlRaw(findSchool, new SqlParameter("@id", id))
          .FirstOrDefaultAsync();

        if (existingSchool is null)
        {
          return new SchoolResType
          {
            StatusCode = 404,
            Message = "Lỗi xảy ra khi xác thực dữ liệu...",
            Errors =
             [
               new("SchoolId", "Không tìm thấy trường học")
             ]
          };
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

          return new SchoolResType(200, "Cập nhật thành công");
        }
        else
        {
          return new SchoolResType(200, "No changes detected");
        }
      }
      catch (Exception ex)
      {
        return new SchoolResType(500, $"Server Error: {ex.Message}");
      }
    }

    public async Task<string> ImportExcelFile(IFormFile file)
    {
      try
      {
        System.Text.Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        if (file == null || file.Length == 0)
        {
          return "No file uploaded";
        }


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

                Console.WriteLine($"ProvinceId: {reader.GetValue(1)}");
                Console.WriteLine($"DistrictId: {reader.GetValue(2)}");
                Console.WriteLine($"NameSchcool: {reader.GetValue(3)}");
                Console.WriteLine($"Address: {reader.GetValue(4)}");
                Console.WriteLine($"PhoneNumber: {reader.GetValue(5)}");
                Console.WriteLine($"SchoolType: {reader.GetValue(6)}");
                Console.WriteLine($"Description: {reader.GetValue(7)}");

                // Check if there are no more rows or empty rows
                if (reader.GetValue(1) == null && reader.GetValue(2) == null && reader.GetValue(3) == null && reader.GetValue(4) == null
                  && reader.GetValue(5) == null && reader.GetValue(6) == null && reader.GetValue(7) == null)
                {
                  break;
                }

                var provinceIdValue = reader.GetValue(1);
                var districtIdValue = reader.GetValue(2);

                var mySchool = new Models.School
                {
                  ProvinceId = provinceIdValue != null ? (byte)Math.Min(255, Math.Max(0, Convert.ToInt32(provinceIdValue))) : (byte)0,
                  DistrictId = districtIdValue != null ? (byte)Math.Min(255, Math.Max(0, Convert.ToInt32(districtIdValue))) : (byte)0,
                  NameSchcool = reader.GetValue(3).ToString()! ?? "Default School Name",
                  Address = reader.GetValue(4).ToString()!.Trim() ?? "Default Address",
                  PhoneNumber = reader.GetValue(5).ToString()!.Trim() ?? "Default Phone",
                  SchoolType = reader.GetValue(6) != null ? Convert.ToBoolean(reader.GetValue(6)) : false,
                  Description = reader.GetValue(7).ToString()?.Trim() ?? "Default Description",
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
        throw new Exception($"Error while uploading file: {ex.Message}, Inner Exception: {ex.InnerException?.Message}");
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

    public async Task<Data_Response<string>> ExportSchoolsExcel(List<int> ids, string filePath)
    {
      try
      {
        if (ids is null || ids.Count == 0)
        {
          return new Data_Response<string>(400, "Không có id nào!");
        }

        Console.WriteLine($"ID: {string.Join(",", ids)}");

        var schools = await _context.Schools
          .Where(x => ids.Contains(x.SchoolId))
          .ToListAsync();

        if (schools is null || !schools.Any())
        {
          return new Data_Response<string>(404, "Không tìm thấy id");
        }

        using (var workbook = new XLWorkbook())
        {
          var worksheet = workbook.Worksheets.Add("Schools");

          // Add headers: 1 row => 3 columns
          worksheet.Cell(1, 1).Value = "Mã trường học";
          worksheet.Cell(1, 2).Value = "Mã tỉnh";
          worksheet.Cell(1, 3).Value = "Mã huyện";
          worksheet.Cell(1, 4).Value = "Tên trường học";
          worksheet.Cell(1, 5).Value = "Địa chỉ trường học";
          worksheet.Cell(1, 6).Value = "Số điện thoại";
          worksheet.Cell(1, 7).Value = "Loại trường";
          worksheet.Cell(1, 8).Value = "Mô tả";

          // Add body data: by row
          for (int i = 0; i < schools.Count; i++)
          {
            var role = schools[i];
            worksheet.Cell(i + 2, 1).Value = role.SchoolId;
            worksheet.Cell(i + 2, 2).Value = role.ProvinceId;
            worksheet.Cell(i + 2, 3).Value = role.DistrictId;
            worksheet.Cell(i + 2, 4).Value = role.NameSchcool;
            worksheet.Cell(i + 2, 5).Value = role.Address;
            worksheet.Cell(i + 2, 6).Value = role.PhoneNumber;
            worksheet.Cell(i + 2, 7).Value = role.SchoolType;
            worksheet.Cell(i + 2, 8).Value = role.Description;
          }

          // Fit columns
          worksheet.Columns().AdjustToContents();

          // Save
          workbook.SaveAs(filePath);
        }

        return new Data_Response<string>(200, "Successfull");
      }
      catch (Exception ex)
      {
        return new Data_Response<string>(500, $"Server error: {ex.Message}");
      }
    }
  }
}
