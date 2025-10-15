using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GRA.Data
{
    public abstract class Context : DbContext, IDataProtectionKeyContext
    {
        protected Context(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Model.Answer> Answers { get; set; }
        public DbSet<Model.Attachment> Attachments { get; set; }
        public DbSet<Model.AuditLog> AuditLogs { get; set; }
        public DbSet<Model.AuthorizationCode> AuthorizationCodes { get; set; }
        public DbSet<Model.AvatarBundleItem> AvatarBundleItems { get; set; }
        public DbSet<Model.AvatarBundle> AvatarBundles { get; set; }
        public DbSet<Model.AvatarColor> AvatarColors { get; set; }
        public DbSet<Model.AvatarColorText> AvatarColorTexts { get; set; }
        public DbSet<Model.AvatarElement> AvatarElements { get; set; }
        public DbSet<Model.AvatarItem> AvatarItems { get; set; }
        public DbSet<Model.AvatarItemText> AvatarItemTexts { get; set; }
        public DbSet<Model.AvatarLayer> AvatarLayers { get; set; }
        public DbSet<Model.AvatarLayerText> AvatarLayerTexts { get; set; }
        public DbSet<Model.Badge> Badges { get; set; }
        public DbSet<Model.Book> Books { get; set; }
        public DbSet<Model.Branch> Branches { get; set; }
        public DbSet<Model.Broadcast> Broadcasts { get; set; }
        public DbSet<Model.CarouselItem> CarouselItems { get; set; }
        public DbSet<Model.Carousel> Carousels { get; set; }
        public DbSet<Model.Category> Categories { get; set; }
        public DbSet<Model.ChallengeCategory> ChallengeCategories { get; set; }
        public DbSet<Model.ChallengeGroupChallenge> ChallengeGroupChallenges { get; set; }
        public DbSet<Model.ChallengeGroup> ChallengeGroups { get; set; }
        public DbSet<Model.Challenge> Challenges { get; set; }
        public DbSet<Model.ChallengeTask> ChallengeTasks { get; set; }
        public DbSet<Model.ChallengeTaskType> ChallengeTaskTypes { get; set; }
        public DbSet<Model.DashboardContent> DashboardContents { get; set; }
        public DbSet<DataProtectionKey> DataProtectionKeys { get; set; }
        public DbSet<Model.DirectEmailHistory> DirectEmailHistories { get; set; }
        public DbSet<Model.DirectEmailTemplate> DirectEmailTemplates { get; set; }
        public DbSet<Model.DirectEmailTemplateText> DirectEmailTemplateTexts { get; set; }
        public DbSet<Model.DrawingCriterion> DrawingCriteria { get; set; }
        public DbSet<Model.DrawingCriterionProgram> DrawingCriterionPrograms { get; set; }
        public DbSet<Model.Drawing> Drawings { get; set; }
        public DbSet<Model.EmailBase> EmailBases { get; set; }
        public DbSet<Model.EmailBaseText> EmailBaseTexts { get; set; }
        public DbSet<Model.EmailReminder> EmailReminders { get; set; }
        public DbSet<Model.EmailSubscriptionAuditLog> EmailSubscriptionAuditLogs { get; set; }
        public DbSet<Model.Event> Events { get; set; }
        public DbSet<Model.ExitLandingMessageSet> ExitLandingMessageSets { get; set; }
        public DbSet<Model.FeaturedChallengeGroup> FeaturedChallengeGroups { get; set; }
        public DbSet<Model.FeaturedChallengeGroupText> FeaturedChallengeGroupTexts { get; set; }
        public DbSet<Model.GroupInfo> GroupInfos { get; set; }
        public DbSet<Model.GroupType> GroupTypes { get; set; }
        public DbSet<Model.Job> Jobs { get; set; }
        public DbSet<Model.Language> Languages { get; set; }
        public DbSet<Model.Location> Locations { get; set; }
        public DbSet<Model.Mail> Mails { get; set; }
        public DbSet<Model.MessageTemplate> MessageTemplates { get; set; }
        public DbSet<Model.MessageTemplateText> MessageTemplateTexts { get; set; }
        public DbSet<Model.NewsCategory> NewsCateories { get; set; }
        public DbSet<Model.NewsPost> NewsPosts { get; set; }
        public DbSet<Model.Notification> Notifications { get; set; }
        public DbSet<Model.PageHeader> PageHeaders { get; set; }
        public DbSet<Model.Page> Pages { get; set; }
        public DbSet<Model.Permission> Permissions { get; set; }
        public DbSet<Model.PointTranslation> PointTranslations { get; set; }
        public DbSet<Model.PrizeWinner> PrizeWinners { get; set; }
        public DbSet<Model.Program> Programs { get; set; }
        public DbSet<Model.PsAgeGroup> PsAgeGroups { get; set; }
        public DbSet<Model.PsBackToBack> PsBackToBack { get; set; }
        public DbSet<Model.PsBlackoutDate> PsBlackoutDates { get; set; }
        public DbSet<Model.PsBranchSelection> PsBranchSelections { get; set; }
        public DbSet<Model.PsExcludeBranch> PsExcludeBranches { get; set; }
        public DbSet<Model.PsKitAgeGroup> PsKitAgeGroups { get; set; }
        public DbSet<Model.PsKitImage> PsKitImages { get; set; }
        public DbSet<Model.PsKit> PsKits { get; set; }
        public DbSet<Model.PsPerformerBranch> PsPerformerBranches { get; set; }
        public DbSet<Model.PsPerformerImage> PsPerformerImages { get; set; }
        public DbSet<Model.PsPerformer> PsPerformers { get; set; }
        public DbSet<Model.PsPerformerSchedule> PsPerformerSchedules { get; set; }
        public DbSet<Model.PsProgramAgeGroup> PsProgramAgeGroups { get; set; }
        public DbSet<Model.PsProgramImage> PsProgramImages { get; set; }
        public DbSet<Model.PsProgram> PsPrograms { get; set; }
        public DbSet<Model.PsSettings> PsSettings { get; set; }
        public DbSet<Model.Questionnaire> Questionnaires { get; set; }
        public DbSet<Model.Question> Questions { get; set; }
        public DbSet<Model.RecoveryToken> RecoveryTokens { get; set; }
        public DbSet<Model.ReportCriterion> ReportCriteria { get; set; }
        public DbSet<Model.ReportRequest> ReportRequests { get; set; }
        public DbSet<Model.RequiredQuestionnaire> RequiredQuestionnaires { get; set; }
        public DbSet<Model.RolePermission> RolePermissions { get; set; }
        public DbSet<Model.Role> Roles { get; set; }
        public DbSet<Model.SchoolDistrict> SchoolDistricts { get; set; }
        public DbSet<Model.School> Schools { get; set; }
        public DbSet<Model.Segment> Segments { get; set; }
        public DbSet<Model.SegmentText> SegmentTexts { get; set; }
        public DbSet<Model.Site> Sites { get; set; }
        public DbSet<Model.SiteSetting> SiteSettings { get; set; }
        public DbSet<Model.SocialHeader> SocialHeaders { get; set; }
        public DbSet<Model.Social> Socials { get; set; }
        public DbSet<Model.SpatialDistanceDetail> SpatialDistanceDetails { get; set; }
        public DbSet<Model.SpatialDistanceHeader> SpatialDistanceHeaders { get; set; }
        public DbSet<Model.System> Systems { get; set; }
        public DbSet<Model.TriggerBadge> TriggerBadges { get; set; }
        public DbSet<Model.TriggerChallenge> TriggerChallenges { get; set; }
        public DbSet<Model.Trigger> Triggers { get; set; }
        public DbSet<Model.UserAnswer> UserAnswers { get; set; }
        public DbSet<Model.UserAvatarItem> UserAvatarItems { get; set; }
        public DbSet<Model.UserAvatar> UserAvatars { get; set; }
        public DbSet<Model.UserBadge> UserBadges { get; set; }
        public DbSet<Model.UserBook> UserBooks { get; set; }
        public DbSet<Model.UserChallengeTask> UserChallengeTasks { get; set; }
        public DbSet<Model.UserFavoriteChallenge> UserFavoriteChallenges { get; set; }
        public DbSet<Model.UserFavoriteEvent> UserFavoriteEvents { get; set; }
        public DbSet<Model.UserLog> UserLogs { get; set; }
        public DbSet<Model.UserPackingSlipView> UserPackingSlipViews { get; set; }
        public DbSet<Model.UserQuestionnaire> UserQuestionnaires { get; set; }
        public DbSet<Model.UserRole> UserRoles { get; set; }
        public DbSet<Model.User> Users { get; set; }
        public DbSet<Model.UserTrigger> UserTriggers { get; set; }
        public DbSet<Model.VendorCodePackingSlip> VendorCodePackingSlips { get; set; }
        public DbSet<Model.VendorCode> VendorCodes { get; set; }
        public DbSet<Model.VendorCodeType> VendorCodeTypes { get; set; }

        public async Task<string> GetCurrentMigrationAsync()
        {
            return (await Database.GetAppliedMigrationsAsync()).Last();
        }

        public async Task<IEnumerable<string>> GetMigrationsListAsync()
        {
            return await Database.GetAppliedMigrationsAsync();
        }

        public IEnumerable<string> GetPendingMigrations()
        {
            return Database.GetPendingMigrations();
        }

        public async Task<IEnumerable<string>> GetPendingMigrationsAsync()
        {
            return await Database.GetPendingMigrationsAsync();
        }

        public void Migrate()
        {
            Database.Migrate();
        }

        public async Task MigrateAsync()
        {
            await Database.MigrateAsync();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ArgumentNullException.ThrowIfNull(modelBuilder);

            // turn off cascading deletes
            foreach (var relationship in modelBuilder.Model
                .GetEntityTypes()
                .SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }

            // configure composite keys
            // https://docs.microsoft.com/en-us/ef/core/modeling/keys
            modelBuilder.Entity<Model.AvatarBundleItem>()
                .HasKey(_ => new { _.AvatarBundleId, _.AvatarItemId });
            modelBuilder.Entity<Model.AvatarColorText>()
                .HasKey(_ => new { _.AvatarColorId, _.LanguageId });
            modelBuilder.Entity<Model.AvatarItemText>()
                .HasKey(_ => new { _.AvatarItemId, _.LanguageId });
            modelBuilder.Entity<Model.AvatarLayerText>()
                .HasKey(_ => new { _.AvatarLayerId, _.LanguageId });
            modelBuilder.Entity<Model.ChallengeCategory>()
                .HasKey(_ => new { _.ChallengeId, _.CategoryId });
            modelBuilder.Entity<Model.ChallengeGroupChallenge>()
                .HasKey(_ => new { _.ChallengeGroupId, _.ChallengeId });
            modelBuilder.Entity<Model.DirectEmailTemplateText>()
                .HasKey(_ => new { _.DirectEmailTemplateId, _.LanguageId });
            modelBuilder.Entity<Model.DrawingCriterionProgram>()
                .HasKey(_ => new { _.DrawingCriterionId, _.ProgramId });
            modelBuilder.Entity<Model.EmailBaseText>()
                .HasKey(_ => new { _.EmailBaseId, _.LanguageId });
            modelBuilder.Entity<Model.FeaturedChallengeGroupText>()
                .HasKey(_ => new { _.FeaturedChallengeGroupId, _.LanguageId });
            modelBuilder.Entity<Model.MessageTemplateText>()
                .HasKey(_ => new { _.MessageTemplateId, _.LanguageId });
            modelBuilder.Entity<Model.UserPackingSlipView>()
                .HasKey(_ => new { _.UserId, _.PackingSlip });
            modelBuilder.Entity<Model.PsBackToBack>()
                .HasKey(_ => new { _.PsAgeGroupId, _.BranchId });
            modelBuilder.Entity<Model.PsKitAgeGroup>()
                .HasKey(_ => new { _.KitId, _.AgeGroupId });
            modelBuilder.Entity<Model.PsPerformerBranch>()
                .HasKey(_ => new { _.PsPerformerId, _.BranchId });
            modelBuilder.Entity<Model.PsProgramAgeGroup>()
                .HasKey(_ => new { _.ProgramId, _.AgeGroupId });
            modelBuilder.Entity<Model.RolePermission>()
                .HasKey(_ => new { _.RoleId, _.PermissionId });
            modelBuilder.Entity<Model.SegmentText>()
                .HasKey(_ => new { _.LanguageId, _.SegmentId });
            modelBuilder.Entity<Model.Social>()
                .HasKey(_ => new { _.SocialHeaderId, _.LanguageId });
            modelBuilder.Entity<Model.TriggerBadge>()
                .HasKey(_ => new { _.TriggerId, _.BadgeId });
            modelBuilder.Entity<Model.TriggerChallenge>()
                .HasKey(_ => new { _.TriggerId, _.ChallengeId });
            modelBuilder.Entity<Model.UserAnswer>()
                .HasKey(_ => new { _.UserId, _.AnswerId });
            modelBuilder.Entity<Model.UserAvatar>()
                .HasKey(_ => new { _.UserId, _.AvatarElementId });
            modelBuilder.Entity<Model.UserAvatarItem>()
                .HasKey(_ => new { _.UserId, _.AvatarItemId });
            modelBuilder.Entity<Model.UserBadge>()
                .HasKey(_ => new { _.UserId, _.BadgeId });
            modelBuilder.Entity<Model.UserBook>()
                .HasKey(_ => new { _.UserId, _.BookId });
            modelBuilder.Entity<Model.UserChallengeTask>()
                .HasKey(_ => new { _.UserId, _.ChallengeTaskId });
            modelBuilder.Entity<Model.UserFavoriteChallenge>()
                .HasKey(_ => new { _.UserId, _.ChallengeId });
            modelBuilder.Entity<Model.UserFavoriteEvent>()
                .HasKey(_ => new { _.UserId, _.EventId });
            modelBuilder.Entity<Model.UserQuestionnaire>()
                .HasKey(_ => new { _.UserId, _.QuestionnaireId });
            modelBuilder.Entity<Model.UserRole>()
                .HasKey(_ => new { _.UserId, _.RoleId });
            modelBuilder.Entity<Model.UserTrigger>()
                .HasKey(_ => new { _.UserId, _.TriggerId });

            // add indexing as needed
            // https://docs.microsoft.com/en-us/ef/core/modeling/indexes
            modelBuilder.Entity<Model.EmailReminder>()
                .HasIndex(_ => new { _.Email, _.SignUpSource })
                .IsUnique();
            modelBuilder.Entity<Model.Job>()
                .HasIndex(_ => new { _.JobToken })
                .IsUnique();
            modelBuilder.Entity<Model.Mail>()
                .HasIndex(_ => new { _.ToUserId, _.IsDeleted, _.IsNew });
            modelBuilder.Entity<Model.Notification>()
                .HasIndex(_ => _.UserId);
            modelBuilder.Entity<Model.PrizeWinner>()
                .HasIndex(_ => new { _.DrawingId, _.UserId, _.RedeemedAt })
                .IsUnique();
            modelBuilder.Entity<Model.PageHeader>()
                .HasIndex(_ => new { _.SiteId, _.Stub })
                .IsUnique();
            modelBuilder.Entity<Model.User>()
                .HasIndex(_ => new { _.SiteId, _.IsDeleted, _.Username })
                .IsUnique();
            modelBuilder.Entity<Model.User>()
                .HasIndex(_ => new { _.SiteId, _.Id, _.IsDeleted, _.HouseholdHeadUserId })
                .IsUnique();
            modelBuilder.Entity<Model.UserLog>()
                .HasIndex(_ => new { _.UserId, _.IsDeleted, _.ChallengeId });
            modelBuilder.Entity<Model.UserLog>()
                .HasIndex(_ => new { _.UserId, _.IsDeleted, _.BadgeId });
            modelBuilder.Entity<Model.UserLog>()
                .HasIndex(_ => new { _.UserId, _.IsDeleted, _.PointTranslationId, _.ActivityEarned });

            // call the base OnModelCreating
            base.OnModelCreating(modelBuilder);
        }
    }
}
