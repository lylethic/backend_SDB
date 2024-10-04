using AutoMapper;
using ExcelDataReader;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using server.Dtos;
using server.IService;
using server.Models;
using System.Text;
using System.Text.RegularExpressions;

namespace server.Repositories
{
  public class AccountRespositories : IService.IAccount
  {
    private readonly Data.SoDauBaiContext _context;
    private readonly IAuth _auth;
    private readonly IMapper _map;

    public AccountRespositories(Data.SoDauBaiContext context, IMapper map, IAuth auth)
    {
      _context = context;
      _map = map;
      _auth = auth;
    }

    private static bool IsValidEmail(string email)
    {
      var regex = @"^[\w\.-]+@gmail\.com$";
      return Regex.IsMatch(email, regex);
    }

    public async Task<Data_Response<AccountDto>> AddAccount(RegisterDto acc)
    {
      try
      {
        if (!IsValidEmail(acc.Email))
        {
          return new Data_Response<AccountDto>(400, "Invalid Email format");
        }

        var query = "SELECT * FROM ACCOUNT WHERE Email = @email";

        var account = await _context.Accounts
            .FromSqlRaw(query, new SqlParameter("@email", acc.Email))
            .FirstOrDefaultAsync();

        if (account != null)
        {
          return new Data_Response<AccountDto>(409, "Email already registered");
        }

        // Hash the password
        byte[] passwordHash, passwordSalt;
        _auth.GenerateHash(acc.Password, out passwordHash, out passwordSalt);

        /*
         The SCOPE_IDENTITY() function is used to return the ID of the newly inserted row,
         which is crucial for getting the AccountId after the insertion.
        */
        string sqlInsert = @"
            INSERT INTO Account (Email, RoleId, SchoolId, Password, PasswordSalt, DateCreated, DateUpdated)
            VALUES (@Email, @RoleId, @SchoolId, @Password, @PasswordSalt, @DateCreated, @DateUpdated);
            SELECT CAST(SCOPE_IDENTITY() as int);
        ";

        acc.DateCreated = DateTime.Now;
        acc.DateUpdated = null;

        var accountId = await _context.Database.ExecuteSqlRawAsync(sqlInsert,
            new SqlParameter("@Email", acc.Email),
            new SqlParameter("@RoleId", acc.RoleId),
            new SqlParameter("@SchoolId", acc.SchoolId),
            new SqlParameter("@Password", passwordHash),
            new SqlParameter("@PasswordSalt", passwordSalt),
            new SqlParameter("@DateCreated", acc.DateCreated),
            new SqlParameter("@DateUpdated", DBNull.Value)
        );

        var accountDto = new AccountDto
        {
          AccountId = accountId,
          Email = acc.Email,
          RoleId = acc.RoleId,
          SchoolId = acc.SchoolId,
          DateCreated = acc.DateCreated,
          DateUpdated = acc.DateUpdated,
        };

        return new Data_Response<AccountDto>(200, accountDto);
      }
      catch (Exception ex)
      {
        return new Data_Response<AccountDto>(500, $"Server error: {ex.Message}");
      }
    }

    public async Task<Data_Response<AccountDto>> GetAccount(int id)
    {
      try
      {
        var query = @"
            SELECT acc.AccountId, acc.RoleId, acc.SchoolId, acc.Email, 
                   tea.TeacherId, tea.Fullname, tea.Status
            FROM Account as acc 
            LEFT JOIN Teacher as tea ON acc.AccountId = tea.AccountId
            WHERE acc.AccountId = @id";

        var account = await _context.Accounts
            .FromSqlRaw(query, new SqlParameter("@id", id))
            .Select(acc => new AccountDto
            {
              AccountId = acc.AccountId,
              RoleId = acc.RoleId,
              SchoolId = acc.SchoolId,
              Email = acc.Email,
              Teacher = acc.Teachers.Select(tea => new TeacherDto
              {
                TeacherId = tea.TeacherId,
                Fullname = tea.Fullname,
                Status = tea.Status
              }).FirstOrDefault()
            })
            .FirstOrDefaultAsync();

        if (account is null)
        {
          return new Data_Response<AccountDto>(404, "Account not found");
        }

        return new Data_Response<AccountDto>(200, account);
      }
      catch (Exception ex)
      {
        return new Data_Response<AccountDto>(500, $"Server error: {ex.Message}");
      }
    }

    //public async Task<Data_Response<AccountDto>> GetAccount(int accountId)
    //{
    //  try
    //  {
    //    var query = "SELECT * FROM ACCOUNT WHERE AccountId = @accountId";

    //    var acc = await _context.Accounts
    //        .FromSqlRaw(query, new SqlParameter("@accountId", accountId))
    //        .FirstOrDefaultAsync();

    //    if (acc is null)
    //    {
    //      return new Data_Response<AccountDto>(404, "Account not founnd");
    //    }

    //    var result = new AccountDto
    //    {
    //      AccountId = acc.AccountId,
    //      RoleId = acc.RoleId,
    //      SchoolId = acc.SchoolId,
    //      Email = acc.Email,
    //      DateCreated = acc.DateCreated,
    //      DateUpdated = acc.DateUpdated
    //    };

    //    return new Data_Response<AccountDto>(200, result);
    //  }
    //  catch (Exception ex)
    //  {
    //    return new Data_Response<AccountDto>(500, $"Server error: {ex.Message}");
    //  }
    //}

    public async Task<List<AccountDto>> GetAccounts(int pageNumber, int pageSize)
    {
      try
      {
        var skip = (pageNumber - 1) * pageSize;

        var query = @"SELECT * FROM ACCOUNT 
                      ORDER BY EMAIL 
                      OFFSET @skip ROWS
                      FETCH NEXT @pageSize ROWS ONLY;";

        var accsList = await _context.Accounts
          .FromSqlRaw(query,
                      new SqlParameter("@skip", skip),
                      new SqlParameter("@pageSize", pageSize)
          ).ToListAsync() ?? throw new Exception("Empty");

        var result = accsList.Select(acc => new AccountDto
        {
          AccountId = acc.AccountId,
          RoleId = acc.RoleId,
          SchoolId = acc.SchoolId,
          Email = acc.Email,
          DateCreated = acc.DateCreated,
          DateUpdated = acc.DateUpdated
        }).ToList();

        return result;
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
        throw new Exception($"Server error: {ex.Message}");
      }
    }

    public async Task<List<AccountDto>> GetAccountsByRole(int pageNumber, int pageSize, int roleId)
    {
      try
      {
        var skip = (pageNumber - 1) * pageSize;

        var query = @"SELECT * FROM ACCOUNT 
                      WHERE RoleId = @roleId
                      ORDER BY EMAIL 
                      OFFSET @skip ROWS
                      FETCH NEXT @pageSize ROWS ONLY;";

        var accsList = await _context.Accounts
            .FromSqlRaw(query,
                        new SqlParameter("@roleId", roleId),
                        new SqlParameter("@skip", skip),
                        new SqlParameter("@pageSize", pageSize)
            ).ToListAsync() ?? throw new Exception("Empty");

        var result = accsList.Select(acc => new AccountDto
        {
          AccountId = acc.AccountId,
          RoleId = acc.RoleId,
          SchoolId = acc.SchoolId,
          Email = acc.Email,
          DateCreated = acc.DateCreated,
          DateUpdated = acc.DateUpdated
        }).ToList();

        return result;
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
        throw new Exception($"Server error: {ex.Message}");
      }
    }

    public async Task<Data_Response<AccountDto>> UpdateAccount(int accountId, AccountDto model)
    {
      try
      {
        if (!IsValidEmail(model.Email))
        {
          return new Data_Response<AccountDto>(400, "Invalid Email format");
        }

        var query = "SELECT * FROM ACCOUNT WHERE AccountId = @accountId";

        var existingAccount = await _context.Accounts
          .FromSqlRaw(query, new SqlParameter("@accountId", accountId))
          .FirstOrDefaultAsync();

        if (existingAccount is null)
        {
          return new Data_Response<AccountDto>(404, "Account not found");
        }

        bool hasChanges = false;

        var queryBuilder = new StringBuilder("UPDATE Account SET ");
        var parameters = new List<SqlParameter>();

        if (!string.IsNullOrEmpty(model.Email) && model.Email != existingAccount.Email)
        {
          queryBuilder.Append("Email = @Email, ");
          parameters.Add(new SqlParameter("@Email", model.Email));
          hasChanges = true;
        }

        if (model.RoleId != 0 && model.RoleId != existingAccount.RoleId)
        {
          queryBuilder.Append("RoleId = @RoleId, ");
          parameters.Add(new SqlParameter("@RoleId", model.RoleId));
          hasChanges = true;
        }

        if (model.SchoolId != 0 && model.SchoolId != existingAccount.SchoolId)
        {
          queryBuilder.Append("SchoolId = @SchoolId, ");
          parameters.Add(new SqlParameter("@SchoolId", model.SchoolId));
          hasChanges = true;
        }

        if (model.DateCreated.HasValue)
        {
          queryBuilder.Append("DateCreated = @DateCreated, ");
          parameters.Add(new SqlParameter("@DateCreated", model.DateCreated.Value));
          hasChanges = true;
        }

        var currentDate = DateTime.Now;
        if (currentDate != model.DateUpdated)
        {
          queryBuilder.Append("DateUpdated = @DateUpdated, ");
          parameters.Add(new SqlParameter("@DateUpdated", currentDate));
          hasChanges = true;
        }

        if (hasChanges)
        {
          queryBuilder.Length -= 2;

          queryBuilder.Append(" WHERE AccountId = @accountId");
          parameters.Add(new SqlParameter("@accountId", accountId));

          // Execute the update query
          var updateQuery = queryBuilder.ToString();
          await _context.Database.ExecuteSqlRawAsync(updateQuery, [.. parameters]);
          return new Data_Response<AccountDto>(200, "Updated");

        }
        else
        {
          return new Data_Response<AccountDto>(200, "No changes detected");
        }
      }
      catch (Exception ex)
      {
        return new Data_Response<AccountDto>(500, $"Server Error: {ex.Message}");
      }
    }

    public async Task<Data_Response<AccountDto>> DeleteAccount(int accountId)
    {
      try
      {
        // Find account
        string findAccount = "SELECT * FROM Account WHERE AccountId = @accountId";
        var account = await _context.Accounts
          .FromSqlRaw(findAccount, new SqlParameter("@accountId", accountId))
          .FirstOrDefaultAsync();

        // Check account null??
        if (account is null)
        {
          return new Data_Response<AccountDto>(404, "Account not found!");
        }

        // Delete query
        var deleteQuery = "DELETE FROM Account WHERE AccountId = @accountId";
        await _context.Database
          .ExecuteSqlRawAsync(deleteQuery, new SqlParameter("@accountId", accountId));

        return new Data_Response<AccountDto>(200, "Deleted");
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
        return new Data_Response<AccountDto>(500, $"Error deleting account: {ex.Message}");
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

                  var accountDto = new Dtos.RegisterDto
                  {
                    RoleId = Convert.ToInt16(reader.GetValue(1)),
                    SchoolId = Convert.ToInt16(reader.GetValue(2)),
                    Email = reader.GetValue(3).ToString() ?? "email",
                    Password = reader.GetValue(4)?.ToString() ?? string.Empty,
                    DateCreated = DateTime.Now,
                    DateUpdated = null
                  };

                  if (!IsValidEmail(accountDto.Email))
                  {
                    continue;
                  }

                  var query = "SELECT * FROM ACCOUNT WHERE Email = @email";

                  var existingAccount = await _context.Accounts
                      .FromSqlRaw(query, new SqlParameter("@email", accountDto.Email))
                      .FirstOrDefaultAsync();

                  if (existingAccount is not null)
                  {
                    // Skip account if it already exists
                    continue;
                    throw new Exception("Email already exists");
                  }

                  // Hash the password
                  byte[] passwordHash, passwordSalt;
                  _auth.GenerateHash(accountDto.Password, out passwordHash, out passwordSalt);

                  var newAccount = new Account
                  {
                    RoleId = accountDto.RoleId,
                    SchoolId = accountDto.SchoolId,
                    Email = accountDto.Email,
                    MatKhau = passwordHash,
                    PasswordSalt = passwordSalt,
                    DateCreated = accountDto.DateCreated,
                    DateUpdated = accountDto.DateUpdated,
                  };


                  await _context.Accounts.AddAsync(newAccount);
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

        var deleteQuery = $"DELETE FROM Account WHERE AccountId IN ({idList})";

        var delete = await _context.Database.ExecuteSqlRawAsync(deleteQuery);

        if (delete == 0)
        {
          return new Data_Response<string>(404, "No AccountId found to delete");
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
  }
}
