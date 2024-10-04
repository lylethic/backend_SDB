namespace server.Dtos
{
  public class SubjectDto
  {
    public int SubjectId { get; set; }

    public string SubjectName { get; set; } = null!;

    public int AcademicYearId { get; set; }

    public string? DisplayAcademicYear_Name { get; set; }

    public DateOnly? YearStart { get; set; }

    public DateOnly? YearEnd { get; set; }

  }
}
