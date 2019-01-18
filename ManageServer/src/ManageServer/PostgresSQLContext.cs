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
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseNpgsql("Host=localhost;Database=MangeServer;Username=postgres;Password=zaq12wsx");
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserMachine>().HasKey(x => new { x.UserID, x.MachineKey });

        }
    }
}
