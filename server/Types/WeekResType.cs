namespace server.Types
{
  public class WeekResType
  {
    public int WeekId { get; set; }

    public string WeekName { get; set; } = null!;

    public DateOnly WeekStart { get; set; }

    public DateOnly WeekEnd { get; set; }

    public bool Status { get; set; }

    public int SemesterId { get; set; }

    public string SemesterName { get; set; } = null!;

    public DateOnly DateStart { get; set; }

    public DateOnly DateEnd { get; set; }
  }
}
