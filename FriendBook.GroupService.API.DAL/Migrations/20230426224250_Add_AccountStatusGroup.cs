using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FriendBook.GroupService.API.DAL.Migrations
{
    /// <inheritdoc />
    public partial class Add_AccountStatusGroup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "comments");

            migrationBuilder.CreateTable(
                name: "groups",
                columns: table => new
                {
                    pk_group_id = table.Column<Guid>(type: "uuid", nullable: false),
                    account_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying", nullable: false),
                    create_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_groups", x => x.pk_group_id);
                });

            migrationBuilder.CreateTable(
                name: " account_status_groups",
                columns: table => new
                {
                    pk_account_status_groups_id = table.Column<Guid>(type: "uuid", nullable: false),
                    account_id = table.Column<Guid>(type: "uuid", nullable: false),
                    group_id = table.Column<Guid>(type: "uuid", nullable: false),
                    role_account = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ account_status_groups", x => x.pk_account_status_groups_id);
                    table.ForeignKey(
                        name: "FK_ account_status_groups_groups_group_id",
                        column: x => x.group_id,
                        principalTable: "groups",
                        principalColumn: "pk_group_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ account_status_groups_group_id",
                table: " account_status_groups",
                column: "group_id");

            migrationBuilder.CreateIndex(
                name: "IX_groups_account_id",
                table: "groups",
                column: "account_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: " account_status_groups");

            migrationBuilder.DropTable(
                name: "groups");

            migrationBuilder.CreateTable(
                name: "comments",
                columns: table => new
                {
                    pk_group_id = table.Column<Guid>(type: "uuid", nullable: false),
                    account_id = table.Column<string>(type: "character varying", nullable: false),
                    create_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    name = table.Column<string>(type: "character varying", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_comments", x => x.pk_group_id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_comments_account_id",
                table: "comments",
                column: "account_id");
        }
    }
}
