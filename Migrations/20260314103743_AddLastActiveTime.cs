using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PingWatch.Migrations
{
    /// <inheritdoc />
    public partial class AddLastActiveTime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastActiveTime",
                table: "IpAddresses",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastActiveTime",
                table: "IpAddresses");
        }
    }
}
