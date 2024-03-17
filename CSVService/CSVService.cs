using BankAPI.Models;
using CsvHelper;
using System.Globalization;

namespace BankAPI.CSVService
{
    public static class CSVService
    {
        public static void WriteToCsv(List<Account> listToWrite)
        {
            const string _file = "accounts.csv";
            using (var writer = new StringWriter(_file)) ;
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(listToWrite);
            }
            
        }

    }
}
