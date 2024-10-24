using server.Dtos;
using server.Types;

namespace server.IService
{
  public interface ISchool
  {
    Task<SchoolResType> CreateSchool(SchoolDto model);

    Task<SchoolResType> GetSchool(int id);

    Task<SchoolResType> GetSchools(int pageNumber, int pageSize);

    Task<SchoolResType> GetSchoolsNoPagnination();

    Task<SchoolResType> DeleteSchool(int id);

    Task<SchoolResType> UpdateSchool(int id, SchoolDto model);

    Task<Data_Response<string>> BulkDelete(List<int> ids);

    Task<string> ImportExcelFile(IFormFile file);

    Task<Data_Response<string>> ExportSchoolsExcel(List<int> ids, string filePath);
  }
}
