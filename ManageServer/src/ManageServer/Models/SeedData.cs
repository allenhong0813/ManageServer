using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ManageServer.Models
{

    public class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new PostgresSQLContext(
                serviceProvider.GetRequiredService<DbContextOptions<PostgresSQLContext>>()))
            {
                // Look for any machines. note:DB has been seeded
                if (context.Machines.Any())
                {
                    return;   // DB has been seeded
                }

                context.Machines.AddRange(
                     new Machine

                     {
                         Name = "Server 1",
                         IP = "172.0.0.1",
                         LoginID = "Allen",
                         Password = "111111",
                         OS = "Windos",
                         HostIP = "192.168.0.1",
                         Description = "Server 1 Desc",
                     },

                     new Machine
                     {
                         Name = "Server 2",
                         IP = "172.0.0.2",
                         LoginID = "Rong",
                         Password = "222222",
                         OS = "Linux",
                         HostIP = "192.168.0.1",
                         Description = "Server 2 Desc",
                     }
                );

                //insert to DB
                context.SaveChanges();
            }
        }
    }
}
