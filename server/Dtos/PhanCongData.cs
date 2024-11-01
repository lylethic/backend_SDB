namespace server.Dtos;

public class PhanCongData
{
  public int PhanCongId { get; set; }
  public int TeacherId { get; set; }
  public string TeacherName { get; set; } = string.Empty;
  public int ClassId { get; set; }
  public string NameClass { get; set; } = string.Empty;
  public int AcademicYearId { get; set; }
  public string AcademicYearName { get; set; } = string.Empty;
  public bool Status { get; set; }
  public DateTime? DateCreated { get; set; } = DateTime.UtcNow;
  public DateTime? DateUpdated { get; set; }
  public string? Description { get; set; }
}
