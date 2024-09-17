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
    private readonly IHttpContextAccessor httpContextAccessor;

    public RoleRepositories(SoDauBaiContext context, IHttpContextAccessor httpContextAccessor)
    {
      this._context = context;
      this.httpContextAccessor = httpContextAccessor;
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
          return new Data_Response<RoleDto>(404, "Role already exists");
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

    public async Task<List<RoleDto>> GetRoles()
    {
      try
      {
        var query = "SELECT * FROM ROLE";
        var roles = await _context.Roles.FromSqlRaw(query).ToListAsync();

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
        Console.WriteLine(ex.Message);
        throw;
      }
    }

    public async Task<Data_Response<RoleDto>> UpdateRole(int id, RoleDto model)
    {
      try
      {
        var findRole = "SELECT * FROM ROLE WHERE RoleId = @id";
        var roleIdExists = await _context.Roles
          .FromSqlRaw(findRole, new SqlParameter("@id", model.RoleId))
          .FirstOrDefaultAsync();

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
  }
}
