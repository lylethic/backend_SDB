using server.Dtos;

namespace server.IService
{
  public interface IRole
  {
    Task<List<RoleDto>> GetRoles(int pageNumber, int pageSize);

    Task<Data_Response<RoleDto>> GetRole(int id);

    Task<Data_Response<RoleDto>> AddRole(RoleDto role);

    Task<Data_Response<RoleDto>> UpdateRole(int id, RoleDto role);

    Task<Data_Response<RoleDto>> DeleteRole(int id);

    Task<string> ImportExcel(IFormFile file);

    Task<Data_Response<string>> BulkDelete(List<int> ids);
  }
}
