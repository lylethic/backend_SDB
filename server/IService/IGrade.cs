using server.Dtos;

namespace server.IService
{
  public interface IGrade
  {
    Task<ResponseData<GradeDto>> CreateGrade(GradeDto model);

    Task<ResponseData<GradeDto>> GetGrade(int id);

    Task<List<GradeDto>> GetGrades(int pageNumber, int pageSize);

    Task<ResponseData<GradeDto>> DeleteGrade(int id);

    Task<ResponseData<GradeDto>> UpdateGrade(int id, GradeDto model);

    Task<ResponseData<string>> BulkDelete(List<int> ids);

    Task<string> ImportExcelFile(IFormFile file);
  }
}
