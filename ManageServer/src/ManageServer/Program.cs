using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using ManageServer.Models;
using Microsoft.Extensions.Logging;

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

            host.Run();
        }
    }
}
