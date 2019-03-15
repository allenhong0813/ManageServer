using Microsoft.EntityFrameworkCore;
using ManageServer.Models;
namespace ManageServer
{
    public class PostgresSQLContext : DbContext
    {
        public PostgresSQLContext(DbContextOptions<PostgresSQLContext> options): base(options){ }

        public DbSet<User> Users { get; set; }
        public DbSet<Machine> Machines { get; set; }
        public DbSet<UserMachine> UserMachines { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserMachine>().HasKey(x => new { x.UserID, x.MachineKey });
            modelBuilder.Entity<UserMachine>()
                .HasOne(um => um.User)
                .WithMany(m => m.UserMachines)
                .HasForeignKey(um => um.UserID);
            modelBuilder.Entity<UserMachine>()
                .HasOne(um => um.Machine)
                .WithMany(m => m.UserMachines)
                .HasForeignKey(um => um.MachineKey);
        }
    }
}
