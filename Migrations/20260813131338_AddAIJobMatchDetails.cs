using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HireAI.API.Migrations
{
    /// <inheritdoc />
    public partial class AddAIJobMatchDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EducationRelevance",
                table: "JobMatches",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ExperienceRelevance",
                table: "JobMatches",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Strengths",
                table: "JobMatches",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Weaknesses",
                table: "JobMatches",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EducationRelevance",
                table: "JobMatches");

            migrationBuilder.DropColumn(
                name: "ExperienceRelevance",
                table: "JobMatches");

            migrationBuilder.DropColumn(
                name: "Strengths",
                table: "JobMatches");

            migrationBuilder.DropColumn(
                name: "Weaknesses",
                table: "JobMatches");
        }
    }
}
