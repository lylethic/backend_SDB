using server.Dtos;

namespace server.IService
{
  public interface IClassify
  {
    Task<ResponseData<ClassifyDto>> CreateClassify(ClassifyDto model);
    Task<ResponseData<ClassifyDto>> GetClassify(int id);
    Task<List<ClassifyDto>> GetClassifys(int pageNumber, int pageSize);
    Task<ResponseData<ClassifyDto>> DeleteClassify(int id);
    Task<ResponseData<ClassifyDto>> UpdateClassify(int id, ClassifyDto model);
    Task<ResponseData<string>> BulkDelete(List<int> ids);
    Task<string> ImportExcel(IFormFile file);
  }
}
