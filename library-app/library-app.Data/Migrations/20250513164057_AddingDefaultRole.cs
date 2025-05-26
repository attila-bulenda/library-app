using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace library_app.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddingDefaultRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "288407f6-be60-4483-87f0-31eaa2265f96", null, "Librarian", "LIBRARIAN" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "288407f6-be60-4483-87f0-31eaa2265f96");
        }
    }
}
