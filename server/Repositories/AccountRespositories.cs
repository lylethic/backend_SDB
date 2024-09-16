using AutoMapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using server.Data;
using server.Dtos;
using server.IService;
using server.Models;
using System.Text;
using System.Text.RegularExpressions;

namespace server.Repositories
{
  public class AccountRespositories : IAccount
  {
    private readonly SoDauBaiContext _context;
    private readonly IAuth _auth;
    private readonly IMapper _map;

    public AccountRespositories(SoDauBaiContext context, IMapper map, IAuth auth)
    {
      _context = context;
      _map = map;
      _auth = auth;
    }

    public async Task<AccountResponse<AccountDto>> AddAccount(RegisterDto acc)
    {
      try
      {
        if (!IsValidEmail(acc.Email))
        {
          return new AccountResponse<AccountDto>(400, "Invalid Email format");
        }

        var query = "SELECT * FROM ACCOUNT WHERE Email = @email";

        var account = await _context.Accounts
            .FromSqlRaw(query, new SqlParameter("@email", acc.Email))
            .FirstOrDefaultAsync();

        if (account != null)
        {
          return new AccountResponse<AccountDto>(409, "Email already registered");
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

        return new AccountResponse<AccountDto>(200, accountDto);
      }
      catch (Exception ex)
      {
        return new AccountResponse<AccountDto>(500, "Server error");
      }
    }

    public async Task<AccountResponse<AccountDto>> GetAccount(int accountId)
    {
      try
      {
        var query = "SELECT * FROM ACCOUNT WHERE AccountId = @accountId";

        var acc = await _context.Accounts
            .FromSqlRaw(query, new SqlParameter("@accountId", accountId))
            .FirstOrDefaultAsync();

        if (acc == null)
        {
          return new AccountResponse<AccountDto>(404, "Account not founnd");
        }

        var result = new AccountDto
        {
          AccountId = acc.AccountId,
          RoleId = acc.RoleId,
          SchoolId = acc.SchoolId,
          Email = acc.Email
        };

        return new AccountResponse<AccountDto>(200, result);
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
        return new AccountResponse<AccountDto>(500, "Server error");
      }
    }

    public async Task<List<AccountDto>> GetAccounts()
    {
      try
      {
        var query = "SELECT * FROM ACCOUNT";

        var accsList = await _context.Accounts.FromSqlRaw(query).ToListAsync();

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
        throw;
      }
    }

    public async Task<AccountResponse<AccountDto>> UpdateAccount(int accountId, AccountDto acc)
    {
      try
      {
        if (!IsValidEmail(acc.Email))
        {
          return new AccountResponse<AccountDto>(400, "Invalid Email format");
        }

        var query = "SELECT * FROM ACCOUNT WHERE AccountId = @accountId";

        var account = await _context.Accounts
          .FromSqlRaw(query, new SqlParameter("@accountId", accountId))
          .FirstOrDefaultAsync();

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

        return new AccountResponse<AccountDto>(200, "Updated");
      }
      catch (Exception ex)
      {
        return new AccountResponse<AccountDto>(500, "Server Error");
      }
    }

    public async Task<AccountResponse<AccountDto>> DeleteAccount(int accountId)
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
          return new AccountResponse<AccountDto>(404, "Account not found!");
        }

        // Delete query
        var deleteQuery = "DELETE FROM Account WHERE AccountId = @accountId";
        await _context.Database
          .ExecuteSqlRawAsync(deleteQuery, new SqlParameter("@accountId", accountId));

        return new AccountResponse<AccountDto>(200, "Deleted");
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
        return new AccountResponse<AccountDto>(500, $"Error deleting account: {ex.Message}");
      }
    }

    public bool IsValidEmail(string email)
    {
      var regex = @"^[\w\.-]+@gmail\.com$";
      return Regex.IsMatch(email, regex);
    }
  }
}
