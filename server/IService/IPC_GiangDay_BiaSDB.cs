using server.Dtos;

namespace server.IService
{
  public interface IPC_GiangDay_BiaSDB
  {
    Task<Data_Response<PC_GiangDay_BiaSDBDto>> CreatePC_GiangDay_BiaSDB(PC_GiangDay_BiaSDBDto model);
    Task<Data_Response<PC_GiangDay_BiaSDBDto>> GetPC_GiangDay_BiaSDB(int id);
    Task<List<PC_GiangDay_BiaSDBDto>> GetPC_GiangDay_BiaSDBs();
    Task<Data_Response<PC_GiangDay_BiaSDBDto>> DeletePC_GiangDay_BiaSDB(int id);
    Task<Data_Response<PC_GiangDay_BiaSDBDto>> UpdatePC_GiangDay_BiaSDB(int id, PC_GiangDay_BiaSDBDto model);
  }
}
