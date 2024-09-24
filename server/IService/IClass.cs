using server.Dtos;

namespace server.IService
{
  public interface IClass
  {
    Task<Data_Response<ClassDto>> CreateClass(ClassDto model);

    Task<string> ImportClassExcel(IFormFile file);

    Task<Data_Response<ClassDto>> GetClass(int id);

    Task<List<ClassDto>> GetClasses();

    Task<Data_Response<ClassDto>> DeleteClass(int id);

    Task<Data_Response<string>> BulkDelete(List<int> ids);

    Task<Data_Response<ClassDto>> UpdateClass(int id, ClassDto model);
  }
}
