using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using server.Data;
using server.Dtos;
using server.IService;

namespace server.Repositories
{
  public class PCChuNhiemRepositories : IPC_ChuNhiem
  {
    private readonly SoDauBaiContext _context;

    public PCChuNhiemRepositories(SoDauBaiContext context)
    {
      this._context = context;
    }

    public Task<Data_Response<string>> BulkDelete(List<int> ids)
    {
      throw new NotImplementedException();
    }

    public async Task<Data_Response<PC_ChuNhiemDto>> CreatePC_ChuNhiem(PC_ChuNhiemDto model)
    {
      using var transaction = await _context.Database.BeginTransactionAsync();

      try
      {
        var find = "SELECT PhanCongChuNhiemId FROM PhanCongChuNhiem WHERE phanCongChuNhiemId = @id";

        var phanCongChuNhiem = await _context.PhanCongChuNhiems
            .FromSqlRaw(find, new SqlParameter("@id", model.PhanCongChuNhiemId))
            .FirstOrDefaultAsync();

        // Tao moi => id Duy nhat ko trung
        if (phanCongChuNhiem is not null)
        {
          return new Data_Response<PC_ChuNhiemDto>(409, "PhanCongChuNhiemID already exists");
        }

        // Check if teacherId exists
        var teacherExists = await _context.Teachers
            .AnyAsync(c => c.TeacherId == model.TeacherId);

        if (!teacherExists)
        {
          return new Data_Response<PC_ChuNhiemDto>(404, "ClassId not found");
        }

        // Check if classId exists
        var classExists = await _context.Classes
            .AnyAsync(c => c.ClassId == model.ClassId);

        if (!classExists)
        {
          return new Data_Response<PC_ChuNhiemDto>(404, "ClassId not found");
        }

        // Check if semesterId exists
        var semesterExists = await _context.Semesters
            .AnyAsync(s => s.SemesterId == model.SemesterId);

        if (!semesterExists)
        {
          return new Data_Response<PC_ChuNhiemDto>(404, "SemesterId not found");
        }

        model.DateCreated = DateTime.UtcNow;
        model.DateUpdated = null;

        var queryInsert = @"INSERT INTO PhanCongChuNhiem 
                                        (teacherId, classId, semesterId, 
                                          status, dateCreated, dateUpdated, description)
                                VALUES 
                                        (@teacherId, @classId, @semesterId, 
                                          @status, @dateCreated, @dateUpdated, @description);
                                SELECT CAST(SCOPE_IDENTITY() AS INT);"
        ;

        var insert = await _context.Database.ExecuteSqlRawAsync(queryInsert,
            new SqlParameter("@teacherId", model.TeacherId),
            new SqlParameter("@classId", model.ClassId),
            new SqlParameter("@semesterId", model.SemesterId),
            new SqlParameter("@status", model.Status),
            new SqlParameter("@dateCreated", model.DateCreated),
            new SqlParameter("@dateUpdated", DBNull.Value),
            new SqlParameter("@description", model.Description)
        );

        // Commit the transaction after the insert succeeds
        await transaction.CommitAsync();

        var result = new PC_ChuNhiemDto
        {
          PhanCongChuNhiemId = model.PhanCongChuNhiemId,
          TeacherId = model.TeacherId,
          ClassId = model.ClassId,
          SemesterId = model.SemesterId,
          Status = model.Status,
          DateCreated = model.DateCreated,
          DateUpdated = model.DateUpdated,
          Description = model.Description,
        };

        return new Data_Response<PC_ChuNhiemDto>(200, result);
      }
      catch (Exception ex)
      {
        // Rollback the transaction if any exception occurs
        await transaction.RollbackAsync();

        return new Data_Response<PC_ChuNhiemDto>(500, $"Server Error: {ex.Message}");
      }
    }

    public async Task<Data_Response<PC_ChuNhiemDto>> DeletePC_ChuNhiem(int id)
    {
      try
      {
        var find = "SELECT * FROM PhanCongChuNhiem WHERE PhanCongChuNhiem = @id";

        var phanCongChuNhiem = await _context.BiaSoDauBais
          .FromSqlRaw(find, new SqlParameter("@id", id))
          .FirstOrDefaultAsync();

        if (phanCongChuNhiem is null)
        {
          return new Data_Response<PC_ChuNhiemDto>(404, "PC_ChuNhiemId not found");
        }

        var deleteQuery = "DELETE FROM PhanCongChuNhiem WHERE PhanCongChuNhiemId = @id";

        await _context.Database.ExecuteSqlRawAsync(deleteQuery, new SqlParameter("@id", id));

        return new Data_Response<PC_ChuNhiemDto>(200, "Deleted");
      }
      catch (Exception ex)
      {
        return new Data_Response<PC_ChuNhiemDto>(500, $"Server error: {ex.Message}");
      }
    }

    public Task<Data_Response<PC_ChuNhiemDto>> GetPC_ChuNhiem(int id)
    {
      throw new NotImplementedException();
    }

    public Task<List<PC_ChuNhiemDto>> GetPC_ChuNhiems(int pagenNumber, int pageSize)
    {
      throw new NotImplementedException();
    }

    public Task<Data_Response<PC_ChuNhiemDto>> Get_ChuNhiem_Teacher_Class(int chuNhiemId)
    {
      throw new NotImplementedException();
    }

    public Task<string> ImportExcelFile(IFormFile file)
    {
      throw new NotImplementedException();
    }

    public Task<Data_Response<PC_ChuNhiemDto>> UpdatePC_ChuNhiem(int id, PC_ChuNhiemDto model)
    {
      throw new NotImplementedException();
    }
  }
}
