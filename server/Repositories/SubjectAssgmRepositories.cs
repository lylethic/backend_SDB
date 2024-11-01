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
    public class SubjectAssgmRepositories : ISubject_Assgm
  {
    readonly SoDauBaiContext _context;

    public SubjectAssgmRepositories(SoDauBaiContext context)
    {

      this._context = context;
    }

    public async Task<ResponseData<SubjectAssgmDto>> CreateSubjectAssgm(SubjectAssgmDto model)
    {
      try
      {
        // check teacher
        var findTeacher = @"SELECT * FROM TEACHER WHERE TeacherId = @id";
        var teacherExists = await _context.Teachers
          .FromSqlRaw(findTeacher, new SqlParameter("@id", model.TeacherId))
          .FirstOrDefaultAsync();

        if (teacherExists is null)
        {
          return new ResponseData<SubjectAssgmDto>(404, "Teacher or Subject not found");
        }

        // check subject
        var findSubject = "SELECT * FROM Subject WHERE SubjectId = @id";
        var subjectExists = await _context.Subjects
         .FromSqlRaw(findSubject, new SqlParameter("@id", model.SubjectId))
         .FirstOrDefaultAsync();

        if (subjectExists is null)
        {
          return new ResponseData<SubjectAssgmDto>(404, "Teacher or Subject not found");
        }

        //check subject assignment
        var find = "SELECT * FROM SubjectAssignment WHERE subjectAssignmentId = @id";
        var subjectAssgmt = await _context.SubjectAssignments
          .FromSqlRaw(find, new SqlParameter("@id", model.SubjectAssignmentId))
          .FirstOrDefaultAsync();

        if (subjectAssgmt is not null)
        {
          return new ResponseData<SubjectAssgmDto>(409, "This Subject Assignment already exists");
        }

        var sqlInsert = @"INSERT INTO SubjectAssignment (teacherId, subjectId, description, dateCreated, dateUpdated)
                          VALUES (@teacherId, @subjectId, @description, @dateCreated, @dateUpdated);
                          SELECT CAST(SCOPE_IDENTITY() as int);";

        var insert = await _context.Database.ExecuteSqlRawAsync(sqlInsert,
          new SqlParameter("@teacherId", model.TeacherId),
          new SqlParameter("@subjectId", model.SubjectId),
          new SqlParameter("@description", model.Description),
          new SqlParameter("@dateCreated", DateTime.UtcNow),
          new SqlParameter("@dateUpdated", DBNull.Value)
          );

        var result = new SubjectAssgmDto
        {
          SubjectAssignmentId = insert,
          TeacherId = model.TeacherId,
          SubjectId = model.SubjectId,
          Description = model.Description,
          DateCreated = model.DateCreated,
          DateUpdated = model.DateUpdated,
        };

        return new ResponseData<SubjectAssgmDto>(200, result);
      }
      catch (Exception ex)
      {
        return new ResponseData<SubjectAssgmDto>(500, $"Server error: {ex.Message}");
      }
    }

    public async Task<ResponseData<SubjectAssgmDto>> DeleteSubjectAssgm(int id)
    {
      try
      {
        var find = "SELECT * FROM SUBJECTASSIGNMENT WHERE SubjectAssignmentId = @id";
        var subjectAssgmt = await _context.SubjectAssignments
          .FromSqlRaw(find, new SqlParameter("@id", id))
          .FirstOrDefaultAsync();

        if (subjectAssgmt is null)
        {
          return new ResponseData<SubjectAssgmDto>(404, "This Subject Assignment not found");
        }

        var deleteQuery = "DELETE FROM SUBJECTASSIGNMENT WHERE SubjectAssignmentId = @id";
        await _context.Database.ExecuteSqlRawAsync(deleteQuery, new SqlParameter("@id", id));

        return new ResponseData<SubjectAssgmDto>(200, "Deleted");
      }
      catch (Exception ex)
      {
        return new ResponseData<SubjectAssgmDto>(500, $"Server error: {ex.Message}");
      }
    }

    public async Task<ResponseData<SubjectAssgmDto>> GetSubjectAssgm(int id)
    {
      try
      {
        var find = @"SELECT s.subjectName , sa.subjectId , sa.subjectAssignmentId, sa.teacherId 
                      FROM 
                            SUBJECTASSIGNMENT sa 
                      LEFT JOIN 
                            Subject s ON sa.subjectId = s.subjectId
                      WHERE 
                            SubjectAssignmentId = @id";

        var subjectAssgmt = await _context.SubjectAssignments
          .FromSqlRaw(find, new SqlParameter("@id", id))
          .Select(static x => new SubjectAssignment
          {
            SubjectAssignmentId = x.SubjectAssignmentId,
            SubjectId = x.SubjectId,
            TeacherId = x.TeacherId,
            Subject = new Subject
            {
              SubjectName = x.Subject.SubjectName,
            }
          })
          .FirstOrDefaultAsync();

        if (subjectAssgmt is null)
        {
          return new ResponseData<SubjectAssgmDto>(404, "This Subject Assignment not found");
        }

        var result = new SubjectAssgmDto
        {
          SubjectAssignmentId = subjectAssgmt.SubjectAssignmentId,
          SubjectId = subjectAssgmt.SubjectId,
          TeacherId = subjectAssgmt.TeacherId,
          SubjectName = subjectAssgmt.Subject.SubjectName,
        };

        return new ResponseData<SubjectAssgmDto>(200, result);

      }
      catch (Exception ex)
      {
        return new ResponseData<SubjectAssgmDto>(500, $"Server error: {ex.Message}");
      }
    }

    public async Task<List<SubjectAssgmDto>> GetSubjectAssgms(int pageNumber, int pageSize)
    {
      try
      {
        var skip = (pageNumber - 1) * pageSize;
        var find = @"SELECT * FROM SubjectAssignment 
                      ORDER BY  SubjectAssignmentId 
                      OFFSET @skip ROWS
                      FETCH NEXT @pageSize ROWS ONLY;";

        var subjectAssgmt = await _context.SubjectAssignments
          .FromSqlRaw(find,
          new SqlParameter("@skip", skip),
          new SqlParameter("@pageSize", pageSize)
          ).ToListAsync() ?? throw new Exception("Empty");

        var result = subjectAssgmt.Select(x => new SubjectAssgmDto
        {
          SubjectAssignmentId = x.SubjectAssignmentId,
          SubjectId = x.SubjectId,
          TeacherId = x.TeacherId,
          Description = x.Description,
          DateCreated = x.DateCreated,
          DateUpdated = x.DateUpdated,
        }).ToList();

        return result;
      }
      catch (Exception ex)
      {
        throw new Exception($"Server Error: {ex.Message}");
      }
    }

    public async Task<ResponseData<SubjectAssgmDto>> UpdateSubjectAssgm(int id, SubjectAssgmDto model)
    {
      try
      {
        var find = "SELECT * FROM SUBJECTASSIGNMENT WHERE SubjectAssignmentId = @id";

        var subjectAssgmt = await _context.SubjectAssignments
          .FromSqlRaw(find, new SqlParameter("@id", id))
          .FirstOrDefaultAsync();

        if (subjectAssgmt is null)
        {
          return new ResponseData<SubjectAssgmDto>(404, "Subject Assigment not found");
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

        return new ResponseData<SubjectAssgmDto>(200, "Updated");
      }
      catch (Exception ex)
      {
        return new ResponseData<SubjectAssgmDto>(500, $"Server error: {ex.Message}");
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
                if (reader.GetValue(1) == null && reader.GetValue(2) == null && reader.GetValue(3) == null)
                {
                  // Stop processing when an empty row is encountered
                  break;
                }

                var mySubjects = new Models.SubjectAssignment
                {
                  TeacherId = Convert.ToInt16(reader.GetValue(1)),
                  SubjectId = Convert.ToInt16(reader.GetValue(2)),
                  Description = reader.GetValue(3).ToString()?.Trim() ?? "null",
                  DateCreated = DateTime.UtcNow,
                  DateUpdated = null
                };

                await _context.SubjectAssignments.AddAsync(mySubjects);
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

        var deleteQuery = $"DELETE FROM SubjectAssignments WHERE subjectAssignmentId IN ({idList})";

        var delete = await _context.Database.ExecuteSqlRawAsync(deleteQuery);

        if (delete == 0)
        {
          return new ResponseData<string>(404, "No subjectAssignmentId found to delete");
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
