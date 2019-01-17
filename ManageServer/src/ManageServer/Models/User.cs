using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ManageServer.Models
{
    public class User
    {
        [Key]
        public string UserID { get; set; }
        public bool IsAdmin { get; set; }
        List<UserMachine> UserMachines { get; set; }
    }
}


