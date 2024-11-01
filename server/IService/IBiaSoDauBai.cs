using server.Dtos;

namespace server.IService
{
  public interface IBiaSoDauBai
  {
    Task<ResponseData<BiaSoDauBaiDto>> CreateBiaSoDauBai(BiaSoDauBaiDto model);

    Task<ResponseData<BiaSoDauBaiDto>> GetBiaSoDauBai(int id);

    Task<List<BiaSoDauBaiDto>> GetBiaSoDauBais(int pageNumber, int pageSize);

    Task<List<BiaSoDauBaiDto>> GetBiaSoDauBaisBySchoolId(int pageNumber, int pageSize, int schoolId);

    Task<ResponseData<BiaSoDauBaiDto>> DeleteBiaSoDauBai(int id);

    Task<ResponseData<BiaSoDauBaiDto>> UpdateBiaSoDauBai(int id, BiaSoDauBaiDto model);

    Task<string> ImportExcel(IFormFile file);

    Task<ResponseData<string>> BulkDelete(List<int> ids);

    Task<List<BiaSoDauBaiDto>> SearchBiaSoDauBais(int? schoolId, int? classId);
  }
}
