using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using ManageServer.Models;
using Microsoft.Extensions.Logging;
using Novell.Directory.Ldap;

namespace ManageServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();

            //初始化SeedData
            //using (var scope = host.Services.CreateScope())
            //{
            //    var services = scope.ServiceProvider;

            //    try
            //    {
            //        // Requires using ManageServer.Models;
            //        SeedData.Initialize(services);
            //    }
            //    catch (Exception ex)
            //    {

            //        var logger = services.GetRequiredService<ILogger<Program>>();
            //        logger.LogError(default(EventId), ex, "createSeedError");
            //    }
            //}

            //var User = new { username = "allen", password = "Shine@0813" };
            ////var User = new { username = "password", password = "password" };
            //var result = SelfLdapAuth(User.username, User.password);
            //Console.WriteLine($"LDAP Auth Result: {result}");



            host.Run();
        }

        public static bool SelfLdapAuth(string username, string password)
        {
            var Host = "172.21.50.254";
            var BaseDC = "cn=users,dc=pbg";
            //var Host = "dap.forumsys.com";
            //var BaseDC = "cn=read-only-admin,dc=example,dc=com";
            try
            {
                using (var ldapConn = new LdapConnection())
                {
                    ldapConn.Connect(Host, LdapConnection.DEFAULT_PORT);
                    ldapConn.Bind($"uid={username},{BaseDC}", password);
                    return ldapConn.Bound;
                }
            }
            catch (LdapException ldapex)
            {
                throw ldapex;
            }
        }
    }
}
