namespace server.Dtos
{
  public class AccountResponse<T>
  {
    public int StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;

    public T Data { get; set; }

    public AccountResponse()
    {
    }

    public AccountResponse(int statusCode, string message)
    {
      StatusCode = statusCode;
      Message = message;
    }

    public AccountResponse(int statusCode, T data)
    {
      this.StatusCode = statusCode;
      this.Data = data;
    }
  }
}
