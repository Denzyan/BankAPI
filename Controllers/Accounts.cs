using BankApi.Models;
using Microsoft.AspNetCore.Mvc;
using BankApi.CsvHelperService;
using BankApi.Enums;
using BankApi.IdService;
using System.Net.NetworkInformation;
using Microsoft.VisualBasic;
using BankApi.Requests;

namespace BankApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class Accounts : ControllerBase
    {
        private readonly CsvService<Account> _csvAccountService;
        private readonly CsvService<Transaction> _csvTransactionService;
        private readonly ILogger<Accounts> _logger;

        // DB Sqlite
        private readonly IAccountsService _accountsService;

        public Accounts(
            CsvService<Account> csvAccountService,
            CsvService<Transaction> csvTransactionService,
            ILogger<Accounts> logger,
            IAccountsService accountsService)
        {
            _csvAccountService = csvAccountService;
            _csvTransactionService = csvTransactionService;
            _logger = logger;
            _accountsService = accountsService;
        }

        private const string _accountFileName = "accounts.csv";
        private const string _transactionFileName = "transactions.csv";
        private const string _accountIdFileName = "id.txt";
        private const string _transactionIdFileName = "t_id.txt";

        [HttpGet]

        public ActionResult<List<Account>> GetAccounts()
        {
            _logger.LogWarning("Getting all accounts");

            try
            {
                var accountList = csvAccountService.ReadFromCsv(_accountFileName);

                foreach (var account in accountList) 
                {
                    account.Transactions = TransactionService.GetTransactionsById
                        (account.Id, _transactionFileName);
                }

                _logger.LogError("Successfully got all accounts.");
                return Ok(accountList);
            }
            catch (Exception ex)
            {
                _logger.LogError("ERROR");
                return BadRequest(ex.Message);
            }

        }

        [HttpGet("{id}")]

        public ActionResult<Account> GetAccountById([FromRoute] int id)
        {
            try
            {
                var account = csvAccountService.GetEntityById(id, _accountFileName);

                if (account.Id == -1)
                {
                    return BadRequest($"Account with ID: {id} not found.");
                }

                account.Transactions = TransactionService.GetTransactionsById
                    (account.Id, _transactionFileName);

                return Ok(account);
            }
            catch (Exception ex)
            {
                return NotFound("File not found");
            }
        }

        [HttpPost]
        public ActionResult<Account> CreateAccount([FromBody] CreateAccountRequest accountRequest)
        {
            var rnd = new Random();
            var account = new Account();

            account.Number = rnd.Next(100, 99999);
            account.Owner = accountRequest.Owner;
            account.Id = IdHelper.GetNextId(_accountIdFileName);

            var listAccounts = new List<Account>()
            {
                account
            };

            try
            {
                csvAccountService.WriteToCsv(listAccounts, _accountFileName);
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
            [FromBody]DepositRequest depositRequest)
        {
            var accountToDeposit = csvAccountService.GetEntityById(id, _accountFileName);

            accountToDeposit.Balance += depositRequest.Amount;

            if (accountToDeposit.Id == -1)
            {
                return BadRequest($"Account with ID: {id} not found.");
            }

            accountToDeposit.Transactions = TransactionService.GetTransactionsById
                (accountToDeposit.Id, _transactionFileName);

            var transaction = new Transaction
            {
                Id = IdHelper.GetNextId(_transactionIdFileName),
                Amount = depositRequest.Amount,
                Date = DateTime.Now,
                TrasactionType = TransactionType.Deposit,
                AccountId = accountToDeposit.Id,
                OldBalance = accountToDeposit.Balance - depositRequest.Amount,
                NewBalance = accountToDeposit.Balance
            };

            accountToDeposit.Transactions.Add(transaction);

            csvAccountService.UpdateEntityInformation(accountToDeposit, _accountFileName);
            csvTransactionService.WriteToCsv(new List<Transaction>() 
            { transaction }, _transactionFileName);


            return Ok(accountToDeposit);
        }

        [HttpPost("{id}/withdraw")]

        public ActionResult<Account> WithdrawToAccount(
            [FromRoute] int id,
            [FromBody] WithdrawRequest withdrawRequest)
        {
            var accountToWithdraw= CsvService<Account>.GetEntityById(id, _accountFileName);

            accountToWithdraw.Balance -= withdrawRequest.Amount;

            if (accountToWithdraw.Id == -1)
            {
                return BadRequest($"Account with ID: {id} not found.");
            }

            accountToWithdraw.Transactions = TransactionService.GetTransactionsById
                (accountToWithdraw.Id, _transactionFileName);

            var transaction = new Transaction
            {
                Id = IdHelper.GetNextId(_transactionIdFileName),
                Amount = withdrawRequest.Amount,
                Date = DateTime.Now,
                TrasactionType = TransactionType.Withdraw,
                AccountId = accountToWithdraw.Id,
                OldBalance = accountToWithdraw.Balance + withdrawRequest.Amount,
                NewBalance = accountToWithdraw.Balance
            };

            accountToWithdraw.Transactions.Add(transaction);

            CsvService<Account>.UpdateEntityInformation(accountToWithdraw, _accountFileName);
            CsvService<Transaction>.WriteToCsv(new List<Transaction>()
            { transaction }, _transactionFileName);

            return Ok(accountToWithdraw);
        }

        [HttpPost("transfer")]

        public ActionResult Transfer(TransferRequest request)
        {
            var fromAccount = csvAccountService.GetEntityById(request.FromId, _accountFileName);
            var toAccount = csvAccountService.GetEntityById(request.ToId, _accountFileName);

            if (fromAccount.Id == -1 || toAccount.Id == -1)
            {
                return BadRequest("One of the accounts not found.");
            }

            fromAccount.Balance -= request.Amount;
            toAccount.Balance += request.Amount;

            fromAccount.Transactions = TransactionService.GetTransactionsById
                (fromAccount.Id, _transactionFileName);
            toAccount.Transactions = TransactionService.GetTransactionsById
                (toAccount.Id, _transactionFileName);

            var transactionFrom = new Transaction
            {
                Id = IdHelper.GetNextId(_transactionIdFileName),
                Amount = request.Amount,
                Date = DateTime.Now,
                TrasactionType = TransactionType.Transfer,
                AccountId = fromAccount.Id,
                OldBalance = fromAccount.Balance + request.Amount,
                NewBalance = fromAccount.Balance
            };

            var transationTo = new Transaction
            {
                Id = IdHelper.GetNextId(_transactionIdFileName),
                Amount = request.Amount,
                Date = DateTime.Now,
                TrasactionType = TransactionType.Transfer,
                AccountId = toAccount.Id,
                OldBalance = toAccount.Balance - request.Amount,
                NewBalance = toAccount.Balance
            };

            fromAccount.Transactions.Add(transactionFrom);
            toAccount.Transactions.Add(transationTo);

            csvAccountService.UpdateEntityInformation(toAccount, _accountFileName);
            csvTransactionService.WriteToCsv(new List<Transaction>() 
            { transationTo }, _transactionFileName);

            csvAccountService.UpdateEntityInformation(fromAccount, _accountFileName);
            csvTransactionService.WriteToCsv(new List<Transaction>() 
            { transactionFrom }, _transactionFileName);

            return Ok(fromAccount);
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteAccountById([FromRoute]int id) 
        {
            csvAccountService.DeleteEntity(id, _accountFileName);
            return Ok();
        }

        [HttpPut("{id}")]
        public ActionResult UpdateOwnerName(
            [FromRoute] int id,
            [FromBody] UpdateOwnerNameRequest updateRequest)
        {
            var accountToUpdate = csvAccountService.GetEntityById(id, _accountFileName);

            if (accountToUpdate.Id == -1)
            {
                return BadRequest($"Account with ID: {id} not found.");
            }

            accountToUpdate.Owner = updateRequest.Owner;

            csvAccountService.UpdateEntityInformation(accountToUpdate, _accountFileName);

            return Accepted(accountToUpdate);
        }

        [HttpGet("ping")]

        public ActionResult Ping()
        {
            
            return Ok();
        }
    }

}
