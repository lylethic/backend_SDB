using server.Dtos;

namespace server.Types.BiaSoDauBai
{
  public class BiaSoDauBaiResType : ModelResType
  {
    public List<BiaSoDauBaiDto>? ListBiaSoDauBaiDto { get; set; }

    public BiaSoDauBaiDto? BiaSoDauBaiDto { get; set; }

    public BiaSoDauBaiRes? BiaSoDauBaiRes { get; set; }

    public List<BiaSoDauBaiRes>? ListBiaSoDauBaiRes { get; set; }


    public BiaSoDauBaiResType() { }

    public BiaSoDauBaiResType(int statusCode, string message)
    {
      StatusCode = statusCode;
      Message = message;
    }

    public BiaSoDauBaiResType(int statusCode, string message, BiaSoDauBaiDto biaSoDauBaiDto)
    {
      StatusCode = statusCode;
      Message = message;
      BiaSoDauBaiDto = biaSoDauBaiDto;
    }

    public BiaSoDauBaiResType(int statusCode, string message, BiaSoDauBaiRes biaSoDauBaiRes)
    {
      StatusCode = statusCode;
      Message = message;
      BiaSoDauBaiRes = biaSoDauBaiRes;
    }

    public BiaSoDauBaiResType(int statusCode, string message, List<BiaSoDauBaiRes> listBiaSoDauBaiRes)
    {
      StatusCode = statusCode;
      Message = message;
      ListBiaSoDauBaiRes = listBiaSoDauBaiRes;
    }

    public BiaSoDauBaiResType(int statusCode, string message, List<BiaSoDauBaiDto> listBiaSoDauBaiDto)
    {
      StatusCode = statusCode;
      Message = message;
      ListBiaSoDauBaiDto = listBiaSoDauBaiDto;
    }
  }
}
