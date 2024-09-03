using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Test3enc.Data.Migrations
{
    /// <inheritdoc />
    public partial class iv : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "IV",
                table: "EncryptedFiles",
                type: "varbinary(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IV",
                table: "EncryptedFiles");
        }
    }
}
