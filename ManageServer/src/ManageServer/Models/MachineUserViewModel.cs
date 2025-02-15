﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ManageServer.Models
{
    public class MachineUserViewModel
    {
        public string MachineKey { get; set; }
        public string IP { get; set; }
        public string Name { get; set; }
        public string LoginID { get; set; }
        public string Password { get; set; }
        public string OS { get; set; }
        public string HostIP { get; set; }
        public string Description { get; set; }
        public List<User> UserList { get; set; }
        public List<string> AssignUserKeys { get; set; }
        //public List<UserDetailGridViewModel> UserDetail { get; set; }

    }
}
