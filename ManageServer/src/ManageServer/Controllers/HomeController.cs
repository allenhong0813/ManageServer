using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ManageServer.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using ManageServer;

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

        //[HttpGet]
        //public List<Machine> LoadMachineData()
        //{
        //    // Use LINQ SQL syntax to control loaddata.
        //    var _machines = from m in _context.Machines
        //                    orderby m.IP, m.Name
        //                    select m;

        //    return _machines.ToList();            
        //}

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
                if (ex.InnerException.GetType().Name.Equals("PostgresException"))
                {
                    Npgsql.PostgresException pgex = (Npgsql.PostgresException)ex.InnerException;
                    if (pgex.Code.Equals("23502"))
                    {
                        return StatusCode(400, pgex.ColumnName + "IsNull");
                    }
                    //if (pgex.ColumnName.Equals("IP"))
                    //{
                    //    if (pgex.Code.Equals("23505"))
                    //    {
                    //        return StatusCode(400, "Duplicate"+ pgex.ColumnName);
                    //    }
                    //}
                }
                
                    

                
                return StatusCode(500, "Insert Error:" + ex);
            }
        }

        [HttpPut]
        public void UpdateMachineData(Machine machine)
        {
            try
            {
                _context.Update(machine);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {

                if (ex.GetType().FullName.Equals("Microsoft.EntityFrameworkCore.DbUpdateException"))
                {
                    Npgsql.PostgresException pgex = (Npgsql.PostgresException)ex.InnerException;
                    if (pgex.Code.Equals("23502"))
                    {
                        throw new Exception(pgex.ColumnName + " is null.");
                    }
                }
                else
                {
                    throw new Exception("Update Error", ex);
                }
            }
        }

        [HttpDelete]
        public void DeleteMachineData(Machine machine)
        {
            try
            {
                var _machine = _context.Machines.SingleOrDefault(m => m.Key == machine.Key);
                if (_machine == null)
                {
                    //written delete error message of movie data.
                }
                _context.Remove(_machine);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Delete Error");
            }
        }

        [HttpGet]
        public List<Machine> GetServerInf(string ipName, string serverName)
        {
            try
            {
                var _machine = from m in _context.Machines
                               select m;
                List<Machine> machines = _machine.ToList();

                foreach (Machine item in machines)
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
                return _machine.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("GetServerInf Error");
            }
        }
    }
}
