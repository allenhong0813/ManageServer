using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManageServer.Models
{
    public class LdapResultModel
    {
        public bool IsSuccess { get; set; }
        public string ExceptionMessage { get; set; }

        public int ResultCode { get; set; }
    }
}
