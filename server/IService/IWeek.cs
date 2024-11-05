using server.Dtos;
using server.Types.Week;

namespace server.IService
{
  public interface IWeek
  {
    Task<ResponseData<WeekDto>> CreateWeek(WeekDto model);

    Task<ResponseData<WeekData>> GetWeek(int id);

    Task<List<WeekDto>> GetWeeks(int pageNumber, int pageSize);

    Task<List<WeekDto>> GetWeeksBySemester(int pageNumber, int pageSize, int semesterId);

    Task<ResponseData<WeekDto>> DeleteWeek(int id);

    Task<ResponseData<WeekDto>> UpdateWeek(int id, WeekDto model);

    Task<string> ImportExcelFile(IFormFile file);

    Task<ResponseData<string>> BulkDelete(List<int> ids);

    Task<WeekResType> Get7DaysInWeek(int selectedWeekId);
  }
}
