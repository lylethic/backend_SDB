using Microsoft.EntityFrameworkCore;
using server.Data;
using server.Dtos;
using server.IService;
using server.Models;
using server.Types.RollCall;

namespace server.Repositories
{
  public class RollCallRepositories : IRollCall
  {
    private readonly SoDauBaiContext _context;

    public RollCallRepositories(SoDauBaiContext context)
    {
      this._context = context;
    }

    public async Task<RollCallResType> Create(RollCallDto model, List<RollCallDetailDto> absenceDtos)
    {
      using var transaction = await _context.Database.BeginTransactionAsync();
      try
      {
        if (model is null || absenceDtos is null || !absenceDtos.Any())
        {
          return new RollCallResType(400, "Vui lòng cung cấp thông tin!");
        }

        var newRollCall = new RollCall
        {
          ClassId = model.ClassId,
          WeekId = model.WeekId,
          DayOfTheWeek = model.DayOfTheWeek,
          DateAt = model.DateAt,
          DateCreated = DateTime.UtcNow,
          DateUpdated = null,
          NumberOfAttendants = model.NumberOfAttendants,
        };

        await _context.RollCalls.AddAsync(newRollCall);
        await _context.SaveChangesAsync();

        foreach (var absenceDto in absenceDtos)
        {
          var newAbsence = new RollCallDetail
          {
            RollCallId = newRollCall.RollCallId,
            Description = absenceDto.Description,
            StudentId = absenceDto.StudentId,
            IsExcused = absenceDto.IsExecute,
          };
          await _context.RollCallDetails.AddAsync(newAbsence);
        }

        await _context.SaveChangesAsync();
        await transaction.CommitAsync();

        return new RollCallResType(200, $"Tạo điểm danh thành công.", newRollCall);
      }
      catch (Exception ex)
      {
        await transaction.RollbackAsync();
        return new RollCallResType(500, "Đang xảy ra lỗi tại server...");
        throw new Exception(ex.Message);
      }
    }

    public async Task<RollCallResType> BulkDelete(List<int> rollCallIds)
    {
      try
      {
        if (rollCallIds is null)
        {
          return new RollCallResType(400, "Dữ liệu không được cung cấp.");
        }

        var rollCalls = await _context.RollCalls
         .Include(x => x.RollCallDetails)  // Include Absences to delete them as well
         .Where(x => rollCallIds.Contains(x.RollCallId))
         .ToListAsync();

        if (rollCalls == null)
        {
          return new RollCallResType(404, "Không tìm thấy dữ liệu.");
        }

        var absencesToDelete = rollCalls.SelectMany(x => x.RollCallDetails).ToList();
        _context.RollCallDetails.RemoveRange(absencesToDelete);

        _context.RollCalls.RemoveRange(rollCalls);

        await _context.SaveChangesAsync();

        return new RollCallResType(200, "Xóa thành công");
      }
      catch (Exception ex)
      {
        return new RollCallResType(500, "Đang xảy ra lỗi tại server...");
        throw new Exception(ex.Message);
      }
    }

    public async Task<RollCallResType> Delete(int rollCallId)
    {
      try
      {
        if (rollCallId == 0)
        {
          return new RollCallResType(400, "Dữ liệu không được cung cấp.");
        }

        var rollCall = await _context.RollCalls
         .Include(x => x.RollCallDetails)  // Include Absences to delete them as well
         .FirstOrDefaultAsync(x => x.RollCallId == rollCallId);

        if (rollCall == null)
        {
          return new RollCallResType(404, "Không tìm thấy dữ liệu.");
        }
        _context.RollCallDetails.RemoveRange(rollCall.RollCallDetails);

        _context.RollCalls.Remove(rollCall);

        await _context.SaveChangesAsync();

        return new RollCallResType(200, "Xóa thành công");
      }
      catch (Exception ex)
      {
        return new RollCallResType(500, "Đang xảy ra lỗi tại server...");
        throw new Exception(ex.Message);
      }
    }

    public async Task<RollCallResType> Update(int rollCallId, RollCallDto model)
    {
      try
      {
        // Find the RollCall to update
        var rollCall = await _context.RollCalls
            .FirstOrDefaultAsync(x => x.RollCallId == rollCallId);

        if (rollCall == null)
        {
          return new RollCallResType(400, "Dữ liệu không được cung cấp.");
        }

        // Update properties
        rollCall.ClassId = model.ClassId;
        rollCall.WeekId = model.WeekId;
        rollCall.DayOfTheWeek = model.DayOfTheWeek;
        rollCall.DateAt = model.DateAt;
        rollCall.NumberOfAttendants = model.NumberOfAttendants;
        rollCall.DateUpdated = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return new RollCallResType(200, "Cập nhật thành công", rollCall);
      }
      catch (Exception ex)
      {
        return new RollCallResType(500, "Có lỗi cảy ra tại server...");
        throw new Exception(ex.Message);
      }
    }

    public async Task<RollCallResType> RollCall(int rollCallId)
    {
      try
      {
        var rollCall = await _context.RollCalls
          .Where(x => x.RollCallId == rollCallId)
          .AsNoTracking()
          .FirstOrDefaultAsync();

        if (rollCall is null) return new RollCallResType(404, "Không tìm thấy dữ liệu hoặc dữ liệu không tồn tại");

        TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

        rollCall.DateAt = rollCall.DateAt.HasValue
          ? rollCall.DateAt.Value.ToLocalTime()
          : null;

        rollCall.DateCreated = rollCall.DateCreated.HasValue
          ? rollCall.DateCreated.Value.ToLocalTime()
          : null;

        rollCall.DateUpdated = rollCall.DateUpdated.HasValue
          ? rollCall.DateUpdated.Value.ToLocalTime()
          : null;

        return new RollCallResType(200, $"Thành công", rollCall);
      }
      catch (Exception ex)
      {
        return new RollCallResType(500, $"Đang xảy ra lỗi tại server... {ex.Message}");
        throw new Exception(ex.Message);
      }
    }

    public async Task<RollCallResType> RollCalls()
    {
      try
      {
        var rollCalls = await _context.RollCalls
          .Include(x => x.RollCallDetails)
          .AsNoTracking()
          .ToListAsync();

        TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

        rollCalls.ForEach(x =>
        {
          if (x.DateAt.HasValue) x.DateAt = TimeZoneInfo.ConvertTimeFromUtc(x.DateAt.Value, vietnamTimeZone);
          if (x.DateCreated.HasValue) x.DateCreated = TimeZoneInfo.ConvertTimeFromUtc(x.DateCreated.Value, vietnamTimeZone);
          if (x.DateUpdated.HasValue) x.DateUpdated = TimeZoneInfo.ConvertTimeFromUtc(x.DateUpdated.Value, vietnamTimeZone);
        });

        return new RollCallResType(200, $"Thành công", rollCalls);
      }
      catch (Exception ex)
      {
        return new RollCallResType(500, "Đang xảy ra lỗi tại server...");
        throw new Exception(ex.Message);
      }
    }

    public Task<RollCallResType> Export(int weekId, int classId)
    {
      throw new NotImplementedException();
    }

    public Task<RollCallResType> Import(int weekId, int classId)
    {
      throw new NotImplementedException();
    }

  }
}
