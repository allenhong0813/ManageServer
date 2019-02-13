using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;


// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace ManageServer.Controllers
{
    public class AccountController : Controller
    {
        // GET: /<controller>/
        //public IActionResult Login()
        //{
        //    return View();
        //}

        private readonly LdapAuthenticationService _authService;
        public AccountController(LdapAuthenticationService authService)
        {
            _authService = authService;
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult Login(string username, string password)
        {
            try
            {
                var user = _authService.Login(username, password);
                if(user)
                {
                    return StatusCode(200, "backend suc!"+username+" "+password);
                }
                return StatusCode(500, "Login Error.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Login Error.");
            }

            
        }
    }
}
