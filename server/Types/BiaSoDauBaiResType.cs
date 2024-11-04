using server.Dtos;

namespace server.Types
{
  public class BiaSoDauBaiResType : ModelResType
  {
    public List<BiaSoDauBaiDto>? Datas { get; set; }
    public BiaSoDauBaiDto? Data { get; set; }

    public BiaSoDauBaiResType() { }

    public BiaSoDauBaiResType(int statusCode, string message)
    {
      this.StatusCode = statusCode;
      this.Message = message;
    }

    public BiaSoDauBaiResType(int statusCode, string message, BiaSoDauBaiDto data)
    {
      this.StatusCode = statusCode;
      this.Message = message;
      this.Data = data;
    }

    public BiaSoDauBaiResType(int statusCode, string message, List<BiaSoDauBaiDto> datas)
    {
      this.StatusCode = statusCode;
      this.Message = message;
      this.Datas = datas;
    }
  }
}
