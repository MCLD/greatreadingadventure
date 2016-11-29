using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore.Metadata;

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
            modelBuilder.Entity<Model.UserRole>()
                .HasKey(_ => new { _.UserId, _.RoleId });
            modelBuilder.Entity<Model.RolePermission>()
                .HasKey(_ => new { _.RoleId, _.PermissionId });

            // add indexing as needed
            // https://docs.microsoft.com/en-us/ef/core/modeling/indexes
            modelBuilder.Entity<Model.User>()
                .HasIndex(_ => new { _.SiteId, _.Username })
                .IsUnique();
            modelBuilder.Entity<Model.User>()
                .HasIndex(_ => new { _.SiteId, _.HouseholdHeadUserId });
            modelBuilder.Entity<Model.UserLog>()
                .HasIndex(_ => _.UserId);

            // call the base OnModelCreating
            base.OnModelCreating(modelBuilder);
        }

        public void Migrate()
        {
            Database.Migrate();
        }

        public DbSet<Model.AuditLog> AuditLogs { get; set; }
        public DbSet<Model.Branch> Branches { get; set; }
        public DbSet<Model.Challenge> Challenges { get; set; }
        public DbSet<Model.ChallengeTask> ChallengeTasks { get; set; }
        public DbSet<Model.ChallengeTaskType> ChallengeTaskTypes { get; set; }
        public DbSet<Model.Mail> Mails { get; set; }
        public DbSet<Model.Permission> Permissions { get; set; }
        public DbSet<Model.PointTranslation> PointTranslations { get; set; }
        public DbSet<Model.Program> Programs { get; set; }
        public DbSet<Model.Role> Roles { get; set; }
        public DbSet<Model.RolePermission> RolePermissions { get; set; }
        public DbSet<Model.Site> Sites { get; set; }
        public DbSet<Model.System> Systems { get; set; }
        public DbSet<Model.UserLog> UserLogs { get; set; }
        public DbSet<Model.User> Users { get; set; }
        public DbSet<Model.UserRole> UserRoles { get; set; }
    }
}
