using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TestRunResult",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Passed = table.Column<int>(type: "INTEGER", nullable: false),
                    Failed = table.Column<int>(type: "INTEGER", nullable: false),
                    Ignored = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestRunResult", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TestAssemblyResult",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AssemblyName = table.Column<string>(type: "TEXT", nullable: false),
                    Passed = table.Column<int>(type: "INTEGER", nullable: false),
                    Failed = table.Column<int>(type: "INTEGER", nullable: false),
                    Ignored = table.Column<int>(type: "INTEGER", nullable: false),
                    TestRunResultId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestAssemblyResult", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestAssemblyResult_TestRunResult_TestRunResultId",
                        column: x => x.TestRunResultId,
                        principalTable: "TestRunResult",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TestRuns",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LaunchTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ResultId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestRuns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestRuns_TestRunResult_ResultId",
                        column: x => x.ResultId,
                        principalTable: "TestRunResult",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TestClassResult",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TestClassName = table.Column<string>(type: "TEXT", nullable: false),
                    Passed = table.Column<int>(type: "INTEGER", nullable: false),
                    Failed = table.Column<int>(type: "INTEGER", nullable: false),
                    Ignored = table.Column<int>(type: "INTEGER", nullable: false),
                    Errored = table.Column<int>(type: "INTEGER", nullable: false),
                    TestAssemblyResultId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestClassResult", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestClassResult_TestAssemblyResult_TestAssemblyResultId",
                        column: x => x.TestAssemblyResultId,
                        principalTable: "TestAssemblyResult",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TestResult",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Message = table.Column<string>(type: "TEXT", nullable: false),
                    TimeElapsed = table.Column<TimeSpan>(type: "TEXT", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    TestClassResultId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestResult", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestResult_TestClassResult_TestClassResultId",
                        column: x => x.TestClassResultId,
                        principalTable: "TestClassResult",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_TestAssemblyResult_TestRunResultId",
                table: "TestAssemblyResult",
                column: "TestRunResultId");

            migrationBuilder.CreateIndex(
                name: "IX_TestClassResult_TestAssemblyResultId",
                table: "TestClassResult",
                column: "TestAssemblyResultId");

            migrationBuilder.CreateIndex(
                name: "IX_TestResult_TestClassResultId",
                table: "TestResult",
                column: "TestClassResultId");

            migrationBuilder.CreateIndex(
                name: "IX_TestRuns_ResultId",
                table: "TestRuns",
                column: "ResultId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TestResult");

            migrationBuilder.DropTable(
                name: "TestRuns");

            migrationBuilder.DropTable(
                name: "TestClassResult");

            migrationBuilder.DropTable(
                name: "TestAssemblyResult");

            migrationBuilder.DropTable(
                name: "TestRunResult");
        }
    }
}
