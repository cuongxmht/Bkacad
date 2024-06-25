using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VicemMvcIdentity.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddEmployee : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FullName",
                table: "Employee",
                newName: "NonSignName");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Employee",
                newName: "EmployeeId");

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Employee",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfBirth",
                table: "Employee",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Employee",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "Employee",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "HireDate",
                table: "Employee",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "Employee",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Position",
                table: "Employee",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "Employee");

            migrationBuilder.DropColumn(
                name: "DateOfBirth",
                table: "Employee");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Employee");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "Employee");

            migrationBuilder.DropColumn(
                name: "HireDate",
                table: "Employee");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "Employee");

            migrationBuilder.DropColumn(
                name: "Position",
                table: "Employee");

            migrationBuilder.RenameColumn(
                name: "NonSignName",
                table: "Employee",
                newName: "FullName");

            migrationBuilder.RenameColumn(
                name: "EmployeeId",
                table: "Employee",
                newName: "Id");
        }
    }
}
