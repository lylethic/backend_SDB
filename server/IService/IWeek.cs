using server.Dtos;

namespace server.IService
{
  public interface IWeek
  {
    Task<Data_Response<WeekDto>> CreateWeek(WeekDto model);
    Task<Data_Response<WeekDto>> GetWeek(int id);
    Task<List<WeekDto>> GetWeeks();
    Task<Data_Response<WeekDto>> DeleteWeek(int id);
    Task<Data_Response<WeekDto>> UpdateWeek(int id, WeekDto model);
  }
}
