namespace server.Types
{
  public class AccountResType
  {
    public int AccountId { get; set; }

    public int RoleId { get; set; }

    public int? SchoolId { get; set; }

    public string Email { get; set; } = null!;

    // Teacher table
    public int TeacherId { get; set; }

    public string? FullName { get; set; }

    public bool? StatusTeacher { get; set; }
  }
}
