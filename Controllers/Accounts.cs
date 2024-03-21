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

        public ActionResult<List<Account>> GetAccounts()
        {
            try
            {
                var accountList = CSVService.ReadFromCsv();
                return Ok(accountList);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpGet("account/{id}")]

        public ActionResult<Account> GetAccountById([FromRoute] int id)
        {
            try
            {
                var account = CSVService.GetAccountById(id);

                if (account.Id == -1)
                {
                    return BadRequest($"Account with ID: {id} not found.");
                }

                return Ok(account);
            }
            catch (Exception)
            {
                return NotFound("File not found");
            }
        }

        [HttpPost]
        public ActionResult<Account> CreateAccount([FromBody] Account account)
        {
            var rnd = new Random();
            account.Number = rnd.Next(100, 99999);
            int id = 0;
            var allAccounts = CSVService.ReadFromCsv();

            if (allAccounts.Count == 0)
            {
                id = 1;
            }
            else
            {
                var lastAccount = allAccounts.LastOrDefault();
                id = lastAccount.Id;
                id++;
            }

            account.Id = id;

            var listAccounts = new List<Account>
            {
                account
            };

            try
            {
                CSVService.WriteToCsv(listAccounts);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(account);
        }

        [HttpDelete("delete/{id}")]
        public ActionResult DeleteAccountById([FromRoute]int id) 
        { 
            CSVService.DeleteAccount(id);
            return Ok();
        }
    }

}
