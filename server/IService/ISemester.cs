using server.Dtos;

namespace server.IService
{
  public interface ISemester
  {
    Task<Data_Response<SemesterDto>> CreateSemester(SemesterDto model);
    Task<Data_Response<SemesterDto>> GetSemester(int id);
    Task<List<SemesterDto>> GetSemesters(int pageNumber, int pageSize);
    Task<Data_Response<SemesterDto>> DeleteSemester(int id);
    Task<Data_Response<SemesterDto>> UpdateSemester(int id, SemesterDto model);
    Task<Data_Response<string>> BulkDelete(List<int> ids);
    Task<string> ImportExcelFile(IFormFile file);
  }
}
