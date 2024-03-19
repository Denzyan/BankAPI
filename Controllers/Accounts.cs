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

        public ActionResult <List<Account>> GetAccounts()
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

        [HttpGet("{id}")]

        public ActionResult<Account> GetAccountById([FromRoute] int id)
        {
            try 
            {
                var account = CSVService.GetAccountById(id);

                if (account.Id == -1)
                {
                    return BadRequest($"Account with ID: {id} not found");
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

    }

}
