using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Data
{
    public abstract class Context : DbContext
    {
        protected readonly IConfigurationRoot config;
        public Context(IConfigurationRoot config)
        {
            this.config = config;
        }

        public DbSet<Model.Site> Sites { get; set; }
    }
}
