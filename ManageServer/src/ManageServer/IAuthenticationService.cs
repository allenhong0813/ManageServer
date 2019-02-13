using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManageServer
{
    public interface IAuthenticationService
    {
        Boolean Login(string username, string password);
    }
}
