using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManageServer.Models
{
    public class Machine
    {
        //GUID
        public string Key { get; set; }
        public string Name { get; set; }
        public string IP { get; set; }
        public string LoginID { get; set; }
        public string Password { get; set; }
        public string OS { get; set; }
        public string HostIP { get; set; }
        public string Description { get; set; }

    }
}
