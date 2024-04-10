using BankApi.Context;
using BankApi.Models;

namespace BankApi.Services
{
    public interface IAccountService
    {
        public void AddAccount(Account account);
        public void UpdateAccount(Account account);
        public void DeleteAccount(int id);
        public Account GetAccountById(int id);
        public List<Account> GetAccounts();
    }
    public class AccountsService : IAccountService
    {
        private readonly BankContext _context;
        public AccountsService(BankContext context) 
        {  
            _context = context;
        }

        public void AddAccount(Account account)
        {
            _context.Accounts.Add(account);
            _context.SaveChanges();
        }

        public void DeleteAccount(int id)
        {
            throw new NotImplementedException();
        }

        public Account GetAccountById(int id)
        {
            var account = _context.Accounts.FirstOrDefault(x => x.Id == id);
            return account;
        }

        public List<Account> GetAccounts()
        {
            var accounts = _context.Accounts.ToList();
            return accounts;
        }

        public void UpdateAccount(Account account)
        {
            throw new NotImplementedException();
        }
    }
}
