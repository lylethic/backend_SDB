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

    public async Task<RollCallResType> Create(RollCallDto model, List<AbsenceDto> absenceDtos)
    {
      using (var transaction = await _context.Database.BeginTransactionAsync())
      {
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
            DateUpdated = DateTime.UtcNow,
            NumberOfAttendants = model.NumberOfAttendants,
          };

          await _context.RollCalls.AddAsync(newRollCall);
          await _context.SaveChangesAsync();

          foreach (var absenceDto in absenceDtos)
          {
            var newAbsence = new Absence
            {
              CallRollId = newRollCall.CallRollId,
              Description = absenceDto.Description,
              StudentId = absenceDto.StudentId,
            };

            await _context.Absences.AddAsync(newAbsence);
          }

          await _context.SaveChangesAsync();
          await transaction.CommitAsync();

          return new RollCallResType(200, $"Tạo điểm danh thành công. {newRollCall.CallRollId}", newRollCall);
        }
        catch (Exception ex)
        {
          await transaction.RollbackAsync();
          return new RollCallResType(500, "Đang xảy ra lỗi tại server...");
          throw new Exception(ex.Message);
        }
      }
    }

    public Task<RollCallResType> BulkDelete(List<int> rollCallIds)
    {
      throw new NotImplementedException();
    }

    public Task<RollCallResType> Delete(int rollCallId)
    {
      throw new NotImplementedException();
    }

    public Task<RollCallResType> Export(int weekId, int classId)
    {
      throw new NotImplementedException();
    }

    public Task<RollCallResType> Import(int weekId, int classId)
    {
      throw new NotImplementedException();
    }

    public Task<RollCallResType> Update(int rollCallId, RollCallDto model)
    {
      throw new NotImplementedException();
    }

    public Task<RollCallResType> RollCall(int rollCallIds)
    {
      throw new NotImplementedException();
    }

    public async Task<RollCallResType> RollCalls()
    {
      try
      {
        var rollCalls = await _context.RollCalls
          .Include(x => x.Absences)
          .AsNoTracking()
          .ToListAsync();
        TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

        rollCalls.ForEach(x =>
        {
          x.CallRollId = x.CallRollId;
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
  }
}
