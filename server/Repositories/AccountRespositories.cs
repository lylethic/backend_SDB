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
  public class AccountRespositories : IAccount
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
            INSERT INTO Account (Email, RoleId, SchoolId, Password, PasswordSalt)
            VALUES (@Email, @RoleId, @SchoolId, @Password, @PasswordSalt);
            SELECT CAST(SCOPE_IDENTITY() as int);
        ";

        var accountId = await _context.Database.ExecuteSqlRawAsync(sqlInsert,
            new SqlParameter("@Email", acc.Email),
            new SqlParameter("@RoleId", acc.RoleId),
            new SqlParameter("@SchoolId", acc.SchoolId),
            new SqlParameter("@Password", passwordHash),
            new SqlParameter("@PasswordSalt", passwordSalt)
        );

        var accountDto = new AccountDto
        {
          AccountId = accountId,
          Email = acc.Email,
          RoleId = acc.RoleId,
          SchoolId = acc.SchoolId,
        };

        return new Data_Response<AccountDto>(200, accountDto);
      }
      catch (Exception ex)
      {
        return new Data_Response<AccountDto>(500, $"Server error: {ex.Message}");
      }
    }

    public async Task<Data_Response<AccountDto>> GetAccount(int accountId)
    {
      try
      {
        var query = "SELECT * FROM ACCOUNT WHERE AccountId = @accountId";

        var acc = await _context.Accounts
            .FromSqlRaw(query, new SqlParameter("@accountId", accountId))
            .FirstOrDefaultAsync();

        if (acc is null)
        {
          return new Data_Response<AccountDto>(404, "Account not founnd");
        }

        var result = new AccountDto
        {
          AccountId = acc.AccountId,
          RoleId = acc.RoleId,
          SchoolId = acc.SchoolId,
          Email = acc.Email
        };

        return new Data_Response<AccountDto>(200, result);
      }
      catch (Exception ex)
      {
        return new Data_Response<AccountDto>(500, $"Server error: {ex.Message}");
      }
    }

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
          Email = acc.Email
        }).ToList();

        return result;
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
        throw new Exception($"Server error: {ex.Message}");
      }
    }

    public async Task<Data_Response<AccountDto>> UpdateAccount(int accountId, AccountDto acc)
    {
      try
      {
        if (!IsValidEmail(acc.Email))
        {
          return new Data_Response<AccountDto>(400, "Invalid Email format");
        }

        var query = "SELECT * FROM ACCOUNT WHERE AccountId = @accountId";

        var account = await _context.Accounts
          .FromSqlRaw(query, new SqlParameter("@accountId", accountId))
          .FirstOrDefaultAsync();

        if (account is null)
        {
          return new Data_Response<AccountDto>(404, "Account not found");
        }

        var queryBuilder = new StringBuilder("UPDATE Account SET ");
        var parameters = new List<SqlParameter>();

        if (!string.IsNullOrEmpty(acc.Email))
        {
          queryBuilder.Append("Email = @Email, ");
          parameters.Add(new SqlParameter("@Email", acc.Email));
        }

        if (acc.RoleId != 0)
        {
          queryBuilder.Append("RoleId = @RoleId, ");
          parameters.Add(new SqlParameter("@RoleId", acc.RoleId));
        }

        if (acc.SchoolId != 0)
        {
          queryBuilder.Append("SchoolId = @SchoolId, ");
          parameters.Add(new SqlParameter("@SchoolId", acc.SchoolId));
        }

        // Remove the last comma and space
        if (queryBuilder.Length > 0)
        {
          queryBuilder.Length -= 2; // Remove the trailing comma and space
        }

        queryBuilder.Append(" WHERE AccountId = @accountId");
        parameters.Add(new SqlParameter("@accountId", accountId));

        // Execute the update query
        var updateQuery = queryBuilder.ToString();
        await _context.Database.ExecuteSqlRawAsync(updateQuery, parameters.ToArray());

        return new Data_Response<AccountDto>(200, "Updated");
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
                    Password = reader.GetValue(4)?.ToString() ?? string.Empty
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
                  }

                  // Hash the password
                  byte[] passwordHash, passwordSalt;
                  _auth.GenerateHash(accountDto.Password, out passwordHash, out passwordSalt);

                  var newAccount = new Account
                  {
                    RoleId = accountDto.RoleId,
                    SchoolId = accountDto.SchoolId,
                    Email = accountDto.Email,
                    Password = passwordHash,
                    PasswordSalt = passwordSalt
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
