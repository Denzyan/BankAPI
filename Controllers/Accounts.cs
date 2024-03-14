using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BankAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Accounts : ControllerBase
    {
        [HttpGet]
        public string GetAccount()
        {
            return "Acc N123, Name Scotty";
        }

    }
}
