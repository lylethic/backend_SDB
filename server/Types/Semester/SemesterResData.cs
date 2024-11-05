namespace server.Types.Semester
{
  public class SemesterResData
  {
    public int SemesterId { get; set; }

    public int AcademicYearId { get; set; }

    public string SemesterName { get; set; } = null!;

    public DateOnly DateStart { get; set; }

    public DateOnly DateEnd { get; set; }

    public string? Description { get; set; }

    public string? DisplayAcademicYearName { get; set; } = null;

    public DateOnly? YearStart { get; set; }

    public DateOnly? YearEnd { get; set; }
  }
}
