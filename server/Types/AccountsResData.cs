namespace server.Types
{
  public class AccountsResData
  {
    public int AccountId { get; set; }
    public int? RoleId { get; set; }
    public int? SchoolId { get; set; }

    public string? RoleName { get; set; }

    public string? SchoolName { get; set; }

    public string? Email { get; set; }

    public DateTime? DateCreated { get; set; }

    public DateTime? DateUpdated { get; set; }
  }
}
