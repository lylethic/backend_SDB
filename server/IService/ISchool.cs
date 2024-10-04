using server.Dtos;

namespace server.IService
{
  public interface ISchool
  {
    Task<Data_Response<SchoolDto>> CreateSchool(SchoolDto model);

    Task<Data_Response<SchoolDto>> GetSchool(int id);

    Task<List<SchoolDto>> GetSchools(int pageNumber, int pageSize);

    Task<Data_Response<SchoolDto>> DeleteSchool(int id);

    Task<Data_Response<SchoolDto>> UpdateSchool(int id, SchoolDto model);

    Task<Data_Response<string>> BulkDelete(List<int> ids);

    Task<string> ImportExcelFile(IFormFile file);
  }
}
