using server.Dtos;
using server.IService;
using server.Models;

namespace server.Repositories
{
  public class MonthlyEvaluationRepositories : IMonthlyEvaluation
  {
    public Task<ResponseData<MonthlyEvaluation>> BulkDelete(List<int> ids)
    {
      throw new NotImplementedException();
    }

    public Task<ResponseData<MonthlyEvaluation>> Create(MonthlyEvaluationDto model)
    {
      throw new NotImplementedException();
    }

    public Task<ResponseData<MonthlyEvaluation>> Delete(int id)
    {
      throw new NotImplementedException();
    }

    public Task<ResponseData<MonthlyEvaluation>> GetAll()
    {
      throw new NotImplementedException();
    }

    public Task<ResponseData<MonthlyEvaluation>> GetById(int id)
    {
      throw new NotImplementedException();
    }

    public Task<ResponseData<MonthlyEvaluation>> Update(int id, MonthlyEvaluationDto model)
    {
      throw new NotImplementedException();
    }
  }
}
