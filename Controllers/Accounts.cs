using BankApiService.Models;
using Microsoft.AspNetCore.Mvc;
using BankApiService.Enums;
using BankApiService.Requests;
using BankApiService.Services;

namespace BankApiService.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class Accounts : ControllerBase
    {
        private readonly ILogger<Accounts> _logger;

        // DB Sqlite
        private readonly IAccountsService _accountsService;
        private readonly ITransactionService _transactionService;

        public Accounts(
            ILogger<Accounts> logger,
            IAccountsService accountsService,
            ITransactionService transactionService)
        {
            _logger = logger;
            _accountsService = accountsService;
            _transactionService = transactionService;
        }

        /// <summary>
        /// Request for list of all accounts
        /// </summary>
        /// <response code="200">Returns list of accounts</response>
        /// <response code="400">Returns nothing</response>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<List<Account>> GetAccounts()
        {
            _logger.LogWarning("Getting all accounts");

            try
            {
                var accountList = _accountsService.GetAccounts();

                
                return Ok(accountList);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        /// <summary>
        /// Return account with requested id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public ActionResult<Account> GetAccountById([FromRoute] int id)
        {
            
            var account = _accountsService.GetAccountById(id);

            if (account == null)
            {
                return BadRequest($"Account with ID: {id} not found.");
            }

            return Ok(account);
            
        }

        /// <summary>
        /// Create and return account with requested name
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST / account
        ///     {
        ///        "owner": "anyName"
        ///     }
        ///     
        /// </remarks>
        /// <param name="accountRequest"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult<Account> CreateAccount([FromBody] CreateAccountRequest accountRequest)
        {

            Account account;

            try
            {
               account = _accountsService.CreateAccount(accountRequest.Owner);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(account);
        }

        /// <summary>
        /// Create transaction, update balance, return account with new balance
        /// </summary>
        /// <param name="id"></param>
        /// <param name="depositRequest"></param>
        /// <returns></returns>
        [HttpPost("{id}/deposit")]
        public ActionResult<Account> DepositToAccount(
            [FromRoute] int id,
            [FromBody]DepositRequest depositRequest)
        {
            var accountToDeposit = _accountsService.GetAccountById(id);

            if (accountToDeposit == null)
            {
                return BadRequest($"Account with ID: {id} not found.");
            }

            accountToDeposit.Balance += depositRequest.Amount;

            var transaction = new Transaction
            {
                Amount = depositRequest.Amount,
                Date = DateTime.Now,
                TrasactionType = TransactionType.Deposit,
                AccountId = accountToDeposit.Id,
                OldBalance = accountToDeposit.Balance - depositRequest.Amount,
                NewBalance = accountToDeposit.Balance
            };

            _accountsService.UpdateAccount(accountToDeposit);
            _transactionService.AddTransaction(transaction);

            return Ok(accountToDeposit);
        }

        /// <summary>
        /// Create transaction, update balance, return account with new balance
        /// </summary>
        /// <param name="id"></param>
        /// <param name="withdrawRequest"></param>
        /// <returns></returns>
        [HttpPost("{id}/withdraw")]
        public ActionResult<Account> WithdrawToAccount(
            [FromRoute] int id,
            [FromBody] WithdrawRequest withdrawRequest)
        {
            var accountWithdrawFrom= _accountsService.GetAccountById(id);

            if (accountWithdrawFrom == null)
            {
                return BadRequest($"Account with ID: {id} not found.");
            }

            accountWithdrawFrom.Balance -= withdrawRequest.Amount;

            var transaction = new Transaction
            {
                Amount = withdrawRequest.Amount,
                Date = DateTime.Now,
                TrasactionType = TransactionType.Withdraw,
                AccountId = accountWithdrawFrom.Id,
                OldBalance = accountWithdrawFrom.Balance + withdrawRequest.Amount,
                NewBalance = accountWithdrawFrom.Balance
            };

            _accountsService.UpdateAccount(accountWithdrawFrom);
            _transactionService.AddTransaction(transaction);

            return Ok(accountWithdrawFrom);
        }

        /// <summary>
        /// Create transactions, update balances, return accounts with new balance
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("transfer")]
        public ActionResult Transfer(TransferRequest request)
        {
            var fromAccount = _accountsService.GetAccountById(request.FromId);
            var toAccount = _accountsService.GetAccountById(request.ToId);

            if (fromAccount.Id == null || toAccount.Id == null)
            {
                return BadRequest("One of the accounts not found.");
            }

            fromAccount.Balance -= request.Amount;
            toAccount.Balance += request.Amount;

            var transactionFrom = new Transaction
            {
                Amount = request.Amount,
                Date = DateTime.Now,
                TrasactionType = TransactionType.Transfer,
                AccountId = fromAccount.Id,
                OldBalance = fromAccount.Balance + request.Amount,
                NewBalance = fromAccount.Balance
            };

            var transationTo = new Transaction
            {
                Amount = request.Amount,
                Date = DateTime.Now,
                TrasactionType = TransactionType.Transfer,
                AccountId = toAccount.Id,
                OldBalance = toAccount.Balance - request.Amount,
                NewBalance = toAccount.Balance
            };

            _accountsService.UpdateAccount(fromAccount);
            _accountsService.UpdateAccount(toAccount);

            _transactionService.AddTransaction(transactionFrom);
            _transactionService.AddTransaction(transationTo);
            

            return Ok(fromAccount);
        }

        /// <summary>
        /// Deleted account with requested id, return result.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public ActionResult DeleteAccountById([FromRoute]int id) 
        {
            var operationResult = _accountsService.DeleteAccount(id);

            if (operationResult == OperationResult.Failed)
            {
                return BadRequest($"Account with ID: {id} not found.");
            }

            return Ok();
        }

        /// <summary>
        /// Renamed account
        /// </summary>
        /// <param name="id"></param>
        /// <param name="updateRequest"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public ActionResult UpdateOwnerName(
            [FromRoute] int id,
            [FromBody] UpdateOwnerNameRequest updateRequest)
        {
            var operationResult = _accountsService.UpdateOwnerName(id, updateRequest.Owner);

            if (operationResult == OperationResult.Failed)
            {
                return BadRequest($"Account with ID: {id} not found.");
            }

            return Ok();
        }

        /// <summary>
        /// Just for check connect
        /// </summary>
        /// <returns></returns>
        [HttpGet("ping")]
        public ActionResult Ping()
        { 
        return Ok(); 
        }

    }

}