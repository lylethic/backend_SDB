using server.Dtos;

namespace server.IService
{
  public interface IRole
  {
    Task<List<RoleDto>> GetRoles();
    Task<Data_Response<RoleDto>> GetRole(int id);
    Task<Data_Response<RoleDto>> AddRole(RoleDto role);
    Task<Data_Response<RoleDto>> UpdateRole(int id, RoleDto role);
    Task<Data_Response<RoleDto>> DeleteRole(int id);
  }
}
