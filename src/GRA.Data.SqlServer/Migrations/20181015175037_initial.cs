using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace GRA.Data.SqlServer.Migrations
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    CurrentValue = table.Column<string>(nullable: true),
                    EntityId = table.Column<int>(nullable: false),
                    EntityType = table.Column<string>(maxLength: 255, nullable: false),
                    PreviousValue = table.Column<string>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AvatarBundles",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CanBeUnlocked = table.Column<bool>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    HasBeenAwarded = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(maxLength: 255, nullable: false),
                    SiteId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AvatarBundles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AvatarLayers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CanBeEmpty = table.Column<bool>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    DefaultLayer = table.Column<bool>(nullable: false),
                    GroupId = table.Column<int>(nullable: false),
                    Icon = table.Column<string>(maxLength: 255, nullable: true),
                    Name = table.Column<string>(maxLength: 255, nullable: false),
                    Position = table.Column<int>(nullable: false),
                    ShowColorSelector = table.Column<bool>(nullable: false),
                    ShowItemSelector = table.Column<bool>(nullable: false),
                    SiteId = table.Column<int>(nullable: false),
                    SortOrder = table.Column<int>(nullable: false),
                    ZoomScale = table.Column<decimal>(nullable: false),
                    ZoomYOffset = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AvatarLayers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Badges",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    Filename = table.Column<string>(nullable: true),
                    SiteId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Badges", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Books",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Author = table.Column<string>(maxLength: 255, nullable: true),
                    ChallengeId = table.Column<int>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    Isbn = table.Column<string>(maxLength: 30, nullable: true),
                    Title = table.Column<string>(maxLength: 500, nullable: false),
                    Url = table.Column<string>(maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Books", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Broadcasts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Body = table.Column<string>(maxLength: 1000, nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    SendAt = table.Column<DateTime>(nullable: false),
                    SendToNewUsers = table.Column<bool>(nullable: false),
                    SiteId = table.Column<int>(nullable: false),
                    Subject = table.Column<string>(maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Broadcasts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Color = table.Column<string>(maxLength: 10, nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    Description = table.Column<string>(maxLength: 500, nullable: true),
                    Name = table.Column<string>(maxLength: 255, nullable: false),
                    SiteId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChallengeGroups",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    Description = table.Column<string>(maxLength: 1000, nullable: false),
                    Name = table.Column<string>(maxLength: 255, nullable: false),
                    SiteId = table.Column<int>(nullable: false),
                    Stub = table.Column<string>(maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChallengeGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChallengeTaskTypes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ActivityCount = table.Column<int>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 255, nullable: false),
                    PointTranslationId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChallengeTaskTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DailyLiteracyTip",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    Message = table.Column<string>(maxLength: 50, nullable: false),
                    Name = table.Column<string>(maxLength: 255, nullable: false),
                    SiteId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyLiteracyTip", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DashboardContents",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Content = table.Column<string>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    SiteId = table.Column<int>(nullable: false),
                    StartTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DashboardContents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmailReminders",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    Email = table.Column<string>(nullable: false),
                    SignUpSource = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailReminders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GroupTypes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 255, nullable: false),
                    SiteId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Locations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Address = table.Column<string>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 255, nullable: false),
                    SiteId = table.Column<int>(nullable: false),
                    Telephone = table.Column<string>(maxLength: 50, nullable: true),
                    Url = table.Column<string>(maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Mails",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Body = table.Column<string>(maxLength: 2000, nullable: false),
                    CanParticipantDelete = table.Column<bool>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    DrawingId = table.Column<int>(nullable: true),
                    FromUserId = table.Column<int>(nullable: false),
                    InReplyToId = table.Column<int>(nullable: true),
                    IsBroadcast = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    IsNew = table.Column<bool>(nullable: false),
                    IsRepliedTo = table.Column<bool>(nullable: false),
                    SiteId = table.Column<int>(nullable: false),
                    Subject = table.Column<string>(maxLength: 500, nullable: false),
                    ThreadId = table.Column<int>(nullable: true),
                    ToUserId = table.Column<int>(nullable: true),
                    TriggerId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mails", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BadgeFilename = table.Column<string>(nullable: true),
                    BadgeId = table.Column<int>(nullable: true),
                    ChallengeId = table.Column<int>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    IsAchiever = table.Column<bool>(nullable: false),
                    IsJoiner = table.Column<bool>(nullable: false),
                    PointsEarned = table.Column<int>(nullable: true),
                    Text = table.Column<string>(nullable: false),
                    UserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Pages",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Content = table.Column<string>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    FooterText = table.Column<string>(maxLength: 255, nullable: true),
                    IsPublished = table.Column<bool>(nullable: false),
                    NavText = table.Column<string>(maxLength: 255, nullable: true),
                    SiteId = table.Column<int>(nullable: false),
                    Stub = table.Column<string>(maxLength: 255, nullable: true),
                    Title = table.Column<string>(maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Permissions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PointTranslations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ActivityAmount = table.Column<int>(nullable: false),
                    ActivityDescription = table.Column<string>(maxLength: 255, nullable: false),
                    ActivityDescriptionPlural = table.Column<string>(maxLength: 255, nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    IsSingleEvent = table.Column<bool>(nullable: false),
                    PointsEarned = table.Column<int>(nullable: false),
                    SiteId = table.Column<int>(nullable: false),
                    TranslationDescriptionPastTense = table.Column<string>(nullable: false),
                    TranslationDescriptionPresentTense = table.Column<string>(nullable: false),
                    TranslationName = table.Column<string>(maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PointTranslations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PsAgeGroups",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 255, nullable: true)
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
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    Reason = table.Column<string>(maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PsBlackoutDates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PsKits",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    Description = table.Column<string>(maxLength: 2000, nullable: false),
                    Name = table.Column<string>(maxLength: 255, nullable: false),
                    Website = table.Column<string>(maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PsKits", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Questionnaires",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BadgeId = table.Column<int>(nullable: true),
                    BadgeName = table.Column<string>(maxLength: 255, nullable: true),
                    BadgeNotificationMessage = table.Column<string>(maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    IsLocked = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(maxLength: 255, nullable: false),
                    RelatedBranchId = table.Column<int>(nullable: false),
                    RelatedSystemId = table.Column<int>(nullable: false),
                    SiteId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Questionnaires", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RecoveryTokens",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    Token = table.Column<string>(maxLength: 255, nullable: false),
                    UserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecoveryTokens", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReportCriteria",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BadgeRequiredList = table.Column<string>(nullable: true),
                    BranchId = table.Column<int>(nullable: true),
                    ChallengeRequiredList = table.Column<string>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: true),
                    Favorite = table.Column<bool>(nullable: false),
                    GroupInfoId = table.Column<int>(nullable: true),
                    Name = table.Column<string>(maxLength: 255, nullable: true),
                    ProgramId = table.Column<int>(nullable: true),
                    SchoolDistrictId = table.Column<int>(nullable: true),
                    SchoolId = table.Column<int>(nullable: true),
                    SiteId = table.Column<int>(nullable: false),
                    StartDate = table.Column<DateTime>(nullable: true),
                    SystemId = table.Column<int>(nullable: true),
                    VendorCodeTypeId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportCriteria", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReportRequests",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    Favorite = table.Column<bool>(nullable: false),
                    Finished = table.Column<DateTime>(nullable: true),
                    InstanceName = table.Column<string>(maxLength: 255, nullable: true),
                    Name = table.Column<string>(maxLength: 255, nullable: true),
                    ReportCriteriaId = table.Column<int>(nullable: false),
                    ReportId = table.Column<int>(nullable: false),
                    ResultJson = table.Column<string>(nullable: true),
                    Started = table.Column<DateTime>(nullable: true),
                    Success = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportRequests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RequiredQuestionnaires",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AgeMaximum = table.Column<int>(nullable: true),
                    AgeMinimum = table.Column<int>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: true),
                    QuestionnaireId = table.Column<int>(nullable: false),
                    SiteId = table.Column<int>(nullable: false),
                    StartDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequiredQuestionnaires", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    IsAdmin = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SchoolDistricts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    IsCharter = table.Column<bool>(nullable: false),
                    IsPrivate = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(maxLength: 255, nullable: false),
                    SiteId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SchoolDistricts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SchoolTypes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 255, nullable: false),
                    SiteId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SchoolTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sites",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AccessClosed = table.Column<DateTime>(nullable: true),
                    AccessClosedPage = table.Column<int>(nullable: true),
                    AvatarCardDescription = table.Column<string>(maxLength: 150, nullable: true),
                    BeforeRegistrationPage = table.Column<int>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    Domain = table.Column<string>(maxLength: 255, nullable: true),
                    ExternalEventListUrl = table.Column<string>(nullable: true),
                    FacebookAppId = table.Column<string>(maxLength: 100, nullable: true),
                    FacebookImageUrl = table.Column<string>(maxLength: 255, nullable: true),
                    Footer = table.Column<string>(nullable: true),
                    FromEmailAddress = table.Column<string>(nullable: true),
                    FromEmailName = table.Column<string>(nullable: true),
                    GoogleAnalyticsTrackingId = table.Column<string>(nullable: true),
                    IsDefault = table.Column<bool>(nullable: false),
                    IsHttpsForced = table.Column<bool>(nullable: false),
                    MaxPointsPerChallengeTask = table.Column<int>(nullable: true),
                    MetaDescription = table.Column<string>(maxLength: 150, nullable: true),
                    Name = table.Column<string>(maxLength: 255, nullable: false),
                    OutgoingMailHost = table.Column<string>(nullable: true),
                    OutgoingMailLogin = table.Column<string>(nullable: true),
                    OutgoingMailPassword = table.Column<string>(nullable: true),
                    OutgoingMailPort = table.Column<int>(nullable: true),
                    PageTitle = table.Column<string>(maxLength: 255, nullable: false),
                    Path = table.Column<string>(maxLength: 255, nullable: false),
                    ProgramEndedPage = table.Column<int>(nullable: true),
                    ProgramEnds = table.Column<DateTime>(nullable: true),
                    ProgramOpenPage = table.Column<int>(nullable: true),
                    ProgramStarts = table.Column<DateTime>(nullable: true),
                    RegistrationOpenPage = table.Column<int>(nullable: true),
                    RegistrationOpens = table.Column<DateTime>(nullable: true),
                    RequirePostalCode = table.Column<bool>(nullable: false),
                    SinglePageSignUp = table.Column<bool>(nullable: false),
                    SiteLogoUrl = table.Column<string>(maxLength: 255, nullable: true),
                    TwitterAvatarHashtags = table.Column<string>(maxLength: 100, nullable: true),
                    TwitterAvatarMessage = table.Column<string>(maxLength: 255, nullable: true),
                    TwitterCardImageUrl = table.Column<string>(maxLength: 255, nullable: true),
                    TwitterLargeCard = table.Column<bool>(nullable: true),
                    TwitterUsername = table.Column<string>(maxLength: 15, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sites", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SiteSettings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    Key = table.Column<string>(maxLength: 255, nullable: false),
                    SiteId = table.Column<int>(nullable: false),
                    Value = table.Column<string>(maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiteSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AvatarColors",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AvatarLayerId = table.Column<int>(nullable: false),
                    Color = table.Column<string>(maxLength: 15, nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    SortOrder = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AvatarColors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AvatarColors_AvatarLayers_AvatarLayerId",
                        column: x => x.AvatarLayerId,
                        principalTable: "AvatarLayers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AvatarItems",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AvatarLayerId = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 255, nullable: false),
                    SortOrder = table.Column<int>(nullable: false),
                    Thumbnail = table.Column<string>(maxLength: 255, nullable: true),
                    Unlockable = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AvatarItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AvatarItems_AvatarLayers_AvatarLayerId",
                        column: x => x.AvatarLayerId,
                        principalTable: "AvatarLayers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DailyLiteracyTipImage",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    DailyLiteracyTipId = table.Column<int>(nullable: false),
                    Day = table.Column<int>(nullable: false),
                    Extension = table.Column<string>(maxLength: 10, nullable: false),
                    Name = table.Column<string>(maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyLiteracyTipImage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DailyLiteracyTipImage_DailyLiteracyTip_DailyLiteracyTipId",
                        column: x => x.DailyLiteracyTipId,
                        principalTable: "DailyLiteracyTip",
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
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    Filename = table.Column<string>(maxLength: 255, nullable: true),
                    KitId = table.Column<int>(nullable: false)
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
                name: "Questions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CorrectAnswerId = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(maxLength: 255, nullable: false),
                    QuestionnaireId = table.Column<int>(nullable: false),
                    SortOrder = table.Column<int>(nullable: false),
                    Text = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Questions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Questions_Questionnaires_QuestionnaireId",
                        column: x => x.QuestionnaireId,
                        principalTable: "Questionnaires",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AuthorizationCodes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(maxLength: 255, nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    IsSingleUse = table.Column<bool>(nullable: false),
                    RoleId = table.Column<int>(nullable: false),
                    SiteId = table.Column<int>(nullable: false),
                    Uses = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthorizationCodes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuthorizationCodes_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RolePermissions",
                columns: table => new
                {
                    RoleId = table.Column<int>(nullable: false),
                    PermissionId = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermissions", x => new { x.RoleId, x.PermissionId });
                    table.ForeignKey(
                        name: "FK_RolePermissions_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RolePermissions_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Schools",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 255, nullable: false),
                    SchoolDistrictId = table.Column<int>(nullable: false),
                    SchoolTypeId = table.Column<int>(nullable: true),
                    SiteId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Schools", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Schools_SchoolDistricts_SchoolDistrictId",
                        column: x => x.SchoolDistrictId,
                        principalTable: "SchoolDistricts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Schools_SchoolTypes_SchoolTypeId",
                        column: x => x.SchoolTypeId,
                        principalTable: "SchoolTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Challenges",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BadgeId = table.Column<int>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    Description = table.Column<string>(maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    IsValid = table.Column<bool>(nullable: false),
                    LimitToBranchId = table.Column<int>(nullable: true),
                    LimitToProgramId = table.Column<int>(nullable: true),
                    LimitToSystemId = table.Column<int>(nullable: true),
                    Name = table.Column<string>(maxLength: 255, nullable: false),
                    PointsAwarded = table.Column<int>(nullable: false),
                    RelatedBranchId = table.Column<int>(nullable: false),
                    RelatedSystemId = table.Column<int>(nullable: false),
                    SiteId = table.Column<int>(nullable: false),
                    TasksToComplete = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Challenges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Challenges_Sites_SiteId",
                        column: x => x.SiteId,
                        principalTable: "Sites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Programs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AchieverPointAmount = table.Column<int>(nullable: false),
                    AgeMaximum = table.Column<int>(nullable: true),
                    AgeMinimum = table.Column<int>(nullable: true),
                    AgeRequired = table.Column<bool>(nullable: false),
                    AskAge = table.Column<bool>(nullable: false),
                    AskSchool = table.Column<bool>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    DailyLiteracyTipId = table.Column<int>(nullable: true),
                    JoinBadgeId = table.Column<int>(nullable: true),
                    JoinBadgeName = table.Column<string>(maxLength: 255, nullable: true),
                    Name = table.Column<string>(maxLength: 255, nullable: false),
                    PointTranslationId = table.Column<int>(nullable: false),
                    Position = table.Column<int>(nullable: false),
                    SchoolRequired = table.Column<bool>(nullable: false),
                    SiteId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Programs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Programs_DailyLiteracyTip_DailyLiteracyTipId",
                        column: x => x.DailyLiteracyTipId,
                        principalTable: "DailyLiteracyTip",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Programs_PointTranslations_PointTranslationId",
                        column: x => x.PointTranslationId,
                        principalTable: "PointTranslations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Programs_Sites_SiteId",
                        column: x => x.SiteId,
                        principalTable: "Sites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PsDates",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    RegistrationClosed = table.Column<DateTime>(nullable: true),
                    RegistrationOpen = table.Column<DateTime>(nullable: true),
                    ScheduleEndDate = table.Column<DateTime>(nullable: true),
                    SchedulePosted = table.Column<DateTime>(nullable: true),
                    ScheduleStartDate = table.Column<DateTime>(nullable: true),
                    SchedulingClosed = table.Column<DateTime>(nullable: true),
                    SchedulingOpen = table.Column<DateTime>(nullable: true),
                    SchedulingPreview = table.Column<DateTime>(nullable: true),
                    SiteId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PsDates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PsDates_Sites_SiteId",
                        column: x => x.SiteId,
                        principalTable: "Sites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Systems",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 255, nullable: false),
                    SiteId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Systems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Systems_Sites_SiteId",
                        column: x => x.SiteId,
                        principalTable: "Sites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VendorCodeTypes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    Description = table.Column<string>(maxLength: 255, nullable: false),
                    DonationMail = table.Column<string>(maxLength: 1250, nullable: true),
                    DonationMessage = table.Column<string>(maxLength: 255, nullable: true),
                    DonationOptionMail = table.Column<string>(maxLength: 1250, nullable: true),
                    DonationOptionSubject = table.Column<string>(nullable: true),
                    DonationSubject = table.Column<string>(maxLength: 255, nullable: true),
                    Mail = table.Column<string>(maxLength: 1250, nullable: false),
                    MailSubject = table.Column<string>(maxLength: 255, nullable: false),
                    SiteId = table.Column<int>(nullable: false),
                    Url = table.Column<string>(maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VendorCodeTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VendorCodeTypes_Sites_SiteId",
                        column: x => x.SiteId,
                        principalTable: "Sites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AvatarBundleItems",
                columns: table => new
                {
                    AvatarBundleId = table.Column<int>(nullable: false),
                    AvatarItemId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AvatarBundleItems", x => new { x.AvatarBundleId, x.AvatarItemId });
                    table.ForeignKey(
                        name: "FK_AvatarBundleItems_AvatarBundles_AvatarBundleId",
                        column: x => x.AvatarBundleId,
                        principalTable: "AvatarBundles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AvatarBundleItems_AvatarItems_AvatarItemId",
                        column: x => x.AvatarItemId,
                        principalTable: "AvatarItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AvatarElements",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AvatarColorId = table.Column<int>(nullable: true),
                    AvatarItemId = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    Filename = table.Column<string>(maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AvatarElements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AvatarElements_AvatarColors_AvatarColorId",
                        column: x => x.AvatarColorId,
                        principalTable: "AvatarColors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AvatarElements_AvatarItems_AvatarItemId",
                        column: x => x.AvatarItemId,
                        principalTable: "AvatarItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Answers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    QuestionId = table.Column<int>(nullable: false),
                    SortOrder = table.Column<int>(nullable: false),
                    Text = table.Column<string>(maxLength: 1500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Answers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Answers_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ChallengeCategories",
                columns: table => new
                {
                    ChallengeId = table.Column<int>(nullable: false),
                    CategoryId = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChallengeCategories", x => new { x.ChallengeId, x.CategoryId });
                    table.ForeignKey(
                        name: "FK_ChallengeCategories_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChallengeCategories_Challenges_ChallengeId",
                        column: x => x.ChallengeId,
                        principalTable: "Challenges",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ChallengeGroupChallenges",
                columns: table => new
                {
                    ChallengeGroupId = table.Column<int>(nullable: false),
                    ChallengeId = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChallengeGroupChallenges", x => new { x.ChallengeGroupId, x.ChallengeId });
                    table.ForeignKey(
                        name: "FK_ChallengeGroupChallenges_ChallengeGroups_ChallengeGroupId",
                        column: x => x.ChallengeGroupId,
                        principalTable: "ChallengeGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChallengeGroupChallenges_Challenges_ChallengeId",
                        column: x => x.ChallengeId,
                        principalTable: "Challenges",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ChallengeTasks",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Author = table.Column<string>(maxLength: 255, nullable: true),
                    ChallengeId = table.Column<int>(nullable: false),
                    ChallengeTaskTypeId = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    Filename = table.Column<string>(maxLength: 255, nullable: true),
                    Isbn = table.Column<string>(maxLength: 30, nullable: true),
                    Position = table.Column<int>(nullable: false),
                    Title = table.Column<string>(nullable: false),
                    Url = table.Column<string>(maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChallengeTasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChallengeTasks_Challenges_ChallengeId",
                        column: x => x.ChallengeId,
                        principalTable: "Challenges",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Events",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AllDay = table.Column<bool>(nullable: false),
                    AtBranchId = table.Column<int>(nullable: true),
                    AtLocationId = table.Column<int>(nullable: true),
                    ChallengeGroupId = table.Column<int>(nullable: true),
                    ChallengeId = table.Column<int>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    Description = table.Column<string>(maxLength: 1500, nullable: false),
                    EndDate = table.Column<DateTime>(nullable: true),
                    ExternalLink = table.Column<string>(maxLength: 300, nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    IsCommunityExperience = table.Column<bool>(nullable: false),
                    IsValid = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(maxLength: 255, nullable: false),
                    ParentEventId = table.Column<int>(nullable: true),
                    ProgramId = table.Column<int>(nullable: true),
                    RelatedBranchId = table.Column<int>(nullable: false),
                    RelatedSystemId = table.Column<int>(nullable: false),
                    RelatedTriggerId = table.Column<int>(nullable: true),
                    SiteId = table.Column<int>(nullable: false),
                    StartDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Events_ChallengeGroups_ChallengeGroupId",
                        column: x => x.ChallengeGroupId,
                        principalTable: "ChallengeGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Events_Challenges_ChallengeId",
                        column: x => x.ChallengeId,
                        principalTable: "Challenges",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Branches",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Address = table.Column<string>(maxLength: 255, nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 255, nullable: false),
                    SystemId = table.Column<int>(nullable: false),
                    Telephone = table.Column<string>(maxLength: 50, nullable: true),
                    Url = table.Column<string>(maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Branches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Branches_Systems_SystemId",
                        column: x => x.SystemId,
                        principalTable: "Systems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VendorCodes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    DateUsed = table.Column<DateTime>(nullable: false),
                    IsDonated = table.Column<bool>(nullable: true),
                    IsUsed = table.Column<bool>(nullable: false),
                    OrderDate = table.Column<DateTime>(nullable: true),
                    ShipDate = table.Column<DateTime>(nullable: true),
                    SiteId = table.Column<int>(nullable: false),
                    UserId = table.Column<int>(nullable: true),
                    VendorCodeTypeId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VendorCodes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VendorCodes_VendorCodeTypes_VendorCodeTypeId",
                        column: x => x.VendorCodeTypeId,
                        principalTable: "VendorCodeTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DrawingCriteria",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BranchId = table.Column<int>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    EndOfPeriod = table.Column<DateTime>(nullable: true),
                    ExcludePreviousWinners = table.Column<bool>(nullable: false),
                    IncludeAdmin = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(maxLength: 255, nullable: false),
                    PointsMaximum = table.Column<int>(nullable: true),
                    PointsMinimum = table.Column<int>(nullable: true),
                    ProgramId = table.Column<int>(nullable: true),
                    ReadABook = table.Column<bool>(nullable: false),
                    RelatedBranchId = table.Column<int>(nullable: false),
                    RelatedSystemId = table.Column<int>(nullable: false),
                    SiteId = table.Column<int>(nullable: false),
                    StartOfPeriod = table.Column<DateTime>(nullable: true),
                    SystemId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DrawingCriteria", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DrawingCriteria_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DrawingCriteria_Systems_SystemId",
                        column: x => x.SystemId,
                        principalTable: "Systems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Triggers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ActivationDate = table.Column<DateTime>(nullable: true),
                    AwardAvatarBundleId = table.Column<int>(nullable: true),
                    AwardBadgeId = table.Column<int>(nullable: false),
                    AwardMail = table.Column<string>(maxLength: 2000, nullable: true),
                    AwardMailSubject = table.Column<string>(maxLength: 500, nullable: true),
                    AwardMessage = table.Column<string>(maxLength: 1000, nullable: false),
                    AwardPoints = table.Column<int>(nullable: false),
                    AwardPrizeName = table.Column<string>(maxLength: 255, nullable: true),
                    AwardPrizeRedemptionInstructions = table.Column<string>(maxLength: 1000, nullable: true),
                    AwardVendorCodeTypeId = table.Column<int>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ItemsRequired = table.Column<int>(nullable: false),
                    LimitToBranchId = table.Column<int>(nullable: true),
                    LimitToProgramId = table.Column<int>(nullable: true),
                    LimitToSystemId = table.Column<int>(nullable: true),
                    Name = table.Column<string>(maxLength: 255, nullable: false),
                    Points = table.Column<int>(nullable: false),
                    RelatedBranchId = table.Column<int>(nullable: false),
                    RelatedSystemId = table.Column<int>(nullable: false),
                    SecretCode = table.Column<string>(maxLength: 50, nullable: true),
                    SiteId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Triggers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Triggers_AvatarBundles_AwardAvatarBundleId",
                        column: x => x.AwardAvatarBundleId,
                        principalTable: "AvatarBundles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Triggers_Badges_AwardBadgeId",
                        column: x => x.AwardBadgeId,
                        principalTable: "Badges",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Triggers_VendorCodeTypes_AwardVendorCodeTypeId",
                        column: x => x.AwardVendorCodeTypeId,
                        principalTable: "VendorCodeTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Triggers_Branches_LimitToBranchId",
                        column: x => x.LimitToBranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Triggers_Programs_LimitToProgramId",
                        column: x => x.LimitToProgramId,
                        principalTable: "Programs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Triggers_Systems_LimitToSystemId",
                        column: x => x.LimitToSystemId,
                        principalTable: "Systems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AchievedAt = table.Column<DateTime>(nullable: true),
                    Age = table.Column<int>(nullable: true),
                    BranchId = table.Column<int>(nullable: false),
                    CanBeDeleted = table.Column<bool>(nullable: false),
                    CardNumber = table.Column<string>(maxLength: 64, nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    DailyPersonalGoal = table.Column<int>(nullable: true),
                    Email = table.Column<string>(maxLength: 254, nullable: true),
                    FirstName = table.Column<string>(maxLength: 255, nullable: false),
                    HouseholdHeadUserId = table.Column<int>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    IsAdmin = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    IsFirstTime = table.Column<bool>(nullable: false),
                    IsHomeschooled = table.Column<bool>(nullable: false),
                    IsLockedOut = table.Column<bool>(nullable: false),
                    LastAccess = table.Column<DateTime>(nullable: true),
                    LastActivityDate = table.Column<DateTime>(nullable: true),
                    LastBroadcast = table.Column<DateTime>(nullable: true),
                    LastName = table.Column<string>(maxLength: 255, nullable: true),
                    LockedOutAt = table.Column<DateTime>(nullable: false),
                    LockedOutFor = table.Column<string>(nullable: true),
                    PasswordHash = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(maxLength: 15, nullable: true),
                    PointsEarned = table.Column<int>(nullable: false),
                    PostalCode = table.Column<string>(maxLength: 32, nullable: true),
                    PreregistrationReminderRequested = table.Column<bool>(nullable: false),
                    ProgramId = table.Column<int>(nullable: false),
                    SchoolId = table.Column<int>(nullable: true),
                    SchoolNotListed = table.Column<bool>(nullable: false),
                    SiteId = table.Column<int>(nullable: false),
                    SystemId = table.Column<int>(nullable: false),
                    Username = table.Column<string>(maxLength: 36, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Users_Programs_ProgramId",
                        column: x => x.ProgramId,
                        principalTable: "Programs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Users_Sites_SiteId",
                        column: x => x.SiteId,
                        principalTable: "Sites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Users_Systems_SystemId",
                        column: x => x.SystemId,
                        principalTable: "Systems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Drawings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    DrawingCriterionId = table.Column<int>(nullable: false),
                    IsArchived = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(maxLength: 255, nullable: false),
                    NotificationMessage = table.Column<string>(maxLength: 2000, nullable: true),
                    NotificationSubject = table.Column<string>(maxLength: 255, nullable: true),
                    RedemptionInstructions = table.Column<string>(maxLength: 1000, nullable: true),
                    RelatedBranchId = table.Column<int>(nullable: false),
                    RelatedSystemId = table.Column<int>(nullable: false),
                    WinnerCount = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Drawings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Drawings_DrawingCriteria_DrawingCriterionId",
                        column: x => x.DrawingCriterionId,
                        principalTable: "DrawingCriteria",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DrawingCriterionPrograms",
                columns: table => new
                {
                    DrawingCriterionId = table.Column<int>(nullable: false),
                    ProgramId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DrawingCriterionPrograms", x => new { x.DrawingCriterionId, x.ProgramId });
                    table.ForeignKey(
                        name: "FK_DrawingCriterionPrograms_DrawingCriteria_DrawingCriterionId",
                        column: x => x.DrawingCriterionId,
                        principalTable: "DrawingCriteria",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TriggerBadges",
                columns: table => new
                {
                    TriggerId = table.Column<int>(nullable: false),
                    BadgeId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TriggerBadges", x => new { x.TriggerId, x.BadgeId });
                    table.ForeignKey(
                        name: "FK_TriggerBadges_Badges_BadgeId",
                        column: x => x.BadgeId,
                        principalTable: "Badges",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TriggerBadges_Triggers_TriggerId",
                        column: x => x.TriggerId,
                        principalTable: "Triggers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TriggerChallenges",
                columns: table => new
                {
                    TriggerId = table.Column<int>(nullable: false),
                    ChallengeId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TriggerChallenges", x => new { x.TriggerId, x.ChallengeId });
                    table.ForeignKey(
                        name: "FK_TriggerChallenges_Challenges_ChallengeId",
                        column: x => x.ChallengeId,
                        principalTable: "Challenges",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TriggerChallenges_Triggers_TriggerId",
                        column: x => x.TriggerId,
                        principalTable: "Triggers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GroupInfos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    GroupTypeId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 255, nullable: false),
                    UserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupInfos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GroupInfos_GroupTypes_GroupTypeId",
                        column: x => x.GroupTypeId,
                        principalTable: "GroupTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GroupInfos_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PsPerformers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AllBranches = table.Column<bool>(nullable: false),
                    BillingAddress = table.Column<string>(maxLength: 500, nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    Email = table.Column<string>(maxLength: 255, nullable: false),
                    HasFingerprintCard = table.Column<bool>(nullable: false),
                    IsApproved = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(maxLength: 255, nullable: false),
                    Phone = table.Column<string>(maxLength: 255, nullable: false),
                    PhonePreferred = table.Column<bool>(nullable: false),
                    ReferencesFilename = table.Column<string>(maxLength: 255, nullable: true),
                    RegistrationCompleted = table.Column<bool>(nullable: false),
                    SetSchedule = table.Column<bool>(nullable: false),
                    UserId = table.Column<int>(nullable: false),
                    VendorId = table.Column<string>(maxLength: 255, nullable: false),
                    Website = table.Column<string>(maxLength: 255, nullable: true)
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
                name: "UserAnswers",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false),
                    AnswerId = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAnswers", x => new { x.UserId, x.AnswerId });
                    table.ForeignKey(
                        name: "FK_UserAnswers_Answers_AnswerId",
                        column: x => x.AnswerId,
                        principalTable: "Answers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserAnswers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserAvatars",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false),
                    AvatarElementId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAvatars", x => new { x.UserId, x.AvatarElementId });
                    table.ForeignKey(
                        name: "FK_UserAvatars_AvatarElements_AvatarElementId",
                        column: x => x.AvatarElementId,
                        principalTable: "AvatarElements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserAvatars_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserAvatarItems",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false),
                    AvatarItemId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAvatarItems", x => new { x.UserId, x.AvatarItemId });
                    table.ForeignKey(
                        name: "FK_UserAvatarItems_AvatarItems_AvatarItemId",
                        column: x => x.AvatarItemId,
                        principalTable: "AvatarItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserAvatarItems_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserBadges",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false),
                    BadgeId = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserBadges", x => new { x.UserId, x.BadgeId });
                    table.ForeignKey(
                        name: "FK_UserBadges_Badges_BadgeId",
                        column: x => x.BadgeId,
                        principalTable: "Badges",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserBadges_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserBooks",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false),
                    BookId = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserBooks", x => new { x.UserId, x.BookId });
                    table.ForeignKey(
                        name: "FK_UserBooks_Books_BookId",
                        column: x => x.BookId,
                        principalTable: "Books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserBooks_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserChallengeTasks",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false),
                    ChallengeTaskId = table.Column<int>(nullable: false),
                    BookId = table.Column<int>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    IsCompleted = table.Column<bool>(nullable: false),
                    UserLogId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserChallengeTasks", x => new { x.UserId, x.ChallengeTaskId });
                    table.ForeignKey(
                        name: "FK_UserChallengeTasks_ChallengeTasks_ChallengeTaskId",
                        column: x => x.ChallengeTaskId,
                        principalTable: "ChallengeTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserChallengeTasks_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserFavoriteChallenges",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false),
                    ChallengeId = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFavoriteChallenges", x => new { x.UserId, x.ChallengeId });
                    table.ForeignKey(
                        name: "FK_UserFavoriteChallenges_Challenges_ChallengeId",
                        column: x => x.ChallengeId,
                        principalTable: "Challenges",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserFavoriteChallenges_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserLogs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ActivityEarned = table.Column<int>(nullable: true),
                    AvatarBundleId = table.Column<int>(nullable: true),
                    AwardedBy = table.Column<int>(nullable: true),
                    BadgeId = table.Column<int>(nullable: true),
                    ChallengeId = table.Column<int>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    DeletedBy = table.Column<int>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    PointTranslationId = table.Column<int>(nullable: true),
                    PointsEarned = table.Column<int>(nullable: false),
                    UserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserLogs_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserQuestionnaires",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false),
                    QuestionnaireId = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserQuestionnaires", x => new { x.UserId, x.QuestionnaireId });
                    table.ForeignKey(
                        name: "FK_UserQuestionnaires_Questionnaires_QuestionnaireId",
                        column: x => x.QuestionnaireId,
                        principalTable: "Questionnaires",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserQuestionnaires_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false),
                    RoleId = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserTriggers",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false),
                    TriggerId = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTriggers", x => new { x.UserId, x.TriggerId });
                    table.ForeignKey(
                        name: "FK_UserTriggers_Triggers_TriggerId",
                        column: x => x.TriggerId,
                        principalTable: "Triggers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserTriggers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PrizeWinners",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    DrawingId = table.Column<int>(nullable: true),
                    MailId = table.Column<int>(nullable: true),
                    RedeemedAt = table.Column<DateTime>(nullable: true),
                    RedeemedBy = table.Column<int>(nullable: true),
                    SiteId = table.Column<int>(nullable: false),
                    TriggerId = table.Column<int>(nullable: true),
                    UserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrizeWinners", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PrizeWinners_Drawings_DrawingId",
                        column: x => x.DrawingId,
                        principalTable: "Drawings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PrizeWinners_Triggers_TriggerId",
                        column: x => x.TriggerId,
                        principalTable: "Triggers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PrizeWinners_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
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
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    Filename = table.Column<string>(maxLength: 255, nullable: true),
                    PerformerId = table.Column<int>(nullable: false)
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
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    EndTime = table.Column<DateTime>(nullable: true),
                    PerformerId = table.Column<int>(nullable: false),
                    StartTime = table.Column<DateTime>(nullable: true)
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
                    AllowArchiving = table.Column<bool>(nullable: false),
                    AllowStreaming = table.Column<bool>(nullable: false),
                    BackToBackMinutes = table.Column<int>(nullable: false),
                    BreakdownTimeMinutes = table.Column<int>(nullable: false),
                    Cost = table.Column<decimal>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    Description = table.Column<string>(maxLength: 375, nullable: false),
                    MaximumCapacity = table.Column<int>(nullable: false),
                    MinimumCapacity = table.Column<int>(nullable: false),
                    PerformerId = table.Column<int>(nullable: false),
                    ProgramLengthMinutes = table.Column<int>(nullable: false),
                    Setup = table.Column<string>(maxLength: 1000, nullable: true),
                    SetupTimeMinutes = table.Column<int>(nullable: false),
                    Title = table.Column<string>(maxLength: 50, nullable: false)
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
                    AgeGroupId = table.Column<int>(nullable: false),
                    BackToBackProgram = table.Column<bool>(nullable: false),
                    BranchId = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    KitId = table.Column<int>(nullable: true),
                    ProgramId = table.Column<int>(nullable: true),
                    RequestedStartTime = table.Column<DateTime>(nullable: false),
                    ScheduleDuration = table.Column<int>(nullable: false),
                    ScheduleStartTime = table.Column<DateTime>(nullable: false),
                    SecretCode = table.Column<string>(maxLength: 50, nullable: true),
                    SelectedAt = table.Column<DateTime>(nullable: false),
                    UserId = table.Column<int>(nullable: false)
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
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    Filename = table.Column<string>(maxLength: 255, nullable: true),
                    ProgramId = table.Column<int>(nullable: false)
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
                name: "IX_Answers_QuestionId",
                table: "Answers",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_AuthorizationCodes_RoleId",
                table: "AuthorizationCodes",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_AvatarBundleItems_AvatarItemId",
                table: "AvatarBundleItems",
                column: "AvatarItemId");

            migrationBuilder.CreateIndex(
                name: "IX_AvatarColors_AvatarLayerId",
                table: "AvatarColors",
                column: "AvatarLayerId");

            migrationBuilder.CreateIndex(
                name: "IX_AvatarElements_AvatarColorId",
                table: "AvatarElements",
                column: "AvatarColorId");

            migrationBuilder.CreateIndex(
                name: "IX_AvatarElements_AvatarItemId",
                table: "AvatarElements",
                column: "AvatarItemId");

            migrationBuilder.CreateIndex(
                name: "IX_AvatarItems_AvatarLayerId",
                table: "AvatarItems",
                column: "AvatarLayerId");

            migrationBuilder.CreateIndex(
                name: "IX_Branches_SystemId",
                table: "Branches",
                column: "SystemId");

            migrationBuilder.CreateIndex(
                name: "IX_Challenges_SiteId",
                table: "Challenges",
                column: "SiteId");

            migrationBuilder.CreateIndex(
                name: "IX_ChallengeCategories_CategoryId",
                table: "ChallengeCategories",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ChallengeGroupChallenges_ChallengeId",
                table: "ChallengeGroupChallenges",
                column: "ChallengeId");

            migrationBuilder.CreateIndex(
                name: "IX_ChallengeTasks_ChallengeId",
                table: "ChallengeTasks",
                column: "ChallengeId");

            migrationBuilder.CreateIndex(
                name: "IX_DailyLiteracyTipImage_DailyLiteracyTipId",
                table: "DailyLiteracyTipImage",
                column: "DailyLiteracyTipId");

            migrationBuilder.CreateIndex(
                name: "IX_Drawings_DrawingCriterionId",
                table: "Drawings",
                column: "DrawingCriterionId");

            migrationBuilder.CreateIndex(
                name: "IX_DrawingCriteria_BranchId",
                table: "DrawingCriteria",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_DrawingCriteria_SystemId",
                table: "DrawingCriteria",
                column: "SystemId");

            migrationBuilder.CreateIndex(
                name: "IX_EmailReminders_Email_SignUpSource",
                table: "EmailReminders",
                columns: new[] { "Email", "SignUpSource" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Events_ChallengeGroupId",
                table: "Events",
                column: "ChallengeGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Events_ChallengeId",
                table: "Events",
                column: "ChallengeId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupInfos_GroupTypeId",
                table: "GroupInfos",
                column: "GroupTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupInfos_UserId",
                table: "GroupInfos",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Mails_ToUserId_IsDeleted_IsNew",
                table: "Mails",
                columns: new[] { "ToUserId", "IsDeleted", "IsNew" });

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId",
                table: "Notifications",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Pages_SiteId_Stub",
                table: "Pages",
                columns: new[] { "SiteId", "Stub" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PrizeWinners_TriggerId",
                table: "PrizeWinners",
                column: "TriggerId");

            migrationBuilder.CreateIndex(
                name: "IX_PrizeWinners_UserId",
                table: "PrizeWinners",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PrizeWinners_DrawingId_UserId_RedeemedAt",
                table: "PrizeWinners",
                columns: new[] { "DrawingId", "UserId", "RedeemedAt" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Programs_DailyLiteracyTipId",
                table: "Programs",
                column: "DailyLiteracyTipId");

            migrationBuilder.CreateIndex(
                name: "IX_Programs_PointTranslationId",
                table: "Programs",
                column: "PointTranslationId");

            migrationBuilder.CreateIndex(
                name: "IX_Programs_SiteId",
                table: "Programs",
                column: "SiteId");

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
                name: "IX_PsDates_SiteId",
                table: "PsDates",
                column: "SiteId");

            migrationBuilder.CreateIndex(
                name: "IX_PsKitAgeGroups_AgeGroupId",
                table: "PsKitAgeGroups",
                column: "AgeGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_PsKitImages_KitId",
                table: "PsKitImages",
                column: "KitId");

            migrationBuilder.CreateIndex(
                name: "IX_PsPerformers_UserId",
                table: "PsPerformers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PsPerformerBranches_BranchId",
                table: "PsPerformerBranches",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_PsPerformerImages_PerformerId",
                table: "PsPerformerImages",
                column: "PerformerId");

            migrationBuilder.CreateIndex(
                name: "IX_PsPerformerSchedules_PerformerId",
                table: "PsPerformerSchedules",
                column: "PerformerId");

            migrationBuilder.CreateIndex(
                name: "IX_PsPrograms_PerformerId",
                table: "PsPrograms",
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
                name: "IX_Questions_QuestionnaireId",
                table: "Questions",
                column: "QuestionnaireId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_PermissionId",
                table: "RolePermissions",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_Schools_SchoolDistrictId",
                table: "Schools",
                column: "SchoolDistrictId");

            migrationBuilder.CreateIndex(
                name: "IX_Schools_SchoolTypeId",
                table: "Schools",
                column: "SchoolTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Systems_SiteId",
                table: "Systems",
                column: "SiteId");

            migrationBuilder.CreateIndex(
                name: "IX_Triggers_AwardAvatarBundleId",
                table: "Triggers",
                column: "AwardAvatarBundleId");

            migrationBuilder.CreateIndex(
                name: "IX_Triggers_AwardBadgeId",
                table: "Triggers",
                column: "AwardBadgeId");

            migrationBuilder.CreateIndex(
                name: "IX_Triggers_AwardVendorCodeTypeId",
                table: "Triggers",
                column: "AwardVendorCodeTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Triggers_LimitToBranchId",
                table: "Triggers",
                column: "LimitToBranchId");

            migrationBuilder.CreateIndex(
                name: "IX_Triggers_LimitToProgramId",
                table: "Triggers",
                column: "LimitToProgramId");

            migrationBuilder.CreateIndex(
                name: "IX_Triggers_LimitToSystemId",
                table: "Triggers",
                column: "LimitToSystemId");

            migrationBuilder.CreateIndex(
                name: "IX_TriggerBadges_BadgeId",
                table: "TriggerBadges",
                column: "BadgeId");

            migrationBuilder.CreateIndex(
                name: "IX_TriggerChallenges_ChallengeId",
                table: "TriggerChallenges",
                column: "ChallengeId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_BranchId",
                table: "Users",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_ProgramId",
                table: "Users",
                column: "ProgramId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_SystemId",
                table: "Users",
                column: "SystemId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_SiteId_IsDeleted_Username",
                table: "Users",
                columns: new[] { "SiteId", "IsDeleted", "Username" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_SiteId_Id_IsDeleted_HouseholdHeadUserId",
                table: "Users",
                columns: new[] { "SiteId", "Id", "IsDeleted", "HouseholdHeadUserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserAnswers_AnswerId",
                table: "UserAnswers",
                column: "AnswerId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAvatars_AvatarElementId",
                table: "UserAvatars",
                column: "AvatarElementId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAvatarItems_AvatarItemId",
                table: "UserAvatarItems",
                column: "AvatarItemId");

            migrationBuilder.CreateIndex(
                name: "IX_UserBadges_BadgeId",
                table: "UserBadges",
                column: "BadgeId");

            migrationBuilder.CreateIndex(
                name: "IX_UserBooks_BookId",
                table: "UserBooks",
                column: "BookId");

            migrationBuilder.CreateIndex(
                name: "IX_UserChallengeTasks_ChallengeTaskId",
                table: "UserChallengeTasks",
                column: "ChallengeTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_UserFavoriteChallenges_ChallengeId",
                table: "UserFavoriteChallenges",
                column: "ChallengeId");

            migrationBuilder.CreateIndex(
                name: "IX_UserLogs_UserId_IsDeleted_BadgeId",
                table: "UserLogs",
                columns: new[] { "UserId", "IsDeleted", "BadgeId" });

            migrationBuilder.CreateIndex(
                name: "IX_UserLogs_UserId_IsDeleted_ChallengeId",
                table: "UserLogs",
                columns: new[] { "UserId", "IsDeleted", "ChallengeId" });

            migrationBuilder.CreateIndex(
                name: "IX_UserLogs_UserId_IsDeleted_PointTranslationId_ActivityEarned",
                table: "UserLogs",
                columns: new[] { "UserId", "IsDeleted", "PointTranslationId", "ActivityEarned" });

            migrationBuilder.CreateIndex(
                name: "IX_UserQuestionnaires_QuestionnaireId",
                table: "UserQuestionnaires",
                column: "QuestionnaireId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleId",
                table: "UserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserTriggers_TriggerId",
                table: "UserTriggers",
                column: "TriggerId");

            migrationBuilder.CreateIndex(
                name: "IX_VendorCodes_VendorCodeTypeId",
                table: "VendorCodes",
                column: "VendorCodeTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_VendorCodeTypes_SiteId",
                table: "VendorCodeTypes",
                column: "SiteId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "AuthorizationCodes");

            migrationBuilder.DropTable(
                name: "AvatarBundleItems");

            migrationBuilder.DropTable(
                name: "Broadcasts");

            migrationBuilder.DropTable(
                name: "ChallengeCategories");

            migrationBuilder.DropTable(
                name: "ChallengeGroupChallenges");

            migrationBuilder.DropTable(
                name: "ChallengeTaskTypes");

            migrationBuilder.DropTable(
                name: "DailyLiteracyTipImage");

            migrationBuilder.DropTable(
                name: "DashboardContents");

            migrationBuilder.DropTable(
                name: "DrawingCriterionPrograms");

            migrationBuilder.DropTable(
                name: "EmailReminders");

            migrationBuilder.DropTable(
                name: "Events");

            migrationBuilder.DropTable(
                name: "GroupInfos");

            migrationBuilder.DropTable(
                name: "Locations");

            migrationBuilder.DropTable(
                name: "Mails");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "Pages");

            migrationBuilder.DropTable(
                name: "PrizeWinners");

            migrationBuilder.DropTable(
                name: "PsBlackoutDates");

            migrationBuilder.DropTable(
                name: "PsBranchSelections");

            migrationBuilder.DropTable(
                name: "PsDates");

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
                name: "RecoveryTokens");

            migrationBuilder.DropTable(
                name: "ReportCriteria");

            migrationBuilder.DropTable(
                name: "ReportRequests");

            migrationBuilder.DropTable(
                name: "RequiredQuestionnaires");

            migrationBuilder.DropTable(
                name: "RolePermissions");

            migrationBuilder.DropTable(
                name: "Schools");

            migrationBuilder.DropTable(
                name: "SiteSettings");

            migrationBuilder.DropTable(
                name: "TriggerBadges");

            migrationBuilder.DropTable(
                name: "TriggerChallenges");

            migrationBuilder.DropTable(
                name: "UserAnswers");

            migrationBuilder.DropTable(
                name: "UserAvatars");

            migrationBuilder.DropTable(
                name: "UserAvatarItems");

            migrationBuilder.DropTable(
                name: "UserBadges");

            migrationBuilder.DropTable(
                name: "UserBooks");

            migrationBuilder.DropTable(
                name: "UserChallengeTasks");

            migrationBuilder.DropTable(
                name: "UserFavoriteChallenges");

            migrationBuilder.DropTable(
                name: "UserLogs");

            migrationBuilder.DropTable(
                name: "UserQuestionnaires");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "UserTriggers");

            migrationBuilder.DropTable(
                name: "VendorCodes");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "ChallengeGroups");

            migrationBuilder.DropTable(
                name: "GroupTypes");

            migrationBuilder.DropTable(
                name: "Drawings");

            migrationBuilder.DropTable(
                name: "PsKits");

            migrationBuilder.DropTable(
                name: "PsAgeGroups");

            migrationBuilder.DropTable(
                name: "PsPrograms");

            migrationBuilder.DropTable(
                name: "Permissions");

            migrationBuilder.DropTable(
                name: "SchoolDistricts");

            migrationBuilder.DropTable(
                name: "SchoolTypes");

            migrationBuilder.DropTable(
                name: "Answers");

            migrationBuilder.DropTable(
                name: "AvatarElements");

            migrationBuilder.DropTable(
                name: "Books");

            migrationBuilder.DropTable(
                name: "ChallengeTasks");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Triggers");

            migrationBuilder.DropTable(
                name: "DrawingCriteria");

            migrationBuilder.DropTable(
                name: "PsPerformers");

            migrationBuilder.DropTable(
                name: "Questions");

            migrationBuilder.DropTable(
                name: "AvatarColors");

            migrationBuilder.DropTable(
                name: "AvatarItems");

            migrationBuilder.DropTable(
                name: "Challenges");

            migrationBuilder.DropTable(
                name: "AvatarBundles");

            migrationBuilder.DropTable(
                name: "Badges");

            migrationBuilder.DropTable(
                name: "VendorCodeTypes");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Questionnaires");

            migrationBuilder.DropTable(
                name: "AvatarLayers");

            migrationBuilder.DropTable(
                name: "Branches");

            migrationBuilder.DropTable(
                name: "Programs");

            migrationBuilder.DropTable(
                name: "Systems");

            migrationBuilder.DropTable(
                name: "DailyLiteracyTip");

            migrationBuilder.DropTable(
                name: "PointTranslations");

            migrationBuilder.DropTable(
                name: "Sites");
        }
    }
}
