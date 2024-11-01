using server.Dtos;

namespace server.IService
{
  public interface IPC_GiangDay_BiaSDB
  {
    Task<ResponseData<PC_GiangDay_BiaSDBDto>> CreatePC_GiangDay_BiaSDB(PC_GiangDay_BiaSDBDto model);

    Task<ResponseData<PC_GiangDay_BiaSDBDto>> GetPC_GiangDay_BiaSDB(int id);

    Task<List<PC_GiangDay_BiaSDBDto>> GetPC_GiangDay_BiaSDBs(int pageNumber, int pageSize);

    Task<ResponseData<PC_GiangDay_BiaSDBDto>> DeletePC_GiangDay_BiaSDB(int id);

    Task<ResponseData<PC_GiangDay_BiaSDBDto>> UpdatePC_GiangDay_BiaSDB(int id, PC_GiangDay_BiaSDBDto model);

    Task<ResponseData<string>> BulkDelete(List<int> ids);

    Task<string> ImportExcelFile(IFormFile file);
  }
}
