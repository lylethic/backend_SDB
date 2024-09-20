using server.Dtos;

namespace server.IService
{
  public interface IPC_ChuNhiem
  {
    Task<Data_Response<PC_ChuNhiemDto>> CreatePC_ChuNhiem(PC_ChuNhiemDto model);
    Task<Data_Response<PC_ChuNhiemDto>> GetPC_ChuNhiem(int id);
    Task<List<PC_ChuNhiemDto>> GetPC_ChuNhiems();
    Task<Data_Response<PC_ChuNhiemDto>> DeletePC_ChuNhiem(int id);
    Task<Data_Response<PC_ChuNhiemDto>> UpdatePC_ChuNhiem(int id, PC_ChuNhiemDto model);
  }
}
