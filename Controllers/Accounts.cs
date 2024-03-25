using BankApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BankApi.CSVHelperService;
using BankApi.Enums;
using BankApi.IdService;

namespace BankApi.Controllers
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

            account.Id = IdHelper.GetNextId();

            var listAccounts = new List<Account>()
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

            accountToDeposit.Transactions.Add(
                new Transaction
                {
                    Amount = depositRequest.DepositAmount,
                    Date = DateTime.Now,
                    TrasactionType = TransactionType.Deposit,
                    Account = accountToDeposit.Number,
                    OldBalance = accountToDeposit.Balance - depositRequest.DepositAmount,
                    NewBalance = accountToDeposit.Balance
                });

            CsvService.UpdateAccountInformation(accountToDeposit);


            return Ok(accountToDeposit);
        }

        [HttpPost("{id}/withdraw")]

        public ActionResult<Account> WithdrawToAccount(
            [FromRoute] int id,
            [FromBody] WithdrawRequest withdrawRequest)
        {
            var accountToWithdraw= CsvService.GetAccountById(id);

            accountToWithdraw.Balance -= withdrawRequest.WithdrawAmount;

            accountToWithdraw.Transactions.Add(
               new Transaction
               {
                   Amount = withdrawRequest.WithdrawAmount,
                   Date = DateTime.Now,
                   TrasactionType = TransactionType.Withdraw,
                   Account = accountToWithdraw.Number,
                   OldBalance = accountToWithdraw.Balance + withdrawRequest.WithdrawAmount,
                   NewBalance = accountToWithdraw.Balance
               });

            CsvService.UpdateAccountInformation(accountToWithdraw);

            return Ok(accountToWithdraw);
        }

        [HttpDelete("{id}/delete")]
        public ActionResult DeleteAccountById([FromRoute]int id) 
        { 
            CsvService.DeleteAccount(id);
            return Ok();
        }

        [HttpPut("{id}/update")]
        public ActionResult UpdateName(
            [FromRoute] int id,
            [FromBody] string name)
        {
            var accountToUpdate = CsvService.GetAccountById(id);

            accountToUpdate.Owner = name;

            CsvService.UpdateAccountInformation(accountToUpdate);

            return Ok(accountToUpdate);
        }
    }

}
