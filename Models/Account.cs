namespace BankApi.Models
{
    public class Account : EntityBase
    {
        public DateTime OpenDate { get; set; } = DateTime.Now;
        public int Number {  get; set; }
        public string Owner { get; set; }
        public int Balance { get; set; }

        // Navigation property
        public List<Transaction> Transactions { get; set; } = new List<Transaction>();

    }
}
