﻿using server.Dtos;
using server.Types;

namespace server.IService
{
  public interface IAccount
  {
    Task<List<AccountDto>> GetAccounts(int pageNumber, int pageSize);

    Task<List<AccountDto>> GetAccountsByRole(int pageNumber, int pageSize, int roleId);

    Task<Data_Response<AccountResType>> GetAccount(int id);

    Task<Data_Response<AccountDto>> AddAccount(RegisterDto acc);

    Task<Data_Response<AccountDto>> UpdateAccount(int accountId, AccountDto acc);

    Task<Data_Response<AccountDto>> DeleteAccount(int id);

    Task<string> ImportExcel(IFormFile file);

    Task<Data_Response<string>> BulkDelete(List<int> ids);
  }
}
