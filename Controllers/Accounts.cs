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
                var accountList = CsvService.ReadFromCsv();
                return Ok(accountList);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpGet("{id}/account")]

        public ActionResult<Account> GetAccountById([FromRoute] int id)
        {
            try
            {
                var account = CsvService.GetAccountById(id);

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
            var allAccounts = CsvService.ReadFromCsv();

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
                CsvService.WriteToCsv(listAccounts);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(account);
        }

        [HttpPost("{id}/deposit")]

        public ActionResult<Account> DepositToAccount(
            [FromRoute] int id,
            [FromBody]DepositRequest depositRequest )
        {
            var accountToDeposit = CsvService.GetAccountById(id);

            accountToDeposit.Balance += depositRequest.DepositAmount;

            CsvService.UpdateAccountInformation(accountToDeposit);

            return Ok(accountToDeposit);
        }

        [HttpPost("{id}/withdrow")]

        public ActionResult<Account> WithdrawToAccount(
            [FromRoute] int id,
            [FromBody] DepositRequest withdrawRequest)
        {
            var accountToWithdraw= CsvService.GetAccountById(id);

            accountToWithdraw.Balance -= withdrawRequest.DepositAmount;

            CsvService.UpdateAccountInformation(accountToWithdraw);

            return Ok(accountToWithdraw);
        }



        [HttpDelete("{id}/delete")]
        public ActionResult DeleteAccountById([FromRoute]int id) 
        { 
            CsvService.DeleteAccount(id);
            return Ok();
        }
    }

}
