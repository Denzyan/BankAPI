using BankApi.Enums;
using System.Text.Json.Serialization;

namespace BankApi.Models

{
    public class Transaction : EntityBase
    {
        public DateTime Date { get; set; } = DateTime.Now;
        public int Amount { get; set; }

        // Foreign key
        public int AccountId {  get; set; }
        public int OldBalance { get; set; }
        public int NewBalance { get; set; }
        public TransactionType TrasactionType { get; set; }

        // Navigation property
        [JsonIgnore]    
        public Account Account { get; set; }
    }
}
