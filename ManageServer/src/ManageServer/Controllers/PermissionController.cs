using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

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
        public IActionResult GetAdminInf()
        {

            try
            {
                var Users = _context.Users.Select(m => m);
                return Ok(Users.ToList());
            }
            catch (Exception ex)
            {

            }

            return Ok();
        }
    }
}
