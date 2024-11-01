using server.Dtos;
using server.Types;

namespace server.IService
{
  public interface IAccount
  {
    Task<int> GetCountAccounts();

    Task<int> GetCountAccountsBySchool(int schoolId);

    Task<AccountsResType> GetAccounts(int pageNumber, int pageSize);

    Task<List<AccountDto>> GetAccountsByRole(int pageNumber, int pageSize, int roleId);

    Task<ResponseData<AccountResType>> GetAccount(int id);

    Task<ResponseData<AccountData>> GetAccountById(int id);

    Task<AccountsResType> AddAccount(RegisterDto acc);

    Task<ResponseData<AccountDto>> UpdateAccount(int accountId, AccountDto acc);

    Task<ResponseData<AccountsResType>> DeleteAccount(int id);

    Task<string> ImportExcel(IFormFile file);

    Task<ResponseData<string>> BulkDelete(List<int> ids);

    Task<AccountsResType> GetAccountsBySchoolId(int pageNumber, int pageSize, int schoolId);

    Task<List<AccountResType>> RelativeSearchAccounts(string? TeacherName, int? schoolId, int? roleId, int pageNumber, int pageSize);
  }
}
