using server.Dtos;

namespace server.IService
{
  public interface IAcademicYear
  {
    Task<ResponseData<AcademicYearDto>> CreateAcademicYear(AcademicYearDto model);

    Task<ResponseData<AcademicYearDto>> GetAcademicYear(int id);

    Task<List<AcademicYearDto>> GetAcademicYears(int pageNumber, int pageSize);

    Task<ResponseData<AcademicYearDto>> DeleteAcademicYear(int id);

    Task<ResponseData<AcademicYearDto>> UpdateAcademicYear(int id, AcademicYearDto model);

    Task<ResponseData<string>> BulkDelete(List<int> ids);

    Task<string> ImportExcel(IFormFile file);
  }
}
