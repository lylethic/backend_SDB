using server.Dtos;

namespace server.IService
{
  public interface ISubject_Assgm
  {
    Task<ResponseData<SubjectAssgmDto>> CreateSubjectAssgm(SubjectAssgmDto model);
    Task<ResponseData<SubjectAssgmDto>> GetSubjectAssgm(int id);
    Task<List<SubjectAssgmDto>> GetSubjectAssgms(int pageNumber, int pageSize);
    Task<ResponseData<SubjectAssgmDto>> DeleteSubjectAssgm(int id);
    Task<ResponseData<SubjectAssgmDto>> UpdateSubjectAssgm(int id, SubjectAssgmDto model);
    Task<ResponseData<string>> BulkDelete(List<int> ids);
    Task<string> ImportExcel(IFormFile file);
  }
}
