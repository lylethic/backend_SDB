using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using server.Data;
using server.Dtos;
using server.IService;
using server.Models;
using System.Collections.Generic;
using System.Text;

namespace server.Repositories
{
  public class SemesterRepositories : ISemester
  {
    readonly SoDauBaiContext _context;

    public SemesterRepositories(SoDauBaiContext context) { this._context = context; }

    public async Task<Data_Response<SemesterDto>> CreateSemester(SemesterDto model)
    {
      try
      {
        var find = "SELECT * FROM Semester WHERE SemesterId = @id";

        var academicYear = await _context.Semesters
          .FromSqlRaw(find, new SqlParameter("@id", model.SemesterId))
          .FirstOrDefaultAsync();

        if (academicYear is not null)
        {
          return new Data_Response<SemesterDto>(409, "Semester already exists");
        }

        var sqlInsert = @"INSERT INTO Semester (academicYearId, semesterName, dateStart, dateEnd, description) 
                          VALUES (@academicYearId, @semesterName, @dateStart, @dateEnd, @description);
                          SELECT CAST(SCOPE_IDENTITY() as int);"
        ;

        var dateStart = new DateTime(model.DateStart.Year, model.DateStart.Month, model.DateStart.Day);
        var dateEnd = new DateTime(model.DateEnd.Year, model.DateEnd.Month, model.DateEnd.Day);


        var insert = await _context.Database.ExecuteSqlRawAsync(sqlInsert,
          new SqlParameter("@academicYearId", model.AcademicYearId),
          new SqlParameter("@semesterName", model.SemesterName),
          new SqlParameter("@dateStart", dateStart),
          new SqlParameter("@dateEnd", dateEnd),
          new SqlParameter("@description", model.Description)
        );

        var result = new SemesterDto
        {
          SemesterId = insert,
          AcademicYearId = model.AcademicYearId,
          SemesterName = model.SemesterName,
          DateStart = model.DateStart,
          DateEnd = model.DateEnd,
          Description = model.Description
        };

        return new Data_Response<SemesterDto>(200, result);
      }
      catch (Exception ex)
      {
        return new Data_Response<SemesterDto>(200, $"Server error: {ex.Message}");
      }
    }

    public async Task<Data_Response<SemesterDto>> DeleteSemester(int id)
    {
      try
      {
        var find = "SELECT * FROM Semester WHERE SemesterId = @id";
        var semester = await _context.Semesters
          .FromSqlRaw(find, new SqlParameter("@id", id))
          .FirstOrDefaultAsync();

        if (semester is null)
        {
          return new Data_Response<SemesterDto>(404, "Semester not found");
        }

        var deleteQuery = "DELETE FROM Semester WHERE SemesterId = @id";
        await _context.Database.ExecuteSqlRawAsync(deleteQuery, new SqlParameter("@id", id));

        return new Data_Response<SemesterDto>(200, "Deleted");
      }
      catch (Exception ex)
      {
        return new Data_Response<SemesterDto>(500, $"Server error: {ex.Message}");
      }
    }

    public async Task<Data_Response<SemesterDto>> GetSemester(int id)
    {
      try
      {
        var find = "SELECT * FROM Semester WHERE SemesterId = @id";
        var semester = await _context.Semesters
          .FromSqlRaw(find, new SqlParameter("@id", id))
          .FirstOrDefaultAsync();

        if (semester is null)
        {
          return new Data_Response<SemesterDto>(404, "Semester not found");
        }


        var result = new SemesterDto
        {
          SemesterId = id,
          AcademicYearId = semester.AcademicYearId,
          SemesterName = semester.SemesterName,
          DateStart = semester.DateStart,
          DateEnd = semester.DateEnd,
          Description = semester.Description
        };

        return new Data_Response<SemesterDto>(200, result);
      }
      catch (Exception ex)
      {
        return new Data_Response<SemesterDto>(500, $"Server error: {ex.Message}");
      }
    }

    public async Task<List<SemesterDto>> GetSemesters()
    {
      try
      {
        var query = "SELECT * FROM Semester";
        var semester = await _context.Semesters.FromSqlRaw(query).ToListAsync();

        if (semester is null)
        {
          throw new Exception("No content");
        }

        var result = semester.Select(x => new SemesterDto
        {
          SemesterId = x.SemesterId,
          AcademicYearId = x.AcademicYearId,
          DateStart = x.DateStart,
          DateEnd = x.DateEnd,
          Description = x.Description
        }).ToList();

        return result;
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
        throw;
      }
    }

    public async Task<Data_Response<SemesterDto>> UpdateSemester(int id, SemesterDto model)
    {
      try
      {
        // Check if the teacher exists in the database
        var findQuery = "SELECT * FROM Semester WHERE semesterId = @id";
        var existingTeacher = await _context.Semesters
            .FromSqlRaw(findQuery, new SqlParameter("@id", id))
            .FirstOrDefaultAsync();

        if (existingTeacher == null)
        {
          return new Data_Response<SemesterDto>(404, "Semester not found");
        }

        // Build update query dynamically based on non-null fields
        var queryBuilder = new StringBuilder("UPDATE Semester SET ");
        var parameters = new List<SqlParameter>();

        if(model.AcademicYearId != 0)
        {
          queryBuilder.Append("AcademicYearId = @AcademicYearId, ");
          parameters.Add(new SqlParameter("@AcademicYearId", model.AcademicYearId));
        }

        if (!string.IsNullOrEmpty(model.SemesterName))
        {
          queryBuilder.Append("SemesterName = @SemesterName, ");
          parameters.Add(new SqlParameter("@SemesterName", model.SemesterName));
        }

        queryBuilder.Append("DateStart = @DateStart, ");
        parameters.Add(new SqlParameter("@DateStart", model.DateStart));

        queryBuilder.Append("DateEnd = @DateEnd, ");
        parameters.Add(new SqlParameter("@DateEnd", model.DateEnd));

        if (!string.IsNullOrEmpty(model.Description))
        {
          queryBuilder.Append("Description = @Description, ");
          parameters.Add(new SqlParameter("@Description", model.Description));
        }
        // Remove trailing comma from the query if necessary
        if (queryBuilder[queryBuilder.Length - 2] == ',')
        {
          queryBuilder.Length -= 2;
        }

        queryBuilder.Append(" WHERE semesterId = @id");
        parameters.Add(new SqlParameter("@id", id));

        // Execute the update query
        var updateQuery = queryBuilder.ToString();
        await _context.Database.ExecuteSqlRawAsync(updateQuery, parameters.ToArray());

        return new Data_Response<SemesterDto>(200, "Semester updated successfully");
      }
      catch (Exception ex)
      {
        return new Data_Response<SemesterDto>(500, $"Server Error: {ex.Message}");
      }
    }
  }
}
