namespace server.Types
{
  public class LoginResData
  {
    public string Token { get; set; }
    public string RefreshToken { get; set; }
    public string ExpiresAt { get; set; }
    public AccountData Account { get; set; }
  }
}
