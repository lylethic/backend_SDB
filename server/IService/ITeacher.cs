using server.Dtos;

namespace server.IService
{
  public interface ITeacher
  {
    Task<Data_Response<TeacherDto>> CreateTeacher(TeacherDto model);

    Task<Data_Response<TeacherDto>> GetTeacher(int id);

    Task<List<TeacherDto>> GetTeachers(int pageNumber, int pageSize);

    Task<Data_Response<TeacherDto>> DeleteTeacher(int id);

    Task<Data_Response<TeacherDto>> UpdateTeacher(int id, TeacherDto model);

    Task<string> ImportExcelFile(IFormFile file);

    Task<Data_Response<string>> BulkDelete(List<int> ids);
  }
}
