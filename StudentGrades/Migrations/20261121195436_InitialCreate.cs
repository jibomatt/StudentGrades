using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace StudentGrades.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "mst_subject",
                columns: table => new
                {
                    subject_key = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    subject_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mst_subject", x => x.subject_key);
                });

            migrationBuilder.CreateTable(
                name: "mst_student",
                columns: table => new
                {
                    student_key = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    student_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    subject_key = table.Column<int>(type: "integer", nullable: false),
                    grade = table.Column<decimal>(type: "numeric(5,2)", nullable: false),
                    remarks = table.Column<string>(type: "character varying(4)", maxLength: 4, nullable: true, computedColumnSql: "CASE WHEN grade >= 75 THEN 'PASS' ELSE 'FAIL' END", stored: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mst_student", x => x.student_key);
                    table.ForeignKey(
                        name: "FK_mst_student_mst_subject_subject_key",
                        column: x => x.subject_key,
                        principalTable: "mst_subject",
                        principalColumn: "subject_key",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_mst_student_subject_key",
                table: "mst_student",
                column: "subject_key");

            migrationBuilder.CreateIndex(
                name: "IX_mst_subject_subject_name",
                table: "mst_subject",
                column: "subject_name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "mst_student");

            migrationBuilder.DropTable(
                name: "mst_subject");
        }
    }
}
