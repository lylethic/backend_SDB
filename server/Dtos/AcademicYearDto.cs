namespace server.Dtos
{
  public class AcademicYearDto
  {
    public int AcademicYearId { get; set; }

    public string DisplayAcademicYearName { get; set; } = null!;

    public DateOnly YearStart { get; set; }

    public DateOnly YearEnd { get; set; }

    public string? Description { get; set; }
  }
}
