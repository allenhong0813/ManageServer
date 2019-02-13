using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManageServer
{
    public class AppUser
    {
        public string DisplayName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool IsAdmin { get; set; }
        public string Email { get; set; }
        public string Roles { get; set; }
    }
}
