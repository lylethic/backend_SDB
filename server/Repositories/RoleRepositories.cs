using ExcelDataReader;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using server.Data;
using server.Dtos;
using server.IService;
using System.Text;

namespace server.Repositories
{
  public class RoleRepositories : IRole
  {
    private readonly SoDauBaiContext _context;

    public RoleRepositories(SoDauBaiContext context)
    {
      this._context = context;
    }

    public async Task<Data_Response<RoleDto>> GetRole(int id)
    {
      try
      {
        var query = "SELECT * FROM ROLE WHERE RoleId = @id";
        var role = await _context.Roles
          .FromSqlRaw(query, new SqlParameter("@id", id))
          .FirstOrDefaultAsync();

        if (role is null)
        {
          return new Data_Response<RoleDto>(404, "Role not found");
        }

        var result = new RoleDto
        {
          RoleId = id,
          NameRole = role.NameRole,
          Description = role.Description,
        };

        return new Data_Response<RoleDto>(200, result);
      }
      catch (Exception ex)
      {
        return new Data_Response<RoleDto>(500, $"Server error: {ex.Message}");
      }
    }

    public async Task<List<RoleDto>> GetRoles(int pageNumber, int pageSize)
    {
      try
      {
        var skip = (pageNumber - 1) * pageSize;

        var query = @"SELECT * FROM Role
                      ORDER BY RoleId 
                      OFFSET @skip ROWS
                      FETCH NEXT  @pageSize ROWS ONLY";

        var roles = await _context.Roles
          .FromSqlRaw(query,
                      new SqlParameter("@skip", skip),
                      new SqlParameter("@pageSize", pageSize)
          ).ToListAsync() ?? throw new Exception("Empty");

        var result = roles.Select(x => new RoleDto
        {
          RoleId = x.RoleId,
          NameRole = x.NameRole,
          Description = x.Description,
        }).ToList();

        return result;
      }
      catch (Exception ex)
      {
        throw new Exception($"Server error: {ex.Message}");
      }
    }

    public async Task<Data_Response<RoleDto>> AddRole(RoleDto model)
    {
      try
      {
        var findRole = "SELECT * FROM ROLE WHERE RoleId = @roleId";

        var role = await _context.Roles
          .FromSqlRaw(findRole, new SqlParameter("@roleId", model.RoleId))
          .FirstOrDefaultAsync();

        if (role is not null)
        {
          return new Data_Response<RoleDto>(409, "Role already exists");
        }

        var sqlInsert = @"INSERT INTO ROLE (NameRole, Description) 
                          VALUES (@NameRole, @Description);
                          SELECT CAST(SCOPE_IDENTITY() as int);";

        var roleInsert = await _context.Database.ExecuteSqlRawAsync(sqlInsert,
          new SqlParameter("@NameRole", model.NameRole),
          new SqlParameter("@Description", model.Description)
        );

        var result = new RoleDto
        {
          RoleId = roleInsert,
          NameRole = model.NameRole,
          Description = model.Description,
        };

        return new Data_Response<RoleDto>(200, result);
      }
      catch (Exception ex)
      {
        return new Data_Response<RoleDto>(200, $"Server error: {ex.Message}");
      }
    }

    public async Task<Data_Response<RoleDto>> DeleteRole(int id)
    {
      try
      {
        var findRole = "SELECT * FROM ROLE WHERE RoleId = @id";
        var role = await _context.Roles
          .FromSqlRaw(findRole, new SqlParameter("@id", id))
          .FirstOrDefaultAsync();

        if (role is null)
        {
          return new Data_Response<RoleDto>(404, "Role not found");
        }

        var deleteQuery = "DELETE FROM ROLE WHERE RoleId = @id";
        await _context.Database.ExecuteSqlRawAsync(deleteQuery, new SqlParameter("@id", id));
        return new Data_Response<RoleDto>(200, "Deleted");
      }
      catch (Exception ex)
      {
        return new Data_Response<RoleDto>(500, $"Server error: {ex.Message}");
      }
    }

    public async Task<Data_Response<RoleDto>> UpdateRole(int id, RoleDto model)
    {
      try
      {
        var findRole = "SELECT * FROM ROLE WHERE RoleId = @id";
        var roleIdExists = await _context.Roles
          .FromSqlRaw(findRole, new SqlParameter("@id", id))
          .FirstOrDefaultAsync();

        if (roleIdExists is null)
        {
          return new Data_Response<RoleDto>(404, "Role not found");
        }

        var queryBuilder = new StringBuilder("UPDATE ROLE SET ");
        var parameters = new List<SqlParameter>();

        if (!string.IsNullOrEmpty(model.NameRole))
        {
          queryBuilder.Append("NameRole = @NameRole, ");
          parameters.Add(new SqlParameter("@NameRole", model.NameRole));

          queryBuilder.Append("Description = @Description, ");
          parameters.Add(new SqlParameter("@Description", model.Description));
        }

        // Remove the last comma and space
        if (queryBuilder.Length > 0)
        {
          queryBuilder.Length -= 2; // Remove the trailing comma and space
        }

        queryBuilder.Append(" WHERE RoleId = @id");
        parameters.Add(new SqlParameter("@id", id));

        var updateQuery = queryBuilder.ToString();
        await _context.Database.ExecuteSqlRawAsync(updateQuery, parameters.ToArray());

        return new Data_Response<RoleDto>(200, "Updated");
      }
      catch (Exception ex)
      {
        return new Data_Response<RoleDto>(500, $"Server Error: {ex.Message}");
      }
    }

    public async Task<string> ImportExcel(IFormFile file)
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

                  var myRole = new Models.Role
                  {
                    NameRole = reader.GetValue(1).ToString() ?? "role",
                    Description = reader.GetValue(2).ToString() ?? "Mo ta"
                  };


                  await _context.Roles.AddAsync(myRole);
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

        var deleteQuery = $"DELETE FROM Role WHERE RoleId IN ({idList})";

        var delete = await _context.Database.ExecuteSqlRawAsync(deleteQuery);

        if (delete == 0)
        {
          return new Data_Response<string>(404, "No RoleId found to delete");
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
  }
}
