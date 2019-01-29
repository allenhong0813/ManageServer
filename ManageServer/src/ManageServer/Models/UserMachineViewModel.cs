using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManageServer.Models
{
    public class UserMachineViewModel
    {
        
        public string UserID { get; set; }
        public bool IsAdmin { get; set; }
        public List<Machine> AssignMachines { get; set; }

    }
    

}
