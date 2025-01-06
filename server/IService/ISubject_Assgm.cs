using server.Dtos;

namespace server.IService
{
  public interface ISubject_Assgm
  {
    Task<ResponseData<SubjectAssgmDto>> CreateSubjectAssgm(SubjectAssgmDto model);
    Task<ResponseData<SubjectAssgmDto>> GetSubjectAssgm(int id);
    Task<ResponseData<List<SubjectAssgmDto>>> GetSubjectAssgms();
    Task<ResponseData<SubjectAssgmDto>> DeleteSubjectAssgm(int id);
    Task<ResponseData<SubjectAssgmDto>> UpdateSubjectAssgm(int id, SubjectAssgmDto model);
    Task<ResponseData<string>> BulkDelete(List<int> ids);
    Task<ResponseData<string>> ImportExcel(IFormFile file);
  }
}
