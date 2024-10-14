namespace server.Dtos.Account
{
  public class AccountSearchDto
  {
    public string? AccountName { get; set; } // FullName teacher
    public int? AccountId { get; set; }
    public int? SchoolId { get; set; }
    public int? RoleId { get; set; }

    // Pagination parameters
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
  }
}
