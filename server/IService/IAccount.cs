using server.Dtos;
using server.Models;

namespace server.IService
{
  public interface IAccount
  {
    Task<List<AccountDto>> GetAccounts();
    Task<Data_Response<AccountDto>> GetAccount(int accountId);
    Task<Data_Response<AccountDto>> AddAccount(RegisterDto acc);
    Task<Data_Response<AccountDto>> UpdateAccount(int accountId, AccountDto acc);
    Task<Data_Response<AccountDto>> DeleteAccount(int id);
  }
}
