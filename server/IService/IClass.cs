using server.Dtos;
using server.Types;

namespace server.IService
{
  public interface IClass
  {
    Task<ResponseData<ClassDto>> CreateClass(ClassDto model);

    Task<ResponseData<ClassDto>> GetClass(int id);

    Task<List<ClassDto>> GetClasses(int pageNumber, int pageSize);

    Task<ResponseData<ClassDto>> DeleteClass(int id);

    Task<ResponseData<ClassDto>> UpdateClass(int id, ClassDto model);

    Task<string> ImportExcel(IFormFile file);

    Task<ResponseData<string>> BulkDelete(List<int> ids);

    Task<List<ClassDto>> GetClassesBySchool(int pageNumber, int pageSize, int schoolId);
  }
}
