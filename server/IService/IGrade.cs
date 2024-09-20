using server.Dtos;

namespace server.IService
{
  public interface IGrade
  {
    Task<Data_Response<GradeDto>> CreateGrade(GradeDto model);
    Task<Data_Response<GradeDto>> GetGrade(int id);
    Task<List<GradeDto>> GetGrades();
    Task<Data_Response<GradeDto>> DeleteGrade(int id);
    Task<Data_Response<GradeDto>> UpdateGrade(int id, GradeDto model);
  }
}
