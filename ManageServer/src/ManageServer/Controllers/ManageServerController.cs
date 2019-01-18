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
        
        [HttpGet]
        public List<Machine> LoadMachineData()
        {
            // Use LINQ to loaddata.
            var machines = from m in _context.Machines
                           select m;

            return machines.ToList();
        }

        [HttpPost]
        public void InsertMachineData(Machine machine)
        {
            try
            {
                _context.Add(machine);
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
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
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }

        [HttpDelete]
        public void DeleteMachineData(Machine machine)
        {
            var _machine = _context.Machines.SingleOrDefault(m => m.Key == machine.Key);
            if(_machine == null)
            {
                //written delete error message of movie data.
            }
            _context.Remove(_machine);
            _context.SaveChanges();
        }
    }
}
