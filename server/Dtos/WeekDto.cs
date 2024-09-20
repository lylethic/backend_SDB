namespace server.Dtos
{
  public class WeekDto
  {
    public int WeekId { get; set; }

    public int SemesterId { get; set; }

    public string WeekName { get; set; } = null!;

    public DateOnly WeekStart { get; set; }

    public DateOnly WeekEnd { get; set; }

    public bool Status { get; set; }
  }
}
