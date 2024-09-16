using server.Dtos;
using server.Models;

namespace server.IService
{
  public interface IAccount
  {
    Task<List<AccountDto>> GetAccounts();
    Task<AccountResponse<AccountDto>> GetAccount(int accountId);
    Task<AccountResponse<AccountDto>> AddAccount(RegisterDto acc);
    Task<AccountResponse<AccountDto>> UpdateAccount(int accountId, AccountDto acc);
    Task<AccountResponse<AccountDto>> DeleteAccount(int id);
  }
}
