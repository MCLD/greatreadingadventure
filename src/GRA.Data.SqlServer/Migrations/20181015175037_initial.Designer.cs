using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using GRA.Data.SqlServer;

namespace GRA.Data.SqlServer.Migrations
{
    [DbContext(typeof(SqlServerContext))]
    [Migration("20181015175037_initial")]
    partial class initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.6")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("GRA.Data.Model.Answer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedAt");

                    b.Property<int>("CreatedBy");

                    b.Property<int>("QuestionId");

                    b.Property<int>("SortOrder");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasMaxLength(1500);

                    b.HasKey("Id");

                    b.HasIndex("QuestionId");

                    b.ToTable("Answers");
                });

            modelBuilder.Entity("GRA.Data.Model.AuditLog", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedAt");

                    b.Property<int>("CreatedBy");

                    b.Property<string>("CurrentValue");

                    b.Property<int>("EntityId");

                    b.Property<string>("EntityType")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<string>("PreviousValue");

                    b.Property<DateTime>("UpdatedAt");

                    b.Property<int>("UpdatedBy");

                    b.HasKey("Id");

                    b.ToTable("AuditLogs");
                });

            modelBuilder.Entity("GRA.Data.Model.AuthorizationCode", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<DateTime>("CreatedAt");

                    b.Property<int>("CreatedBy");

                    b.Property<string>("Description");

                    b.Property<bool>("IsSingleUse");

                    b.Property<int>("RoleId");

                    b.Property<int>("SiteId");

                    b.Property<int>("Uses");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AuthorizationCodes");
                });

            modelBuilder.Entity("GRA.Data.Model.AvatarBundle", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("CanBeUnlocked");

                    b.Property<DateTime>("CreatedAt");

                    b.Property<int>("CreatedBy");

                    b.Property<bool>("HasBeenAwarded");

                    b.Property<bool>("IsDeleted");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<int>("SiteId");

                    b.HasKey("Id");

                    b.ToTable("AvatarBundles");
                });

            modelBuilder.Entity("GRA.Data.Model.AvatarBundleItem", b =>
                {
                    b.Property<int>("AvatarBundleId");

                    b.Property<int>("AvatarItemId");

                    b.HasKey("AvatarBundleId", "AvatarItemId");

                    b.HasIndex("AvatarItemId");

                    b.ToTable("AvatarBundleItems");
                });

            modelBuilder.Entity("GRA.Data.Model.AvatarColor", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AvatarLayerId");

                    b.Property<string>("Color")
                        .IsRequired()
                        .HasMaxLength(15);

                    b.Property<DateTime>("CreatedAt");

                    b.Property<int>("CreatedBy");

                    b.Property<int>("SortOrder");

                    b.HasKey("Id");

                    b.HasIndex("AvatarLayerId");

                    b.ToTable("AvatarColors");
                });

            modelBuilder.Entity("GRA.Data.Model.AvatarElement", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("AvatarColorId");

                    b.Property<int>("AvatarItemId");

                    b.Property<DateTime>("CreatedAt");

                    b.Property<int>("CreatedBy");

                    b.Property<string>("Filename")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.HasKey("Id");

                    b.HasIndex("AvatarColorId");

                    b.HasIndex("AvatarItemId");

                    b.ToTable("AvatarElements");
                });

            modelBuilder.Entity("GRA.Data.Model.AvatarItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AvatarLayerId");

                    b.Property<DateTime>("CreatedAt");

                    b.Property<int>("CreatedBy");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<int>("SortOrder");

                    b.Property<string>("Thumbnail")
                        .HasMaxLength(255);

                    b.Property<bool>("Unlockable");

                    b.HasKey("Id");

                    b.HasIndex("AvatarLayerId");

                    b.ToTable("AvatarItems");
                });

            modelBuilder.Entity("GRA.Data.Model.AvatarLayer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("CanBeEmpty");

                    b.Property<DateTime>("CreatedAt");

                    b.Property<int>("CreatedBy");

                    b.Property<bool>("DefaultLayer");

                    b.Property<int>("GroupId");

                    b.Property<string>("Icon")
                        .HasMaxLength(255);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<int>("Position");

                    b.Property<bool>("ShowColorSelector");

                    b.Property<bool>("ShowItemSelector");

                    b.Property<int>("SiteId");

                    b.Property<int>("SortOrder");

                    b.Property<decimal>("ZoomScale");

                    b.Property<int>("ZoomYOffset");

                    b.HasKey("Id");

                    b.ToTable("AvatarLayers");
                });

            modelBuilder.Entity("GRA.Data.Model.Badge", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedAt");

                    b.Property<int>("CreatedBy");

                    b.Property<string>("Filename");

                    b.Property<int>("SiteId");

                    b.HasKey("Id");

                    b.ToTable("Badges");
                });

            modelBuilder.Entity("GRA.Data.Model.Book", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Author")
                        .HasMaxLength(255);

                    b.Property<int?>("ChallengeId");

                    b.Property<DateTime>("CreatedAt");

                    b.Property<int>("CreatedBy");

                    b.Property<string>("Isbn")
                        .HasMaxLength(30);

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(500);

                    b.Property<string>("Url")
                        .HasMaxLength(500);

                    b.HasKey("Id");

                    b.ToTable("Books");
                });

            modelBuilder.Entity("GRA.Data.Model.Branch", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Address")
                        .HasMaxLength(255);

                    b.Property<DateTime>("CreatedAt");

                    b.Property<int>("CreatedBy");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<int>("SystemId");

                    b.Property<string>("Telephone")
                        .HasMaxLength(50);

                    b.Property<string>("Url")
                        .HasMaxLength(500);

                    b.HasKey("Id");

                    b.HasIndex("SystemId");

                    b.ToTable("Branches");
                });

            modelBuilder.Entity("GRA.Data.Model.Broadcast", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Body")
                        .IsRequired()
                        .HasMaxLength(1000);

                    b.Property<DateTime>("CreatedAt");

                    b.Property<int>("CreatedBy");

                    b.Property<DateTime>("SendAt");

                    b.Property<bool>("SendToNewUsers");

                    b.Property<int>("SiteId");

                    b.Property<string>("Subject")
                        .IsRequired()
                        .HasMaxLength(500);

                    b.HasKey("Id");

                    b.ToTable("Broadcasts");
                });

            modelBuilder.Entity("GRA.Data.Model.Category", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Color")
                        .HasMaxLength(10);

                    b.Property<DateTime>("CreatedAt");

                    b.Property<int>("CreatedBy");

                    b.Property<string>("Description")
                        .HasMaxLength(500);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<int>("SiteId");

                    b.HasKey("Id");

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("GRA.Data.Model.Challenge", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("BadgeId");

                    b.Property<DateTime>("CreatedAt");

                    b.Property<int>("CreatedBy");

                    b.Property<string>("Description")
                        .HasMaxLength(500);

                    b.Property<bool>("IsActive");

                    b.Property<bool>("IsDeleted");

                    b.Property<bool>("IsValid");

                    b.Property<int?>("LimitToBranchId");

                    b.Property<int?>("LimitToProgramId");

                    b.Property<int?>("LimitToSystemId");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<int>("PointsAwarded");

                    b.Property<int>("RelatedBranchId");

                    b.Property<int>("RelatedSystemId");

                    b.Property<int>("SiteId");

                    b.Property<int>("TasksToComplete");

                    b.HasKey("Id");

                    b.HasIndex("SiteId");

                    b.ToTable("Challenges");
                });

            modelBuilder.Entity("GRA.Data.Model.ChallengeCategory", b =>
                {
                    b.Property<int>("ChallengeId");

                    b.Property<int>("CategoryId");

                    b.Property<DateTime>("CreatedAt");

                    b.Property<int>("CreatedBy");

                    b.HasKey("ChallengeId", "CategoryId");

                    b.HasIndex("CategoryId");

                    b.ToTable("ChallengeCategories");
                });

            modelBuilder.Entity("GRA.Data.Model.ChallengeGroup", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedAt");

                    b.Property<int>("CreatedBy");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(1000);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<int>("SiteId");

                    b.Property<string>("Stub")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.HasKey("Id");

                    b.ToTable("ChallengeGroups");
                });

            modelBuilder.Entity("GRA.Data.Model.ChallengeGroupChallenge", b =>
                {
                    b.Property<int>("ChallengeGroupId");

                    b.Property<int>("ChallengeId");

                    b.Property<DateTime>("CreatedAt");

                    b.Property<int>("CreatedBy");

                    b.HasKey("ChallengeGroupId", "ChallengeId");

                    b.HasIndex("ChallengeId");

                    b.ToTable("ChallengeGroupChallenges");
                });

            modelBuilder.Entity("GRA.Data.Model.ChallengeTask", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Author")
                        .HasMaxLength(255);

                    b.Property<int>("ChallengeId");

                    b.Property<int>("ChallengeTaskTypeId");

                    b.Property<DateTime>("CreatedAt");

                    b.Property<int>("CreatedBy");

                    b.Property<string>("Filename")
                        .HasMaxLength(255);

                    b.Property<string>("Isbn")
                        .HasMaxLength(30);

                    b.Property<int>("Position");

                    b.Property<string>("Title")
                        .IsRequired();

                    b.Property<string>("Url")
                        .HasMaxLength(500);

                    b.HasKey("Id");

                    b.HasIndex("ChallengeId");

                    b.ToTable("ChallengeTasks");
                });

            modelBuilder.Entity("GRA.Data.Model.ChallengeTaskType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("ActivityCount");

                    b.Property<DateTime>("CreatedAt");

                    b.Property<int>("CreatedBy");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<int?>("PointTranslationId");

                    b.HasKey("Id");

                    b.ToTable("ChallengeTaskTypes");
                });

            modelBuilder.Entity("GRA.Data.Model.DailyLiteracyTip", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedAt");

                    b.Property<int>("CreatedBy");

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<int>("SiteId");

                    b.HasKey("Id");

                    b.ToTable("DailyLiteracyTip");
                });

            modelBuilder.Entity("GRA.Data.Model.DailyLiteracyTipImage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedAt");

                    b.Property<int>("CreatedBy");

                    b.Property<int>("DailyLiteracyTipId");

                    b.Property<int>("Day");

                    b.Property<string>("Extension")
                        .IsRequired()
                        .HasMaxLength(10);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.HasKey("Id");

                    b.HasIndex("DailyLiteracyTipId");

                    b.ToTable("DailyLiteracyTipImage");
                });

            modelBuilder.Entity("GRA.Data.Model.DashboardContent", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Content");

                    b.Property<DateTime>("CreatedAt");

                    b.Property<int>("CreatedBy");

                    b.Property<int>("SiteId");

                    b.Property<DateTime>("StartTime");

                    b.HasKey("Id");

                    b.ToTable("DashboardContents");
                });

            modelBuilder.Entity("GRA.Data.Model.Drawing", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedAt");

                    b.Property<int>("CreatedBy");

                    b.Property<int>("DrawingCriterionId");

                    b.Property<bool>("IsArchived");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<string>("NotificationMessage")
                        .HasMaxLength(2000);

                    b.Property<string>("NotificationSubject")
                        .HasMaxLength(255);

                    b.Property<string>("RedemptionInstructions")
                        .HasMaxLength(1000);

                    b.Property<int>("RelatedBranchId");

                    b.Property<int>("RelatedSystemId");

                    b.Property<int>("WinnerCount");

                    b.HasKey("Id");

                    b.HasIndex("DrawingCriterionId");

                    b.ToTable("Drawings");
                });

            modelBuilder.Entity("GRA.Data.Model.DrawingCriterion", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("BranchId");

                    b.Property<DateTime>("CreatedAt");

                    b.Property<int>("CreatedBy");

                    b.Property<DateTime?>("EndOfPeriod");

                    b.Property<bool>("ExcludePreviousWinners");

                    b.Property<bool>("IncludeAdmin");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<int?>("PointsMaximum");

                    b.Property<int?>("PointsMinimum");

                    b.Property<int?>("ProgramId");

                    b.Property<bool>("ReadABook");

                    b.Property<int>("RelatedBranchId");

                    b.Property<int>("RelatedSystemId");

                    b.Property<int>("SiteId");

                    b.Property<DateTime?>("StartOfPeriod");

                    b.Property<int?>("SystemId");

                    b.HasKey("Id");

                    b.HasIndex("BranchId");

                    b.HasIndex("SystemId");

                    b.ToTable("DrawingCriteria");
                });

            modelBuilder.Entity("GRA.Data.Model.DrawingCriterionProgram", b =>
                {
                    b.Property<int>("DrawingCriterionId");

                    b.Property<int>("ProgramId");

                    b.HasKey("DrawingCriterionId", "ProgramId");

                    b.ToTable("DrawingCriterionPrograms");
                });

            modelBuilder.Entity("GRA.Data.Model.EmailReminder", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedAt");

                    b.Property<int>("CreatedBy");

                    b.Property<string>("Email")
                        .IsRequired();

                    b.Property<string>("SignUpSource")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("Email", "SignUpSource")
                        .IsUnique();

                    b.ToTable("EmailReminders");
                });

            modelBuilder.Entity("GRA.Data.Model.Event", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("AllDay");

                    b.Property<int?>("AtBranchId");

                    b.Property<int?>("AtLocationId");

                    b.Property<int?>("ChallengeGroupId");

                    b.Property<int?>("ChallengeId");

                    b.Property<DateTime>("CreatedAt");

                    b.Property<int>("CreatedBy");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(1500);

                    b.Property<DateTime?>("EndDate");

                    b.Property<string>("ExternalLink")
                        .HasMaxLength(300);

                    b.Property<bool>("IsActive");

                    b.Property<bool>("IsCommunityExperience");

                    b.Property<bool>("IsValid");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<int?>("ParentEventId");

                    b.Property<int?>("ProgramId");

                    b.Property<int>("RelatedBranchId");

                    b.Property<int>("RelatedSystemId");

                    b.Property<int?>("RelatedTriggerId");

                    b.Property<int>("SiteId");

                    b.Property<DateTime>("StartDate");

                    b.HasKey("Id");

                    b.HasIndex("ChallengeGroupId");

                    b.HasIndex("ChallengeId");

                    b.ToTable("Events");
                });

            modelBuilder.Entity("GRA.Data.Model.GroupInfo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedAt");

                    b.Property<int>("CreatedBy");

                    b.Property<int>("GroupTypeId");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<int>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("GroupTypeId");

                    b.HasIndex("UserId");

                    b.ToTable("GroupInfos");
                });

            modelBuilder.Entity("GRA.Data.Model.GroupType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedAt");

                    b.Property<int>("CreatedBy");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<int>("SiteId");

                    b.HasKey("Id");

                    b.ToTable("GroupTypes");
                });

            modelBuilder.Entity("GRA.Data.Model.Location", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Address");

                    b.Property<DateTime>("CreatedAt");

                    b.Property<int>("CreatedBy");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<int>("SiteId");

                    b.Property<string>("Telephone")
                        .HasMaxLength(50);

                    b.Property<string>("Url")
                        .HasMaxLength(255);

                    b.HasKey("Id");

                    b.ToTable("Locations");
                });

            modelBuilder.Entity("GRA.Data.Model.Mail", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Body")
                        .IsRequired()
                        .HasMaxLength(2000);

                    b.Property<bool>("CanParticipantDelete");

                    b.Property<DateTime>("CreatedAt");

                    b.Property<int>("CreatedBy");

                    b.Property<int?>("DrawingId");

                    b.Property<int>("FromUserId");

                    b.Property<int?>("InReplyToId");

                    b.Property<bool>("IsBroadcast");

                    b.Property<bool>("IsDeleted");

                    b.Property<bool>("IsNew");

                    b.Property<bool>("IsRepliedTo");

                    b.Property<int>("SiteId");

                    b.Property<string>("Subject")
                        .IsRequired()
                        .HasMaxLength(500);

                    b.Property<int?>("ThreadId");

                    b.Property<int?>("ToUserId");

                    b.Property<int?>("TriggerId");

                    b.HasKey("Id");

                    b.HasIndex("ToUserId", "IsDeleted", "IsNew");

                    b.ToTable("Mails");
                });

            modelBuilder.Entity("GRA.Data.Model.Notification", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("BadgeFilename");

                    b.Property<int?>("BadgeId");

                    b.Property<int?>("ChallengeId");

                    b.Property<DateTime>("CreatedAt");

                    b.Property<int>("CreatedBy");

                    b.Property<bool>("IsAchiever");

                    b.Property<bool>("IsJoiner");

                    b.Property<int?>("PointsEarned");

                    b.Property<string>("Text")
                        .IsRequired();

                    b.Property<int>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Notifications");
                });

            modelBuilder.Entity("GRA.Data.Model.Page", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Content")
                        .IsRequired();

                    b.Property<DateTime>("CreatedAt");

                    b.Property<int>("CreatedBy");

                    b.Property<string>("FooterText")
                        .HasMaxLength(255);

                    b.Property<bool>("IsPublished");

                    b.Property<string>("NavText")
                        .HasMaxLength(255);

                    b.Property<int>("SiteId");

                    b.Property<string>("Stub")
                        .HasMaxLength(255);

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.HasKey("Id");

                    b.HasIndex("SiteId", "Stub")
                        .IsUnique();

                    b.ToTable("Pages");
                });

            modelBuilder.Entity("GRA.Data.Model.Permission", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedAt");

                    b.Property<int>("CreatedBy");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.HasKey("Id");

                    b.ToTable("Permissions");
                });

            modelBuilder.Entity("GRA.Data.Model.PointTranslation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("ActivityAmount");

                    b.Property<string>("ActivityDescription")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<string>("ActivityDescriptionPlural")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<DateTime>("CreatedAt");

                    b.Property<int>("CreatedBy");

                    b.Property<bool>("IsSingleEvent");

                    b.Property<int>("PointsEarned");

                    b.Property<int>("SiteId");

                    b.Property<string>("TranslationDescriptionPastTense")
                        .IsRequired();

                    b.Property<string>("TranslationDescriptionPresentTense")
                        .IsRequired();

                    b.Property<string>("TranslationName")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.HasKey("Id");

                    b.ToTable("PointTranslations");
                });

            modelBuilder.Entity("GRA.Data.Model.PrizeWinner", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedAt");

                    b.Property<int>("CreatedBy");

                    b.Property<int?>("DrawingId");

                    b.Property<int?>("MailId");

                    b.Property<DateTime?>("RedeemedAt");

                    b.Property<int?>("RedeemedBy");

                    b.Property<int>("SiteId");

                    b.Property<int?>("TriggerId");

                    b.Property<int>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("TriggerId");

                    b.HasIndex("UserId");

                    b.HasIndex("DrawingId", "UserId", "RedeemedAt")
                        .IsUnique();

                    b.ToTable("PrizeWinners");
                });

            modelBuilder.Entity("GRA.Data.Model.Program", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AchieverPointAmount");

                    b.Property<int?>("AgeMaximum");

                    b.Property<int?>("AgeMinimum");

                    b.Property<bool>("AgeRequired");

                    b.Property<bool>("AskAge");

                    b.Property<bool>("AskSchool");

                    b.Property<DateTime>("CreatedAt");

                    b.Property<int>("CreatedBy");

                    b.Property<int?>("DailyLiteracyTipId");

                    b.Property<int?>("JoinBadgeId");

                    b.Property<string>("JoinBadgeName")
                        .HasMaxLength(255);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<int>("PointTranslationId");

                    b.Property<int>("Position");

                    b.Property<bool>("SchoolRequired");

                    b.Property<int>("SiteId");

                    b.HasKey("Id");

                    b.HasIndex("DailyLiteracyTipId");

                    b.HasIndex("PointTranslationId");

                    b.HasIndex("SiteId");

                    b.ToTable("Programs");
                });

            modelBuilder.Entity("GRA.Data.Model.PsAgeGroup", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedAt");

                    b.Property<int>("CreatedBy");

                    b.Property<string>("Name")
                        .HasMaxLength(255);

                    b.HasKey("Id");

                    b.ToTable("PsAgeGroups");
                });

            modelBuilder.Entity("GRA.Data.Model.PsBlackoutDate", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedAt");

                    b.Property<int>("CreatedBy");

                    b.Property<DateTime>("Date");

                    b.Property<string>("Reason")
                        .HasMaxLength(255);

                    b.HasKey("Id");

                    b.ToTable("PsBlackoutDates");
                });

            modelBuilder.Entity("GRA.Data.Model.PsBranchSelection", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AgeGroupId");

                    b.Property<bool>("BackToBackProgram");

                    b.Property<int>("BranchId");

                    b.Property<DateTime>("CreatedAt");

                    b.Property<int>("CreatedBy");

                    b.Property<int?>("KitId");

                    b.Property<int?>("ProgramId");

                    b.Property<DateTime>("RequestedStartTime");

                    b.Property<int>("ScheduleDuration");

                    b.Property<DateTime>("ScheduleStartTime");

                    b.Property<string>("SecretCode")
                        .HasMaxLength(50);

                    b.Property<DateTime>("SelectedAt");

                    b.Property<int>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("AgeGroupId");

                    b.HasIndex("BranchId");

                    b.HasIndex("KitId");

                    b.HasIndex("ProgramId");

                    b.HasIndex("UserId");

                    b.ToTable("PsBranchSelections");
                });

            modelBuilder.Entity("GRA.Data.Model.PsDates", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedAt");

                    b.Property<int>("CreatedBy");

                    b.Property<DateTime?>("RegistrationClosed");

                    b.Property<DateTime?>("RegistrationOpen");

                    b.Property<DateTime?>("ScheduleEndDate");

                    b.Property<DateTime?>("SchedulePosted");

                    b.Property<DateTime?>("ScheduleStartDate");

                    b.Property<DateTime?>("SchedulingClosed");

                    b.Property<DateTime?>("SchedulingOpen");

                    b.Property<DateTime?>("SchedulingPreview");

                    b.Property<int>("SiteId");

                    b.HasKey("Id");

                    b.HasIndex("SiteId");

                    b.ToTable("PsDates");
                });

            modelBuilder.Entity("GRA.Data.Model.PsKit", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedAt");

                    b.Property<int>("CreatedBy");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(2000);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<string>("Website")
                        .HasMaxLength(255);

                    b.HasKey("Id");

                    b.ToTable("PsKits");
                });

            modelBuilder.Entity("GRA.Data.Model.PsKitAgeGroup", b =>
                {
                    b.Property<int>("KitId");

                    b.Property<int>("AgeGroupId");

                    b.HasKey("KitId", "AgeGroupId");

                    b.HasIndex("AgeGroupId");

                    b.ToTable("PsKitAgeGroups");
                });

            modelBuilder.Entity("GRA.Data.Model.PsKitImage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedAt");

                    b.Property<int>("CreatedBy");

                    b.Property<string>("Filename")
                        .HasMaxLength(255);

                    b.Property<int>("KitId");

                    b.HasKey("Id");

                    b.HasIndex("KitId");

                    b.ToTable("PsKitImages");
                });

            modelBuilder.Entity("GRA.Data.Model.PsPerformer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("AllBranches");

                    b.Property<string>("BillingAddress")
                        .IsRequired()
                        .HasMaxLength(500);

                    b.Property<DateTime>("CreatedAt");

                    b.Property<int>("CreatedBy");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<bool>("HasFingerprintCard");

                    b.Property<bool>("IsApproved");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<bool>("PhonePreferred");

                    b.Property<string>("ReferencesFilename")
                        .HasMaxLength(255);

                    b.Property<bool>("RegistrationCompleted");

                    b.Property<bool>("SetSchedule");

                    b.Property<int>("UserId");

                    b.Property<string>("VendorId")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<string>("Website")
                        .HasMaxLength(255);

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("PsPerformers");
                });

            modelBuilder.Entity("GRA.Data.Model.PsPerformerBranch", b =>
                {
                    b.Property<int>("PsPerformerId");

                    b.Property<int>("BranchId");

                    b.HasKey("PsPerformerId", "BranchId");

                    b.HasIndex("BranchId");

                    b.ToTable("PsPerformerBranches");
                });

            modelBuilder.Entity("GRA.Data.Model.PsPerformerImage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedAt");

                    b.Property<int>("CreatedBy");

                    b.Property<string>("Filename")
                        .HasMaxLength(255);

                    b.Property<int>("PerformerId");

                    b.HasKey("Id");

                    b.HasIndex("PerformerId");

                    b.ToTable("PsPerformerImages");
                });

            modelBuilder.Entity("GRA.Data.Model.PsPerformerSchedule", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedAt");

                    b.Property<int>("CreatedBy");

                    b.Property<DateTime>("Date");

                    b.Property<DateTime?>("EndTime");

                    b.Property<int>("PerformerId");

                    b.Property<DateTime?>("StartTime");

                    b.HasKey("Id");

                    b.HasIndex("PerformerId");

                    b.ToTable("PsPerformerSchedules");
                });

            modelBuilder.Entity("GRA.Data.Model.PsProgram", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("AllowArchiving");

                    b.Property<bool>("AllowStreaming");

                    b.Property<int>("BackToBackMinutes");

                    b.Property<int>("BreakdownTimeMinutes");

                    b.Property<decimal>("Cost");

                    b.Property<DateTime>("CreatedAt");

                    b.Property<int>("CreatedBy");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(375);

                    b.Property<int>("MaximumCapacity");

                    b.Property<int>("MinimumCapacity");

                    b.Property<int>("PerformerId");

                    b.Property<int>("ProgramLengthMinutes");

                    b.Property<string>("Setup")
                        .HasMaxLength(1000);

                    b.Property<int>("SetupTimeMinutes");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.HasKey("Id");

                    b.HasIndex("PerformerId");

                    b.ToTable("PsPrograms");
                });

            modelBuilder.Entity("GRA.Data.Model.PsProgramAgeGroup", b =>
                {
                    b.Property<int>("ProgramId");

                    b.Property<int>("AgeGroupId");

                    b.HasKey("ProgramId", "AgeGroupId");

                    b.HasIndex("AgeGroupId");

                    b.ToTable("PsProgramAgeGroups");
                });

            modelBuilder.Entity("GRA.Data.Model.PsProgramImage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedAt");

                    b.Property<int>("CreatedBy");

                    b.Property<string>("Filename")
                        .HasMaxLength(255);

                    b.Property<int>("ProgramId");

                    b.HasKey("Id");

                    b.HasIndex("ProgramId");

                    b.ToTable("PsProgramImages");
                });

            modelBuilder.Entity("GRA.Data.Model.Question", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("CorrectAnswerId");

                    b.Property<DateTime>("CreatedAt");

                    b.Property<int>("CreatedBy");

                    b.Property<bool>("IsDeleted");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<int>("QuestionnaireId");

                    b.Property<int>("SortOrder");

                    b.Property<string>("Text")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("QuestionnaireId");

                    b.ToTable("Questions");
                });

            modelBuilder.Entity("GRA.Data.Model.Questionnaire", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("BadgeId");

                    b.Property<string>("BadgeName")
                        .HasMaxLength(255);

                    b.Property<string>("BadgeNotificationMessage")
                        .HasMaxLength(1000);

                    b.Property<DateTime>("CreatedAt");

                    b.Property<int>("CreatedBy");

                    b.Property<bool>("IsDeleted");

                    b.Property<bool>("IsLocked");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<int>("RelatedBranchId");

                    b.Property<int>("RelatedSystemId");

                    b.Property<int>("SiteId");

                    b.HasKey("Id");

                    b.ToTable("Questionnaires");
                });

            modelBuilder.Entity("GRA.Data.Model.RecoveryToken", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedAt");

                    b.Property<int>("CreatedBy");

                    b.Property<string>("Token")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<int>("UserId");

                    b.HasKey("Id");

                    b.ToTable("RecoveryTokens");
                });

            modelBuilder.Entity("GRA.Data.Model.ReportCriterion", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("BadgeRequiredList");

                    b.Property<int?>("BranchId");

                    b.Property<string>("ChallengeRequiredList");

                    b.Property<DateTime>("CreatedAt");

                    b.Property<int>("CreatedBy");

                    b.Property<DateTime?>("EndDate");

                    b.Property<bool>("Favorite");

                    b.Property<int?>("GroupInfoId");

                    b.Property<string>("Name")
                        .HasMaxLength(255);

                    b.Property<int?>("ProgramId");

                    b.Property<int?>("SchoolDistrictId");

                    b.Property<int?>("SchoolId");

                    b.Property<int>("SiteId");

                    b.Property<DateTime?>("StartDate");

                    b.Property<int?>("SystemId");

                    b.Property<int?>("VendorCodeTypeId");

                    b.HasKey("Id");

                    b.ToTable("ReportCriteria");
                });

            modelBuilder.Entity("GRA.Data.Model.ReportRequest", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedAt");

                    b.Property<int>("CreatedBy");

                    b.Property<bool>("Favorite");

                    b.Property<DateTime?>("Finished");

                    b.Property<string>("InstanceName")
                        .HasMaxLength(255);

                    b.Property<string>("Name")
                        .HasMaxLength(255);

                    b.Property<int>("ReportCriteriaId");

                    b.Property<int>("ReportId");

                    b.Property<string>("ResultJson");

                    b.Property<DateTime?>("Started");

                    b.Property<bool?>("Success");

                    b.HasKey("Id");

                    b.ToTable("ReportRequests");
                });

            modelBuilder.Entity("GRA.Data.Model.RequiredQuestionnaire", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("AgeMaximum");

                    b.Property<int?>("AgeMinimum");

                    b.Property<DateTime>("CreatedAt");

                    b.Property<int>("CreatedBy");

                    b.Property<DateTime?>("EndDate");

                    b.Property<int>("QuestionnaireId");

                    b.Property<int>("SiteId");

                    b.Property<DateTime?>("StartDate");

                    b.HasKey("Id");

                    b.ToTable("RequiredQuestionnaires");
                });

            modelBuilder.Entity("GRA.Data.Model.Role", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedAt");

                    b.Property<int>("CreatedBy");

                    b.Property<bool>("IsAdmin");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.HasKey("Id");

                    b.ToTable("Roles");
                });

            modelBuilder.Entity("GRA.Data.Model.RolePermission", b =>
                {
                    b.Property<int>("RoleId");

                    b.Property<int>("PermissionId");

                    b.Property<DateTime>("CreatedAt");

                    b.Property<int>("CreatedBy");

                    b.HasKey("RoleId", "PermissionId");

                    b.HasIndex("PermissionId");

                    b.ToTable("RolePermissions");
                });

            modelBuilder.Entity("GRA.Data.Model.School", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedAt");

                    b.Property<int>("CreatedBy");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<int>("SchoolDistrictId");

                    b.Property<int?>("SchoolTypeId");

                    b.Property<int>("SiteId");

                    b.HasKey("Id");

                    b.HasIndex("SchoolDistrictId");

                    b.HasIndex("SchoolTypeId");

                    b.ToTable("Schools");
                });

            modelBuilder.Entity("GRA.Data.Model.SchoolDistrict", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedAt");

                    b.Property<int>("CreatedBy");

                    b.Property<bool>("IsCharter");

                    b.Property<bool>("IsPrivate");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<int>("SiteId");

                    b.HasKey("Id");

                    b.ToTable("SchoolDistricts");
                });

            modelBuilder.Entity("GRA.Data.Model.SchoolType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedAt");

                    b.Property<int>("CreatedBy");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<int>("SiteId");

                    b.HasKey("Id");

                    b.ToTable("SchoolTypes");
                });

            modelBuilder.Entity("GRA.Data.Model.Site", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime?>("AccessClosed");

                    b.Property<int?>("AccessClosedPage");

                    b.Property<string>("AvatarCardDescription")
                        .HasMaxLength(150);

                    b.Property<int?>("BeforeRegistrationPage");

                    b.Property<DateTime>("CreatedAt");

                    b.Property<int>("CreatedBy");

                    b.Property<string>("Domain")
                        .HasMaxLength(255);

                    b.Property<string>("ExternalEventListUrl");

                    b.Property<string>("FacebookAppId")
                        .HasMaxLength(100);

                    b.Property<string>("FacebookImageUrl")
                        .HasMaxLength(255);

                    b.Property<string>("Footer");

                    b.Property<string>("FromEmailAddress");

                    b.Property<string>("FromEmailName");

                    b.Property<string>("GoogleAnalyticsTrackingId");

                    b.Property<bool>("IsDefault");

                    b.Property<bool>("IsHttpsForced");

                    b.Property<int?>("MaxPointsPerChallengeTask");

                    b.Property<string>("MetaDescription")
                        .HasMaxLength(150);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<string>("OutgoingMailHost");

                    b.Property<string>("OutgoingMailLogin");

                    b.Property<string>("OutgoingMailPassword");

                    b.Property<int?>("OutgoingMailPort");

                    b.Property<string>("PageTitle")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<string>("Path")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<int?>("ProgramEndedPage");

                    b.Property<DateTime?>("ProgramEnds");

                    b.Property<int?>("ProgramOpenPage");

                    b.Property<DateTime?>("ProgramStarts");

                    b.Property<int?>("RegistrationOpenPage");

                    b.Property<DateTime?>("RegistrationOpens");

                    b.Property<bool>("RequirePostalCode");

                    b.Property<bool>("SinglePageSignUp");

                    b.Property<string>("SiteLogoUrl")
                        .HasMaxLength(255);

                    b.Property<string>("TwitterAvatarHashtags")
                        .HasMaxLength(100);

                    b.Property<string>("TwitterAvatarMessage")
                        .HasMaxLength(255);

                    b.Property<string>("TwitterCardImageUrl")
                        .HasMaxLength(255);

                    b.Property<bool?>("TwitterLargeCard");

                    b.Property<string>("TwitterUsername")
                        .HasMaxLength(15);

                    b.HasKey("Id");

                    b.ToTable("Sites");
                });

            modelBuilder.Entity("GRA.Data.Model.SiteSetting", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedAt");

                    b.Property<int>("CreatedBy");

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<int>("SiteId");

                    b.Property<string>("Value")
                        .HasMaxLength(255);

                    b.HasKey("Id");

                    b.ToTable("SiteSettings");
                });

            modelBuilder.Entity("GRA.Data.Model.System", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedAt");

                    b.Property<int>("CreatedBy");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<int>("SiteId");

                    b.HasKey("Id");

                    b.HasIndex("SiteId");

                    b.ToTable("Systems");
                });

            modelBuilder.Entity("GRA.Data.Model.Trigger", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime?>("ActivationDate");

                    b.Property<int?>("AwardAvatarBundleId");

                    b.Property<int>("AwardBadgeId");

                    b.Property<string>("AwardMail")
                        .HasMaxLength(2000);

                    b.Property<string>("AwardMailSubject")
                        .HasMaxLength(500);

                    b.Property<string>("AwardMessage")
                        .IsRequired()
                        .HasMaxLength(1000);

                    b.Property<int>("AwardPoints");

                    b.Property<string>("AwardPrizeName")
                        .HasMaxLength(255);

                    b.Property<string>("AwardPrizeRedemptionInstructions")
                        .HasMaxLength(1000);

                    b.Property<int?>("AwardVendorCodeTypeId");

                    b.Property<DateTime>("CreatedAt");

                    b.Property<int>("CreatedBy");

                    b.Property<bool>("IsDeleted");

                    b.Property<int>("ItemsRequired");

                    b.Property<int?>("LimitToBranchId");

                    b.Property<int?>("LimitToProgramId");

                    b.Property<int?>("LimitToSystemId");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<int>("Points");

                    b.Property<int>("RelatedBranchId");

                    b.Property<int>("RelatedSystemId");

                    b.Property<string>("SecretCode")
                        .HasMaxLength(50);

                    b.Property<int>("SiteId");

                    b.HasKey("Id");

                    b.HasIndex("AwardAvatarBundleId");

                    b.HasIndex("AwardBadgeId");

                    b.HasIndex("AwardVendorCodeTypeId");

                    b.HasIndex("LimitToBranchId");

                    b.HasIndex("LimitToProgramId");

                    b.HasIndex("LimitToSystemId");

                    b.ToTable("Triggers");
                });

            modelBuilder.Entity("GRA.Data.Model.TriggerBadge", b =>
                {
                    b.Property<int>("TriggerId");

                    b.Property<int>("BadgeId");

                    b.HasKey("TriggerId", "BadgeId");

                    b.HasIndex("BadgeId");

                    b.ToTable("TriggerBadges");
                });

            modelBuilder.Entity("GRA.Data.Model.TriggerChallenge", b =>
                {
                    b.Property<int>("TriggerId");

                    b.Property<int>("ChallengeId");

                    b.HasKey("TriggerId", "ChallengeId");

                    b.HasIndex("ChallengeId");

                    b.ToTable("TriggerChallenges");
                });

            modelBuilder.Entity("GRA.Data.Model.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime?>("AchievedAt");

                    b.Property<int?>("Age");

                    b.Property<int>("BranchId");

                    b.Property<bool>("CanBeDeleted");

                    b.Property<string>("CardNumber")
                        .HasMaxLength(64);

                    b.Property<DateTime>("CreatedAt");

                    b.Property<int>("CreatedBy");

                    b.Property<int?>("DailyPersonalGoal");

                    b.Property<string>("Email")
                        .HasMaxLength(254);

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<int?>("HouseholdHeadUserId");

                    b.Property<bool>("IsActive");

                    b.Property<bool>("IsAdmin");

                    b.Property<bool>("IsDeleted");

                    b.Property<bool>("IsFirstTime");

                    b.Property<bool>("IsHomeschooled");

                    b.Property<bool>("IsLockedOut");

                    b.Property<DateTime?>("LastAccess");

                    b.Property<DateTime?>("LastActivityDate");

                    b.Property<DateTime?>("LastBroadcast");

                    b.Property<string>("LastName")
                        .HasMaxLength(255);

                    b.Property<DateTime>("LockedOutAt");

                    b.Property<string>("LockedOutFor");

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber")
                        .HasMaxLength(15);

                    b.Property<int>("PointsEarned");

                    b.Property<string>("PostalCode")
                        .HasMaxLength(32);

                    b.Property<bool>("PreregistrationReminderRequested");

                    b.Property<int>("ProgramId");

                    b.Property<int?>("SchoolId");

                    b.Property<bool>("SchoolNotListed");

                    b.Property<int>("SiteId");

                    b.Property<int>("SystemId");

                    b.Property<string>("Username")
                        .HasMaxLength(36);

                    b.HasKey("Id");

                    b.HasIndex("BranchId");

                    b.HasIndex("ProgramId");

                    b.HasIndex("SystemId");

                    b.HasIndex("SiteId", "IsDeleted", "Username")
                        .IsUnique();

                    b.HasIndex("SiteId", "Id", "IsDeleted", "HouseholdHeadUserId")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("GRA.Data.Model.UserAnswer", b =>
                {
                    b.Property<int>("UserId");

                    b.Property<int>("AnswerId");

                    b.Property<DateTime>("CreatedAt");

                    b.HasKey("UserId", "AnswerId");

                    b.HasIndex("AnswerId");

                    b.ToTable("UserAnswers");
                });

            modelBuilder.Entity("GRA.Data.Model.UserAvatar", b =>
                {
                    b.Property<int>("UserId");

                    b.Property<int>("AvatarElementId");

                    b.HasKey("UserId", "AvatarElementId");

                    b.HasIndex("AvatarElementId");

                    b.ToTable("UserAvatars");
                });

            modelBuilder.Entity("GRA.Data.Model.UserAvatarItem", b =>
                {
                    b.Property<int>("UserId");

                    b.Property<int>("AvatarItemId");

                    b.HasKey("UserId", "AvatarItemId");

                    b.HasIndex("AvatarItemId");

                    b.ToTable("UserAvatarItems");
                });

            modelBuilder.Entity("GRA.Data.Model.UserBadge", b =>
                {
                    b.Property<int>("UserId");

                    b.Property<int>("BadgeId");

                    b.Property<DateTime>("CreatedAt");

                    b.HasKey("UserId", "BadgeId");

                    b.HasIndex("BadgeId");

                    b.ToTable("UserBadges");
                });

            modelBuilder.Entity("GRA.Data.Model.UserBook", b =>
                {
                    b.Property<int>("UserId");

                    b.Property<int>("BookId");

                    b.Property<DateTime>("CreatedAt");

                    b.Property<int>("CreatedBy");

                    b.HasKey("UserId", "BookId");

                    b.HasIndex("BookId");

                    b.ToTable("UserBooks");
                });

            modelBuilder.Entity("GRA.Data.Model.UserChallengeTask", b =>
                {
                    b.Property<int>("UserId");

                    b.Property<int>("ChallengeTaskId");

                    b.Property<int?>("BookId");

                    b.Property<DateTime>("CreatedAt");

                    b.Property<bool>("IsCompleted");

                    b.Property<int?>("UserLogId");

                    b.HasKey("UserId", "ChallengeTaskId");

                    b.HasIndex("ChallengeTaskId");

                    b.ToTable("UserChallengeTasks");
                });

            modelBuilder.Entity("GRA.Data.Model.UserFavoriteChallenge", b =>
                {
                    b.Property<int>("UserId");

                    b.Property<int>("ChallengeId");

                    b.Property<DateTime>("CreatedAt");

                    b.Property<int>("CreatedBy");

                    b.HasKey("UserId", "ChallengeId");

                    b.HasIndex("ChallengeId");

                    b.ToTable("UserFavoriteChallenges");
                });

            modelBuilder.Entity("GRA.Data.Model.UserLog", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("ActivityEarned");

                    b.Property<int?>("AvatarBundleId");

                    b.Property<int?>("AwardedBy");

                    b.Property<int?>("BadgeId");

                    b.Property<int?>("ChallengeId");

                    b.Property<DateTime>("CreatedAt");

                    b.Property<int>("CreatedBy");

                    b.Property<int?>("DeletedBy");

                    b.Property<string>("Description");

                    b.Property<bool>("IsDeleted");

                    b.Property<int?>("PointTranslationId");

                    b.Property<int>("PointsEarned");

                    b.Property<int>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("UserId", "IsDeleted", "BadgeId");

                    b.HasIndex("UserId", "IsDeleted", "ChallengeId");

                    b.HasIndex("UserId", "IsDeleted", "PointTranslationId", "ActivityEarned");

                    b.ToTable("UserLogs");
                });

            modelBuilder.Entity("GRA.Data.Model.UserQuestionnaire", b =>
                {
                    b.Property<int>("UserId");

                    b.Property<int>("QuestionnaireId");

                    b.Property<DateTime>("CreatedAt");

                    b.HasKey("UserId", "QuestionnaireId");

                    b.HasIndex("QuestionnaireId");

                    b.ToTable("UserQuestionnaires");
                });

            modelBuilder.Entity("GRA.Data.Model.UserRole", b =>
                {
                    b.Property<int>("UserId");

                    b.Property<int>("RoleId");

                    b.Property<DateTime>("CreatedAt");

                    b.Property<int>("CreatedBy");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("UserRoles");
                });

            modelBuilder.Entity("GRA.Data.Model.UserTrigger", b =>
                {
                    b.Property<int>("UserId");

                    b.Property<int>("TriggerId");

                    b.Property<DateTime>("CreatedAt");

                    b.HasKey("UserId", "TriggerId");

                    b.HasIndex("TriggerId");

                    b.ToTable("UserTriggers");
                });

            modelBuilder.Entity("GRA.Data.Model.VendorCode", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Code")
                        .IsRequired();

                    b.Property<DateTime>("CreatedAt");

                    b.Property<int>("CreatedBy");

                    b.Property<DateTime>("DateUsed");

                    b.Property<bool?>("IsDonated");

                    b.Property<bool>("IsUsed");

                    b.Property<DateTime?>("OrderDate");

                    b.Property<DateTime?>("ShipDate");

                    b.Property<int>("SiteId");

                    b.Property<int?>("UserId")
                        .IsConcurrencyToken();

                    b.Property<int>("VendorCodeTypeId");

                    b.HasKey("Id");

                    b.HasIndex("VendorCodeTypeId");

                    b.ToTable("VendorCodes");
                });

            modelBuilder.Entity("GRA.Data.Model.VendorCodeType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedAt");

                    b.Property<int>("CreatedBy");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<string>("DonationMail")
                        .HasMaxLength(1250);

                    b.Property<string>("DonationMessage")
                        .HasMaxLength(255);

                    b.Property<string>("DonationOptionMail")
                        .HasMaxLength(1250);

                    b.Property<string>("DonationOptionSubject");

                    b.Property<string>("DonationSubject")
                        .HasMaxLength(255);

                    b.Property<string>("Mail")
                        .IsRequired()
                        .HasMaxLength(1250);

                    b.Property<string>("MailSubject")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<int>("SiteId");

                    b.Property<string>("Url")
                        .HasMaxLength(255);

                    b.HasKey("Id");

                    b.HasIndex("SiteId");

                    b.ToTable("VendorCodeTypes");
                });

            modelBuilder.Entity("GRA.Data.Model.Answer", b =>
                {
                    b.HasOne("GRA.Data.Model.Question", "Question")
                        .WithMany("Answers")
                        .HasForeignKey("QuestionId");
                });

            modelBuilder.Entity("GRA.Data.Model.AuthorizationCode", b =>
                {
                    b.HasOne("GRA.Data.Model.Role", "Role")
                        .WithMany()
                        .HasForeignKey("RoleId");
                });

            modelBuilder.Entity("GRA.Data.Model.AvatarBundleItem", b =>
                {
                    b.HasOne("GRA.Data.Model.AvatarBundle", "AvatarBundle")
                        .WithMany("AvatarBundleItems")
                        .HasForeignKey("AvatarBundleId");

                    b.HasOne("GRA.Data.Model.AvatarItem", "AvatarItem")
                        .WithMany()
                        .HasForeignKey("AvatarItemId");
                });

            modelBuilder.Entity("GRA.Data.Model.AvatarColor", b =>
                {
                    b.HasOne("GRA.Data.Model.AvatarLayer", "AvatarLayer")
                        .WithMany("AvatarColors")
                        .HasForeignKey("AvatarLayerId");
                });

            modelBuilder.Entity("GRA.Data.Model.AvatarElement", b =>
                {
                    b.HasOne("GRA.Data.Model.AvatarColor", "AvatarColor")
                        .WithMany()
                        .HasForeignKey("AvatarColorId");

                    b.HasOne("GRA.Data.Model.AvatarItem", "AvatarItem")
                        .WithMany()
                        .HasForeignKey("AvatarItemId");
                });

            modelBuilder.Entity("GRA.Data.Model.AvatarItem", b =>
                {
                    b.HasOne("GRA.Data.Model.AvatarLayer", "AvatarLayer")
                        .WithMany("AvatarItems")
                        .HasForeignKey("AvatarLayerId");
                });

            modelBuilder.Entity("GRA.Data.Model.Branch", b =>
                {
                    b.HasOne("GRA.Data.Model.System", "System")
                        .WithMany("Branches")
                        .HasForeignKey("SystemId");
                });

            modelBuilder.Entity("GRA.Data.Model.Challenge", b =>
                {
                    b.HasOne("GRA.Data.Model.Site")
                        .WithMany("Challenges")
                        .HasForeignKey("SiteId");
                });

            modelBuilder.Entity("GRA.Data.Model.ChallengeCategory", b =>
                {
                    b.HasOne("GRA.Data.Model.Category", "Category")
                        .WithMany()
                        .HasForeignKey("CategoryId");

                    b.HasOne("GRA.Data.Model.Challenge", "Challenge")
                        .WithMany("ChallengeCategories")
                        .HasForeignKey("ChallengeId");
                });

            modelBuilder.Entity("GRA.Data.Model.ChallengeGroupChallenge", b =>
                {
                    b.HasOne("GRA.Data.Model.ChallengeGroup", "ChallengeGroup")
                        .WithMany("ChallengeGroupChallenges")
                        .HasForeignKey("ChallengeGroupId");

                    b.HasOne("GRA.Data.Model.Challenge", "Challenge")
                        .WithMany("ChallengeGroupChallenges")
                        .HasForeignKey("ChallengeId");
                });

            modelBuilder.Entity("GRA.Data.Model.ChallengeTask", b =>
                {
                    b.HasOne("GRA.Data.Model.Challenge")
                        .WithMany("Tasks")
                        .HasForeignKey("ChallengeId");
                });

            modelBuilder.Entity("GRA.Data.Model.DailyLiteracyTipImage", b =>
                {
                    b.HasOne("GRA.Data.Model.DailyLiteracyTip", "DailyLiteracyTip")
                        .WithMany("DailyLiteracyTipImages")
                        .HasForeignKey("DailyLiteracyTipId");
                });

            modelBuilder.Entity("GRA.Data.Model.Drawing", b =>
                {
                    b.HasOne("GRA.Data.Model.DrawingCriterion", "DrawingCriterion")
                        .WithMany()
                        .HasForeignKey("DrawingCriterionId");
                });

            modelBuilder.Entity("GRA.Data.Model.DrawingCriterion", b =>
                {
                    b.HasOne("GRA.Data.Model.Branch", "Branch")
                        .WithMany()
                        .HasForeignKey("BranchId");

                    b.HasOne("GRA.Data.Model.System", "System")
                        .WithMany()
                        .HasForeignKey("SystemId");
                });

            modelBuilder.Entity("GRA.Data.Model.DrawingCriterionProgram", b =>
                {
                    b.HasOne("GRA.Data.Model.DrawingCriterion")
                        .WithMany("CriterionPrograms")
                        .HasForeignKey("DrawingCriterionId");
                });

            modelBuilder.Entity("GRA.Data.Model.Event", b =>
                {
                    b.HasOne("GRA.Data.Model.ChallengeGroup", "ChallengeGroup")
                        .WithMany()
                        .HasForeignKey("ChallengeGroupId");

                    b.HasOne("GRA.Data.Model.Challenge", "Challenge")
                        .WithMany()
                        .HasForeignKey("ChallengeId");
                });

            modelBuilder.Entity("GRA.Data.Model.GroupInfo", b =>
                {
                    b.HasOne("GRA.Data.Model.GroupType", "GroupType")
                        .WithMany()
                        .HasForeignKey("GroupTypeId");

                    b.HasOne("GRA.Data.Model.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("GRA.Data.Model.PrizeWinner", b =>
                {
                    b.HasOne("GRA.Data.Model.Drawing", "Drawing")
                        .WithMany("Winners")
                        .HasForeignKey("DrawingId");

                    b.HasOne("GRA.Data.Model.Trigger", "Trigger")
                        .WithMany()
                        .HasForeignKey("TriggerId");

                    b.HasOne("GRA.Data.Model.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("GRA.Data.Model.Program", b =>
                {
                    b.HasOne("GRA.Data.Model.DailyLiteracyTip", "DailyLiteracy")
                        .WithMany()
                        .HasForeignKey("DailyLiteracyTipId");

                    b.HasOne("GRA.Data.Model.PointTranslation", "PointTranslation")
                        .WithMany()
                        .HasForeignKey("PointTranslationId");

                    b.HasOne("GRA.Data.Model.Site")
                        .WithMany("Programs")
                        .HasForeignKey("SiteId");
                });

            modelBuilder.Entity("GRA.Data.Model.PsBranchSelection", b =>
                {
                    b.HasOne("GRA.Data.Model.PsAgeGroup", "AgeGroup")
                        .WithMany()
                        .HasForeignKey("AgeGroupId");

                    b.HasOne("GRA.Data.Model.Branch", "Branch")
                        .WithMany()
                        .HasForeignKey("BranchId");

                    b.HasOne("GRA.Data.Model.PsKit", "Kit")
                        .WithMany()
                        .HasForeignKey("KitId");

                    b.HasOne("GRA.Data.Model.PsProgram", "Program")
                        .WithMany()
                        .HasForeignKey("ProgramId");

                    b.HasOne("GRA.Data.Model.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("GRA.Data.Model.PsDates", b =>
                {
                    b.HasOne("GRA.Data.Model.Site", "Site")
                        .WithMany()
                        .HasForeignKey("SiteId");
                });

            modelBuilder.Entity("GRA.Data.Model.PsKitAgeGroup", b =>
                {
                    b.HasOne("GRA.Data.Model.PsAgeGroup", "AgeGroup")
                        .WithMany()
                        .HasForeignKey("AgeGroupId");

                    b.HasOne("GRA.Data.Model.PsKit", "Kit")
                        .WithMany("AgeGroups")
                        .HasForeignKey("KitId");
                });

            modelBuilder.Entity("GRA.Data.Model.PsKitImage", b =>
                {
                    b.HasOne("GRA.Data.Model.PsKit", "Kit")
                        .WithMany("Images")
                        .HasForeignKey("KitId");
                });

            modelBuilder.Entity("GRA.Data.Model.PsPerformer", b =>
                {
                    b.HasOne("GRA.Data.Model.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("GRA.Data.Model.PsPerformerBranch", b =>
                {
                    b.HasOne("GRA.Data.Model.Branch", "Branch")
                        .WithMany()
                        .HasForeignKey("BranchId");

                    b.HasOne("GRA.Data.Model.PsPerformer", "PsPerformer")
                        .WithMany("Branches")
                        .HasForeignKey("PsPerformerId");
                });

            modelBuilder.Entity("GRA.Data.Model.PsPerformerImage", b =>
                {
                    b.HasOne("GRA.Data.Model.PsPerformer", "Performer")
                        .WithMany("Images")
                        .HasForeignKey("PerformerId");
                });

            modelBuilder.Entity("GRA.Data.Model.PsPerformerSchedule", b =>
                {
                    b.HasOne("GRA.Data.Model.PsPerformer", "Performer")
                        .WithMany("Schedule")
                        .HasForeignKey("PerformerId");
                });

            modelBuilder.Entity("GRA.Data.Model.PsProgram", b =>
                {
                    b.HasOne("GRA.Data.Model.PsPerformer", "Performer")
                        .WithMany("Programs")
                        .HasForeignKey("PerformerId");
                });

            modelBuilder.Entity("GRA.Data.Model.PsProgramAgeGroup", b =>
                {
                    b.HasOne("GRA.Data.Model.PsAgeGroup", "AgeGroup")
                        .WithMany()
                        .HasForeignKey("AgeGroupId");

                    b.HasOne("GRA.Data.Model.PsProgram", "Program")
                        .WithMany("AgeGroups")
                        .HasForeignKey("ProgramId");
                });

            modelBuilder.Entity("GRA.Data.Model.PsProgramImage", b =>
                {
                    b.HasOne("GRA.Data.Model.PsProgram", "Program")
                        .WithMany("ProgramImages")
                        .HasForeignKey("ProgramId");
                });

            modelBuilder.Entity("GRA.Data.Model.Question", b =>
                {
                    b.HasOne("GRA.Data.Model.Questionnaire", "Questionnaire")
                        .WithMany("Questions")
                        .HasForeignKey("QuestionnaireId");
                });

            modelBuilder.Entity("GRA.Data.Model.RolePermission", b =>
                {
                    b.HasOne("GRA.Data.Model.Permission", "Permission")
                        .WithMany()
                        .HasForeignKey("PermissionId");

                    b.HasOne("GRA.Data.Model.Role", "Role")
                        .WithMany()
                        .HasForeignKey("RoleId");
                });

            modelBuilder.Entity("GRA.Data.Model.School", b =>
                {
                    b.HasOne("GRA.Data.Model.SchoolDistrict", "SchoolDistrict")
                        .WithMany("Schools")
                        .HasForeignKey("SchoolDistrictId");

                    b.HasOne("GRA.Data.Model.SchoolType", "SchoolType")
                        .WithMany()
                        .HasForeignKey("SchoolTypeId");
                });

            modelBuilder.Entity("GRA.Data.Model.System", b =>
                {
                    b.HasOne("GRA.Data.Model.Site")
                        .WithMany("Systems")
                        .HasForeignKey("SiteId");
                });

            modelBuilder.Entity("GRA.Data.Model.Trigger", b =>
                {
                    b.HasOne("GRA.Data.Model.AvatarBundle", "AwardAvatarBundle")
                        .WithMany()
                        .HasForeignKey("AwardAvatarBundleId");

                    b.HasOne("GRA.Data.Model.Badge", "AwardBadge")
                        .WithMany()
                        .HasForeignKey("AwardBadgeId");

                    b.HasOne("GRA.Data.Model.VendorCodeType", "AwardVendorCodeType")
                        .WithMany()
                        .HasForeignKey("AwardVendorCodeTypeId");

                    b.HasOne("GRA.Data.Model.Branch", "LimitToBranch")
                        .WithMany()
                        .HasForeignKey("LimitToBranchId");

                    b.HasOne("GRA.Data.Model.Program", "LimitToProgram")
                        .WithMany()
                        .HasForeignKey("LimitToProgramId");

                    b.HasOne("GRA.Data.Model.System", "LimitToSystem")
                        .WithMany()
                        .HasForeignKey("LimitToSystemId");
                });

            modelBuilder.Entity("GRA.Data.Model.TriggerBadge", b =>
                {
                    b.HasOne("GRA.Data.Model.Badge", "Badge")
                        .WithMany()
                        .HasForeignKey("BadgeId");

                    b.HasOne("GRA.Data.Model.Trigger", "Trigger")
                        .WithMany("RequiredBadges")
                        .HasForeignKey("TriggerId");
                });

            modelBuilder.Entity("GRA.Data.Model.TriggerChallenge", b =>
                {
                    b.HasOne("GRA.Data.Model.Challenge", "Challenge")
                        .WithMany()
                        .HasForeignKey("ChallengeId");

                    b.HasOne("GRA.Data.Model.Trigger", "Trigger")
                        .WithMany("RequiredChallenges")
                        .HasForeignKey("TriggerId");
                });

            modelBuilder.Entity("GRA.Data.Model.User", b =>
                {
                    b.HasOne("GRA.Data.Model.Branch", "Branch")
                        .WithMany("Users")
                        .HasForeignKey("BranchId");

                    b.HasOne("GRA.Data.Model.Program", "Program")
                        .WithMany()
                        .HasForeignKey("ProgramId");

                    b.HasOne("GRA.Data.Model.Site", "Site")
                        .WithMany()
                        .HasForeignKey("SiteId");

                    b.HasOne("GRA.Data.Model.System", "System")
                        .WithMany()
                        .HasForeignKey("SystemId");
                });

            modelBuilder.Entity("GRA.Data.Model.UserAnswer", b =>
                {
                    b.HasOne("GRA.Data.Model.Answer", "Answer")
                        .WithMany()
                        .HasForeignKey("AnswerId");

                    b.HasOne("GRA.Data.Model.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("GRA.Data.Model.UserAvatar", b =>
                {
                    b.HasOne("GRA.Data.Model.AvatarElement", "AvatarElement")
                        .WithMany()
                        .HasForeignKey("AvatarElementId");

                    b.HasOne("GRA.Data.Model.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("GRA.Data.Model.UserAvatarItem", b =>
                {
                    b.HasOne("GRA.Data.Model.AvatarItem", "AvatarItem")
                        .WithMany()
                        .HasForeignKey("AvatarItemId");

                    b.HasOne("GRA.Data.Model.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("GRA.Data.Model.UserBadge", b =>
                {
                    b.HasOne("GRA.Data.Model.Badge", "Badge")
                        .WithMany()
                        .HasForeignKey("BadgeId");

                    b.HasOne("GRA.Data.Model.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("GRA.Data.Model.UserBook", b =>
                {
                    b.HasOne("GRA.Data.Model.Book", "Book")
                        .WithMany()
                        .HasForeignKey("BookId");

                    b.HasOne("GRA.Data.Model.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("GRA.Data.Model.UserChallengeTask", b =>
                {
                    b.HasOne("GRA.Data.Model.ChallengeTask", "ChallengeTask")
                        .WithMany()
                        .HasForeignKey("ChallengeTaskId");

                    b.HasOne("GRA.Data.Model.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("GRA.Data.Model.UserFavoriteChallenge", b =>
                {
                    b.HasOne("GRA.Data.Model.Challenge", "Challenge")
                        .WithMany()
                        .HasForeignKey("ChallengeId");

                    b.HasOne("GRA.Data.Model.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("GRA.Data.Model.UserLog", b =>
                {
                    b.HasOne("GRA.Data.Model.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("GRA.Data.Model.UserQuestionnaire", b =>
                {
                    b.HasOne("GRA.Data.Model.Questionnaire", "Questionnaire")
                        .WithMany()
                        .HasForeignKey("QuestionnaireId");

                    b.HasOne("GRA.Data.Model.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("GRA.Data.Model.UserRole", b =>
                {
                    b.HasOne("GRA.Data.Model.Role", "Role")
                        .WithMany()
                        .HasForeignKey("RoleId");

                    b.HasOne("GRA.Data.Model.User", "User")
                        .WithMany("UserRoles")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("GRA.Data.Model.UserTrigger", b =>
                {
                    b.HasOne("GRA.Data.Model.Trigger", "Trigger")
                        .WithMany()
                        .HasForeignKey("TriggerId");

                    b.HasOne("GRA.Data.Model.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("GRA.Data.Model.VendorCode", b =>
                {
                    b.HasOne("GRA.Data.Model.VendorCodeType", "VendorCodeType")
                        .WithMany()
                        .HasForeignKey("VendorCodeTypeId");
                });

            modelBuilder.Entity("GRA.Data.Model.VendorCodeType", b =>
                {
                    b.HasOne("GRA.Data.Model.Site", "Site")
                        .WithMany()
                        .HasForeignKey("SiteId");
                });
        }
    }
}
