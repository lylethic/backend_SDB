namespace server.Models;

public partial class Absence
{
  public int AbsenceId { get; set; }

  public int? CallRollId { get; set; }

  public int StudentId { get; set; }

  public string? Description { get; set; }

  public virtual RollCall? CallRoll { get; set; }

  public virtual Student Student { get; set; } = null!;
}
