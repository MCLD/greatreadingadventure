using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Data
{
    public class Context : DbContext
    {
        public DbSet<Model.Site> Sites { get; set; }
    }
}
