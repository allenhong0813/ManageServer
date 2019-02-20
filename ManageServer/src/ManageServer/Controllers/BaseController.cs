using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net.Sockets;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace ManageServer.Controllers
{

    public class BaseController : Controller
    {
        private readonly PostgresSQLContext _context;
        private readonly ILogger _logger;

        public BaseController(PostgresSQLContext context, ILogger<Controller> logger)
        {
            _context = context;
            _logger = logger;
        }

        protected IActionResult ExceptionHandler(Exception ex, string errorMethodName)
        {
            try
            {
                if (ex.GetType().Name.Equals("SocketException"))
                {
                    SocketException pgex = (SocketException)ex;
                    if (pgex.SocketErrorCode.ToString().Equals("ConnectionRefused"))
                    {
                        return StatusCode(500, "DBNoConnect");
                    }
                }
                else if (ex.InnerException != null && ex.InnerException.GetType().Name.Equals("PostgresException"))
                {
                    Npgsql.PostgresException pgex = (Npgsql.PostgresException)ex.InnerException;
                    if (pgex.SqlState.Equals("23502"))
                    {
                        return StatusCode(400, pgex.ColumnName + "IsNull");
                    }
                }
            }
            catch (Exception baseEx)
            {
                _logger.LogError(baseEx.StackTrace);
            }
            _logger.LogError(ex.StackTrace);
            return StatusCode(500, errorMethodName);
        }

    }
}
