using server.Dtos;

namespace server.IService
{
  public interface IStudent
  {
    Task<ResponseData<StudentDto>> CreateStudent(StudentDto model);
    Task<ResponseData<StudentDto>> GetStudent(int id);
    Task<List<StudentDto>> GetStudents(int pageNumber, int pageSize);
    Task<ResponseData<StudentDto>> DeleteStudent(int id);
    Task<ResponseData<StudentDto>> UpdateStudent(int id, StudentDto model);
    Task<ResponseData<string>> BulkDelete(List<int> ids);
    Task<string> ImportExcel(IFormFile file);
  }
}
