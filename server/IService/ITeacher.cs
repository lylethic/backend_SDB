using server.Dtos;

namespace server.IService
{
  public interface ITeacher
  {
    Task<ResponseData<TeacherDto>> CreateTeacher(TeacherDto model);

    Task<ResponseData<TeacherDto>> GetTeacher(int id);

    Task<List<TeacherDto>> GetTeachers(int pageNumber, int pageSize);

    Task<List<TeacherDto>> GetTeachersBySchool(int pageNumber, int pageSize, int schoolId);

    Task<ResponseData<TeacherDto>> DeleteTeacher(int id);

    Task<ResponseData<TeacherDto>> UpdateTeacher(int id, TeacherDto model);

    Task<string> ImportExcelFile(IFormFile file);

    Task<ResponseData<string>> BulkDelete(List<int> ids);
  }
}
