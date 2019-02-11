using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using ManageServer;

namespace ManageServer.Migrations
{
    [DbContext(typeof(PostgresSQLContext))]
    [Migration("20190211011937_Update_DB")]
    partial class Update_DB
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "1.1.6");

            modelBuilder.Entity("ManageServer.Models.Machine", b =>
                {
                    b.Property<string>("Key")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Description");

                    b.Property<string>("HostIP");

                    b.Property<string>("IP");

                    b.Property<string>("LoginID");

                    b.Property<string>("Name");

                    b.Property<string>("OS");

                    b.Property<string>("Password");

                    b.HasKey("Key");

                    b.ToTable("Machines");
                });

            modelBuilder.Entity("ManageServer.Models.User", b =>
                {
                    b.Property<string>("UserID")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("IsAdmin");

                    b.HasKey("UserID");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("ManageServer.Models.UserMachine", b =>
                {
                    b.Property<string>("UserID");

                    b.Property<string>("MachineKey");

                    b.HasKey("UserID", "MachineKey");

                    b.HasIndex("MachineKey");

                    b.ToTable("UserMachines");
                });

            modelBuilder.Entity("ManageServer.Models.UserMachine", b =>
                {
                    b.HasOne("ManageServer.Models.Machine", "Machine")
                        .WithMany("UserMachines")
                        .HasForeignKey("MachineKey")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ManageServer.Models.User", "User")
                        .WithMany("UserMachines")
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
