namespace server.Dtos
{
  public class GradeDto
  {
    public int GradeId { get; set; }

    public int AcademicYearId { get; set; }

    public string GradeName { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime? DateCreated { get; set; }

    public DateTime? DateUpdated { get; set; }
  }

  public class GradeDetail : GradeDto
  {
    public string DisplayAcademicYearName { get; set; } = null!;

    public DateOnly YearStart { get; set; }

    public DateOnly YearEnd { get; set; }

    public bool Status { get; set; }
  }
}
