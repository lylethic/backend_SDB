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

        var sqlInsert = @"INSERT INTO SUBJECT (academicYearId, subjectName)
                     VALUES (@academicYearId, @subjectName);
                     SELECT CAST(SCOPE_IDENTITY() as int);";

        var insert = await _context.Database.ExecuteSqlRawAsync(sqlInsert,
          new SqlParameter("@academicYearId", model.AcademicYearId),
          new SqlParameter("@subjectName", model.SubjectName)
          );

        var result = new SubjectDto
        {
          SubjectId = insert,
          AcademicYearId = model.AcademicYearId,
          SubjectName = model.SubjectName,
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
			                     FORMAT(a.yearEnd, 'dd/MM/yyyy') AS formatDateEnd
                    FROM 
			                     dbo.SUBJECT s 
                    RIGHT JOIN 
			                     dbo.AcademicYear a 
                    ON 
			                     S.academicYearId = A.academicYearId
                    WHERE  
			                     S.subjectId = @id";

        var subject = await _context.Subjects
          .FromSqlRaw(find, new SqlParameter("@id", id))
          .Select(static x => new Subject
          {
            SubjectId = x.SubjectId,
            SubjectName = x.SubjectName,
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
          return new ResponseData<SubjectDto>(404, "Subject not found");
        }

        var result = new SubjectDto
        {
          SubjectId = id,
          AcademicYearId = subject.AcademicYearId,
          SubjectName = subject.SubjectName,
          DisplayAcademicYear_Name = subject.AcademicYear.DisplayAcademicYearName,
          YearStart = subject.AcademicYear.YearStart,
          YearEnd = subject.AcademicYear.YearEnd
        };

        return new ResponseData<SubjectDto>(200, result);
      }
      catch (Exception ex)
      {
        return new ResponseData<SubjectDto>(500, $"Server error: {ex.Message}");
      }
    }

    public async Task<List<SubjectDto>> GetSubjects(int pageNumber, int pageSize)
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
        }).ToList();

        return result;
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
        throw;
      }
    }

    public async Task<ResponseData<SubjectDto>> UpdateSubject(int id, SubjectDto model)
    {
      try
      {
        var find = "SELECT * FROM Subject WHERE subjectId = @id";

        var subject = await _context.Subjects
          .FromSqlRaw(find, new SqlParameter("@id", id))
          .FirstOrDefaultAsync();

        if (subject is null)
        {
          return new ResponseData<SubjectDto>(404, "Subject not found");
        }

        var queryBuilder = new StringBuilder("UPDATE Subject SET ");
        var parameters = new List<SqlParameter>();

        if (model.AcademicYearId != 0 || !string.IsNullOrEmpty(model.SubjectName))
        {
          queryBuilder.Append("AcademicYearId = @AcademicYearId, ");
          parameters.Add(new SqlParameter("@AcademicYearId", model.AcademicYearId));

          queryBuilder.Append("SubjectName = @SubjectName, ");
          parameters.Add(new SqlParameter("@SubjectName", model.SubjectName));
        }

        if (queryBuilder[queryBuilder.Length - 2] == ',')
        {
          queryBuilder.Length -= 2;
        }

        queryBuilder.Append(" WHERE subjectId = @id");
        parameters.Add(new SqlParameter("@id", id));

        var updateQuery = queryBuilder.ToString();
        await _context.Database.ExecuteSqlRawAsync(updateQuery, parameters.ToArray());

        return new ResponseData<SubjectDto>(200, "Updated");
      }
      catch (Exception ex)
      {
        return new ResponseData<SubjectDto>(500, $"Server error: {ex.Message}");
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
                if (reader.GetValue(1) == null && reader.GetValue(2) == null)
                {
                  // Stop processing when an empty row is encountered
                  break;
                }

                var mySubjects = new Models.Subject
                {
                  AcademicYearId = Convert.ToInt16(reader.GetValue(1)),
                  SubjectName = reader.GetValue(2).ToString() ?? "null"
                };

                await _context.Subjects.AddAsync(mySubjects);
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

    public async Task<ResponseData<string>> BulkDelete(List<int> ids)
    {
      await using var transaction = await _context.Database.BeginTransactionAsync();

      try
      {
        if (ids is null || ids.Count == 0)
        {
          return new ResponseData<string>(400, "No IDs provided.");
        }

        var idList = string.Join(",", ids);

        var deleteQuery = $"DELETE FROM Subject WHERE SubjectId IN ({idList})";

        var delete = await _context.Database.ExecuteSqlRawAsync(deleteQuery);

        if (delete == 0)
        {
          return new ResponseData<string>(404, "No SubjectId found to delete");
        }

        await transaction.CommitAsync();

        return new ResponseData<string>(200, "Deleted");
      }
      catch (Exception ex)
      {
        await transaction.RollbackAsync();
        return new ResponseData<string>(500, $"Server error: {ex.Message}");
      }
    }
  }
}
