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
        public AccountController(PostgresSQLContext context, IAuthenticationService authService)
        {
            _context = context;
            _authService = authService;
        }

        // GET: /<controller>/   
        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Forbidden()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            try
            {
                var username = model.Username;
                var password = model.Password;

                if (ModelState.IsValid)
                {
                    var user = _authService.LdapLogin(username, password);
                    if (user.IsSuccess)
                    {
                        SetClaim(username,password);                        
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        switch (user.ResultCode)
                        {
                            case 49:
                                ModelState.AddModelError(string.Empty, "您輸入的帳號或密碼錯誤，請重新輸入！");
                                break;
                        }
                    }
                }
                return View(model);
            }
            catch (Exception ex)
            {
                return View(model);
            }
        }

        public async void SetClaim(string username, string password)
        {
            var _user = _context.Users.Where(u => u.UserID == username).SingleOrDefault();
            var claims = new List<Claim> {
                            new Claim("Username",_user.UserID),
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
                    ExpiresUtc = DateTime.UtcNow.AddDays(0.5)
                }
                );
        }
        

        public async Task<IActionResult> Logout()
        {
            await HttpContext.Authentication
                .SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Login", "Account");
        }
    }
}
