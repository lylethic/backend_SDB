using ExcelDataReader;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using server.Data;
using server.Dtos;
using server.IService;
using server.Models;
using System.Text;


namespace server.Repositories
{
  public class SubjectRepositories : ISubject
  {
    readonly SoDauBaiContext _context;
    public SubjectRepositories(SoDauBaiContext context)
    {
      this._context = context;
    }

    public async Task<ResponseData<SubjectDto>> CreateSubject(SubjectDto model)
    {
      try
      {
        var find = "SELECT * FROM Subject WHERE subjectId = @id";

        var subject = await _context.Subjects
          .FromSqlRaw(find, new SqlParameter("@id", model.SubjectId))
          .FirstOrDefaultAsync();

        if (subject is not null)
        {
          return new ResponseData<SubjectDto>(409, "Subject already exists");
        }

        var sqlInsert = @"INSERT INTO SUBJECT (academicYearId, subjectName, status)
                     VALUES (@academicYearId, @subjectName, @status);
                     SELECT CAST(SCOPE_IDENTITY() as int);";

        var insert = await _context.Database.ExecuteSqlRawAsync(sqlInsert,
          new SqlParameter("@academicYearId", model.AcademicYearId),
          new SqlParameter("@subjectName", model.SubjectName),
          new SqlParameter("@status", model.Status)
          );

        var result = new SubjectDto
        {
          SubjectId = insert,
          AcademicYearId = model.AcademicYearId,
          SubjectName = model.SubjectName,
          Status = model.Status,
        };

        return new ResponseData<SubjectDto>(200, result);
      }
      catch (Exception ex)
      {
        return new ResponseData<SubjectDto>(500, $"Server error: {ex.Message}");
      }
    }

    public async Task<ResponseData<SubjectDto>> DeleteSubject(int id)
    {
      try
      {
        var find = "SELECT * FROM Subject WHERE subjectId = @id";

        var subject = await _context.Subjects
          .FromSqlRaw(find, new SqlParameter("@id", id))
          .FirstOrDefaultAsync();

        if (subject is null)
        {
          return new ResponseData<SubjectDto>(409, "Subject not found");
        }

        var deleteQuery = "DELETE FROM SUBJECT WHERE subjectId = @id";
        await _context.Database.ExecuteSqlRawAsync(deleteQuery, new SqlParameter("@id", id));

        return new ResponseData<SubjectDto>(200, "Deleted");
      }
      catch (Exception ex)
      {
        return new ResponseData<SubjectDto>(500, $"Server error: {ex.Message}");
      }
    }

    public async Task<ResponseData<SubjectDto>> GetSubject(int id)
    {
      try
      {
        var find = @"SELECT s.subjectId, 
			                     s.subjectName, 
			                     a.academicYearId, 
			                     a.displayAcademicYear_Name, 
			                     FORMAT(a.yearStart, 'dd/MM/yyyy') AS formatDateStart, 
			                     FORMAT(a.yearEnd, 'dd/MM/yyyy') AS formatDateEnd,
                           s.status
                    FROM dbo.SUBJECT s 
                    RIGHT JOIN dbo.AcademicYear a 
                    ON S.academicYearId = A.academicYearId
                    WHERE S.subjectId = @id";

        var subject = await _context.Subjects
          .FromSqlRaw(find, new SqlParameter("@id", id))
          .Select(static x => new Subject
          {
            SubjectId = x.SubjectId,
            SubjectName = x.SubjectName,
            Status = x.Status,
            AcademicYear = new AcademicYear
            {
              AcademicYearId = x.AcademicYearId,
              DisplayAcademicYearName = x.AcademicYear.DisplayAcademicYearName,
              YearStart = x.AcademicYear.YearStart,
              YearEnd = x.AcademicYear.YearEnd,
            }
          })
          .FirstOrDefaultAsync();

        if (subject is null)
        {
          return new ResponseData<SubjectDto>(404, "Môn học không tồn tại");
        }

        var result = new SubjectDto
        {
          SubjectId = id,
          AcademicYearId = subject.AcademicYearId,
          SubjectName = subject.SubjectName,
          Status = subject.Status,
          DisplayAcademicYear_Name = subject.AcademicYear.DisplayAcademicYearName,
          YearStart = subject.AcademicYear.YearStart,
          YearEnd = subject.AcademicYear.YearEnd,
        };

        return new ResponseData<SubjectDto>(200, result);
      }
      catch (Exception ex)
      {
        return new ResponseData<SubjectDto>(500, $"Server error: {ex.Message}");
      }
    }

    public async Task<ResponseData<List<SubjectDto>>> GetSubjects(int pageNumber, int pageSize)
    {
      try
      {
        var skip = (pageNumber - 1) * pageSize;

        var find = @"SELECT * 
                    FROM Subject ORDER BY SUBJECTNAME 
                    OFFSET @skip ROWS 
                    FETCH NEXT @pageSize ROWS ONLY;";

        var subject = await _context.Subjects.FromSqlRaw(find,
          new SqlParameter("@skip", skip),
          new SqlParameter("@pageSize", pageSize)
          ).ToListAsync() ?? throw new Exception("Empty");

        var result = subject.Select(subject => new SubjectDto
        {
          SubjectId = subject.SubjectId,
          AcademicYearId = subject.AcademicYearId,
          SubjectName = subject.SubjectName,
          Status = subject.Status
        }).ToList();

        return new ResponseData<List<SubjectDto>>(200, result);
      }
      catch (Exception ex)
      {
        return new ResponseData<List<SubjectDto>>(500, $"{ex.Message}");
      }
    }

    public async Task<ResponseData<SubjectDto>> UpdateSubject(int id, SubjectDto model)
    {
      using var transaction = await _context.Database.BeginTransactionAsync();
      try
      {
        var find = "SELECT * FROM Subject WHERE subjectId = @id";

        var subject = await _context.Subjects
          .FromSqlRaw(find, new SqlParameter("@id", id))
          .FirstOrDefaultAsync();

        if (subject is null)
        {
          return new ResponseData<SubjectDto>(404, "Không tìm thấy môn học");
        }
        bool hasChanges = false;

        var queryBuilder = new StringBuilder("UPDATE Subject SET ");
        var parameters = new List<SqlParameter>();

        if (model.AcademicYearId != 0 && model.AcademicYearId != subject.AcademicYearId)
        {
          queryBuilder.Append("AcademicYearId = @AcademicYearId, ");
          parameters.Add(new SqlParameter("@AcademicYearId", model.AcademicYearId));
          hasChanges = true;
        }
        if (!string.IsNullOrEmpty(model.SubjectName) && model.SubjectName != subject.SubjectName)
        {
          queryBuilder.Append("SubjectName = @SubjectName, ");
          parameters.Add(new SqlParameter("@SubjectName", model.SubjectName));
          hasChanges = true;
        }

        if (model.Status != subject.Status)
        {
          queryBuilder.Append("Status = @Status, ");
          parameters.Add(new SqlParameter("@Status", model.Status));
          hasChanges = true;
        }

        if (hasChanges)
        {
          if (queryBuilder[queryBuilder.Length - 2] == ',')
          {
            queryBuilder.Length -= 2;
          }

          queryBuilder.Append(" WHERE subjectId = @id");
          parameters.Add(new SqlParameter("@id", id));

          var updateQuery = queryBuilder.ToString();
          await _context.Database.ExecuteSqlRawAsync(updateQuery, parameters.ToArray());
          await transaction.CommitAsync();
          return new ResponseData<SubjectDto>(200, "Đã cập nhật");
        }
        else
        {
          return new ResponseData<SubjectDto>(200, "Không phát hiện sự thay đổi");
        }
      }
      catch (Exception ex)
      {
        return new ResponseData<SubjectDto>(500, $"Server error: {ex.Message}");
      }
    }

    public async Task<ResponseData<string>> ImportExcelFile(IFormFile file)
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
                  // Stop processing when an empty row is encountered
                  break;
                }

                var mySubjects = new Models.Subject
                {
                  AcademicYearId = Convert.ToInt16(reader.GetValue(1)),
                  SubjectName = reader.GetValue(2).ToString() ?? "null",
                  Status = Convert.ToBoolean(reader.GetValue(3))
                };

                await _context.Subjects.AddAsync(mySubjects);
                await _context.SaveChangesAsync();
              }
            } while (reader.NextResult());
          }

          return new ResponseData<string>(200, "Tải lên thành công");
        }
        return new ResponseData<string>(200, "Không có tệp nào được tải lên");
      }
      catch (Exception ex)
      {
        return new ResponseData<string>(500, $"Server error: {ex.Message}");
      }
    }

    public async Task<ResponseData<string>> BulkDelete(List<int> ids)
    {
      await using var transaction = await _context.Database.BeginTransactionAsync();

      try
      {
        if (ids is null || ids.Count == 0)
        {
          return new ResponseData<string>(400, "Không có mã môn học nào được cung cấp");
        }

        var idList = string.Join(",", ids);

        var deleteQuery = $"DELETE FROM Subject WHERE SubjectId IN ({idList})";

        var delete = await _context.Database.ExecuteSqlRawAsync(deleteQuery);

        if (delete == 0)
        {
          return new ResponseData<string>(404, "Môn học không tồn tại");
        }

        await transaction.CommitAsync();

        return new ResponseData<string>(200, "Đã xóa");
      }
      catch (Exception ex)
      {
        await transaction.RollbackAsync();
        return new ResponseData<string>(500, $"Server error: {ex.Message}");
      }
    }
  }
}
