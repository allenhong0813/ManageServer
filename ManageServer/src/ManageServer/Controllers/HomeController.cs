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

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace ManageServer.Controllers
{
    public class HomeController : Controller
    {
        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }

        //get DB connect object
        private readonly PostgresSQLContext _context;

        //建構函式會使用[相依性插入]將資料庫內容(PostgresSQLContext) 插入到控制器中。 
        //控制器中的每一個 CRUD 方法都會使用資料庫內容。
        public HomeController(PostgresSQLContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult InsertMachineData(Machine machine)
        {
            try
            {
                if (machine.Password == null)
                {
                    return StatusCode(400, "PasswordIsNull");
                }else {
                    byte[] encodedBytes = System.Text.Encoding.UTF8.GetBytes(machine.Password);
                    machine.Password = Convert.ToBase64String(encodedBytes);
                }
                _context.Add(machine);
                _context.SaveChanges();

                return Ok();

            }
            catch (Exception ex)
            {
                if (ex.GetType().Name.Equals("SocketException"))
                {
                    SocketException pgex = (SocketException)ex;
                    if (pgex.SocketErrorCode.ToString().Equals("ConnectionRefused"))
                    {
                        return StatusCode(500, "DBNoConnect");
                    }
                }
                else if (ex.InnerException.GetType().Name.Equals("PostgresException"))
                {
                    Npgsql.PostgresException pgex = (Npgsql.PostgresException)ex.InnerException;
                    if (pgex.SqlState.Equals("23502"))
                    {
                        return StatusCode(400, pgex.ColumnName + "IsNull");
                    }
                }
                    return StatusCode(500, "Insert Error.");
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
                if (ex.GetType().Name.Equals("SocketException"))
                {
                    SocketException pgex = (SocketException)ex;
                    if (pgex.SocketErrorCode.ToString().Equals("ConnectionRefused"))
                    {
                        return StatusCode(500, "DBNoConnect");
                    }
                }
                else if (ex.InnerException.GetType().Name.Equals("PostgresException"))
                {
                    Npgsql.PostgresException pgex = (Npgsql.PostgresException)ex.InnerException;
                    if (pgex.SqlState.Equals("23502"))
                    {
                        return StatusCode(400, pgex.ColumnName + "IsNull");
                    }
                }
                return StatusCode(500, "Update Error.");
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
                if (ex.GetType().Name.Equals("SocketException"))
                {
                    SocketException pgex = (SocketException)ex;
                    if (pgex.SocketErrorCode.ToString().Equals("ConnectionRefused"))
                    {
                        return StatusCode(500, "DBNoConnect");
                    }
                }
                return StatusCode(500, "Delete Error.");
            }
        }

        [HttpGet]
        public IActionResult GetServerInfo(string ipName, string serverName)
        {
            try
            {
                var _machine = from m in _context.Machines
                               select m;
                List<Machine> machines = _machine.ToList();

                foreach (var item in machines)
                {
                    byte[] decodedBytes = Convert.FromBase64String(item.Password);
                    item.Password = System.Text.Encoding.UTF8.GetString(decodedBytes);

                }

                // 如果有輸入IP名稱作為搜尋條件時
                if (!string.IsNullOrWhiteSpace(ipName))
                {
                    _machine = _machine.Where(m => m.IP.Contains(ipName));
                }

                // 如果有輸入伺服器名稱作為搜尋條件時
                if (!string.IsNullOrWhiteSpace(serverName))
                {
                    _machine = _machine.Where(m => m.Name.Contains(serverName));
                }

                _machine = _machine.OrderBy(m => m.IP).ThenBy(m => m.Name);
                return Ok(_machine.ToList());
            }
            catch (Exception ex)
            {
                if (ex.GetType().Name.Equals("SocketException"))
                {
                    SocketException pgex = (SocketException)ex;
                    if (pgex.SocketErrorCode.ToString().Equals("ConnectionRefused"))
                    {
                        return StatusCode(500, "DBNoConnect");
                    }
                }
                return StatusCode(500, "GetServerInf Error.");
            }
        }
    }
}
