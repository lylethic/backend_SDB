namespace server.Models;

public partial class RollCall
{
  public int CallRollId { get; set; }

  public int ClassId { get; set; }

  public int WeekId { get; set; }

  public string? DayOfTheWeek { get; set; }

  public DateTime? DateAt { get; set; }

  public DateTime? DateCreated { get; set; }

  public DateTime? DateUpdated { get; set; }

  public int? NumberOfAttendants { get; set; }

  public virtual ICollection<Absence> Absences { get; set; } = new List<Absence>();

  public virtual Class Class { get; set; } = null!;

  public virtual Week Week { get; set; } = null!;
}
