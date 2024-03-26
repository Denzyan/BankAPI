using BankApi.Enums;

namespace BankApi.Models

{
    public class Transaction
    {
        public int Id { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public int Amount { get; set; }
        public int AccountId {  get; set; }
        public int OldBalance { get; set; }
        public int NewBalance { get; set; }
        public TransactionType TrasactionType { get; set; }
    }
}
