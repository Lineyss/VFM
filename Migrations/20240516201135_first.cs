using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VFM.Migrations
{
    public partial class first : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "user",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    login = table.Column<string>(type: "TEXT", nullable: false),
                    password = table.Column<string>(type: "TEXT", nullable: false),
                    isAdmin = table.Column<bool>(type: "INTEGER", nullable: true),
                    createF = table.Column<bool>(type: "INTEGER", nullable: true),
                    deleteF = table.Column<bool>(type: "INTEGER", nullable: true),
                    updateNameF = table.Column<bool>(type: "INTEGER", nullable: true),
                    downloadF = table.Column<bool>(type: "INTEGER", nullable: true),
                    uploadF = table.Column<bool>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "user");
        }
    }
}
