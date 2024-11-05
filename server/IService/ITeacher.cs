using server.Dtos;
using server.Types.Teacher;

namespace server.IService
{
  public interface ITeacher
  {
    Task<TeacherResType> CreateTeacher(TeacherDto model);

    Task<TeacherResType> GetTeacher(int id);

    Task<int> GetCountTeachersBySchool(int id);

    Task<TeacherResType> GetTeachers(int pageNumber, int pageSize);

    Task<TeacherResType> GetTeachersBySchool(int pageNumber, int pageSize, int schoolId);

    Task<TeacherResType> DeleteTeacher(int id);

    Task<TeacherResType> UpdateTeacher(int id, TeacherDto model);

    Task<string> ImportExcelFile(IFormFile file);

    Task<ResponseData<string>> BulkDelete(List<int> ids);
  }
}
