using System;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using Novell.Directory.Ldap;
using System.Linq;
using System.Threading.Tasks;
using ManageServer.Models;
namespace ManageServer
{
    public class LdapAuthenticationService : IAuthenticationService
    {
        private readonly LdapConfig _config;
        private readonly LdapConnection _connection;

        public LdapAuthenticationService(IOptions<LdapConfig> configAccessor)
        {
            // Config from appsettings, injected through the pipeline
            _config = configAccessor.Value;
            _connection = new LdapConnection();
        }

        public LdapResultModel LdapLogin(string username, string password)
        {
            LdapResultModel result = new LdapResultModel();
            try
            {
                _connection.Connect(_config.Url, LdapConnection.DEFAULT_PORT);
                _connection.Bind($"uid={username},{_config.BindDn}", password);
                result.IsSuccess = _connection.Bound;
            }
            catch (LdapException ldapex)
            {

                result.IsSuccess = false;
                result.ExceptionMessage = ldapex.Message;
                result.ResultCode = ldapex.ResultCode;
            }
            _connection.Disconnect();
            return result;
        }
    }
}
