using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GRA.Data.SqlServer.Migrations
{
    public partial class addperformerregistration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PsAgeGroups",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedBy = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    Name = table.Column<string>(maxLength: 255, nullable: false),
                    IconColor = table.Column<string>(maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PsAgeGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PsBlackoutDates",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedBy = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    Reason = table.Column<string>(maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PsBlackoutDates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PsExcludeBranches",
                columns: table => new
                {
                    BranchId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PsExcludeBranches", x => x.BranchId);
                    table.ForeignKey(
                        name: "FK_PsExcludeBranches_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PsKits",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedBy = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    Name = table.Column<string>(maxLength: 255, nullable: false),
                    Description = table.Column<string>(maxLength: 2000, nullable: false),
                    Website = table.Column<string>(maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PsKits", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PsPerformers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedBy = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UserId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 255, nullable: false),
                    BillingAddress = table.Column<string>(maxLength: 500, nullable: false),
                    Website = table.Column<string>(maxLength: 255, nullable: true),
                    Email = table.Column<string>(maxLength: 255, nullable: false),
                    Phone = table.Column<string>(maxLength: 255, nullable: false),
                    PhonePreferred = table.Column<bool>(nullable: false),
                    VendorId = table.Column<string>(maxLength: 255, nullable: false),
                    HasFingerprintCard = table.Column<bool>(nullable: false),
                    AllBranches = table.Column<bool>(nullable: false),
                    ReferencesFilename = table.Column<string>(maxLength: 255, nullable: true),
                    SetSchedule = table.Column<bool>(nullable: false),
                    RegistrationCompleted = table.Column<bool>(nullable: false),
                    IsApproved = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PsPerformers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PsPerformers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PsSettings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedBy = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    SiteId = table.Column<int>(nullable: false),
                    ContactEmail = table.Column<string>(maxLength: 255, nullable: true),
                    SelectionsPerBranch = table.Column<int>(nullable: true),
                    RegistrationOpen = table.Column<DateTime>(nullable: true),
                    RegistrationClosed = table.Column<DateTime>(nullable: true),
                    SchedulingPreview = table.Column<DateTime>(nullable: true),
                    SchedulingOpen = table.Column<DateTime>(nullable: true),
                    SchedulingClosed = table.Column<DateTime>(nullable: true),
                    SchedulePosted = table.Column<DateTime>(nullable: true),
                    ScheduleStartDate = table.Column<DateTime>(nullable: true),
                    ScheduleEndDate = table.Column<DateTime>(nullable: true),
                    VendorCodeFormat = table.Column<string>(maxLength: 255, nullable: true),
                    BranchAvailabilitySuplimentalText = table.Column<string>(maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PsSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PsSettings_Sites_SiteId",
                        column: x => x.SiteId,
                        principalTable: "Sites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PsBackToBack",
                columns: table => new
                {
                    PsAgeGroupId = table.Column<int>(nullable: false),
                    BranchId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PsBackToBack", x => new { x.PsAgeGroupId, x.BranchId });
                    table.ForeignKey(
                        name: "FK_PsBackToBack_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PsBackToBack_PsAgeGroups_PsAgeGroupId",
                        column: x => x.PsAgeGroupId,
                        principalTable: "PsAgeGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PsKitAgeGroups",
                columns: table => new
                {
                    KitId = table.Column<int>(nullable: false),
                    AgeGroupId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PsKitAgeGroups", x => new { x.KitId, x.AgeGroupId });
                    table.ForeignKey(
                        name: "FK_PsKitAgeGroups_PsAgeGroups_AgeGroupId",
                        column: x => x.AgeGroupId,
                        principalTable: "PsAgeGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PsKitAgeGroups_PsKits_KitId",
                        column: x => x.KitId,
                        principalTable: "PsKits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PsKitImages",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedBy = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    KitId = table.Column<int>(nullable: false),
                    Filename = table.Column<string>(maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PsKitImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PsKitImages_PsKits_KitId",
                        column: x => x.KitId,
                        principalTable: "PsKits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PsPerformerBranches",
                columns: table => new
                {
                    PsPerformerId = table.Column<int>(nullable: false),
                    BranchId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PsPerformerBranches", x => new { x.PsPerformerId, x.BranchId });
                    table.ForeignKey(
                        name: "FK_PsPerformerBranches_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PsPerformerBranches_PsPerformers_PsPerformerId",
                        column: x => x.PsPerformerId,
                        principalTable: "PsPerformers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PsPerformerImages",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedBy = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    PerformerId = table.Column<int>(nullable: false),
                    Filename = table.Column<string>(maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PsPerformerImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PsPerformerImages_PsPerformers_PerformerId",
                        column: x => x.PerformerId,
                        principalTable: "PsPerformers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PsPerformerSchedules",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedBy = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    PerformerId = table.Column<int>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    StartTime = table.Column<DateTime>(nullable: true),
                    EndTime = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PsPerformerSchedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PsPerformerSchedules_PsPerformers_PerformerId",
                        column: x => x.PerformerId,
                        principalTable: "PsPerformers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PsPrograms",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedBy = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    PerformerId = table.Column<int>(nullable: false),
                    Title = table.Column<string>(maxLength: 50, nullable: false),
                    MinimumCapacity = table.Column<int>(nullable: false),
                    MaximumCapacity = table.Column<int>(nullable: false),
                    ProgramLengthMinutes = table.Column<int>(nullable: false),
                    SetupTimeMinutes = table.Column<int>(nullable: false),
                    BreakdownTimeMinutes = table.Column<int>(nullable: false),
                    BackToBackMinutes = table.Column<int>(nullable: false),
                    Description = table.Column<string>(maxLength: 375, nullable: false),
                    Cost = table.Column<decimal>(nullable: false),
                    Setup = table.Column<string>(maxLength: 1000, nullable: true),
                    AllowStreaming = table.Column<bool>(nullable: false),
                    AllowArchiving = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PsPrograms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PsPrograms_PsPerformers_PerformerId",
                        column: x => x.PerformerId,
                        principalTable: "PsPerformers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PsBranchSelections",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedBy = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UserId = table.Column<int>(nullable: false),
                    BranchId = table.Column<int>(nullable: false),
                    AgeGroupId = table.Column<int>(nullable: false),
                    ProgramId = table.Column<int>(nullable: true),
                    KitId = table.Column<int>(nullable: true),
                    RequestedStartTime = table.Column<DateTime>(nullable: false),
                    ScheduleStartTime = table.Column<DateTime>(nullable: false),
                    ScheduleDuration = table.Column<int>(nullable: false),
                    BackToBackProgram = table.Column<bool>(nullable: false),
                    SelectedAt = table.Column<DateTime>(nullable: false),
                    SecretCode = table.Column<string>(maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PsBranchSelections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PsBranchSelections_PsAgeGroups_AgeGroupId",
                        column: x => x.AgeGroupId,
                        principalTable: "PsAgeGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PsBranchSelections_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PsBranchSelections_PsKits_KitId",
                        column: x => x.KitId,
                        principalTable: "PsKits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PsBranchSelections_PsPrograms_ProgramId",
                        column: x => x.ProgramId,
                        principalTable: "PsPrograms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PsBranchSelections_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PsProgramAgeGroups",
                columns: table => new
                {
                    ProgramId = table.Column<int>(nullable: false),
                    AgeGroupId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PsProgramAgeGroups", x => new { x.ProgramId, x.AgeGroupId });
                    table.ForeignKey(
                        name: "FK_PsProgramAgeGroups_PsAgeGroups_AgeGroupId",
                        column: x => x.AgeGroupId,
                        principalTable: "PsAgeGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PsProgramAgeGroups_PsPrograms_ProgramId",
                        column: x => x.ProgramId,
                        principalTable: "PsPrograms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PsProgramImages",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedBy = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    ProgramId = table.Column<int>(nullable: false),
                    Filename = table.Column<string>(maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PsProgramImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PsProgramImages_PsPrograms_ProgramId",
                        column: x => x.ProgramId,
                        principalTable: "PsPrograms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PsBackToBack_BranchId",
                table: "PsBackToBack",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_PsBranchSelections_AgeGroupId",
                table: "PsBranchSelections",
                column: "AgeGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_PsBranchSelections_BranchId",
                table: "PsBranchSelections",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_PsBranchSelections_KitId",
                table: "PsBranchSelections",
                column: "KitId");

            migrationBuilder.CreateIndex(
                name: "IX_PsBranchSelections_ProgramId",
                table: "PsBranchSelections",
                column: "ProgramId");

            migrationBuilder.CreateIndex(
                name: "IX_PsBranchSelections_UserId",
                table: "PsBranchSelections",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PsKitAgeGroups_AgeGroupId",
                table: "PsKitAgeGroups",
                column: "AgeGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_PsKitImages_KitId",
                table: "PsKitImages",
                column: "KitId");

            migrationBuilder.CreateIndex(
                name: "IX_PsPerformerBranches_BranchId",
                table: "PsPerformerBranches",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_PsPerformerImages_PerformerId",
                table: "PsPerformerImages",
                column: "PerformerId");

            migrationBuilder.CreateIndex(
                name: "IX_PsPerformers_UserId",
                table: "PsPerformers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PsPerformerSchedules_PerformerId",
                table: "PsPerformerSchedules",
                column: "PerformerId");

            migrationBuilder.CreateIndex(
                name: "IX_PsProgramAgeGroups_AgeGroupId",
                table: "PsProgramAgeGroups",
                column: "AgeGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_PsProgramImages_ProgramId",
                table: "PsProgramImages",
                column: "ProgramId");

            migrationBuilder.CreateIndex(
                name: "IX_PsPrograms_PerformerId",
                table: "PsPrograms",
                column: "PerformerId");

            migrationBuilder.CreateIndex(
                name: "IX_PsSettings_SiteId",
                table: "PsSettings",
                column: "SiteId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PsBackToBack");

            migrationBuilder.DropTable(
                name: "PsBlackoutDates");

            migrationBuilder.DropTable(
                name: "PsBranchSelections");

            migrationBuilder.DropTable(
                name: "PsExcludeBranches");

            migrationBuilder.DropTable(
                name: "PsKitAgeGroups");

            migrationBuilder.DropTable(
                name: "PsKitImages");

            migrationBuilder.DropTable(
                name: "PsPerformerBranches");

            migrationBuilder.DropTable(
                name: "PsPerformerImages");

            migrationBuilder.DropTable(
                name: "PsPerformerSchedules");

            migrationBuilder.DropTable(
                name: "PsProgramAgeGroups");

            migrationBuilder.DropTable(
                name: "PsProgramImages");

            migrationBuilder.DropTable(
                name: "PsSettings");

            migrationBuilder.DropTable(
                name: "PsKits");

            migrationBuilder.DropTable(
                name: "PsAgeGroups");

            migrationBuilder.DropTable(
                name: "PsPrograms");

            migrationBuilder.DropTable(
                name: "PsPerformers");
        }
    }
}
