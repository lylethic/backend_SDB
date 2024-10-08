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
  public class ChiTietSoDauBaiRepositories : IChiTietSoDauBai
  {
    private readonly SoDauBaiContext _context;

    public ChiTietSoDauBaiRepositories(SoDauBaiContext context)
    {
      this._context = context;
    }

    public async Task<Data_Response<string>> BulkDelete(List<int> ids)
    {
      await using var transaction = await _context.Database.BeginTransactionAsync();

      try
      {
        if (ids is null || ids.Count == 0)
        {
          return new Data_Response<string>(400, "No IDs provided.");
        }

        var idList = string.Join(",", ids);

        var deleteQuery = $"DELETE FROM ChiTietSoDauBai WHERE ChiTietSoDauBaiId IN ({idList})";

        var delete = await _context.Database.ExecuteSqlRawAsync(deleteQuery);

        if (delete == 0)
        {
          return new Data_Response<string>(404, "No ChiTietSoDauBai found to delete");
        }

        await transaction.CommitAsync();

        return new Data_Response<string>(200, "Deleted");
      }
      catch (Exception ex)
      {
        await transaction.RollbackAsync();
        return new Data_Response<string>(500, $"Server error: {ex.Message}");
      }
    }

    public async Task<Data_Response<ChiTietSoDauBaiDto>> CreateChiTietSoDauBai(ChiTietSoDauBaiDto model)
    {
      await using var transaction = await _context.Database.BeginTransactionAsync();

      try
      {
        // Tim bIa so dau bai ton tai khong?
        var findBia = "SELECT * FROM BiaSoDauBai WHERE BiaSoDauBaiId = @id";

        var existingBia = await _context.BiaSoDauBais
          .FromSqlRaw(findBia, new SqlParameter("@id", model.BiaSoDauBaiId))
          .FirstOrDefaultAsync();

        if (existingBia is null)
        {
          return new Data_Response<ChiTietSoDauBaiDto>(404, "Not found");
        }

        // Tim semster ton tai khong?
        var findSemester = "SELECT * FROM Semester WHERE SemesterId = @id";

        var existingSemester = await _context.Semesters
          .FromSqlRaw(findSemester, new SqlParameter("@id", model.SemesterId))
          .FirstOrDefaultAsync();

        if (existingSemester is null)
        {
          return new Data_Response<ChiTietSoDauBaiDto>(404, "Not found");
        }

        // Tim Week ton tai khong?
        var findWeek = "SELECT * FROM Week WHERE WeekId = @id";

        var existingWeek = await _context.Weeks
          .FromSqlRaw(findWeek, new SqlParameter("@id", model.WeekId))
          .FirstOrDefaultAsync();

        if (existingWeek is null)
        {
          return new Data_Response<ChiTietSoDauBaiDto>(404, "Not found");
        }

        // Tim Mon hoc ton tai khong?
        var findSubject = "SELECT * FROM Subject WHERE SubjectId = @id";

        var existingSubject = await _context.Subjects
          .FromSqlRaw(findSubject, new SqlParameter("@id", model.SubjectId))
          .FirstOrDefaultAsync();

        if (existingSubject is null)
        {
          return new Data_Response<ChiTietSoDauBaiDto>(404, "Not found");
        }

        // Tim Xep loai ton tai khong?
        var findXepLoai = "SELECT * FROM Classification WHERE ClassificationId = @id";

        var existingXepLoai = await _context.Classifications
          .FromSqlRaw(findXepLoai, new SqlParameter("@id", model.ClassificationId))
          .FirstOrDefaultAsync();

        if (existingXepLoai is null)
        {
          return new Data_Response<ChiTietSoDauBaiDto>(404, "Not found");
        }

        var find = "SELECT * FROM ChiTietSodauBai WHERE ChiTietSodauBaiId = @id";

        var existingChiTietSoDauBai = await _context.ChiTietSoDauBais
         .FromSqlRaw(find, new SqlParameter("@id", model.ChiTietSoDauBaiId))
         .FirstOrDefaultAsync();

        if (existingChiTietSoDauBai is not null)
        {
          return new Data_Response<ChiTietSoDauBaiDto>(409, "Chi tiet so dau bai already exists");
        }

        var insertdata = @"INSERT INTO ChiTietSoDauBai (biaSoDauBaiId, semesterId, weekId, subjectId, 
                                    classificationId, daysOfTheWeek, thoiGian, buoiHoc, 
                                    tietHoc, lessonContent, attend,
                                    noteComment, createdBy, createdAt, updatedAt)
                           VALUES (@biaSoDauBaiId, @semesterId, @weekId, @subjectId, 
                                    @classificationId, @daysOfTheWeek, @thoiGian, 
                                    @buoiHoc, @tietHoc, @lessonContent, @attend,
                                    @noteComment, @createdBy, @createdAt, @updatedAt);
                           SELECT CAST(SCOPE_IDENTITY() AS int);";

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

        await transaction.CommitAsync();

        return new Data_Response<ChiTietSoDauBaiDto>(200, result);
      }
      catch (Exception ex)
      {
        await transaction.RollbackAsync();
        throw new Exception($"Server Error: {ex.Message}");
      }
    }

    public async Task<Data_Response<ChiTietSoDauBaiDto>> DeleteChiTietSoDauBai(int id)
    {
      try
      {
        var findChiTietSoDauBai = "SELECT * FROM ChiTietSoDauBai WHERE ChiTietSoDauBaiId = @id";

        var chiTietSoDauBai = await _context.ChiTietSoDauBais
          .FromSqlRaw(findChiTietSoDauBai, new SqlParameter("@id", id))
          .FirstOrDefaultAsync();

        if (chiTietSoDauBai is null)
        {
          return new Data_Response<ChiTietSoDauBaiDto>(404, "chiTietSoDauBai not found");
        }

        var deleteQuery = "DELETE FROM Teacher WHERE TeacherId = @id";
        await _context.Database.ExecuteSqlRawAsync(deleteQuery, new SqlParameter("@id", id));
        return new Data_Response<ChiTietSoDauBaiDto>(200, "Deleted");
      }
      catch (Exception ex)
      {
        return new Data_Response<ChiTietSoDauBaiDto>(500, $"Server error: {ex.Message}");
      }
    }
    public async Task<Data_Response<ChiTietSoDauBaiDto>> GetChiTietSoDauBai(int id)
    {
      try
      {
        var find = @"SELECT * FROM CHITIETSODAUBAI WHERE CHITIETSODAUBAIID = @id";

        var chiTietSoDauBai = await _context.ChiTietSoDauBais
          .FromSqlRaw(find, new SqlParameter("@id", id)
          ).FirstOrDefaultAsync();

        if (chiTietSoDauBai is null)
        {
          return new Data_Response<ChiTietSoDauBaiDto>(404, "Chi tiet so dau bai not found");
        }

        var result = new ChiTietSoDauBaiDto
        {
          ChiTietSoDauBaiId = chiTietSoDauBai.ChiTietSoDauBaiId,
          BiaSoDauBaiId = chiTietSoDauBai.BiaSoDauBaiId,
          SemesterId = chiTietSoDauBai.SemesterId,
          WeekId = chiTietSoDauBai.WeekId,
          SubjectId = chiTietSoDauBai.SubjectId,
          ClassificationId = chiTietSoDauBai.ClassificationId,
          DaysOfTheWeek = chiTietSoDauBai.DaysOfTheWeek,
          ThoiGian = chiTietSoDauBai.ThoiGian,
          BuoiHoc = chiTietSoDauBai.BuoiHoc,
          TietHoc = chiTietSoDauBai.TietHoc,
          LessonContent = chiTietSoDauBai.LessonContent,
          Attend = chiTietSoDauBai.Attend,
          NoteComment = chiTietSoDauBai.NoteComment,
          CreatedBy = chiTietSoDauBai.CreatedBy,
          CreatedAt = chiTietSoDauBai.CreatedAt,
          UpdatedAt = chiTietSoDauBai.UpdatedAt,
        };

        return new Data_Response<ChiTietSoDauBaiDto>(200, result);
      }
      catch (Exception ex)
      {
        throw new Exception($"Error: {ex.Message}");
      }
    }

    public async Task<List<ChiTietSoDauBaiDto>> GetChiTietSoDauBais(int pageNumber, int pageSize)
    {
      try
      {
        var skip = (pageNumber - 1) * pageSize;

        var query = @"SELECT * FROM ChiTietSoDauBai
                      ORDER BY ChiTietSoDauBaiId
                      OFFSET @skip ROWS
                      FETCH NEXT @pageSize ROWS ONLY";

        var chiTietSoDauBai = await _context.ChiTietSoDauBais
          .FromSqlRaw(query,
          new SqlParameter("@skip", skip),
          new SqlParameter("@pageSize", pageSize)
          ).ToListAsync() ?? throw new Exception("Empty");

        var result = chiTietSoDauBai.Select(x => new ChiTietSoDauBaiDto
        {
          ChiTietSoDauBaiId = x.ChiTietSoDauBaiId,
          BiaSoDauBaiId = x.BiaSoDauBaiId,
          SemesterId = x.SemesterId,
          WeekId = x.WeekId,
          SubjectId = x.SubjectId,
          ClassificationId = x.ClassificationId,
          DaysOfTheWeek = x.DaysOfTheWeek,
          ThoiGian = x.ThoiGian,
          BuoiHoc = x.BuoiHoc,
          TietHoc = x.TietHoc,
          LessonContent = x.LessonContent,
          Attend = x.Attend,
          NoteComment = x.NoteComment,
          CreatedBy = x.CreatedBy,
          CreatedAt = x.CreatedAt,
          UpdatedAt = x.UpdatedAt,
        }).ToList();

        return result;
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
        throw new Exception($"Server error: {ex.Message}");
      }
    }

    public async Task<string> ImportExcel(IFormFile file)
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
                if (reader.GetValue(1) == null && reader.GetValue(2) == null && reader.GetValue(3) == null
                  && reader.GetValue(4) == null && reader.GetValue(5) == null && reader.GetValue(6) == null
                  && reader.GetValue(7) == null && reader.GetValue(8) == null && reader.GetValue(9) == null
                  && reader.GetValue(10) == null && reader.GetValue(11) == null && reader.GetValue(12) == null
                  && reader.GetValue(13) == null)
                {
                  // Stop processing when an empty row is encountered
                  break;
                }

                var myDetails = new Models.ChiTietSoDauBai
                {
                  BiaSoDauBaiId = Convert.ToInt16(reader.GetValue(1)),
                  SemesterId = Convert.ToInt16(reader.GetValue(2)),
                  WeekId = Convert.ToInt16(reader.GetValue(3)),
                  SubjectId = Convert.ToInt16(reader.GetValue(4)),
                  ClassificationId = Convert.ToInt16(reader.GetValue(5)),
                  DaysOfTheWeek = reader.GetValue(6).ToString() ?? $"Thứ",
                  ThoiGian = Convert.ToDateTime(reader.GetValue(7)),
                  BuoiHoc = reader.GetValue(8).ToString() ?? "Buổi ",
                  TietHoc = Convert.ToInt16((int)reader.GetValue(9)),
                  LessonContent = reader.GetValue(10).ToString() ?? "Nội dung bài học",
                  Attend = Convert.ToInt16((int)reader.GetValue(11)),
                  NoteComment = reader.GetValue(12).ToString() ?? "Ghi chú",
                  CreatedBy = Convert.ToInt16(reader.GetValue(13)),
                  CreatedAt = DateTime.Now,
                  UpdatedAt = null
                };

                await _context.ChiTietSoDauBais.AddAsync(myDetails);
                await _context.SaveChangesAsync();
              }
            } while (reader.NextResult());
          }

          return "Successfully inserted";
        }
        return "No file uploaded";
      }
      catch (Exception ex)
      {
        throw new Exception($"Error while uploading file: {ex.Message}");
      }
    }

    public async Task<Data_Response<ChiTietSoDauBaiDto>> UpdateChiTietSoDauBai(int id, ChiTietSoDauBaiDto model)
    {
      using var transaction = await _context.Database.BeginTransactionAsync();

      try
      {
        // Check if the teacher exists in the database
        var findQuery = "SELECT * FROM ChiTietSoDauBai WHERE ChiTietSoDauBaiId = @id";

        var existingChiTietSoDauBai = await _context.ChiTietSoDauBais
            .FromSqlRaw(findQuery, new SqlParameter("@id", id))
            .FirstOrDefaultAsync();

        if (existingChiTietSoDauBai is null)
        {
          return new Data_Response<ChiTietSoDauBaiDto>(404, "ChiTietSoDauBaiId not found");
        }

        bool hasChanges = false;

        // Build update query dynamically based on non-null fields
        var parameters = new List<SqlParameter>();
        var queryBuilder = new StringBuilder("UPDATE ChiTietSoDauBai SET ");

        if (model.BiaSoDauBaiId != 0 && model.BiaSoDauBaiId != existingChiTietSoDauBai.BiaSoDauBaiId)
        {
          queryBuilder.Append("BiaSoDauBaiId = @BiaSoDauBaiId, ");
          parameters.Add(new SqlParameter("@BiaSoDauBaiId", model.BiaSoDauBaiId));
          hasChanges = true;
        }

        if (model.SemesterId != 0 && model.SemesterId != existingChiTietSoDauBai.SemesterId)
        {
          queryBuilder.Append("SemesterId = @SemesterId, ");
          parameters.Add(new SqlParameter("@SemesterId", model.SemesterId));
          hasChanges = true;
        }

        if (model.WeekId != 0 && model.WeekId != existingChiTietSoDauBai.WeekId)
        {
          queryBuilder.Append("WeekId = @WeekId, ");
          parameters.Add(new SqlParameter("@WeekId", model.WeekId));
          hasChanges = true;
        }

        if (model.SubjectId != 0 && model.SubjectId != existingChiTietSoDauBai.SubjectId)
        {
          queryBuilder.Append("SubjectId = @SubjectId, ");
          parameters.Add(new SqlParameter("@SubjectId", model.SubjectId));
          hasChanges = true;
        }

        if (model.ClassificationId != 0 && model.ClassificationId != existingChiTietSoDauBai.ClassificationId)
        {
          queryBuilder.Append("ClassificationId = @ClassificationId, ");
          parameters.Add(new SqlParameter("@ClassificationId", model.ClassificationId));
          hasChanges = true;
        }

        if (!string.IsNullOrEmpty(model.DaysOfTheWeek) && model.DaysOfTheWeek != existingChiTietSoDauBai.DaysOfTheWeek)
        {
          queryBuilder.Append("DaysOfTheWeek = @DaysOfTheWeek, ");
          parameters.Add(new SqlParameter("@DaysOfTheWeek", model.DaysOfTheWeek));
          hasChanges = true;
        }

        if (model.ThoiGian != existingChiTietSoDauBai.ThoiGian)
        {
          queryBuilder.Append("ThoiGian = @ThoiGian, ");
          parameters.Add(new SqlParameter("@ThoiGian", model.ThoiGian));
          hasChanges = true;
        }

        if (!String.IsNullOrEmpty(model.BuoiHoc) && model.BuoiHoc != existingChiTietSoDauBai.BuoiHoc)
        {
          queryBuilder.Append("BuoiHoc = @BuoiHoc, ");
          parameters.Add(new SqlParameter("@BuoiHoc", model.BuoiHoc));
          hasChanges = true;
        }

        if (model.TietHoc != 0 && model.TietHoc != existingChiTietSoDauBai.TietHoc)
        {
          queryBuilder.Append("TietHoc = @TietHoc, ");
          parameters.Add(new SqlParameter("@TietHoc", model.TietHoc));
          hasChanges = true;
        }

        if (!String.IsNullOrEmpty(model.LessonContent) && model.LessonContent != existingChiTietSoDauBai.LessonContent)
        {
          queryBuilder.Append("LessonContent = @LessonContent, ");
          parameters.Add(new SqlParameter("@LessonContent", model.LessonContent));
          hasChanges = true;
        }

        if (model.Attend != 0 && model.Attend != existingChiTietSoDauBai.Attend)
        {
          queryBuilder.Append("Attend = @Attend, ");
          parameters.Add(new SqlParameter("@Attend", model.Attend));
          hasChanges = true;
        }

        if (model.NoteComment != existingChiTietSoDauBai.NoteComment)
        {
          queryBuilder.Append("NoteComment = @NoteComment, ");
          parameters.Add(new SqlParameter("@NoteComment", model.NoteComment));
          hasChanges = true;
        }

        if (model.CreatedAt.HasValue)
        {
          queryBuilder.Append("CreatedAt = @CreatedAt, ");
          parameters.Add(new SqlParameter("@CreatedAt", model.CreatedAt.Value));
        }

        var update = DateTime.Now;

        queryBuilder.Append("UpdatedAt = @UpdatedAt, ");
        parameters.Add(new SqlParameter("@UpdatedAt", update));

        if (hasChanges)
        {
          if (queryBuilder[queryBuilder.Length - 2] == ',')
          {
            queryBuilder.Length -= 2;
          }

          queryBuilder.Append(" WHERE ChiTietSoDauBaiId = @id");
          parameters.Add(new SqlParameter("@id", id));

          // Execute the update query
          var updateQuery = queryBuilder.ToString();
          await _context.Database.ExecuteSqlRawAsync(updateQuery, [.. parameters]);

          // Commit the transaction
          await transaction.CommitAsync();
          return new Data_Response<ChiTietSoDauBaiDto>(200, "Chi tiet so dau bai updated successfully");
        }
        else
        {
          return new Data_Response<ChiTietSoDauBaiDto>(200, "No changes detected");
        }
      }
      catch (Exception ex)
      {
        await transaction.RollbackAsync();

        return new Data_Response<ChiTietSoDauBaiDto>(500, $"Server Error: {ex.Message}");
      }
    }

    public async Task<Data_Response<ChiTiet_BiaSoDauBaiResType>> GetChiTiet_Bia_Class_Teacher(int chiTietId)
    {
      try
      {
        var find = @"SELECT ct.chiTietSoDauBaiId, 
                            b.biaSoDauBaiId,
                            b.classId, 
                            b.schoolId, 
                            c.className, 
                            t.teacherId, 
                            t.fullname
                    FROM dbo.ChiTietSoDauBai as ct
                    LEFT JOIN dbo.BiaSoDauBai as b ON ct.biaSoDauBaiId = b.biaSoDauBaiId 
                    LEFT JOIN dbo.Class as c ON b.classId = c.classId 
                    LEFT JOIN dbo.Teacher as t ON c.teacherId = t.teacherId
                    WHERE ct.chiTietSoDauBaiId = @id";

        var chitietSoDauBai = await _context.ChiTietSoDauBais.FromSqlRaw(find,
        new SqlParameter("@id", chiTietId))
        .Select(ct => new ChiTiet_BiaSoDauBaiResType
        {
          ChiTietSoDauBaiId = ct.ChiTietSoDauBaiId,
          BiaSoDauBaiId = ct.BiaSoDauBaiId,
          SchoolId = ct.BiaSoDauBai.SchoolId,
          ClassId = ct.BiaSoDauBai.ClassId,
          ClassName = ct.BiaSoDauBai.Class.ClassName,
          TeacherId = ct.BiaSoDauBai.Class.TeacherId,
          TeacherFullName = ct.BiaSoDauBai.Class.Teacher.Fullname
        })
        .FirstOrDefaultAsync();

        if (chitietSoDauBai is null)
        {
          return new Data_Response<ChiTiet_BiaSoDauBaiResType>(404, "Chi tiet so dau bai id not found");
        }

        var result = new ChiTiet_BiaSoDauBaiResType
        {
          ChiTietSoDauBaiId = chitietSoDauBai.ChiTietSoDauBaiId,
          BiaSoDauBaiId = chitietSoDauBai.BiaSoDauBaiId,
          SchoolId = chitietSoDauBai.SchoolId,
          ClassId = chitietSoDauBai.ClassId,
          ClassName = chitietSoDauBai.ClassName,
          TeacherId = chitietSoDauBai.TeacherId,
          TeacherFullName = chitietSoDauBai.TeacherFullName
        };

        return new Data_Response<ChiTiet_BiaSoDauBaiResType>(200, result);

      }
      catch (System.Exception ex)
      {
        throw new Exception($"Server Error: {ex.Message}");
      }
    }

    public async Task<Data_Response<ChiTiet_WeekResType>> GetChiTiet_Week_XepLoai(int chiTietId)
    {
      try
      {
        var find = @"SELECT 
                            ct.chiTietSoDauBaiId, 
                            ct.weekId, 
                            we.weekName, 
                            we.status, 
                            ct.classificationId, 
                            cla.classifyName, 
                            cla.score
                    FROM dbo.ChiTietSoDauBai as ct 
                    RIGHT JOIN dbo.Classification as cla
                          ON ct.classificationId = cla.classificationId
                    LEFT JOIN dbo.Week as we
                          ON ct.weekId = we.weekId
                    WHERE ct.chiTietSoDauBaiId = @id";

        var chitietSoDauBai = await _context.ChiTietSoDauBais.FromSqlRaw(find,
        new SqlParameter("@id", chiTietId))
        .Select(ct => new ChiTiet_WeekResType
        {
          ChiTietSoDauBaiId = ct.ChiTietSoDauBaiId,
          WeekId = ct.WeekId,
          WeekName = ct.Week.WeekName,
          Status = ct.Week.Status,
          XepLoaiId = ct.ClassificationId,
          TenXepLoai = ct.Classification.ClassifyName,
          SoDiem = ct.Classification.Score
        })
        .FirstOrDefaultAsync();

        if (chitietSoDauBai is null)
        {
          return new Data_Response<ChiTiet_WeekResType>(404, "Chi tiet so dau bai id not found");
        }

        var result = new ChiTiet_WeekResType
        {
          ChiTietSoDauBaiId = chitietSoDauBai.ChiTietSoDauBaiId,
          WeekId = chitietSoDauBai.WeekId,
          WeekName = chitietSoDauBai.WeekName,
          Status = chitietSoDauBai.Status,
          XepLoaiId = chitietSoDauBai.XepLoaiId,
          TenXepLoai = chitietSoDauBai.TenXepLoai,
          SoDiem = chitietSoDauBai.SoDiem
        };

        return new Data_Response<ChiTiet_WeekResType>(200, result);

      }
      catch (Exception ex)
      {
        throw new Exception($"Server Error: {ex.Message}");
      }
    }
  }
}
