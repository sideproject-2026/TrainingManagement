using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrainingManagement.Center.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Alter_TrainingCenter_CodeEmail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Code",
                schema: "center",
                table: "TrainingCenters",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                schema: "center",
                table: "TrainingCenters",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Code",
                schema: "center",
                table: "TrainingCenters");

            migrationBuilder.DropColumn(
                name: "Email",
                schema: "center",
                table: "TrainingCenters");
        }
    }
}
