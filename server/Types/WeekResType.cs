using server.Dtos;

namespace server.Types
{
  public class WeekResType
  {
    public string Message { get; set; } = string.Empty;

    public List<SevenDaysInWeek> Data { get; set; }

    public WeekResType(string message)
    {
      this.Message = message;
    }

    public WeekResType(string message, List<SevenDaysInWeek> data)
    {
      this.Message = message;
      this.Data = data;
    }
  }
}
