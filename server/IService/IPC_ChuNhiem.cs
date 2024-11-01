using server.Dtos;
using server.Types;

namespace server.IService
{
  public interface IPC_ChuNhiem
  {
    Task<ChuNhiemResType> CreatePC_ChuNhiem(PC_ChuNhiemDto model);

    Task<ChuNhiemResType> GetPC_ChuNhiem(int id);

    Task<ChuNhiemResType> GetPC_ChuNhiems(int pageNumber, int pageSize);

    Task<ChuNhiemResType> Get_ChuNhiem_Teacher_Class(int idClass);

    Task<ChuNhiemResType> UpdatePC_ChuNhiem(int id, PC_ChuNhiemDto model);

    Task<ChuNhiemResType> DeletePC_ChuNhiem(int id);

    Task<ChuNhiemResType> BulkDelete(List<int> ids);

    Task<string> ImportExcelFile(IFormFile file);
  }
}
