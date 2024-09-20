using server.Data;
using server.Dtos;
using server.IService;

namespace server.Repositories
{
  public class ClassRepositories : IClass
  {
    private readonly SoDauBaiContext _context;

    public ClassRepositories(SoDauBaiContext context)
    {
      this._context = context;
    }

    public Task<Data_Response<ClassDto>> CreateClass(ClassDto model)
    {
      throw new NotImplementedException();
    }

    public Task<Data_Response<ClassDto>> DeleteClass(int id)
    {
      throw new NotImplementedException();
    }

    public Task<Data_Response<ClassDto>> GetClass(int id)
    {
      throw new NotImplementedException();
    }

    public Task<List<ClassDto>> GetClasss()
    {
      throw new NotImplementedException();
    }

    public Task<Data_Response<ClassDto>> UpdateClass(int id, ClassDto model)
    {
      throw new NotImplementedException();
    }
  }
}
