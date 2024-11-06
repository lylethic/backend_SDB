using server.Dtos;
using server.Types.BiaSoDauBai;

namespace server.IService
{
  public interface IBiaSoDauBai
  {
    Task<BiaSoDauBaiResType> CreateBiaSoDauBai(BiaSoDauBaiDto model);

    Task<BiaSoDauBaiResType> GetBiaSoDauBai(int id);

    Task<BiaSoDauBaiResType> GetBiaSoDauBais_Active(int pageNumber, int pageSize);

    Task<BiaSoDauBaiResType> GetBiaSoDauBaisBySchool_Active(int pageNumber, int pageSize, int schoolId);

    // status true && false
    Task<BiaSoDauBaiResType> GetBiaSoDauBais(int pageNumber, int pageSize);

    Task<BiaSoDauBaiResType> GetBiaSoDauBaisBySchool(int pageNumber, int pageSize, int schoolId);

    Task<BiaSoDauBaiResType> DeleteBiaSoDauBai(int id);

    Task<BiaSoDauBaiResType> UpdateBiaSoDauBai(int id, BiaSoDauBaiDto model);

    Task<BiaSoDauBaiResType> ImportExcel(IFormFile file);

    Task<BiaSoDauBaiResType> BulkDelete(List<int> ids);

    Task<BiaSoDauBaiResType> SearchBiaSoDauBais(int? schoolId, int? classId);
  }
}
