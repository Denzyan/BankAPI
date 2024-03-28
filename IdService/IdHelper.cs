namespace BankApi.IdService
{
    public static class IdHelper
    {
        public static int GetNextId(string fileName)
        {
            var newId = 0;

            if (File.Exists(fileName))
            {
                string id = File.ReadAllText(fileName);

                if (int.TryParse(id, out int result))
                {
                    result++;

                    File.WriteAllText(fileName, result.ToString());
                    newId = result;
                }
            }
            else
            {
                File.WriteAllText(fileName, "1");
                newId = 1;
            }

            return newId;
        }

        
    }
}
