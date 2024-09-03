using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Test3enc.Data.Migrations
{
    /// <inheritdoc />
    public partial class key : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PublicKeyBase64",
                table: "Keys",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PublicKeyBase64",
                table: "Keys");
        }
    }
}
