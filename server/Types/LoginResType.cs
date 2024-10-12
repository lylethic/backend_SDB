namespace server.Types
{
  public class LoginResType
  {
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = String.Empty;

    public string AccessToken { get; set; } = String.Empty;

    public LoginResType() { }

    public LoginResType(bool isSccess, string message)
    {
      this.IsSuccess = isSccess;
      this.Message = message;
    }
  }
}
