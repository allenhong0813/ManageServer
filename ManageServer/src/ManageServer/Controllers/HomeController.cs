using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ManageServer.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using ManageServer;
using System.Net.Sockets;
using System.Security.Claims;
using System.Threading;
using System.Security.Principal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using LinqKit;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace ManageServer.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        private readonly PostgresSQLContext _context;
        private readonly ILogger<HomeController> _logger;
        //建構函式會使用[相依性插入]將資料庫內容(PostgresSQLContext) 插入到控制器中。 
        //控制器中的每一個 CRUD 方法都會使用資料庫內容。
        public HomeController(PostgresSQLContext context, ILogger<HomeController> logger) : base(context, logger)
        {
            _context = context;
            _logger = logger;
        }
        // GET: /<controller>/
        public IActionResult Index()
        {
            try
            {
                ViewData["UserList"] = _context.Users.Where(u => u.IsAdmin != true).ToList();
            }
            catch (Exception ex)
            {
                return ExceptionHandler(ex, "Constructor Error.");
            }
            return View();
        }

        [HttpPost]
        public IActionResult InsertMachineData(MachineUserViewModel machineUserViewModel)
        {
            try
            {
                if (machineUserViewModel.Password == null)
                {
                    return StatusCode(400, "PasswordIsNull");
                }
                else
                {
                    byte[] encodedBytes = System.Text.Encoding.UTF8.GetBytes(machineUserViewModel.Password);
                    machineUserViewModel.Password = Convert.ToBase64String(encodedBytes);
                }

                Machine machine = new Machine();
                machine.Key = machineUserViewModel.MachineKey;
                machine.IP = machineUserViewModel.IP;
                machine.Name = machineUserViewModel.Name;
                machine.LoginID = machineUserViewModel.LoginID;
                machine.Password = machineUserViewModel.Password;
                machine.OS = machineUserViewModel.OS;
                machine.HostIP = machineUserViewModel.HostIP;
                machine.Description = machineUserViewModel.Description;

                _context.Add(machine);
                _context.SaveChanges();

                //Insert userMachine table 
                if (machineUserViewModel.AssignUserKeys != null)
                {
                    foreach (var machineUserID in machineUserViewModel.AssignUserKeys)
                    {
                        UserMachine userMachine = new UserMachine();
                        userMachine.MachineKey = machineUserViewModel.MachineKey;
                        userMachine.UserID = machineUserID;
                        _context.UserMachines.Add(userMachine);
                    }
                    _context.SaveChanges();
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return ExceptionHandler(ex, "Insert Error.");
            }
        }

        [HttpPut]
        public IActionResult UpdateMachineData(MachineUserViewModel machineUserViewModel)
        {
            try
            {
                if (machineUserViewModel.Password == null)
                {
                    return StatusCode(400, "PasswordIsNull");
                }
                else
                {
                    byte[] encodedBytes = System.Text.Encoding.UTF8.GetBytes(machineUserViewModel.Password);
                    machineUserViewModel.Password = Convert.ToBase64String(encodedBytes);
                }
                //insert machine table
                Machine machine = new Machine();
                machine.Key = machineUserViewModel.MachineKey;
                machine.IP = machineUserViewModel.IP;
                machine.Name = machineUserViewModel.Name;
                machine.LoginID = machineUserViewModel.LoginID;
                machine.Password = machineUserViewModel.Password;
                machine.OS = machineUserViewModel.OS;
                machine.HostIP = machineUserViewModel.HostIP;
                machine.Description = machineUserViewModel.Description;

                _context.Update(machine);
                _context.SaveChanges();

                /**Delete MachineKey in intermediary table, Then, Insert new MachineKey and User in intermediary talbe**/
                //Delete
                var _machineUser = _context.UserMachines.Where(um => um.MachineKey == machineUserViewModel.MachineKey).ToList();

                if (_machineUser.Count != 0)
                {
                    _context.UserMachines.RemoveRange(_machineUser);
                    _context.SaveChanges();
                }

                //Insert
                if (machineUserViewModel.AssignUserKeys != null)
                {
                    foreach (var machineUserID in machineUserViewModel.AssignUserKeys)
                    {
                        UserMachine userMachine = new UserMachine();
                        userMachine.MachineKey = machineUserViewModel.MachineKey;
                        userMachine.UserID = machineUserID;
                        _context.UserMachines.Add(userMachine);

                    }
                    _context.SaveChanges();
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return ExceptionHandler(ex, "Update Error.");
            }
        }

        [HttpDelete]
        public IActionResult DeleteMachineData(MachineUserViewModel machineUserViewModel)
        {  
            try
            {
                Machine _machine = _context.Machines.SingleOrDefault(m => m.Key == machineUserViewModel.MachineKey);
                _context.Remove(_machine);
                _context.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                return ExceptionHandler(ex, "Delete Error.");
            }
        }

        [HttpGet]
        public IActionResult GetGridData(string ipName, string serverName)
        {

            ClaimsIdentity identity = HttpContext.User.Identity as ClaimsIdentity; ;
            var user = identity.Claims;
            var userName = user.Where(u => u.Type == "Username").SingleOrDefault().Value;
            var isAdmin = user.Where(u => u.Type == "IsAdmin").SingleOrDefault().Value;

            var predicate = PredicateBuilder.New<Machine>(true);
            try
            {
                // 如果有輸入IP名稱作為搜尋條件時
                if (!string.IsNullOrWhiteSpace(ipName))
                {
                    predicate = predicate.And(m => m.IP.Contains(ipName));
                }

                // 如果有輸入伺服器名稱作為搜尋條件時
                if (!string.IsNullOrWhiteSpace(serverName))
                {
                    predicate = predicate.And(m => m.Name.Contains(serverName));
                }

                var _machine = _context.Machines.AsExpandable().Where(predicate).Select(
                u => new MachineUserViewModel
                {
                    MachineKey = u.Key,
                    IP = u.IP,
                    Name = u.Name,
                    LoginID = u.LoginID,
                    Password = u.Password,
                    OS = u.OS,
                    HostIP = u.HostIP,
                    Description = u.Description,
                    AssignUserKeys = u.UserMachines.Select(t => t.User.UserID).ToList()
                }).OrderBy(m => m.IP).ThenBy(m => m.Name).ToList();

                //Decode                
                foreach (var item in _machine)
                {
                    byte[] decodedBytes = Convert.FromBase64String(item.Password);
                    if (Convert.ToBoolean(isAdmin))
                    {
                        item.Password = System.Text.Encoding.UTF8.GetString(decodedBytes);
                    }
                    else
                    {
                        if (item.AssignUserKeys.Count > 0)
                        {
                            foreach (var useritem in item.AssignUserKeys)
                            {
                                if (useritem == userName)
                                {
                                    item.Password = System.Text.Encoding.UTF8.GetString(decodedBytes);
                                    break;
                                }
                                else
                                {
                                    item.Password = "******";
                                }
                            }
                        }
                        else
                        {
                            item.Password = "******";
                        }
                    }
                }
                return Ok(_machine.ToList());
            }
            catch (Exception ex)
            {
                return ExceptionHandler(ex, "GetServerInf Error.");
            }
        }
    }
}
