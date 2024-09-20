using server.Dtos;

namespace server.IService
{
  public interface IBiaSoDauBai
  {
    Task<Data_Response<BiaSoDauBaiDto>> CreateBiaSoDauBai(BiaSoDauBaiDto model);
    Task<Data_Response<BiaSoDauBaiDto>> GetBiaSoDauBai(int id);
    Task<List<BiaSoDauBaiDto>> GetBiaSoDauBais();
    Task<Data_Response<BiaSoDauBaiDto>> DeleteBiaSoDauBai(int id);
    Task<Data_Response<BiaSoDauBaiDto>> UpdateBiaSoDauBai(int id, BiaSoDauBaiDto model);
  }
}
