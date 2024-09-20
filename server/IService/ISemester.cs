using server.Dtos;

namespace server.IService
{
  public interface ISemester
  {
    Task<Data_Response<SemesterDto>> CreateSemester(SemesterDto model);
    Task<Data_Response<SemesterDto>> GetSemester(int id);
    Task<List<SemesterDto>> GetSemesters();
    Task<Data_Response<SemesterDto>> DeleteSemester(int id);
    Task<Data_Response<SemesterDto>> UpdateSemester(int id, SemesterDto model);
  }
}
