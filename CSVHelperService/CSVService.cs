﻿using BankApi.Models;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using BankApi.Controllers;

namespace BankApi.CSVHelperService
{
    public static class CsvService<T> where T : class
    {
        public static void WriteToCsv(List<T> listToWrite)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = !File.Exists("accounts.csv")
            };

            using (var stream = File.Open("accounts.csv", FileMode.Append))
            using (var writer = new StreamWriter(stream))
            using (var csv = new CsvWriter(writer, config))
            {
                csv.WriteRecords(listToWrite);
            }
        }

        public static void OverwriteToCsv(List<T> accounts)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
            };

            using (var stream = File.Open("accounts.csv", FileMode.Create))
            using (var writer = new StreamWriter(stream))
            using (var csv = new CsvWriter(writer, config))
            {
                csv.WriteRecords(accounts);
            }
        }

        public static List<T> ReadFromCsv() 
        {
            if (!File.Exists("accounts.csv"))
            {
                return new List<T>();
            }

            using (var reader = new StreamReader("accounts.csv"))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                return csv.GetRecords<T>().ToList();
            }

        }

        public static T GetAccountById(int id)
        {
            var allAccounts = ReadFromCsv();

            foreach (var account in allAccounts) 
            {
                if (account.Id == id)
                {
                    return account;
                }
            }

            return new T() { Id = -1 };
        }

        public static void DeleteAccount(int id)
        {
            var allAccounts = ReadFromCsv();

            var accountDelete = allAccounts.FirstOrDefault(acc => acc.Id == id);

            allAccounts.Remove(accountDelete);

            OverwriteToCsv(allAccounts);
        }

        public static void UpdateAccountInformation(Account accountToUpdate)
        {
            var allAccounts = ReadFromCsv();

            int accountIndexToReplace = allAccounts.FindIndex(acc => acc.Id == accountToUpdate.Id);
            
            allAccounts.RemoveAt(accountIndexToReplace);

            allAccounts.Insert(accountIndexToReplace, accountToUpdate);

            OverwriteToCsv(allAccounts);    
        } 








    }
    
}
