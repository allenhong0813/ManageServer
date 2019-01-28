using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ManageServer.Models;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace ManageServer.Controllers
{
    public class PermissionController : Controller
    {
        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }

        private readonly PostgresSQLContext _context;

        public PermissionController(PostgresSQLContext context)
        {
            _context = context;
        }

        public IActionResult Permission()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        [HttpGet]
        public IActionResult GetAdminInfo(string userIDName,string isAdminName)
        {

            try
            {
                var _user = _context.Users.Select(m => m);
                //var Users = _context.Users;
                
                // 如果有輸入使用者ID作為搜尋條件時
                if (!string.IsNullOrWhiteSpace(userIDName))
                {
                    _user = _user.Where(m => m.UserID.Contains(userIDName));
                }

                // 如果有輸入是否是管理者作為搜尋條件時
                if (!string.IsNullOrWhiteSpace(isAdminName))
                {
                    _user = _user.Where(m => m.IsAdmin.Equals(Convert.ToBoolean(isAdminName)));
                }

                return Ok(_user.ToList());
            }
            catch (Exception ex)
            {

            }

            return Ok();
        }

        [HttpGet]
        public IActionResult GetUserPermission(string userPermissionName)
        {

            try
            {
                var _user = _context.Users.Select(m => m);
                

                //List<User> machines = _user.ToList();

                // 如果有輸入使用者ID作為搜尋條件時
                //if (!string.IsNullOrWhiteSpace(userIDName))
                //{
                //    _user = _user.Where(m => m.UserID.Contains(userIDName));
                //}

                

                return Ok(_user.ToList());
            }
            catch (Exception ex)
            {

            }

            return Ok();
        }
    }
}
