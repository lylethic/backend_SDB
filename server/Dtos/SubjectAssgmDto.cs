namespace server.Dtos
{
  public class SubjectAssgmDto // Phan cong giang day mon hoc
  {
    public int SubjectAssignmentId { get; set; }

    public int TeacherId { get; set; }

    public int SubjectId { get; set; }

    public string? Description { get; set; }

    public DateTime? DateCreated { get; set; }

    public DateTime? DateUpdated { get; set; }

    public string? SubjectName { get; set; }
  }
}
