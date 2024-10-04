namespace server.Types
{
  public class LoginResType
  {
    public string Message { get; set; } = String.Empty;

    public string AccessToken { get; set; } = String.Empty;

    public string RefreshToken { get; set; } = String.Empty;

    public bool IsSuccess { get; set; }

    public int AccountId { get; set; }

    public int RoleId { get; set; }

    public int? SchoolId { get; set; }

    public string Email { get; set; } = null!;

    public LoginResType() { }

    public LoginResType(bool isSccess, string message)
    {
      this.IsSuccess = isSccess;
      this.Message = message;
    }
  }
}
