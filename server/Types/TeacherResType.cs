using server.Dtos;

namespace server.Types
{
  public class TeacherResType : ModelResType
  {
    public List<TeacherDto>? Datas { get; set; }

    public TeacherDto? Data { get; set; }

    public TeacherResType() { }

    public TeacherResType(int statusCode, string message)
    {
      this.StatusCode = statusCode;
      this.Message = message;
    }

    public TeacherResType(int statusCode, string message, TeacherDto data)
    {
      this.StatusCode = statusCode;
      this.Message = message;
      this.Data = data;
    }

    public TeacherResType(int statusCode, string message, List<TeacherDto> datas)
    {
      this.StatusCode = statusCode;
      this.Message = message;
      this.Datas = datas;
    }

  }
}
