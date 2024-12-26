using server.Dtos;
using server.Types.LopHoc;

namespace server.IService
{
  public interface IClass
  {
    Task<ResponseData<ClassDto>> CreateClass(ClassDto model);

    Task<ResponseData<ClassDetails>> GetClass(int id);
    Task<ResponseData<List<ClassList>>> ClassList(QueryObject? queryObject);

    Task<ResponseData<List<ClassDetails>>> GetClasses(QueryObject? queryObject);

    Task<ResponseData<ClassDto>> DeleteClass(int id);

    Task<ResponseData<ClassDto>> UpdateClass(int id, ClassDto model);

    Task<ResponseData<string>> ImportExcel(IFormFile file);

    Task<ResponseData<string>> BulkDelete(List<int> ids);

    Task<ResponseData<List<ClassList>>> GetClassesBySchool(QueryObject? queryObject, int schoolId);
  }
}
