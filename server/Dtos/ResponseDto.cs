namespace server.Dtos
{
  public class ResponseDto
  {
    public bool IsSuccess { get; set; } = false;

    public string Message { get; set; } = String.Empty;

    public string AccessToken { get; set; } = String.Empty;

    public string RefreshToken { get; set; } = String.Empty;
  }
}
