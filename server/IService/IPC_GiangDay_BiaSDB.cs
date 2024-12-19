using server.Dtos;
using server.Types.PhanCongGDBia;

namespace server.IService
{
  public interface IPC_GiangDay_BiaSDB
  {
    Task<PhanCongGiangDayBiaResType> CreatePC_GiangDay_BiaSDB(PC_GiangDay_BiaSDBDto model);

    Task<PhanCongGiangDayBiaResType> GetPC_GiangDay_BiaSDB(int id);

    Task<PhanCongGiangDayBiaResType> GetPC_GiangDay_BiaSDBs(QueryObject queryObject);

    Task<PhanCongGiangDayBiaResType> DeletePC_GiangDay_BiaSDB(int id);

    Task<PhanCongGiangDayBiaResType> UpdatePC_GiangDay_BiaSDB(int id, PC_GiangDay_BiaSDBDto model);

    Task<PhanCongGiangDayBiaResType> BulkDelete(List<int> ids);

    Task<PhanCongGiangDayBiaResType> ImportExcelFile(IFormFile file);
  }
}
