using server.Dtos;

namespace server.IService
{
  public interface IBiaSoDauBai
  {
    Task<Data_Response<BiaSoDauBaiDto>> CreateBiaSoDauBai(BiaSoDauBaiDto model);

    Task<Data_Response<BiaSoDauBaiDto>> GetBiaSoDauBai(int id);

    Task<List<BiaSoDauBaiDto>> GetBiaSoDauBais(int pageNumber, int pageSize);

    Task<List<BiaSoDauBaiDto>> GetBiaSoDauBaisBySchoolId(int pageNumber, int pageSize, int schoolId);

    Task<Data_Response<BiaSoDauBaiDto>> DeleteBiaSoDauBai(int id);

    Task<Data_Response<BiaSoDauBaiDto>> UpdateBiaSoDauBai(int id, BiaSoDauBaiDto model);

    Task<string> ImportExcel(IFormFile file);

    Task<Data_Response<string>> BulkDelete(List<int> ids);

    Task<List<BiaSoDauBaiDto>> SearchBiaSoDauBais(int? schoolId, int? classId);
  }
}
