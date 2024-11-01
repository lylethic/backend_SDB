using server.Dtos;

namespace server.Types
{
  public class ChuNhiemResType
  {
    public string Message { get; set; } = string.Empty;
    public int StatusCode { get; set; } = 0;

    public List<PhanCongData>? Datas { get; set; }

    public PhanCongData? Data { get; set; }

    public PC_ChuNhiemDto? PC_ChuNhiemDto { get; set; }

    public ChuNhiemResType() { }

    public ChuNhiemResType(int statusCode, string message)
    {
      this.StatusCode = statusCode;
      this.Message = message;
    }

    public ChuNhiemResType(int statusCode, string message, PhanCongData data)
    {
      this.StatusCode = statusCode;
      this.Message = message;
      this.Data = data;
    }

    public ChuNhiemResType(int statusCode, string message, PC_ChuNhiemDto pcChuNhiemDto)
    {
      this.StatusCode = statusCode;
      this.Message = message;
      this.PC_ChuNhiemDto = pcChuNhiemDto;
    }

    public ChuNhiemResType(int statusCode, string message, List<PhanCongData> datas)
    {
      this.StatusCode = statusCode;
      this.Message = message;
      this.Datas = datas;
    }
  }
}
