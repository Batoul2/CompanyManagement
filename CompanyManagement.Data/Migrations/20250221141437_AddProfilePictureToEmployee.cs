﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CompanyManagement.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddProfilePictureToEmployee : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProfilePicturePath",
                table: "Employees",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfilePicturePath",
                table: "Employees");
        }
    }
}
