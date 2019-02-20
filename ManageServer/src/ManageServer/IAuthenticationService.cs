using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ManageServer.Models;
namespace ManageServer
{
    public interface IAuthenticationService
    {
        LdapResultModel LdapValid(string username, string password);
    }
}
