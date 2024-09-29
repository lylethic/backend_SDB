﻿using ExcelDataReader;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using server.Data;
using server.Dtos;
using server.IService;
using System.Text;

namespace server.Repositories
{
  public class ClassifyRepositories : IClassify
  {
    private readonly SoDauBaiContext _context;

    public ClassifyRepositories(SoDauBaiContext context)
    {
      this._context = context;
    }

    public async Task<Data_Response<ClassifyDto>> CreateClassify(ClassifyDto model)
    {
      try
      {
        var find = "SELECT * FROM CLASSIFICATION WHERE classificationId = @id";
        var classify = await _context.Classifications
          .FromSqlRaw(find, new SqlParameter("@id", model.ClassificationId))
          .FirstOrDefaultAsync();

        if (classify is not null)
        {
          return new Data_Response<ClassifyDto>(409, "classificationId already exists");
        }

        var query = @"INSERT INTO CLASSIFICATION (classifyName, score) 
                       VALUES (@classifyName, @score)";

        var insert = await _context.Database.ExecuteSqlRawAsync(query,
          new SqlParameter("@classifyName", model.ClassifyName),
          new SqlParameter("@score", model.Score)
          );

        var result = new ClassifyDto
        {
          ClassificationId = insert,
          ClassifyName = model.ClassifyName,
          Score = model.Score,
        };

        return new Data_Response<ClassifyDto>(200, result);

      }
      catch (Exception ex)
      {
        return new Data_Response<ClassifyDto>(200, $"Server error: {ex.Message}");
      }
    }

    public async Task<Data_Response<ClassifyDto>> GetClassify(int id)
    {
      try
      {
        var query = @"SELECT * from Classification where ClassificationId = @id";

        // Fetch 
        var student = await _context.Classifications
            .FromSqlRaw(query, new SqlParameter("@id", id))
            .FirstOrDefaultAsync();

        if (student is null)
        {
          return new Data_Response<ClassifyDto>(404, "Not found");
        }

        // Map the result to the StudentDto
        var result = new ClassifyDto
        {
          ClassificationId = id,
          ClassifyName = student.ClassifyName,
          Score = student.Score,
        };

        return new Data_Response<ClassifyDto>(200, result);
      }
      catch (Exception ex)
      {
        return new Data_Response<ClassifyDto>(500, $"Server error: {ex.Message}");
      }
    }

    public async Task<List<ClassifyDto>> GetClassifys(int pageNumber, int pageSize)
    {
      try
      {
        // Calculate the number of records to skip based on the page number and size
        var skip = (pageNumber - 1) * pageSize;

        // Modify the query to use Skip and Take for pagination
        var roles = await _context.Classifications
            .Skip(skip)     // Skip the first (pageNumber - 1) * pageSize records
            .Take(pageSize) // Take pageSize records
            .ToListAsync();

        var result = roles.Select(x => new ClassifyDto
        {
          ClassificationId = x.ClassificationId,
          ClassifyName = x.ClassifyName,
          Score = x.Score,
        }).ToList();

        return result;
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
        throw;
      }
    }

    public async Task<Data_Response<ClassifyDto>> UpdateClassify(int id, ClassifyDto model)
    {
      try
      {
        var find = "SELECT * FROM Classification WHERE ClassificationId = @id";
        var exists = await _context.Classifications
          .FromSqlRaw(find, new SqlParameter("@id", id))
          .FirstOrDefaultAsync();

        if (exists == null)
        {
          return new Data_Response<ClassifyDto>(404, "ClassificationId not found");
        }

        var queryBuilder = new StringBuilder("UPDATE Classification SET ");
        var parameters = new List<SqlParameter>();

        if (!string.IsNullOrEmpty(model.ClassifyName))
        {
          queryBuilder.Append("ClassifyName = @ClassifyName, ");
          parameters.Add(new SqlParameter("@ClassifyName", model.ClassifyName));
        }

        queryBuilder.Append("Score = @Score, ");
        parameters.Add(new SqlParameter("@Score", model.Score));

        // Remove the last comma and space
        if (queryBuilder.Length > 0)
        {
          queryBuilder.Length -= 2;
        }

        queryBuilder.Append(" WHERE ClassificationId = @id");
        parameters.Add(new SqlParameter("@id", id));

        var updateQuery = queryBuilder.ToString();
        await _context.Database.ExecuteSqlRawAsync(updateQuery, parameters.ToArray());

        return new Data_Response<ClassifyDto>(200, "Updated");
      }
      catch (Exception ex)
      {
        return new Data_Response<ClassifyDto>(500, $"Server Error: {ex.Message}");
      }
    }

    public async Task<Data_Response<ClassifyDto>> DeleteClassify(int id)
    {
      try
      {
        var find = "SELECT * FROM Classification WHERE ClassificationId = @id";
        var Classify = await _context.Classifications
          .FromSqlRaw(find, new SqlParameter("@id", id))
          .FirstOrDefaultAsync();

        if (Classify is null)
        {
          return new Data_Response<ClassifyDto>(404, "not found");
        }

        var deleteQuery = "DELETE FROM Classification WHERE ClassificationId = @id";
        await _context.Database.ExecuteSqlRawAsync(deleteQuery, new SqlParameter("@id", id));
        return new Data_Response<ClassifyDto>(200, "Deleted");
      }
      catch (Exception ex)
      {
        return new Data_Response<ClassifyDto>(500, $"Server error: {ex.Message}");
      }
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

        var deleteQuery = $"DELETE FROM Classification WHERE ClassificationId IN ({idList})";

        var delete = await _context.Database.ExecuteSqlRawAsync(deleteQuery);

        if (delete == 0)
        {
          return new Data_Response<string>(404, "No ClassificationId found to delete");
        }

        await transaction.CommitAsync();

        return new Data_Response<string>(200, "Deleted succesfully");
      }
      catch (Exception ex)
      {
        await transaction.RollbackAsync();
        return new Data_Response<string>(500, $"Server error: {ex.Message}");
      }
    }

    public async Task<string> ImportExcel(IFormFile file)
    {
      try
      {
        System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

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

          using (var stream = System.IO.File.Open(filePath, FileMode.Open, FileAccess.Read))
          {
            using (var reader = ExcelReaderFactory.CreateReader(stream))
            {
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

                  var myClassify = new Models.Classification
                  {
                    ClassifyName = reader.GetValue(1).ToString() ?? "Xep loai",
                    Score = Convert.ToInt32(reader.GetValue(2)),
                  };

                  await _context.Classifications.AddAsync(myClassify);
                  await _context.SaveChangesAsync();
                }
              } while (reader.NextResult());
            }
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
  }
}