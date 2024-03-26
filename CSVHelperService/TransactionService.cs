using BankApi.Models;
using CsvHelper.Configuration;
using CsvHelper;
using System.Globalization;

namespace BankApi.CSVHelperService
{
    public static class TransactionService
    {
        public static void WriteToCsv(List<Transaction> listToWrite)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = !File.Exists("transaction.csv")
            };

            using (var stream = File.Open("transaction.csv", FileMode.Append))
            using (var writer = new StreamWriter(stream))
            using (var csv = new CsvWriter(writer, config))
            {
                csv.WriteRecords(listToWrite);
            }
        }

        public static List<Transaction> ReadFromCsv()
        {
            if (!File.Exists("transaction.csv"))
            {
                return new List<Transaction>();
            }

            using (var reader = new StreamReader("transaction.csv"))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                return csv.GetRecords<Transaction>().ToList();
            }
        }
    }
}
