using server.Dtos;

namespace server.Types.ChiTietSoDauBai
{
  public class ChiTietSDBResData : ChiTietSoDauBaiDto
  {
    public string? ClassName { get; set; }

    public int? TeacherId { get; set; }

    public string? TeacherName { get; set; }
  }
}
