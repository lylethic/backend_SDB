using server.Dtos;

namespace server.IService
{
  public interface IChiTietSoDauBai
  {
    Task<Data_Response<ChiTietSoDauBaiDto>> CreateChiTietSoDauBai(ChiTietSoDauBaiDto model);

    Task<Data_Response<ChiTietSoDauBaiDto>> GetChiTietSoDauBai(int id);

    Task<List<ChiTietSoDauBaiDto>> GetChiTietSoDauBais();

    Task<Data_Response<ChiTietSoDauBaiDto>> DeleteChiTietSoDauBai(int id);

    Task<Data_Response<ChiTietSoDauBaiDto>> UpdateChiTietSoDauBai(int id, ChiTietSoDauBaiDto model);

    Task<string> ImportExcel(IFormFile file);

    Task<Data_Response<string>> BulkDelete(List<int> ids);
  }
}
