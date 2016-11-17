using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Linq;

namespace GRA.Data
{
    public abstract class Context : IdentityDbContext<Domain.Model.User>
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
        protected internal Context(string connectionString) {
            devConnectionString = connectionString;
        }

        public void Migrate()
        {
            Database.Migrate();
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        public DbSet<Model.AuditLog> AuditLogs { get; set; }
        public DbSet<Model.Branch> Branches { get; set; }
        public DbSet<Model.Challenge> Challenges { get; set; }
        public DbSet<Model.ChallengeTask> ChallengeTasks { get; set; }
        public DbSet<Model.ChallengeTaskType> ChallengeTaskTypes { get; set; }
        public DbSet<Model.Program> Programs { get; set; }
        public DbSet<Model.Site> Sites { get; set; }
        public DbSet<Model.System> Systems { get; set; }
    }
}
