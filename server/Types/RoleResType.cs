using server.Dtos;
using server.Models;

namespace server.Types
{
  public class RoleResType
  {
    public int StatusCode { get; set; }
    public string Message { get; set; } = String.Empty;

    public List<RoleDto>? RoleData { get; set; }
    public RoleDto? RolebyId { get; set; }
    public List<Error>? Errors { get; set; }

    public RoleResType() { }

    public RoleResType(int statusCode, string message)
    {
      this.StatusCode = statusCode;
      this.Message = message;
    }

    public RoleResType(int statusCode, string message, List<RoleDto> data)
    {
      this.StatusCode = statusCode;
      this.Message = message;
      this.RoleData = data;
    }

    public RoleResType(int statusCode, string message, RoleDto data)
    {
      this.StatusCode = statusCode;
      this.Message = message;
      this.RolebyId = data;
    }

    public RoleResType(int statusCode, string message, List<Error> error)
    {
      this.StatusCode = statusCode;
      this.Message = message;
      this.Errors = error;
    }
  }
}
