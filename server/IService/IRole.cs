using server.Dtos;
using server.Types;

namespace server.IService
{
  public interface IRole
  {
    Task<RoleResType> GetRoles(int pageNumber, int pageSize);

    Task<RoleResType> GetRole(int id);

    Task<RoleResType> AddRole(RoleDto role);

    Task<RoleResType> UpdateRole(int id, RoleDto role);

    Task<RoleResType> DeleteRole(int id);

    Task<string> ImportExcel(IFormFile file);

    Task<Data_Response<string>> BulkDelete(List<int> ids);

    Task<Data_Response<string>> ExportRolesExcel(List<int> ids, string filePath);
  }
}
