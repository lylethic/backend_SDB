using server.Dtos;

namespace server.IService
{
  public interface ISubject
  {
    Task<Data_Response<SubjectDto>> CreateSubject(SubjectDto model);
    Task<Data_Response<SubjectDto>> GetSubject(int id);
    Task<List<SubjectDto>> GetSubjects(int pageNumber, int pageSize);
    Task<Data_Response<SubjectDto>> DeleteSubject(int id);
    Task<Data_Response<SubjectDto>> UpdateSubject(int id, SubjectDto model);
    Task<Data_Response<string>> BulkDelete(List<int> ids);
    Task<string> ImportExcelFile(IFormFile file);
  }
}
