﻿using Microsoft.EntityFrameworkCore.Migrations;

namespace ZeroTouchHR.Migrations
{
    public partial class Addedtitlename : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "title",
                table: "Employees",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "title",
                table: "Employees");
        }
    }
}
