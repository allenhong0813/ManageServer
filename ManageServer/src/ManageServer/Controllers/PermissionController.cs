using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ManageServer.Models;
using LinqKit;

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
        public IActionResult GetAdminInfo(string userID, string isAdmin)
        {
            var predicate = PredicateBuilder.New<User>(true);
            try
            {
                // 如果有輸入使用者ID作為搜尋條件時
                if (!string.IsNullOrWhiteSpace(userID))
                {
                    predicate = predicate.And(u => u.UserID.Contains(userID));
                    //_user = _user.Where(m => m.UserID.Contains(userID));
                }

                // 如果有輸入是否是管理者作為搜尋條件時
                if (!string.IsNullOrWhiteSpace(isAdmin))
                {
                    predicate = predicate.And(u => u.IsAdmin.Equals(Convert.ToBoolean(isAdmin)));
                    //_user = _user.Where(m => m.IsAdmin.Equals(Convert.ToBoolean(isAdmin)));
                }

                var _user = _context.Users.AsExpandable().Where(predicate).Select(
                    u => new UserMachineViewModel
                    {
                        UserID = u.UserID,
                        IsAdmin = u.IsAdmin,
                        AssignMachineKeys = u.UserMachines.Select(t => t.Machine.Key).ToList()
                    }).OrderBy(u=>u.UserID);

                return Ok(_user.ToList());
            }
            catch (Exception ex)
            {
                return StatusCode(500, "GetServerInf Error.");
            }


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
                        var _userMachine = _context.UserMachines.Where(m => m.UserID.Contains(userID)).ToList();
                        if (_userMachine != null)
                        {
                            _context.UserMachines.RemoveRange(_userMachine);

                        }
                    }
                    _context.SaveChanges();
                }
                else
                {
                    var _user = _context.Users.SingleOrDefault(m => m.UserID == userID);
                    if (_user != null)//update
                    {
                        _user.IsAdmin = false;
                        _context.SaveChanges();
                    }

                    var _userMachine = _context.UserMachines.Where(m => m.UserID.Contains(userID)).ToList();
                    if (_userMachine != null)
                    {
                        _context.UserMachines.RemoveRange(_userMachine);
                        _context.SaveChanges();

                    }

                    foreach (string machineKey in machineKeys)
                    {
                        userMachine.UserID = userID;
                        userMachine.MachineKey = machineKey;
                        _context.UserMachines.Add(userMachine);
                        _context.SaveChanges();
                    }

                }
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Update Error.");
            }
        }


    }
}



