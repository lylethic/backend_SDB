namespace server.Dtos
{
  public class AbsenceDto
  {
    public int AbsenceId { get; set; }

    public int? CallRollId { get; set; }

    public int StudentId { get; set; }

    public string? Description { get; set; }
  }
}
