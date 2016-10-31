using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;

namespace GRA.Data
{
    public abstract class Context : DbContext
    {
        protected readonly IConfigurationRoot config;
        public Context(IConfigurationRoot config)
        {
            if (config == null)
            {
                throw new ArgumentNullException("config");
            }
            this.config = config;
        }

        public DbSet<Model.Site> Sites { get; set; }
    }
}
