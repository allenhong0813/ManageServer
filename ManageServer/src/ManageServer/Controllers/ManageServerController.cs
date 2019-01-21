using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ManageServer.Models;
using Microsoft.EntityFrameworkCore;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace ManageServer.Controllers
{
    public class ManageServerController : Controller
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
        public ManageServerController(PostgresSQLContext context)
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
        public void InsertMachineData(Machine machine)
        {
            try
            {
                byte[] encodedBytes = System.Text.Encoding.UTF8.GetBytes(machine.Password);
                machine.Password = Convert.ToBase64String(encodedBytes);

                _context.Add(machine);
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

                }else
                {
                    throw new Exception("Insert Error", ex);
                }
                

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
                throw new Exception("Update Error");
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
                


                var decodePasswod = _machine.Select(m => m.Password).ToList();
              
                foreach(Machine item in machines)
                {
                    byte[] decodedBytes = Convert.FromBase64String(item.Password);
                    item.Password = System.Text.Encoding.UTF8.GetString(decodedBytes);

                }
                //byte[] newBytes = Convert.FromBase64String(s);

                //byte[] decodedBytes = Convert.FromBase64String(_machine.p)
                

                // 如果有輸入IP名稱作為搜尋條件時
                if (!string.IsNullOrWhiteSpace(ipName))
                {
                   // machines = machines.Where();
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
