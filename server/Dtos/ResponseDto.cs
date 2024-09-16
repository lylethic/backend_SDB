namespace server.Dtos
{
  public class ResponseDto
  {
    public bool IsSuccess { get; set; } = false;

    public string Message { get; set; } = String.Empty;

    public string AccessToken { get; set; } = String.Empty;

    public string RefreshToken { get; set; } = String.Empty;

    public ResponseDto()
    {
    }

    public ResponseDto(bool isSuccess, string mess)
    {
      this.IsSuccess = isSuccess;
      this.Message = mess;
    }
  }
}
