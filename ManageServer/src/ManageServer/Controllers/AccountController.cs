using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.Authentication;
using ManageServer.Models;


// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace ManageServer.Controllers
{
    public class AccountController : Controller
    {

        private readonly IAuthenticationService _authService;
        private readonly PostgresSQLContext _context;
        public AccountController(PostgresSQLContext context,IAuthenticationService authService)
        {
            _context = context;
            _authService = authService;
        }

        // GET: /<controller>/   
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string username, string password)
        {
            try
            {
                var user = _authService.Login(username, password);
                
                if (user)
                {

                    var _user = _context.Users.Where(u => u.UserID == username).Select(
                       u => new LoginViewModel
                       {
                           Username = u.UserID,
                           IsAdmin = u.IsAdmin
                       }).SingleOrDefault();

                    //取名Username，在登入後的頁面，讀取登入者的帳號會用得到，自己先記在大腦
                    var claims = new List<Claim> {
                        new Claim("Username",_user.Username),
                        new Claim("IsAdmin", _user.IsAdmin.ToString())
                    }; 
                    ClaimsIdentity claimsIdentity = new ClaimsIdentity(
                        claims, 
                        CookieAuthenticationDefaults.AuthenticationScheme);//Scheme必填
                    ClaimsPrincipal principal = new ClaimsPrincipal(claimsIdentity);

                    await HttpContext.Authentication.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme, 
                        new ClaimsPrincipal(claimsIdentity),
                        new AuthenticationProperties()
                        {
                            IsPersistent = false,
                            ExpiresUtc = DateTime.UtcNow.AddMinutes(20)
                        }
                        );


                    return RedirectToAction("Index", "Home");
                }else
                {
                    return StatusCode(500, "Login Error.");
                }
                
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Login Error.");
            }


        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.Authentication.SignOutAsync("MyCookieMiddlewareInstance");

            return RedirectToAction("Account","Login");
        }
    }
}
