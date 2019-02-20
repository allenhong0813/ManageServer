using System;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using Novell.Directory.Ldap;
using System.Linq;
using System.Threading.Tasks;
using ManageServer.Models;
using Microsoft.Extensions.Logging;

namespace ManageServer
{
    public class LdapAuthenticationService : IAuthenticationService
    {
        private readonly LdapConfig _config;
        private readonly LdapConnection _connection;
        private readonly ILogger<LdapAuthenticationService> _logger;

        public LdapAuthenticationService(IOptions<LdapConfig> configAccessor, ILogger<LdapAuthenticationService> logger)
        {
            // Config from appsettings, injected through the pipeline
            _config = configAccessor.Value;
            _connection = new LdapConnection();
            _logger = logger;
        }

        public LdapResultModel LdapValid(string username, string password)
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
                _logger.LogError("[{0}] {1}",ldapex.Message, ldapex.StackTrace);
            }
            _connection.Disconnect();
            return result;
        }
    }
}
