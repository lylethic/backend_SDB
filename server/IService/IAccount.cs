using server.Dtos;

namespace server.IService
{
  public interface IAccount
  {
    Task<List<AccountDto>> GetAccounts(int pageNumber, int pageSize);

    Task<Data_Response<AccountDto>> GetAccount(int accountId);

    Task<Data_Response<AccountDto>> AddAccount(RegisterDto acc);

    Task<Data_Response<AccountDto>> UpdateAccount(int accountId, AccountDto acc);

    Task<Data_Response<AccountDto>> DeleteAccount(int id);

    Task<string> ImportExcel(IFormFile file);

    Task<Data_Response<string>> BulkDelete(List<int> ids);
  }
}
