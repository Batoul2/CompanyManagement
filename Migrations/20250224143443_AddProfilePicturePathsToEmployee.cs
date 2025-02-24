using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CompanyManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddProfilePicturePathsToEmployee : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // migrationBuilder.DropColumn(
            //     name: "ProfilePicturePath",
            //     table: "Employees");

            migrationBuilder.AddColumn<List<string>>(
                name: "ProfilePicturePaths",
                table: "Employees",
                type: "text[]",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfilePicturePaths",
                table: "Employees");

            migrationBuilder.AddColumn<string>(
                name: "ProfilePicturePath",
                table: "Employees",
                type: "text",
                nullable: true);
        }
    }
}
