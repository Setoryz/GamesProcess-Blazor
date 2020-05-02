using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GamesProcess.Blazor.Migrations
{
    public partial class AdvancedSearchParameterAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AdvancedSearchParameter",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TimeSearched = table.Column<DateTime>(nullable: false),
                    NoOfSearchValues = table.Column<int>(nullable: false),
                    NoOfWeeksToDisplay = table.Column<int>(nullable: false),
                    GameSelection = table.Column<int>(nullable: false),
                    GroupSelection = table.Column<int>(nullable: false),
                    ReferenceValue = table.Column<int>(nullable: false),
                    ReferenceLocation = table.Column<int>(nullable: false),
                    ReferencePosition = table.Column<int>(nullable: false),
                    Value2 = table.Column<int>(nullable: false),
                    Value2WeekSelect = table.Column<int>(nullable: false),
                    Value2Week = table.Column<int>(nullable: false),
                    Value2Location = table.Column<int>(nullable: false),
                    Value2Position = table.Column<int>(nullable: false),
                    Value3 = table.Column<int>(nullable: false),
                    Value3WeekSelect = table.Column<int>(nullable: false),
                    Value3Week = table.Column<int>(nullable: false),
                    Value3Location = table.Column<int>(nullable: false),
                    Value3Position = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdvancedSearchParameter", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdvancedSearchParameter");
        }
    }
}
