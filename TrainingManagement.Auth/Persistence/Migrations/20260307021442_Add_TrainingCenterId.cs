using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrainingManagement.Auth.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Add_TrainingCenterId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Company",
                schema: "auth",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "EmployeeCode",
                schema: "auth",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "FirstName",
                schema: "auth",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "MiddleName",
                schema: "auth",
                table: "AspNetUsers",
                newName: "UserCode");

            migrationBuilder.RenameColumn(
                name: "LastName",
                schema: "auth",
                table: "AspNetUsers",
                newName: "FullName");

            migrationBuilder.AddColumn<Guid>(
                name: "TainingCenterId",
                schema: "auth",
                table: "AspNetUsers",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "UserType",
                schema: "auth",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TainingCenterId",
                schema: "auth",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "UserType",
                schema: "auth",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "UserCode",
                schema: "auth",
                table: "AspNetUsers",
                newName: "MiddleName");

            migrationBuilder.RenameColumn(
                name: "FullName",
                schema: "auth",
                table: "AspNetUsers",
                newName: "LastName");

            migrationBuilder.AddColumn<string>(
                name: "Company",
                schema: "auth",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "EmployeeCode",
                schema: "auth",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                schema: "auth",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
