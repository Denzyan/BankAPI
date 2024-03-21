using BankAPI.Models;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;

namespace BankAPI.CSVHelperService
{
    public static class CSVService
    {
        public static void WriteToCsv(List<Account> listToWrite)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = !File.Exists("Accounts.csv")
            };

            using (var stream = File.Open("Accounts.csv", FileMode.Append))
            using (var writer = new StreamWriter(stream))
            using (var csv = new CsvWriter(writer, config))
            {
                csv.WriteRecords(listToWrite);
            }
        }

        public static void RewriteToCsv(List<Account> listToWrite)
        {
            using (var writer = new StreamWriter("Accounts.csv"))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(listToWrite);
            }
        }

        public static List<Account> ReadFromCsv() 
        {
            if (!File.Exists("Accounts.csv"))
            {
                return new List<Account>();
            }

            using (var reader = new StreamReader("Accounts.csv"))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                return csv.GetRecords<Account>().ToList();
            }

        }

        public static Account GetAccountById(int id)
        {
            var allAccounts = ReadFromCsv();

            foreach (var account in allAccounts) 
            {
                if (account.Id == id)
                {
                    return account;
                }
            }

            return new Account() { Id = -1 };
        }

        public static void DeleteAccount(int id)
        {
            var allAccounts = ReadFromCsv();

            var accountDelete = allAccounts.FirstOrDefault(acc => acc.Id == id);

            allAccounts.Remove(accountDelete);

            RewriteToCsv(allAccounts);
        }








    }
    
}
