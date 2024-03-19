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

        public static List<Account> ReadFromCsv() 
        {
            if (File.Exists("Accounts.csv"))
            {
                using (var reader = new StreamReader("Accounts.csv"))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    return csv.GetRecords<Account>().ToList();
                }
            }
            throw new Exception();
           
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







    }
    
}
