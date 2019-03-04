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
using Microsoft.Extensions.Logging;


// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace ManageServer.Controllers
{
    public class AccountController : BaseController
    {

        private readonly IAuthenticationService _authService;
        private readonly PostgresSQLContext _context;
        private readonly ILogger<AccountController> _logger;

        public AccountController(PostgresSQLContext context, IAuthenticationService authService, ILogger<AccountController> logger) : base(context, logger)
        {
            _authService = authService;
            _context = context;
            _logger = logger;
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
        public IActionResult Login(LoginViewModel model)
        {
            try
            {
                var username = model.Username.ToLower();
                var password = model.Password;

                if (ModelState.IsValid)
                {
                    var user = _authService.LdapValid(username,password);
                    //if (user.IsSuccess)
                    if (true)
                    {
                        DBHasUser(username);
                        SetClaim(username, password);
                        return RedirectToAction("index", "Home");
                    }
                    else
                    {
                        switch (user.ResultCode)
                        {
                            case 49:
                                ModelState.AddModelError(string.Empty, "您輸入的帳號或密碼錯誤，請重新輸入！");
                                break;
                            default:
                                ModelState.AddModelError(string.Empty, "Error");
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler(ex, "Login Error.");
            }
            return View(model);
        }

        public void DBHasUser(string username)
        {
            var HasUser = _context.Users.Where(u => u.UserID == username).FirstOrDefault();
            if(HasUser == null)
            {
                User addUser = new User();
                addUser.UserID = username;
                addUser.IsAdmin = false;
                _context.Users.Add(addUser);
                _context.SaveChanges();
            }
            
        }

        public async void SetClaim(string username, string password)
        {
            try
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
            catch (Exception ex)
            {
                ExceptionHandler(ex, "SetClaim Error.");
            }
        }


        public async Task<IActionResult> Logout()
        {
            try
            {
                await HttpContext.Authentication
                .SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            }
            catch (Exception ex)
            {
                ExceptionHandler(ex, "Logout Error.");
            }
            return RedirectToAction("Login", "Account");
        }
    }
}
