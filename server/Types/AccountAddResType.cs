namespace server.Types
{
  public class AccountAddResType
  {
    public int RoleId { get; set; }
    public int SchoolId { get; set; }
    public string Email { get; set; } = string.Empty;
    public DateTime? DateCreated { get; set; }
    public DateTime? DateUpdated { get; set; }

    public AccountAddResType() { }
  }
}
