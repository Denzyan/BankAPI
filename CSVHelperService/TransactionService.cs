using BankApi.Models;
using CsvHelper.Configuration;
using CsvHelper;
using System.Globalization;

namespace BankApi.CSVHelperService
{
    public static class TransactionService
    {
        public static List<Transaction> GetTransactionsById(int id, string fileName)
        {
            var allTransactions = CsvService<Transaction>.ReadFromCsv(fileName);
            var transactions = new List<Transaction>();

            foreach (var transaction in allTransactions)
            {
                if (transaction.AccountId == id)
                {
                    transactions.Add(transaction);
                }
            }

            return transactions;
        }

    }
}
