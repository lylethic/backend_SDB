namespace server.Dtos
{
  public class GradeDto
  {
    public int GradeId { get; set; }

    public int AcademicYearId { get; set; }

    public string GradeName { get; set; } = null!;

    public string? Description { get; set; }
  }
}
