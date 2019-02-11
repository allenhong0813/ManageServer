using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ManageServer.Migrations
{
    public partial class Update_DB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserMachines_Machines_Key",
                table: "UserMachines");

            migrationBuilder.DropIndex(
                name: "IX_UserMachines_Key",
                table: "UserMachines");

            migrationBuilder.DropColumn(
                name: "Key",
                table: "UserMachines");

            migrationBuilder.CreateIndex(
                name: "IX_UserMachines_MachineKey",
                table: "UserMachines",
                column: "MachineKey");

            migrationBuilder.AddForeignKey(
                name: "FK_UserMachines_Machines_MachineKey",
                table: "UserMachines",
                column: "MachineKey",
                principalTable: "Machines",
                principalColumn: "Key",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserMachines_Machines_MachineKey",
                table: "UserMachines");

            migrationBuilder.DropIndex(
                name: "IX_UserMachines_MachineKey",
                table: "UserMachines");

            migrationBuilder.AddColumn<string>(
                name: "Key",
                table: "UserMachines",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserMachines_Key",
                table: "UserMachines",
                column: "Key");

            migrationBuilder.AddForeignKey(
                name: "FK_UserMachines_Machines_Key",
                table: "UserMachines",
                column: "Key",
                principalTable: "Machines",
                principalColumn: "Key",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
