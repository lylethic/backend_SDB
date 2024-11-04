using server.Dtos;
using server.Types;

namespace server.IService
{
  public interface IBiaSoDauBai
  {
    Task<BiaSoDauBaiResType> CreateBiaSoDauBai(BiaSoDauBaiDto model);

    Task<BiaSoDauBaiResType> GetBiaSoDauBai(int id);

    Task<BiaSoDauBaiResType> GetBiaSoDauBais(int pageNumber, int pageSize);

    Task<BiaSoDauBaiResType> GetBiaSoDauBaisBySchoolId(int pageNumber, int pageSize, int schoolId);

    Task<BiaSoDauBaiResType> DeleteBiaSoDauBai(int id);

    Task<BiaSoDauBaiResType> UpdateBiaSoDauBai(int id, BiaSoDauBaiDto model);

    Task<BiaSoDauBaiResType> ImportExcel(IFormFile file);

    Task<BiaSoDauBaiResType> BulkDelete(List<int> ids);

    Task<BiaSoDauBaiResType> SearchBiaSoDauBais(int? schoolId, int? classId);
  }
}
