using server.Models;

namespace server.Types
{
  public class LoginResType
  {
    public bool IsSuccess { get; set; }
    public int StatusCode { get; set; }
    public string Message { get; set; } = String.Empty;

    public LoginResData? Data { get; set; }
    public List<Error>? Errors { get; set; }
    public LoginResType() { }

    public LoginResType(bool isSccess, string message)
    {
      this.IsSuccess = isSccess;
      this.Message = message;
    }

    public LoginResType(bool isSuccess, string message, LoginResData? data)
    {
      IsSuccess = isSuccess;
      Message = message;
      Data = data;
    }

    public LoginResType(bool isSuccess, int statusCode, string message, List<Error>? errors = null)
    {
      Message = message;
      Errors = errors;
      StatusCode = statusCode;
      IsSuccess = isSuccess;
    }
  }
}
