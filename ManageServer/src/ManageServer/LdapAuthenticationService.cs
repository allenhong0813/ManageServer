using System;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using Novell.Directory.Ldap;
using System.Linq;
using System.Threading.Tasks;

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

        public Boolean Login(string username, string password)
        {
            try
            {
                _connection.Connect(_config.Url, LdapConnection.DEFAULT_PORT);
                _connection.Bind($"uid={username},{_config.BindDn}", password);
                return _connection.Bound;
            }
            catch (LdapException ldapex)
            {
                throw ldapex;
            }
            finally
            {
                _connection.Disconnect();
            }
        }
    }
}
