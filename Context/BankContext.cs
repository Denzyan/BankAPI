using BankApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BankApi.Context
{
    public class BankContext : DbContext
    {
        public DbSet<Account> Accounts { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=mydatabase.db"); 
        }
    }
}
