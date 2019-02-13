﻿using System;
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
            _connection = new LdapConnection
            {
                SecureSocketLayer = true
            };
        }

        public bool Login(string username, string password)
        {
            try
            {
                _connection.Connect(_config.Url, LdapConnection.DEFAULT_PORT);
                _connection.Bind(_config.UserName, _config.Password);
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
