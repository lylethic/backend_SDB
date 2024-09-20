namespace server.Dtos
{
  public class TeacherDto
  {
    public int TeacherId { get; set; }

    public int AccountId { get; set; }

    public int SchoolId { get; set; }

    public string Fullname { get; set; } = null!;

    public DateTime DateOfBirth { get; set; }

    public byte Gender { get; set; }

    public string Address { get; set; } = null!;

    public bool Status { get; set; }
  }
}
