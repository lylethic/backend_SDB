using server.Dtos;

namespace server.IService
{
  public interface IAcademicYear
  {
    Task<Data_Response<AcademicYearDto>> CreateAcademicYear(AcademicYearDto model);
    Task<Data_Response<AcademicYearDto>> GetAcademicYear(int id);
    Task<List<AcademicYearDto>> GetAcademicYears();
    Task<Data_Response<AcademicYearDto>> DeleteAcademicYear(int id);
    Task<Data_Response<AcademicYearDto>> UpdateAcademicYear(int id, AcademicYearDto model);
    Task<Data_Response<string>> BulkDelete(List<int> ids);
    Task<string> ImportExcel(IFormFile file);
  }
}
