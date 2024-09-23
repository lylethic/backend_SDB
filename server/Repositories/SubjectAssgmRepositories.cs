using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using server.Data;
using server.Dtos;
using server.IService;
using System.Text;

namespace server.Repositories
{
  public class SubjectAssgmRepositories : ISubject_Assgm
  {
    readonly SoDauBaiContext _context;

    public SubjectAssgmRepositories(SoDauBaiContext context)
    {

      this._context = context;
    }

    public async Task<Data_Response<SubjectAssgmDto>> CreateSubjectAssgm(SubjectAssgmDto model)
    {
      try
      {
        // check teacher
        var findTeacher = "SELECT * FROM TEACHER WHERE TeacherId = @id";
        var teacherExists = await _context.Teachers
          .FromSqlRaw(findTeacher, new SqlParameter("@id", model.TeacherId))
          .FirstOrDefaultAsync();

        if (teacherExists is null)
        {
          return new Data_Response<SubjectAssgmDto>(404, "Teacher or Subject not found");
        }

        // check subject
        var findSubject = "SELECT * FROM Subject WHERE SubjectId = @id";
        var subjectExists = await _context.Subjects
         .FromSqlRaw(findSubject, new SqlParameter("@id", model.SubjectId))
         .FirstOrDefaultAsync();

        if (subjectExists is null)
        {
          return new Data_Response<SubjectAssgmDto>(404, "Teacher or Subject not found");
        }

        //check subject assignment
        var find = "SELECT * FROM SubjectAssignment WHERE subjectAssignmentId = @id";
        var subjectAssgmt = await _context.SubjectAssignments
          .FromSqlRaw(find, new SqlParameter("@id", model.SubjectAssignmentId))
          .FirstOrDefaultAsync();

        if (subjectAssgmt is not null)
        {
          return new Data_Response<SubjectAssgmDto>(409, "This Subject Assignment already exists");
        }

        var sqlInsert = @"INSERT INTO SubjectAssignment (teacherId, subjectId, description)
                          VALUES (@teacherId, @subjectId, @description);
                          SELECT CAST(SCOPE_IDENTITY() as int);";

        var insert = await _context.Database.ExecuteSqlRawAsync(sqlInsert,
          new SqlParameter("@TeacherId", model.TeacherId),
          new SqlParameter("@SubjectId", model.SubjectId),
          new SqlParameter("@Description", model.Description)
          );

        var result = new SubjectAssgmDto
        {
          SubjectAssignmentId = insert,
          TeacherId = model.TeacherId,
          SubjectId = model.SubjectId,
          Description = model.Description,
        };

        return new Data_Response<SubjectAssgmDto>(200, result);
      }
      catch (Exception ex)
      {
        return new Data_Response<SubjectAssgmDto>(500, $"Server error: {ex.Message}");
      }
    }

    public async Task<Data_Response<SubjectAssgmDto>> DeleteSubjectAssgm(int id)
    {
      try
      {
        var find = "SELECT * FROM SUBJECTASSIGNMENT WHERE SubjectAssignmentId = @id";
        var subjectAssgmt = await _context.SubjectAssignments
          .FromSqlRaw(find, new SqlParameter("@id", id))
          .FirstOrDefaultAsync();

        if (subjectAssgmt is null)
        {
          return new Data_Response<SubjectAssgmDto>(404, "This Subject Assignment not found");
        }

        var deleteQuery = "DELETE FROM SUBJECTASSIGNMENT WHERE SubjectAssignmentId = @id";
        await _context.Database.ExecuteSqlRawAsync(deleteQuery, new SqlParameter("@id", id));

        return new Data_Response<SubjectAssgmDto>(200, "Deleted");
      }
      catch (Exception ex)
      {
        return new Data_Response<SubjectAssgmDto>(500, $"Server error: {ex.Message}");
      }
    }

    public async Task<Data_Response<SubjectAssgmDto>> GetSubjectAssgm(int id)
    {
      try
      {
        var find = "SELECT * FROM SUBJECTASSIGNMENT WHERE SubjectAssignmentId = @id";
        var subjectAssgmt = await _context.SubjectAssignments
          .FromSqlRaw(find, new SqlParameter("@id", id))
          .FirstOrDefaultAsync();

        if (subjectAssgmt is null)
        {
          return new Data_Response<SubjectAssgmDto>(404, "This Subject Assignment not found");
        }

        var result = new SubjectAssgmDto
        {
          SubjectAssignmentId = subjectAssgmt.SubjectAssignmentId,
          SubjectId = subjectAssgmt.SubjectId,
          TeacherId = subjectAssgmt.TeacherId,
          Description = subjectAssgmt.Description,
        };

        return new Data_Response<SubjectAssgmDto>(200, result);

      }
      catch (Exception ex)
      {
        return new Data_Response<SubjectAssgmDto>(500, "Server error");
      }
    }

    public async Task<List<SubjectAssgmDto>> GetSubjectAssgms()
    {
      try
      {
        var find = "SELECT * FROM SUBJECTASSIGNMENT";
        var subjectAssgmt = await _context.SubjectAssignments
          .FromSqlRaw(find).ToListAsync();

        if (subjectAssgmt is null)
        {
          throw new Exception("Empty");
        }

        var result = subjectAssgmt.Select(x => new SubjectAssgmDto
        {
          SubjectAssignmentId = x.SubjectAssignmentId,
          SubjectId = x.SubjectId,
          TeacherId = x.TeacherId,
          Description = x.Description,
        }).ToList();

        return result;
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
        throw;
      }
    }

    public async Task<Data_Response<SubjectAssgmDto>> UpdateSubjectAssgm(int id, SubjectAssgmDto model)
    {
      try
      {
        var find = "SELECT * FROM SUBJECTASSIGNMENT WHERE SubjectAssignmentId = @id";

        var subjectAssgmt = await _context.SubjectAssignments
          .FromSqlRaw(find, new SqlParameter("@id", id))
          .FirstOrDefaultAsync();

        if (subjectAssgmt is null)
        {
          return new Data_Response<SubjectAssgmDto>(404, "Subject Assigment not found");
        }

        var queryBuilder = new StringBuilder("UPDATE SUBJECTASSIGNMENT SET ");
        var parameters = new List<SqlParameter>();

        if (model.SubjectId != 0 || model.TeacherId != 0)
        {
          queryBuilder.Append("TeacherId = @TeacherId, ");
          parameters.Add(new SqlParameter("@TeacherId", model.TeacherId));

          queryBuilder.Append("SubjectId = @SubjectId, ");
          parameters.Add(new SqlParameter("@SubjectId", model.SubjectId));

          queryBuilder.Append("Description = @Description, ");
          parameters.Add(new SqlParameter("@Description", model.Description));
        }

        if (queryBuilder[queryBuilder.Length - 2] == ',')
        {
          queryBuilder.Length -= 2;
        }

        queryBuilder.Append(" WHERE SubjectAssignmentId = @id");
        parameters.Add(new SqlParameter("@id", id));

        var updateQuery = queryBuilder.ToString();
        await _context.Database.ExecuteSqlRawAsync(updateQuery, parameters.ToArray());

        return new Data_Response<SubjectAssgmDto>(200, "Updated");
      }
      catch (Exception ex)
      {
        return new Data_Response<SubjectAssgmDto>(500, $"Server error: {ex.Message}");
      }
    }
  }
}
