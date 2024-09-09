using server.Models;

namespace server.IService
{
    public interface IAccount
    {
        public Task<List<Account>> GetAccounts();
        public Task<Account> GetAccount(int id);
        public Task<int> AddAccount(Account acc);
        public Task<int> UpdateAccount(int id, Account acc);
        public Task DeleteAccount(int id);
    }
}
