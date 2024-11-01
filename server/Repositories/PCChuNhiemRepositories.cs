using ExcelDataReader;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using server.Data;
using server.Dtos;
using server.IService;
using server.Types;
using System.Text;

namespace server.Repositories
{
  public class PCChuNhiemRepositories : IPC_ChuNhiem
  {
    private readonly SoDauBaiContext _context;

    public PCChuNhiemRepositories(SoDauBaiContext context)
    {
      this._context = context;
    }

    public async Task<ChuNhiemResType> CreatePC_ChuNhiem(PC_ChuNhiemDto model)
    {
      using var transaction = await _context.Database.BeginTransactionAsync();

      try
      {
        if (model is null)
        {
          return new ChuNhiemResType(400, "Input emtpy");
        }

        // Check if teacherId exists
        var teacherExists = await _context.Teachers
            .AnyAsync(c => c.TeacherId == model.TeacherId);

        if (!teacherExists)
        {
          return new ChuNhiemResType(404, "ClassId not found");
        }

        // Check if classId exists
        var classExists = await _context.Classes
            .AnyAsync(c => c.ClassId == model.ClassId);

        if (!classExists)
        {
          return new ChuNhiemResType(404, "ClassId not found");
        }

        // Check if semesterId exists
        var academicYearExistis = await _context.AcademicYears
            .AnyAsync(s => s.AcademicYearId == model.AcademicYearId);

        if (!academicYearExistis)
        {
          return new ChuNhiemResType(404, "SemesterId not found");
        }

        var queryInsert = @"INSERT INTO PhanCongChuNhiem 
                                        (teacherId, classId, academicYearId, 
                                          status, dateCreated, dateUpdated, description)
                                VALUES 
                                        (@teacherId, @classId, @academicYearId, 
                                          @status, @dateCreated, @dateUpdated, @description);
                                SELECT CAST(SCOPE_IDENTITY() AS INT);"
        ;

        var insert = await _context.Database.ExecuteSqlRawAsync(queryInsert,
            new SqlParameter("@teacherId", model.TeacherId),
            new SqlParameter("@classId", model.ClassId),
            new SqlParameter("@academicYearId", model.AcademicYearId),
            new SqlParameter("@status", model.Status),
            new SqlParameter("@dateCreated", DateTime.UtcNow),
            new SqlParameter("@dateUpdated", DBNull.Value),
            new SqlParameter("@description", model.Description)
        );

        await transaction.CommitAsync();

        var data = new PC_ChuNhiemDto
        {
          PhanCongChuNhiemId = insert,
          TeacherId = model.TeacherId,
          ClassId = model.ClassId,
          AcademicYearId = model.AcademicYearId,
          Status = model.Status,
          Description = model.Description,
        };

        var result = new ChuNhiemResType(200, "Insert thành công", data);

        return result;
      }
      catch (Exception ex)
      {
        await transaction.RollbackAsync();

        return new ChuNhiemResType(500, $"Server Error: {ex.Message}");
      }
    }

    public async Task<ChuNhiemResType> GetPC_ChuNhiem(int id)
    {
      try
      {
        var chuNhiemQuery = from chuNhiem in _context.PhanCongChuNhiems
                            join teacher in _context.Teachers on chuNhiem.TeacherId equals teacher.TeacherId into teacherGroup
                            from teacher in teacherGroup.DefaultIfEmpty()
                            join classes in _context.Classes on chuNhiem.ClassId equals classes.ClassId into classesGroup
                            from classes in classesGroup.DefaultIfEmpty()
                            join academicYear in _context.AcademicYears on chuNhiem.AcademicYearId equals academicYear.AcademicYearId into academicYearGroup
                            from academicYear in academicYearGroup.DefaultIfEmpty()
                            select new PhanCongData
                            {
                              PhanCongId = chuNhiem.PhanCongChuNhiemId,
                              TeacherId = chuNhiem.TeacherId,
                              TeacherName = teacher.Fullname,
                              ClassId = classes.ClassId,
                              NameClass = classes.ClassName,
                              AcademicYearId = (int)(chuNhiem.AcademicYearId ?? null)!,
                              AcademicYearName = academicYear.DisplayAcademicYearName,
                              Status = chuNhiem.Status,
                            };

        var result = await chuNhiemQuery.FirstOrDefaultAsync(x => x.PhanCongId == id);

        if (result is null)
        {
          return new ChuNhiemResType(404, "Không tìm thấy");
        }

        return new ChuNhiemResType(200, "Thành công", result);
      }
      catch (Exception ex)
      {
        return new ChuNhiemResType(500, $"Server erorr: {ex.Message}");
      }
    }

    public async Task<ChuNhiemResType> GetPC_ChuNhiems(int pageNumber, int pageSize)
    {
      try
      {
        var skip = (pageNumber - 1) * pageSize;

        var chuNhiemQuery = from chuNhiem in _context.PhanCongChuNhiems
                            join teacher in _context.Teachers on chuNhiem.TeacherId equals teacher.TeacherId into teacherGroup
                            from teacher in teacherGroup.DefaultIfEmpty()
                            join classes in _context.Classes on chuNhiem.ClassId equals classes.ClassId into classesGroup
                            from classes in classesGroup.DefaultIfEmpty()
                            join academicYear in _context.AcademicYears on chuNhiem.AcademicYearId equals academicYear.AcademicYearId into academicYearGroup
                            from academicYear in academicYearGroup.DefaultIfEmpty()
                            select new PhanCongData
                            {
                              PhanCongId = chuNhiem.PhanCongChuNhiemId,
                              TeacherId = chuNhiem.TeacherId,
                              TeacherName = teacher.Fullname,
                              ClassId = chuNhiem.ClassId,
                              NameClass = classes.ClassName,
                              AcademicYearId = (int)(chuNhiem.AcademicYearId ?? null)!,
                              AcademicYearName = academicYear.DisplayAcademicYearName,
                              Status = chuNhiem.Status,
                            };

        var result = await chuNhiemQuery
        .OrderBy(x => x.PhanCongId)
        .Skip(skip).Take(pageSize)
        .ToListAsync();

        if (result is null || result.Count == 0)
        {
          return new ChuNhiemResType(404, "Không tìm thấy");
        }

        return new ChuNhiemResType(200, "Thành công", result);
      }
      catch (Exception ex)
      {
        return new ChuNhiemResType(500, $"Server erorr: {ex.Message}");
      }
    }

    public async Task<ChuNhiemResType> Get_ChuNhiem_Teacher_Class(int idClass)
    {
      try
      {
        var chuNhiemQuery = from chuNhiem in _context.PhanCongChuNhiems
                            join teacher in _context.Teachers on chuNhiem.TeacherId equals teacher.TeacherId into teacherGroup
                            from teacher in teacherGroup.DefaultIfEmpty()
                            join classes in _context.Classes on chuNhiem.ClassId equals classes.ClassId into classesGroup
                            from classes in classesGroup.DefaultIfEmpty()
                            join academicYear in _context.AcademicYears on chuNhiem.AcademicYearId equals academicYear.AcademicYearId into academicYearGroup
                            from academicYear in academicYearGroup.DefaultIfEmpty()
                            select new PhanCongData
                            {
                              PhanCongId = chuNhiem.PhanCongChuNhiemId,
                              TeacherId = chuNhiem.TeacherId,
                              TeacherName = teacher.Fullname,
                              ClassId = classes.ClassId,
                              NameClass = classes.ClassName,
                              AcademicYearId = (int)(chuNhiem.AcademicYearId ?? null)!,
                              AcademicYearName = academicYear.DisplayAcademicYearName,
                              Status = chuNhiem.Status,
                            };

        var result = await chuNhiemQuery.FirstOrDefaultAsync(x => x.ClassId == idClass);

        if (result is null)
        {
          return new ChuNhiemResType(404, "Không tìm thấy");
        }

        return new ChuNhiemResType(200, "Thành công", result);
      }
      catch (Exception ex)
      {
        return new ChuNhiemResType(500, $"Server erorr: {ex.Message}");
      }
    }

    public async Task<ChuNhiemResType> UpdatePC_ChuNhiem(int id, PC_ChuNhiemDto model)
    {
      using var transaction = await _context.Database.BeginTransactionAsync();
      try
      {
        var existing = await _context.PhanCongChuNhiems.FindAsync(id);
        if (existing is null)
        {
          return new ChuNhiemResType(404, "Không tìm thấy");
        }

        existing.TeacherId = model.TeacherId;
        existing.ClassId = model.ClassId;
        existing.AcademicYearId = model.AcademicYearId;
        existing.Status = model.Status;
        existing.DateUpdated = DateTime.UtcNow;
        existing.Description = model.Description;

        await _context.SaveChangesAsync();
        await transaction.CommitAsync();

        var updatedDto = new PhanCongData
        {
          PhanCongId = existing.PhanCongChuNhiemId,
          TeacherId = model.TeacherId,
          ClassId = model.ClassId,
          AcademicYearId = model.AcademicYearId,
          Status = existing.Status,
          DateCreated = existing.DateCreated,
          DateUpdated = existing.DateUpdated,
          Description = model.Description
        };

        return new ChuNhiemResType(200, "Cập nhật thành công", updatedDto);
      }
      catch (Exception ex)
      {
        await transaction.RollbackAsync();
        return new ChuNhiemResType(500, $"Server error: {ex.Message}");
      }
    }

    public async Task<ChuNhiemResType> DeletePC_ChuNhiem(int id)
    {
      try
      {
        var find = "SELECT * FROM PhanCongChuNhiem WHERE PhanCongChuNhiem = @id";

        var phanCongChuNhiem = await _context.PhanCongChuNhiems
          .FromSqlRaw(find, new SqlParameter("@id", id))
          .FirstOrDefaultAsync();

        if (phanCongChuNhiem is null)
        {
          return new ChuNhiemResType(404, "PC_ChuNhiemId not found");
        }

        var deleteQuery = "DELETE FROM PhanCongChuNhiem WHERE PhanCongChuNhiemId = @id";

        await _context.Database.ExecuteSqlRawAsync(deleteQuery, new SqlParameter("@id", id));

        return new ChuNhiemResType(200, "Xóa thành công");
      }
      catch (Exception ex)
      {
        return new ChuNhiemResType(500, $"Server error: {ex.Message}");
      }
    }

    public async Task<ChuNhiemResType> BulkDelete(List<int> ids)
    {
      await using var transaction = await _context.Database.BeginTransactionAsync();

      try
      {
        if (ids is null || ids.Count == 0)
        {
          return new ChuNhiemResType(400, "No IDs provided.");
        }

        var idList = string.Join(",", ids);

        var deleteQuery = $"DELETE FROM PhanCongChuNhiem WHERE PhanCongChuNhiemId IN ({idList})";

        var delete = await _context.Database.ExecuteSqlRawAsync(deleteQuery);

        if (delete == 0)
        {
          return new ChuNhiemResType(404, "No PhanCongChuNhiemId found to delete");
        }

        await transaction.CommitAsync();

        return new ChuNhiemResType(200, "Deleted succesfully");
      }
      catch (Exception ex)
      {
        await transaction.RollbackAsync();
        return new ChuNhiemResType(500, $"Server error: {ex.Message}");
      }
    }

    public async Task<string> ImportExcelFile(IFormFile file)
    {
      try
      {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        if (file is not null && file.Length > 0)
        {
          var uploadsFolder = $"{Directory.GetCurrentDirectory()}\\Uploads";

          if (!Directory.Exists(uploadsFolder))
          {
            Directory.CreateDirectory(uploadsFolder);
          }

          var filePath = Path.Combine(uploadsFolder, file.FileName);

          using (var stream = new FileStream(filePath, FileMode.Create))
          {
            await file.CopyToAsync(stream);
          }

          using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
          {
            using var reader = ExcelReaderFactory.CreateReader(stream);

            bool isHeaderSkipped = false;

            do
            {
              while (reader.Read())
              {
                if (!isHeaderSkipped)
                {
                  isHeaderSkipped = true;
                  continue;
                }

                // Check if there are no more rows or empty rows
                if (reader.GetValue(1) == null && reader.GetValue(2) == null && reader.GetValue(3) == null)
                {
                  break;
                }

                var myPhanCongChuNhiem = new Models.PhanCongChuNhiem
                {
                  TeacherId = Convert.ToInt16(reader.GetValue(1)),
                  ClassId = Convert.ToInt16(reader.GetValue(2)),
                  AcademicYearId = Convert.ToInt16(reader.GetValue(3)),
                  Status = Convert.ToBoolean(reader.GetValue(4)),
                  Description = reader.GetValue(5).ToString()?.Trim() ?? "",
                  DateCreated = DateTime.UtcNow,
                  DateUpdated = null,
                };

                await _context.PhanCongChuNhiems.AddAsync(myPhanCongChuNhiem);
                await _context.SaveChangesAsync();
              }
            } while (reader.NextResult());
          }

          return "Successfully inserted.";
        }

        return "No file uploaded";
      }
      catch (Exception ex)
      {
        throw new Exception($"Error while uploading file: {ex.Message}");
      }
    }

  }
}
