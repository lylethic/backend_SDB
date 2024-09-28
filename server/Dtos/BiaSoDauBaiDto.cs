namespace server.Dtos
{
  public class BiaSoDauBaiDto
  {
    public int BiaSoDauBaiId { get; set; }

    public int SchoolId { get; set; }

    public int AcademicyearId { get; set; }

    public int ClassId { get; set; }

    public bool Status { get; set; }

    public DateOnly? DateCreated { get; set; }

    public DateOnly? DateUpdated { get; set; }
  }
}
