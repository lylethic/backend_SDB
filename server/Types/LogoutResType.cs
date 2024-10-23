namespace server.Types
{
  public class LogoutResType
  {
    public bool IsSuccess { get; set; } = false;
    public int StatusCode { get; set; }

    public string Message { get; set; } = String.Empty;

    public LogoutResType()
    {
    }

    public LogoutResType(int statusCode, bool isSuccess, string mess)
    {
      this.StatusCode = statusCode;
      this.IsSuccess = isSuccess;
      this.Message = mess;
    }
  }
}
