using server.Dtos;

namespace server.Types
{
  public class WeekResType : ModelResType
  {
    public List<SevenDaysInWeek>? Data { get; set; }

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
