using server.Dtos;

namespace server.IService
{
  public interface IStudent
  {
    Task<Data_Response<StudentDto>> CreateStudent(StudentDto model);
    Task<Data_Response<StudentDto>> GetStudent(int id);
    Task<List<StudentDto>> GetStudents();
    Task<Data_Response<StudentDto>> DeleteStudent(int id);
    Task<Data_Response<StudentDto>> UpdateStudent(int id, StudentDto model);
    Task<Data_Response<string>> BulkDelete(List<int> ids);
    Task<string> ImportExcel(IFormFile file);
  }
}
