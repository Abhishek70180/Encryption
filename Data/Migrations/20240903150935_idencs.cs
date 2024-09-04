using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Test3enc.Data.Migrations
{
    /// <inheritdoc />
    public partial class idencs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "EncryptedKey",
                table: "EncryptedFiles",
                type: "varbinary(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EncryptedKey",
                table: "EncryptedFiles");
        }
    }
}
