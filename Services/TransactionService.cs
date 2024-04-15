using BankApiService.Context;
using BankApiService.Models;

namespace BankApiService.Services
{
    public interface ITransactionService
    { 
        public void AddTransaction(Transaction transaction);
    }

    public class TransactionService : ITransactionService
    {
        private readonly BankContext _context;

        public TransactionService(BankContext context)
        {
            _context = context;
        }

        public void AddTransaction(Transaction transaction)
        {
            _context.Transactions.Add(transaction);
            _context.SaveChanges();
        }
    }
}