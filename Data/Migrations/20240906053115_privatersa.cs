using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Test3enc.Data.Migrations
{
    /// <inheritdoc />
    public partial class privatersa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PrivateKey",
                table: "EncryptedFiles",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PrivateKey",
                table: "EncryptedFiles");
        }
    }
}
