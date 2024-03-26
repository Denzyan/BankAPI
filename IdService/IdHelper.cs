namespace BankApi.IdService
{
    public static class IdHelper
    {
        public static int GetNextId()
        {
            var newId = 0;

            if (File.Exists("id.txt"))
            {
                string id = File.ReadAllText("id.txt");

                if (int.TryParse(id, out int result))
                {
                    result++;

                    File.WriteAllText("id.txt", result.ToString());
                    newId = result;
                }
            }
            else
            {
                File.WriteAllText("id.txt", "1");
                newId = 1;
            }

            return newId;
        }

        public static int GetNextTransactionId()
        {
            var newId = 0;

            if (File.Exists("t_id.txt"))
            {
                string id = File.ReadAllText("t_id.txt");

                if (int.TryParse(id, out int result))
                {
                    result++;

                    File.WriteAllText("t_id.txt", result.ToString());
                    newId = result;
                }
            }
            else
            {
                File.WriteAllText("t_id.txt", "1");
                newId = 1;
            }

            return newId;
        }
    }
}
