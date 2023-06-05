using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FriendBook.GroupService.API.DAL.Migrations
{
    /// <inheritdoc />
    public partial class addIndexNameTaskWithIdGroup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "accounts_id",
                table: "group_tasks");

            migrationBuilder.CreateIndex(
                name: "IX_group_tasks_name_group_id",
                table: "group_tasks",
                columns: new[] { "name", "group_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_group_tasks_name_group_id",
                table: "group_tasks");

            migrationBuilder.AddColumn<Guid[]>(
                name: "accounts_id",
                table: "group_tasks",
                type: "uuid[]",
                nullable: false,
                defaultValue: new Guid[0]);
        }
    }
}
