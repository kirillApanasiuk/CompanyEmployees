using Microsoft.EntityFrameworkCore.Migrations;

namespace CompanyEmployees.Migrations
{
    public partial class AddedRolesToDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "c9277b24-3a3e-4f06-a502-8989d4f5dd8f", "5720fc2a-5191-45d9-ba74-0ff5a860728c", "Manager", "Manager" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "31fd4acd-86cb-4a59-b7e6-1c1ab0b0e455", "faa59b74-7b7a-4811-abef-1ecdecb4dccb", "Administrator", "ADMINISTRATOR" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "31fd4acd-86cb-4a59-b7e6-1c1ab0b0e455");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c9277b24-3a3e-4f06-a502-8989d4f5dd8f");
        }
    }
}
