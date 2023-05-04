using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FriendBook.GroupService.API.DAL.Migrations
{
    /// <inheritdoc />
    public partial class Addgroup_task_in_database : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "creater_account_id",
                table: " account_status_groups");

            migrationBuilder.CreateTable(
                name: "group_tasks",
                columns: table => new
                {
                    pk_group_task_id = table.Column<Guid>(type: "uuid", nullable: false),
                    creater_id = table.Column<Guid>(type: "uuid", nullable: false),
                    group_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    accounts_id = table.Column<Guid[]>(type: "uuid[]", nullable: false),
                    status_task = table.Column<short>(type: "smallint", nullable: false),
                    date_start_work = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    date_end_work = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_group_tasks", x => x.pk_group_task_id);
                    table.ForeignKey(
                        name: "FK_group_tasks_groups_group_id",
                        column: x => x.group_id,
                        principalTable: "groups",
                        principalColumn: "pk_group_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_group_tasks_group_id",
                table: "group_tasks",
                column: "group_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "group_tasks");

            migrationBuilder.AddColumn<Guid>(
                name: "creater_account_id",
                table: " account_status_groups",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }
    }
}
