using server.Dtos;
using server.Types;

namespace server.IService
{
  public interface IWeek
  {
    Task<Data_Response<WeekDto>> CreateWeek(WeekDto model);

    Task<Data_Response<WeekResType>> GetWeek(int id);

    Task<List<WeekDto>> GetWeeks(int pageNumber, int pageSize);

    Task<List<WeekDto>> GetWeeksBySemester(int pageNumber, int pageSize, int semesterId);

    Task<Data_Response<WeekDto>> DeleteWeek(int id);

    Task<Data_Response<WeekDto>> UpdateWeek(int id, WeekDto model);

    Task<string> ImportExcelFile(IFormFile file);

    Task<Data_Response<string>> BulkDelete(List<int> ids);
  }
}
