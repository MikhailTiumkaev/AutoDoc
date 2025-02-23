using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoDocApi.Migrations
{
    /// <inheritdoc />
    public partial class Update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payloads_TodoTasks_TodoTaskId",
                table: "Payloads");

            migrationBuilder.DropIndex(
                name: "IX_Payloads_TodoTaskId",
                table: "Payloads");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Payloads_TodoTaskId",
                table: "Payloads",
                column: "TodoTaskId");

            migrationBuilder.AddForeignKey(
                name: "FK_Payloads_TodoTasks_TodoTaskId",
                table: "Payloads",
                column: "TodoTaskId",
                principalTable: "TodoTasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
