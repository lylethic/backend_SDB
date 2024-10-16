namespace server.Types
{
  public class LoginResType
  {
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = String.Empty;

    public LoginResData? Data { get; set; }

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
  }
}
