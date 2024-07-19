using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VicemAPI.Migrations
{
    /// <inheritdoc />
    public partial class addNewDaily : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DaiLy",
                columns: table => new
                {
                    DaiLyId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TenDaiLy = table.Column<string>(type: "TEXT", nullable: false),
                    DiaChi = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DaiLy", x => x.DaiLyId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DaiLy");
        }
    }
}
