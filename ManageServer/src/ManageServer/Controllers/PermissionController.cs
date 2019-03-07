using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ManageServer.Models;
using LinqKit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace ManageServer.Controllers
{
    [Authorize(Policy = "Admin")]
    public class PermissionController : BaseController
    {
        private readonly ILogger<PermissionController> _logger;

        private readonly PostgresSQLContext _context;

        public PermissionController(PostgresSQLContext context, ILogger<PermissionController> logger) : base(context, logger)
        {
            _context = context;
            _logger = logger;
        }
        // GET: /<controller>/
        public IActionResult Permission()
        {
            try
            {
                ViewData["MachineList"] = _context.Machines.ToList();
            }
            catch (Exception ex)
            {
                return ExceptionHandler(ex, "Constructor Error.");
            }

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
                }

                // 如果有輸入是否是管理者作為搜尋條件時
                if (!string.IsNullOrWhiteSpace(isAdmin))
                {
                    predicate = predicate.And(u => u.IsAdmin.Equals(Convert.ToBoolean(isAdmin)));
                }

                var _user = _context.Users.AsExpandable().Where(predicate).Select(
                    u => new UserMachineViewModel
                    {
                        UserID = u.UserID,
                        IsAdmin = u.IsAdmin,
                        AssignMachineKeys = u.UserMachines.Select(t => t.Machine.Key).ToList()

                    }).OrderBy(u => u.UserID);

                return Ok(_user.ToList());
            }
            catch (Exception ex)
            {
                return ExceptionHandler(ex, "GetServerInfo Error.");
            }
        }

        [HttpPut]
        public IActionResult UpdateUserMachineData(bool isAdmin, string userID, string[] machineKeys)
        {
            try
            {
                var _user = _context.Users.SingleOrDefault(m => m.UserID == userID);
                if (_user != null)//update
                {
                    _user.IsAdmin = isAdmin;
                    var _userMachine = _context.UserMachines.Where(m => m.UserID.Contains(userID)).ToList();
                    if (_userMachine != null)
                    {
                        _context.UserMachines.RemoveRange(_userMachine);
                    }
                }
                _context.SaveChanges();
                foreach (string machineKey in machineKeys)
                {
                    UserMachine userMachine = new UserMachine();
                    userMachine.UserID = userID;
                    userMachine.MachineKey = machineKey;
                    _context.UserMachines.Add(userMachine);
                    
                }
                _context.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                return ExceptionHandler(ex, "Update Error.");
            }
        }
    }
}



