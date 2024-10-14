namespace server.Dtos
{
  public class Data_Response<T>
  {
    public int StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;

    public T? Data { get; set; }

    public Data_Response()
    {
    }

    public Data_Response(int statusCode, string message)
    {
      this.StatusCode = statusCode;
      this.Message = message;
    }

    public Data_Response(int statusCode, T data)
    {
      this.StatusCode = statusCode;
      this.Data = data;
    }

    public Data_Response(int statusCode, string message, T data)
    {
      this.StatusCode = statusCode;
      this.Message = message;
      this.Data = data;
    }
  }
}
