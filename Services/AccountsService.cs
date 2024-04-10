using BankApi.Context;
using BankApi.Enums;
using BankApi.Models;

namespace BankApi.Services
{
    public interface IAccountsService
    {
        public void AddAccount(Account account);
        public OperationResult UpdateOwnerName(int id, string ownerName);
        public OperationResult DeleteAccount(int id);
        public Account GetAccountById(int id);
        public List<Account> GetAccounts();
        public void UpdateAccount(Account account);
    }
    public class AccountsService : IAccountsService
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

        public OperationResult UpdateOwnerName(int id, string ownerName) 
        {
            var account = _context.Accounts.FirstOrDefault(x => x.Id == id);
            if (account == null) 
            {
                return OperationResult.Failed;
            }

            account.Owner = ownerName;

            _context.Accounts.Update(account);
            _context.SaveChanges();

            return OperationResult.Success;

        }

        public OperationResult DeleteAccount(int id)
        {
            var account = _context.Accounts.FirstOrDefault(x => x.Id == id);
            if (account == null)
            { 
                return OperationResult.Failed;
            }

            _context.Accounts.Remove(account);
            _context.SaveChanges();
            return OperationResult.Success;
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
            _context.Accounts.Update(account);
            _context.SaveChanges();
        }

    }
}
