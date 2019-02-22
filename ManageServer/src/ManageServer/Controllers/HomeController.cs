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
                ViewData["UserList"] = _context.Users.ToList();
            }
            catch (Exception ex)
            {
                return ExceptionHandler(ex, "Constructor Error.");
            }
            return View();
        }

        [HttpPost]
        public IActionResult InsertMachineData(Machine machine)
        {
            try
            {
                if (machine.Password == null)
                {
                    return StatusCode(400, "PasswordIsNull");
                }
                else
                {
                    byte[] encodedBytes = System.Text.Encoding.UTF8.GetBytes(machine.Password);
                    machine.Password = Convert.ToBase64String(encodedBytes);
                }
                _context.Add(machine);
                _context.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                return ExceptionHandler(ex, "Insert Error.");
            }
        }

        [HttpPut]
        public IActionResult UpdateMachineData(Machine machine)
        {
            try
            {
                if (machine.Password == null)
                {
                    return StatusCode(400, "PasswordIsNull");
                }
                else
                {
                    byte[] encodedBytes = System.Text.Encoding.UTF8.GetBytes(machine.Password);
                    machine.Password = Convert.ToBase64String(encodedBytes);
                }
                _context.Update(machine);
                _context.SaveChanges();
                return Ok();

            }
            catch (Exception ex)
            {
                return ExceptionHandler(ex, "Update Error.");
            }
        }

        [HttpDelete]
        public IActionResult DeleteMachineData(Machine machine)
        {
            try
            {
                var _machine = _context.Machines.SingleOrDefault(m => m.Key == machine.Key);
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
        public IActionResult GetServerInfo(string ipName, string serverName)
        {
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

                var _machine = _context.Machines.Select(
                u => new MachineUserViewModel
                {
                    MachineKeys = u.Key,
                    IP = u.IP,
                    Name = u.Name,
                    LoginID = u.LoginID,
                    Password = u.Password,
                    OS = u.OS,
                    HostIP = u.HostIP,
                    Description = u.Description,
                    AssignUserKeys = u.UserMachines.Select(t => t.User.UserID).ToList()
                });

                //Decode
                List<MachineUserViewModel> machines = _machine.ToList();
                foreach (var item in machines)
                {
                    byte[] decodedBytes = Convert.FromBase64String(item.Password);
                    item.Password = System.Text.Encoding.UTF8.GetString(decodedBytes);
                }
                //Sort
                _machine = _machine.OrderBy(m => m.IP).ThenBy(m => m.Name);
                return Ok(_machine.ToList());
            }
            catch (Exception ex)
            {
                return ExceptionHandler(ex, "GetServerInf Error.");
            }
        }
    }
}
