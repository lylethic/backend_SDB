using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using server.Data;
using server.Dtos;
using server.IService;
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

    public async Task<Data_Response<SubjectDto>> CreateSubject(SubjectDto model)
    {
      try
      {
        var find = "SELECT * FROM Subject WHERE subjectId = @id";

        var subject = await _context.Subjects
          .FromSqlRaw(find, new SqlParameter("@id", model.SubjectId))
          .FirstOrDefaultAsync();

        if (subject is not null)
        {
          return new Data_Response<SubjectDto>(409, "Subject already exists");
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

        return new Data_Response<SubjectDto>(200, result);
      }
      catch (Exception ex)
      {
        return new Data_Response<SubjectDto>(500, $"Server error: {ex.Message}");
      }
    }

    public async Task<Data_Response<SubjectDto>> DeleteSubject(int id)
    {
      try
      {
        var find = "SELECT * FROM Subject WHERE subjectId = @id";

        var subject = await _context.Subjects
          .FromSqlRaw(find, new SqlParameter("@id", id))
          .FirstOrDefaultAsync();

        if (subject is null)
        {
          return new Data_Response<SubjectDto>(409, "Subject not found");
        }

        var deleteQuery = "DELETE FROM SUBJECT WHERE subjectId = @id";
        await _context.Database.ExecuteSqlRawAsync(deleteQuery, new SqlParameter("@id", id));

        return new Data_Response<SubjectDto>(200, "Deleted");
      }
      catch (Exception ex)
      {
        return new Data_Response<SubjectDto>(500, $"Server error: {ex.Message}");
      }
    }

    public async Task<Data_Response<SubjectDto>> GetSubject(int id)
    {
      try
      {
        var find = "SELECT * FROM Subject WHERE subjectId = @id";

        var subject = await _context.Subjects
          .FromSqlRaw(find, new SqlParameter("@id", id))
          .FirstOrDefaultAsync();

        if (subject is null)
        {
          return new Data_Response<SubjectDto>(404, "Subject not found");
        }

        var result = new SubjectDto
        {
          SubjectId = id,
          AcademicYearId = subject.AcademicYearId,
          SubjectName = subject.SubjectName,
        };

        return new Data_Response<SubjectDto>(200, result);
      }
      catch (Exception ex)
      {
        return new Data_Response<SubjectDto>(500, $"Server error: {ex.Message}");
      }
    }

    public async Task<List<SubjectDto>> GetSubjects()
    {
      try
      {
        var find = "SELECT * FROM Subject";

        var subject = await _context.Subjects.FromSqlRaw(find).ToListAsync();

        if (subject is null)
        {
          throw new Exception("Empty");
        }

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

    public async Task<Data_Response<SubjectDto>> UpdateSubject(int id, SubjectDto model)
    {
      try
      {
        var find = "SELECT * FROM Subject WHERE subjectId = @id";

        var subject = await _context.Subjects
          .FromSqlRaw(find, new SqlParameter("@id", id))
          .FirstOrDefaultAsync();

        if (subject is null)
        {
          return new Data_Response<SubjectDto>(404, "Subject not found");
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

        return new Data_Response<SubjectDto>(200, "Updated");
      }
      catch (Exception ex)
      {
        return new Data_Response<SubjectDto>(500, $"Server error: {ex.Message}");
      }
    }
  }
}
