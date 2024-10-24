namespace server.Dtos
{
  public class SchoolDto
  {
    public int SchoolId { get; set; }

    public byte ProvinceId { get; set; }

    public byte DistrictId { get; set; }

    public string NameSchool { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public bool SchoolType { get; set; }

    public string? Description { get; set; }
    public DateTime? DateCreated { get; set; }
    public DateTime? DateUpdated { get; set; }
  }
}
