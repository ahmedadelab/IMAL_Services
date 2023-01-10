using Microsoft.AspNetCore.Mvc;

namespace IMAL_Services.Controllers
{
    [ApiController]
    [Route("[controller]")]

    public class ReturnGeneralAccount : Controller
    {
      
        DLL_GetGeneralAccount DLL_Code = new DLL_GetGeneralAccount();
        [HttpPost(Name = "CS_AccountDetails")]
       
        public ActionResult <string> OnGet([FromBody] CS_GetGeneralAccount x)
        {
            string accountRef = x.accountRef;
            string username =x.UserName;
            string password = x.PasswordImal;
            // string[] result = (string[])DLL_Code.ReturnGeneralAccount(accountRef,username,password);
            string result = DLL_Code.GetaccountDetails(accountRef, username, password);
            return (result);
         
        }
    }
}
