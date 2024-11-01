using server.Dtos;

namespace server.IService
{
  public interface ISubject
  {
    Task<ResponseData<SubjectDto>> CreateSubject(SubjectDto model);
    Task<ResponseData<SubjectDto>> GetSubject(int id);
    Task<List<SubjectDto>> GetSubjects(int pageNumber, int pageSize);
    Task<ResponseData<SubjectDto>> DeleteSubject(int id);
    Task<ResponseData<SubjectDto>> UpdateSubject(int id, SubjectDto model);
    Task<ResponseData<string>> BulkDelete(List<int> ids);
    Task<string> ImportExcelFile(IFormFile file);
  }
}
