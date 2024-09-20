namespace server.Dtos
{
  public class SemesterDto
  {
    public int SemesterId { get; set; }

    public int AcademicYearId { get; set; }

    public string SemesterName { get; set; } = null!;

    public DateOnly DateStart { get; set; }

    public DateOnly DateEnd { get; set; }

    public string? Description { get; set; }
  }
}
