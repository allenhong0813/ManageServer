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
            ViewData["MachineList"] = _context.Machines.ToList();
            return View();
        }

        [HttpGet]
        public IActionResult GetAdminInfo(string userIDName, string isAdminName)
        {



            try
            {
                var _user = _context.Users.Select(
                    u => new UserMachineViewModel
                    {
                        UserID = u.UserID,
                        IsAdmin = u.IsAdmin,
                        AssignMachineKeys = u.UserMachines.Select(t => t.Machine.Key).ToList()
                    });

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

        [HttpPut]
        public IActionResult UpdateUserMachineData(bool isAdmin, string userID, string[] machineKeys)
        {
            try
            {
                UserMachine userMachine = new UserMachine();
                User users = new Models.User();

                if (isAdmin == true)
                {
                    var _user = _context.Users.SingleOrDefault(m => m.UserID == userID);
                    if (_user != null)//update
                    {
                        _user.IsAdmin = true;
                    }
                    else//insert
                    {
                        users.UserID = userID;
                        users.IsAdmin = isAdmin;
                    }
                    _context.SaveChanges();
                }
                else
                {
                    var _user = _context.Users.SingleOrDefault(m => m.UserID == userID);
                    if (_user != null)//update
                    {
                        _user.IsAdmin = false;
                    }

                    var _userMachine = _context.UserMachines.Where(m => m.UserID.Contains(userID)).ToList();
                    if (_userMachine != null)
                    {
                        _context.UserMachines.RemoveRange(_userMachine);
                        
                    }
                    foreach (string machineKey in machineKeys)
                    {
                        userMachine.UserID = userID;
                        userMachine.MachineKey = machineKey;
                        _context.UserMachines.Add(userMachine);
                        
                    }
                    _context.SaveChanges();
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Delete Error.");
            }
        }


    }
}



