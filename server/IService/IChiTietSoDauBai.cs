using server.Dtos;
using server.Types;
using server.Types.ChiTietSoDauBai;
using server.Types.Week;

namespace server.IService
{
  public interface IChiTietSoDauBai
  {
    Task<ResponseData<ChiTietSoDauBaiDto>> CreateChiTietSoDauBai(ChiTietSoDauBaiDto model);

    Task<ResponseData<ChiTietSoDauBaiDto>> GetChiTietSoDauBai(int id);

    Task<List<ChiTietSoDauBaiDto>> GetChiTietSoDauBais(int pageNumber, int pageSize);

    Task<ResponseData<ChiTietSoDauBaiDto>> DeleteChiTietSoDauBai(int id);

    Task<ResponseData<ChiTietSoDauBaiDto>> UpdateChiTietSoDauBai(int id, ChiTietSoDauBaiDto model);

    Task<string> ImportExcel(IFormFile file);

    Task<ResponseData<string>> BulkDelete(List<int> ids);

    /// <summary>
    /// ###Retrieves: 
    /* SELECT ct.chiTietSoDauBaiId, b.biaSoDauBaiId, b.classId, b.schoolId, c.className, t.teacherId, t.fullname
        FROM dbo.ChiTietSoDauBai as ct
        LEFT JOIN dbo.BiaSoDauBai as b ON ct.biaSoDauBaiId = b.biaSoDauBaiId 
        LEFT JOIN dbo.Class as c ON b.classId = c.classId 
        LEFT JOIN dbo.Teacher as t ON c.teacherId = t.teacherId
        WHERE ct.chiTietSoDauBaiId = @id
    */
    /// </summary>
    /// <returns></returns>
    Task<ResponseData<ChiTiet_BiaSoDauBaiResData>> GetChiTiet_Bia_Class_Teacher(int chiTietId);

    /// <summary>Get chitietid show info Week</summary>
    /// <param name="chiTietId"></param>
    /// <returns>
    /// {
    ///  "statusCode": 200,
    /// "message": "",
    ///  "data": {
    ///   "chiTietSoDauBaiId": 2,
    ///    "weekId": 1,
    ///    "weekName": "Tuần 1",
    ///    "status": true,
    ///    "xepLoaiId": 1,
    ///    "tenXepLoai": "A",
    ///    "soDiem": 10
    ///  }
    ///}
    /// </returns>
    Task<ResponseData<ChiTiet_WeekResData>> GetChiTiet_Week_XepLoai(int chiTietId);

    /// <summary>
    /// Get chi tiet sdb by SchoolId and weekId and BiaSoDauBaiId and ClassId
    /// </summary>
    /// <param name="schoolId"></param>
    /// <param name="weekId"></param>
    /// <returns>chiTiet.*, 
    ///  c.className, 
    ///  t.teacherId, 
    ///  t.fullname
    ///  </returns>
    Task<ResponseData<IEnumerable<ChiTietSDBResData>>> GetChiTietBySchool(int schoolId, int weekId, int biaId, int classId, int pageNumber, int pageSize);

  }
}
