using server.Dtos;

namespace server.IService
{
  public interface IClassify
  {
    Task<Data_Response<ClassifyDto>> CreateClassify(ClassifyDto model);
    Task<Data_Response<ClassifyDto>> GetClassify(int id);
    Task<List<ClassifyDto>> GetClassifys();
    Task<Data_Response<ClassifyDto>> DeleteClassify(int id);
    Task<Data_Response<ClassifyDto>> UpdateClassify(int id, ClassifyDto model);
  }
}
