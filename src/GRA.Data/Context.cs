using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Data
{
    public abstract class Context : DbContext
    {
        protected readonly string devConnectionString;
        protected readonly IConfigurationRoot config;
        public Context(IConfigurationRoot config)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }
            this.config = config;
            devConnectionString = null;
        }
        protected internal Context(string connectionString)
        {
            devConnectionString = connectionString;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // turn off cascading deletes
            foreach (var relationship in modelBuilder.Model
                .GetEntityTypes()
                .SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }

            // configure composite keys
            // https://docs.microsoft.com/en-us/ef/core/modeling/keys
            modelBuilder.Entity<Model.ChallengeCategory>()
                .HasKey(_ => new { _.ChallengeId, _.CategoryId });
            modelBuilder.Entity<Model.DrawingCriterionProgram>()
                .HasKey(_ => new { _.DrawingCriterionId, _.ProgramId });
            modelBuilder.Entity<Model.DynamicAvatarBundleItem>()
                .HasKey(_ => new { _.DynamicAvatarBundleId, _.DynamicAvatarItemId });
            modelBuilder.Entity<Model.RolePermission>()
                .HasKey(_ => new { _.RoleId, _.PermissionId });
            modelBuilder.Entity<Model.TriggerBadge>()
                .HasKey(_ => new { _.TriggerId, _.BadgeId });
            modelBuilder.Entity<Model.TriggerChallenge>()
                .HasKey(_ => new { _.TriggerId, _.ChallengeId });
            modelBuilder.Entity<Model.UserAnswer>()
                .HasKey(_ => new { _.UserId, _.AnswerId });
            modelBuilder.Entity<Model.UserAvatarItem>()
                .HasKey(_ => new { _.UserId, _.DynamicAvatarItemId });
            modelBuilder.Entity<Model.UserBadge>()
                .HasKey(_ => new { _.UserId, _.BadgeId });
            modelBuilder.Entity<Model.UserBook>()
                .HasKey(_ => new { _.UserId, _.BookId });
            modelBuilder.Entity<Model.UserChallengeTask>()
                .HasKey(_ => new { _.UserId, _.ChallengeTaskId });
            modelBuilder.Entity<Model.UserDynamicAvatar>()
                .HasKey(_ => new { _.UserId, _.DynamicAvatarElementId });
            modelBuilder.Entity<Model.UserFavoriteChallenge>()
                .HasKey(_ => new { _.UserId, _.ChallengeId });
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
            modelBuilder.Entity<Model.Mail>()
                .HasIndex(_ => new { _.ToUserId, _.IsDeleted, _.IsNew });
            modelBuilder.Entity<Model.Notification>()
                .HasIndex(_ => _.UserId);
            modelBuilder.Entity<Model.PrizeWinner>()
                .HasIndex(_ => new { _.DrawingId, _.UserId, _.RedeemedAt })
                .IsUnique();
            modelBuilder.Entity<Model.Page>()
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

        public void Migrate()
        {
            Database.Migrate();
        }

        public async Task MigrateAsync()
        {
            await Database.MigrateAsync();
        }

        public async Task<IEnumerable<string>> GetMigrationsListAsync()
        {
            return await Database.GetAppliedMigrationsAsync();
        }

        public async Task<string> GetCurrentMigrationAsync()
        {
            return (await Database.GetAppliedMigrationsAsync()).Last();
        }

        public DbSet<Model.Answer> Answers { get; set; }
        public DbSet<Model.AuditLog> AuditLogs { get; set; }
        public DbSet<Model.AuthorizationCode> AuthorizationCodes { get; set; }
        public DbSet<Model.Badge> Badges { get; set; }
        public DbSet<Model.Book> Books { get; set; }
        public DbSet<Model.Branch> Branches { get; set; }
        public DbSet<Model.Broadcast> Broadcasts { get; set; }
        public DbSet<Model.Category> Categories { get; set; }
        public DbSet<Model.ChallengeCategory> ChallengeCategories { get; set; }
        public DbSet<Model.Challenge> Challenges { get; set; }
        public DbSet<Model.ChallengeTask> ChallengeTasks { get; set; }
        public DbSet<Model.ChallengeTaskType> ChallengeTaskTypes { get; set; }
        public DbSet<Model.Drawing> Drawings { get; set; }
        public DbSet<Model.DrawingCriterion> DrawingCriteria { get; set; }
        public DbSet<Model.DrawingCriterionProgram> DrawingCriterionPrograms { get; set; }
        public DbSet<Model.DynamicAvatarBundle> DynamicAvatarBundles { get; set; }
        public DbSet<Model.DynamicAvatarBundleItem> DynamicAvatarBundleItems { get; set; }
        public DbSet<Model.DynamicAvatarColor> DynamicAvatarColors { get; set; }
        public DbSet<Model.DynamicAvatarElement> DynamicAvatarElements { get; set; }
        public DbSet<Model.DynamicAvatarItem> DynamicAvatarItems { get; set; }
        public DbSet<Model.DynamicAvatarLayer> DynamicAvatarLayers { get; set; }
        public DbSet<Model.EmailReminder> EmailReminders { get; set; }
        public DbSet<Model.EnteredSchool> EnteredSchools { get; set; }
        public DbSet<Model.Event> Events { get; set; }
        public DbSet<Model.Location> Locations { get; set; }
        public DbSet<Model.Mail> Mails { get; set; }
        public DbSet<Model.Notification> Notifications { get; set; }
        public DbSet<Model.Page> Pages { get; set; }
        public DbSet<Model.Permission> Permissions { get; set; }
        public DbSet<Model.PointTranslation> PointTranslations { get; set; }
        public DbSet<Model.PrizeWinner> PrizeWinners { get; set; }
        public DbSet<Model.Program> Programs { get; set; }
        public DbSet<Model.Questionnaire> Questionnaires { get; set; }
        public DbSet<Model.Question> Questions { get; set; }
        public DbSet<Model.RecoveryToken> RecoveryTokens { get; set; }
        public DbSet<Model.ReportCriterion> ReportCriteria { get; set; }
        public DbSet<Model.ReportRequest> ReportRequests { get; set; }
        public DbSet<Model.RequiredQuestionnaire> RequiredQuestionnaires { get; set; }
        public DbSet<Model.Role> Roles { get; set; }
        public DbSet<Model.RolePermission> RolePermissions { get; set; }
        public DbSet<Model.School> Schools { get; set; }
        public DbSet<Model.SchoolDistrict> SchoolDistricts { get; set; }
        public DbSet<Model.SchoolType> SchoolTypes { get; set; }
        public DbSet<Model.Site> Sites { get; set; }
        public DbSet<Model.StaticAvatar> StaticAvatars { get; set; }
        public DbSet<Model.System> Systems { get; set; }
        public DbSet<Model.Trigger> Triggers { get; set; }
        public DbSet<Model.TriggerBadge> TriggerBadges { get; set; }
        public DbSet<Model.TriggerChallenge> TriggerChallenges { get; set; }
        public DbSet<Model.UserLog> UserLogs { get; set; }
        public DbSet<Model.User> Users { get; set; }
        public DbSet<Model.UserAnswer> UserAnswers { get; set; }
        public DbSet<Model.UserAvatarItem> UserAvatarItems { get; set; }
        public DbSet<Model.UserChallengeTask> UserChallengeTasks { get; set; }
        public DbSet<Model.UserBadge> UserBadges { get; set; }
        public DbSet<Model.UserBook> UserBooks { get; set; }
        public DbSet<Model.UserDynamicAvatar> UserDynamicAvatars { get; set; }
        public DbSet<Model.UserFavoriteChallenge> UserFavoriteChallenges { get; set; }
        public DbSet<Model.UserQuestionnaire> UserQuestionnaires { get; set; }
        public DbSet<Model.UserRole> UserRoles { get; set; }
        public DbSet<Model.UserTrigger> UserTriggers { get; set; }
        public DbSet<Model.VendorCode> VendorCodes { get; set; }
        public DbSet<Model.VendorCodeType> VendorCodeTypes { get; set; }
    }
}