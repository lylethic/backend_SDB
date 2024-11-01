namespace server.Dtos
{
  public class ResponseData<T>
  {
    public int StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;

    public T? Data { get; set; }

    public ResponseData()
    {
    }

    public ResponseData(int statusCode, string message)
    {
      this.StatusCode = statusCode;
      this.Message = message;
    }

    public ResponseData(int statusCode, T data)
    {
      this.StatusCode = statusCode;
      this.Data = data;
    }

    public ResponseData(int statusCode, string message, T data)
    {
      this.StatusCode = statusCode;
      this.Message = message;
      this.Data = data;
    }
  }
}
