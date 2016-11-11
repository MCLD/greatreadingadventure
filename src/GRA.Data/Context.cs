using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace GRA.Data
{
    public abstract class Context : IdentityDbContext<Domain.Model.Participant>
    {
        protected readonly string devConnectionString;
        protected readonly IConfigurationRoot config;
        public Context(IConfigurationRoot config)
        {
            if (config == null)
            {
                throw new ArgumentNullException("config");
            }
            this.config = config;
            devConnectionString = null;
        }
        protected internal Context(string connectionString) {
            devConnectionString = connectionString;
        }

        public DbSet<Model.Site> Sites { get; set; }
    }
}
