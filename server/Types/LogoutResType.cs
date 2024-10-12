namespace server.Types
{
  public class LogoutResType
  {
    public bool IsSuccess { get; set; } = false;

    public string Message { get; set; } = String.Empty;

    public LogoutResType()
    {
    }

    public LogoutResType(bool isSuccess, string mess)
    {
      this.IsSuccess = isSuccess;
      this.Message = mess;
    }
  }
}
