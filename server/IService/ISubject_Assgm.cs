using server.Dtos;

namespace server.IService
{
  public interface ISubject_Assgm
  {
    Task<Data_Response<SubjectAssgmDto>> CreateSubjectAssgm(SubjectAssgmDto model);
    Task<Data_Response<SubjectAssgmDto>> GetSubjectAssgm(int id);
    Task<List<SubjectAssgmDto>> GetSubjectAssgms(int pageNumber, int pageSize);
    Task<Data_Response<SubjectAssgmDto>> DeleteSubjectAssgm(int id);
    Task<Data_Response<SubjectAssgmDto>> UpdateSubjectAssgm(int id, SubjectAssgmDto model);
    Task<Data_Response<string>> BulkDelete(List<int> ids);
    Task<string> ImportExcel(IFormFile file);
  }
}
