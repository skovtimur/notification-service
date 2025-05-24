using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NotificationService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence(
                name: "BaseEntitySequence");

            migrationBuilder.CreateTable(
                name: "BaseEntity",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, defaultValueSql: "nextval('\"BaseEntitySequence\"')"),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BaseEntity", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "tasks",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, defaultValueSql: "nextval('\"BaseEntitySequence\"')"),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    json_content = table.Column<string>(type: "text", nullable: false),
                    priority = table.Column<string>(type: "text", nullable: false),
                    status = table.Column<string>(type: "text", nullable: false),
                    must_begin_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    completed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tasks", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "recipients",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, defaultValueSql: "nextval('\"BaseEntitySequence\"')"),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    address = table.Column<string>(type: "text", nullable: false),
                    address_type = table.Column<string>(type: "text", nullable: false),
                    task_id = table.Column<long>(type: "bigint", nullable: false),
                    sending_status = table.Column<string>(type: "text", nullable: false, defaultValue: "None"),
                    error_text = table.Column<string>(type: "text", nullable: true),
                    completed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_recipients", x => x.id);
                    table.ForeignKey(
                        name: "FK_Recipients_Tasks",
                        column: x => x.task_id,
                        principalTable: "tasks",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_recipients_task_id",
                table: "recipients",
                column: "task_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BaseEntity");

            migrationBuilder.DropTable(
                name: "recipients");

            migrationBuilder.DropTable(
                name: "tasks");

            migrationBuilder.DropSequence(
                name: "BaseEntitySequence");
        }
    }
}
