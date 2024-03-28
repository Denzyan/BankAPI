using BankApi.Models;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using BankApi.Controllers;

namespace BankApi.CSVHelperService
{
    public static class CsvService<T> where T : EntityBase, new()
    {
        public static void WriteToCsv(List<T> listToWrite, string fileName)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = !File.Exists(fileName)
            };

            using (var stream = File.Open(fileName, FileMode.Append))
            using (var writer = new StreamWriter(stream))
            using (var csv = new CsvWriter(writer, config))
            {
                csv.WriteRecords(listToWrite);
            }
        }

        public static void OverwriteToCsv(List<T> entities, string fileName)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
            };

            using (var stream = File.Open(fileName, FileMode.Create))
            using (var writer = new StreamWriter(stream))
            using (var csv = new CsvWriter(writer, config))
            {
                csv.WriteRecords(entities);
            }
        }

        public static List<T> ReadFromCsv(string fileName) 
        {
            if (!File.Exists(fileName))
            {
                return new List<T>();
            }

            using (var reader = new StreamReader(fileName))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                return csv.GetRecords<T>().ToList();
            }
        }

        public static T GetEntityById(int id, string fileName)
        {
            var list = ReadFromCsv(fileName);

            foreach (var entity in list) 
            {
                if (entity.Id == id)
                {
                    return entity;
                }
            }

            return new T() { Id = -1 };
        }

        public static void DeleteEntity(int id, string fileName
            )
        {
            var list = ReadFromCsv(fileName);

            var entityToDelete = list.FirstOrDefault(acc => acc.Id == id);

            list.Remove(entityToDelete);

            OverwriteToCsv(list, fileName);
        }

        public static void UpdateEntityInformation(T entityToUpdate, string fileName)
        {
            var list = ReadFromCsv(fileName);

            int entityIndexToReplace = list.FindIndex(ent => ent.Id == entityToUpdate.Id);
            
            list.RemoveAt(entityIndexToReplace);

            list.Insert(entityIndexToReplace, entityToUpdate);

            OverwriteToCsv(list, fileName);    
        } 






    }
    
}
