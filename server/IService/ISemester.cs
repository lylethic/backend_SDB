using server.Dtos;
using server.Types.Semester;

namespace server.IService
{
  public interface ISemester
  {
    Task<ResponseData<SemesterDto>> CreateSemester(SemesterDto model);

    Task<ResponseData<SemesterResData>> GetSemester(int id);

    Task<List<SemesterDto>> GetSemesters(int pageNumber, int pageSize);

    Task<ResponseData<SemesterDto>> DeleteSemester(int id);

    Task<ResponseData<SemesterDto>> UpdateSemester(int id, SemesterDto model);

    Task<ResponseData<string>> BulkDelete(List<int> ids);

    Task<string> ImportExcelFile(IFormFile file);
  }
}
