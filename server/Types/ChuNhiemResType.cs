namespace server.Types
{
  public class ChuNhiemResType
  {
    public int PhanCongChuNhiemId { get; set; }

    public int TeacherId { get; set; }

    public int ClassId { get; set; }

    public bool Status { get; set; }

    public string? ClassName { get; set; }

    public string? Fullname { get; set; }
  }
}
