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
                HasHeaderRecord = false,
            };
            using (var stream = File.Open("Accounts.csv", FileMode.Append))
            using (var writer = new StreamWriter(stream))
            using (var csv = new CsvWriter(writer, config))
            {
                csv.WriteRecords(listToWrite);
            }
        }
    }
    
}
