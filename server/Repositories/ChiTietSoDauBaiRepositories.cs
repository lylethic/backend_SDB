using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using server.Data;
using server.Dtos;
using server.IService;

namespace server.Repositories
{
  public class ChiTietSoDauBaiRepositories : IChiTietSoDauBai
  {
    private readonly SoDauBaiContext _context;

    public ChiTietSoDauBaiRepositories(SoDauBaiContext context)
    {
      this._context = context;
    }

    public Task<Data_Response<string>> BulkDelete(List<int> ids)
    {
      throw new NotImplementedException();
    }

    public async Task<Data_Response<ChiTietSoDauBaiDto>> CreateChiTietSoDauBai(ChiTietSoDauBaiDto model)
    {
      await using var transaction = await _context.Database.BeginTransactionAsync();

      try
      {
        // Tim bIa so dau bai ton tai khong?
        var findBia = "SELECT BiaSoDauBaiId FROM BiaSoDauBai WHERE BiaSoDauBaiId = @id";

        var existingBia = await _context.BiaSoDauBais
          .FromSqlRaw(findBia, new SqlParameter("@id", model.BiaSoDauBaiId))
          .FirstOrDefaultAsync();

        if (existingBia is null)
        {
          return new Data_Response<ChiTietSoDauBaiDto>(404, "Not found");
        }

        // Tim semster ton tai khong?
        var findSemester = "SELECT SemesterId FROM Semester WHERE SemesterId = @id";

        var existingSemester = await _context.Semesters
          .FromSqlRaw(findSemester, new SqlParameter("@id", model.SemesterId))
          .FirstOrDefaultAsync();

        if (existingSemester is null)
        {
          return new Data_Response<ChiTietSoDauBaiDto>(404, "Not found");
        }

        // Tim Week ton tai khong?
        var findWeek = "SELECT WeekId FROM Week WHERE WeekId = @id";

        var existingWeek = await _context.Weeks
          .FromSqlRaw(findWeek, new SqlParameter("@id", model.WeekId))
          .FirstOrDefaultAsync();

        if (existingWeek is null)
        {
          return new Data_Response<ChiTietSoDauBaiDto>(404, "Not found");
        }

        // Tim Mon hoc ton tai khong?
        var findSubject = "SELECT SujectId FROM Suject WHERE SujectId = @id";

        var existingSubject = await _context.Subjects
          .FromSqlRaw(findSubject, new SqlParameter("@id", model.SubjectId))
          .FirstOrDefaultAsync();

        if (existingSubject is null)
        {
          return new Data_Response<ChiTietSoDauBaiDto>(404, "Not found");
        }

        // Tim Xep loai ton tai khong?
        var findXepLoai = "SELECT ClassificationId FROM Classification WHERE ClassificationId = @id";

        var existingXepLoai = await _context.Classifications
          .FromSqlRaw(findXepLoai, new SqlParameter("@id", model.ClassificationId))
          .FirstOrDefaultAsync();

        if (existingXepLoai is null)
        {
          return new Data_Response<ChiTietSoDauBaiDto>(404, "Not found");
        }

        var find = "SELECT ChiTietSodauBaiId FROM ChiTietSodauBai WHERE ChiTietSodauBaiId = @id";

        var existingChiTietSoDauBai = await _context.ChiTietSoDauBais
         .FromSqlRaw(find, new SqlParameter("@id", model.ChiTietSoDauBaiId))
         .FirstOrDefaultAsync();

        if (existingChiTietSoDauBai is not null)
        {
          return new Data_Response<ChiTietSoDauBaiDto>(409, "Chi tiet so dau bai already exists");
        }

        var insertdata = @"INSERT INTO CHITIETSODAUBAI (biaSoDauBaiId, semesterId, weekId, subjectId, 
                                    classificationId, daysOfTheWeek, thoiGian, buoiHoc, 
                                    tietHoc, lessonContent, attend,
                                    noteComment, createdBy, createdAt, updatedAt)
                           VALUES (@biaSoDauBaiId, @semesterId, @weekId, @subjectId, 
                                    @classificationId, @daysOfTheWeek, @thoiGian, 
                                    @buoiHoc, @tietHoc, @lessonContent, @attend,
                                    @noteComment, @createdBy, @createdAt, @updatedAt);
                           SELECT CAST(SCOPE_IDENTITY() AS int;";

        var currentDate = DateTime.Now;

        var insertChiTietSoDauBai = await _context.Database.ExecuteSqlRawAsync(insertdata,
          new SqlParameter("@biaSoDauBaiId", model.BiaSoDauBaiId),
          new SqlParameter("@semesterId", model.SemesterId),
          new SqlParameter("@weekId", model.WeekId),
          new SqlParameter("@subjectId", model.SubjectId),
          new SqlParameter("@classificationId", model.ClassificationId),
          new SqlParameter("@daysOfTheWeek", model.DaysOfTheWeek),
          new SqlParameter("@thoiGian", model.ThoiGian),
          new SqlParameter("@buoiHoc", model.BuoiHoc),
          new SqlParameter("@tietHoc", model.TietHoc),
          new SqlParameter("@lessonContent", model.LessonContent),
          new SqlParameter("@attend", model.Attend),
          new SqlParameter("@noteComment", model.NoteComment),
          new SqlParameter("@createdBy", model.CreatedBy),
          new SqlParameter("@createdAt", currentDate),
          new SqlParameter("@updatedAt", DBNull.Value)
          );

        var result = new ChiTietSoDauBaiDto
        {
          ChiTietSoDauBaiId = model.ChiTietSoDauBaiId,
          BiaSoDauBaiId = model.BiaSoDauBaiId,
          SemesterId = model.SemesterId,
          WeekId = model.WeekId,
          SubjectId = model.SubjectId,
          ClassificationId = model.ClassificationId,
          DaysOfTheWeek = model.DaysOfTheWeek,
          ThoiGian = model.ThoiGian,
          BuoiHoc = model.BuoiHoc,
          TietHoc = model.TietHoc,
          LessonContent = model.LessonContent,
          Attend = model.Attend,
          NoteComment = model.NoteComment,
          CreatedBy = model.CreatedBy,
          CreatedAt = model.CreatedAt,
          UpdatedAt = model.UpdatedAt,
        };

        return new Data_Response<ChiTietSoDauBaiDto>(200, result);
      }
      catch (Exception ex)
      {
        throw new Exception($"Server Error: {ex.Message}");
      }
    }

    public Task<Data_Response<ChiTietSoDauBaiDto>> DeleteChiTietSoDauBai(int id)
    {
      throw new NotImplementedException();
    }

    public Task<Data_Response<ChiTietSoDauBaiDto>> GetChiTietSoDauBai(int id)
    {
      throw new NotImplementedException();
    }

    public Task<List<ChiTietSoDauBaiDto>> GetChiTietSoDauBais()
    {
      throw new NotImplementedException();
    }

    public Task<string> ImportExcel(IFormFile file)
    {
      throw new NotImplementedException();
    }

    public Task<Data_Response<ChiTietSoDauBaiDto>> UpdateChiTietSoDauBai(int id, ChiTietSoDauBaiDto model)
    {
      throw new NotImplementedException();
    }
  }
}
