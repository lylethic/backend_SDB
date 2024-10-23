using server.Models;

namespace server.Types
{
  public class AccountsResType
  {
    public bool IsSuccess { get; set; }
    public int StatusCode { get; set; }
    public string Message { get; set; } = String.Empty;

    public List<AccountsData>? Data { get; set; }
    public AccountAddResType AccountAddResType { get; set; }

    public List<Error>? Errors { get; set; }


    public AccountsResType() { }

    public AccountsResType(int statusCode, string message, List<AccountsData> data)
    {
      this.StatusCode = statusCode;
      this.Message = message;
      this.Data = data;
    }

    public AccountsResType(int statusCode, string message)
    {
      this.StatusCode = statusCode;
      this.Message = message;
    }

    // Error validate
    public AccountsResType(bool isSuccess, int statusCode, string message, List<Error>? error)
    {
      this.Message = message;
      this.Errors = error;
      this.StatusCode = statusCode;
      this.IsSuccess = isSuccess;
    }

    public AccountsResType(int statusCode, string message, AccountAddResType data)
    {
      this.StatusCode = statusCode;
      this.Message = message;
      this.AccountAddResType = data;
    }
  }
}
