namespace server.Dtos
{
  public class StudentDto
  {
    public int StudentId { get; set; }

    public int ClassId { get; set; }

    public int GradeId { get; set; }

    public int AccountId { get; set; }

    public int RoleId { get; set; }

    public int SchoolId { get; set; }

    public string Fullname { get; set; } = null!;

    public bool Status { get; set; }

    public string? Description { get; set; }
  }
}
