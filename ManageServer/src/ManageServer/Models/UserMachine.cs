using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ManageServer.Models
{
    public class UserMachine
    {        
        
        public string MachineKey { get; set; }
        public string UserID { get; set; }

        //FK UserID of User Table
        [ForeignKey("UserID")]
        public User User { get; set; }
        //FK MachineKey of Machine Table
        [ForeignKey("Key")]
        public Machine Machine { get; set; }
    }
}
