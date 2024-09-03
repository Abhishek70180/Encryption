using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Test3enc.Data.Migrations
{
    /// <inheritdoc />
    public partial class encryotioniv : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "EncryptionIv",
                table: "EncryptedFiles",
                type: "varbinary(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EncryptionIv",
                table: "EncryptedFiles");
        }
    }
}
