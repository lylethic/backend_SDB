using server.Dtos;
using server.Types;

namespace server.IService
{
  public interface IChiTietSoDauBai
  {
    Task<Data_Response<ChiTietSoDauBaiDto>> CreateChiTietSoDauBai(ChiTietSoDauBaiDto model);

    Task<Data_Response<ChiTietSoDauBaiDto>> GetChiTietSoDauBai(int id);

    Task<List<ChiTietSoDauBaiDto>> GetChiTietSoDauBais(int pageNumber, int pageSize);

    Task<Data_Response<ChiTietSoDauBaiDto>> DeleteChiTietSoDauBai(int id);

    Task<Data_Response<ChiTietSoDauBaiDto>> UpdateChiTietSoDauBai(int id, ChiTietSoDauBaiDto model);

    Task<string> ImportExcel(IFormFile file);

    Task<Data_Response<string>> BulkDelete(List<int> ids);

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
    Task<Data_Response<ChiTiet_BiaSoDauBaiResType>> GetChiTiet_Bia_Class_Teacher(int chiTietId);

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
    Task<Data_Response<ChiTiet_WeekResType>> GetChiTiet_Week_XepLoai(int chiTietId);

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
    Task<Data_Response<IEnumerable<ChiTietSDBResType>>> GetChiTietBySchool(int schoolId, int weekId, int biaId, int classId, int pageNumber, int pageSize);

  }
}
