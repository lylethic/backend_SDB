namespace server.Types.RollCall
{
  public class RollCallResType
  {
    public int StatusCode { get; set; }

    public string Message { get; set; } = string.Empty;

    public RollCallRes? RollCallRes { get; set; }

    public Models.RollCall? RollCall { get; set; }

    public List<Models.RollCall>? ListRollCallRes { get; set; }

    public RollCallResType() { }

    public RollCallResType(int status, string message)
    {
      this.StatusCode = status;
      this.Message = message;
    }

    public RollCallResType(int status, string message, Models.RollCall rollCall)
    {
      this.StatusCode = status;
      this.Message = message;
      this.RollCall = rollCall;
    }

    public RollCallResType(int status, string message, List<Models.RollCall> listRollCallRes)
    {
      this.StatusCode = status;
      this.Message = message;
      this.ListRollCallRes = listRollCallRes;
    }
  }
}
