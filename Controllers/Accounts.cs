using BankAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BankAPI.CSVHelperService;

namespace BankAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class Accounts : ControllerBase
    {
        [HttpGet]

        public List<Account> GetAccounts()
        {

            return new List<Account>
            {
                new Account {Id = 1, Number = 123, Balance = 1000},
                new Account {Id = 2, Number = 456, Balance = 2000}
            };

        }

        [HttpPost]
        public string CreateAccount(Account account)
        {
            var rnd = new Random();
            account.Number = rnd.Next(100, 99999);

            var listAccounts = new List<Account>
            {
                account
            };

            CSVService.WriteToCsv(listAccounts);

            return "Account created";
        }

    }

}
