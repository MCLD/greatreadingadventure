using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Data.SQLite
{
    public class SQLiteContext : Context
    {
        public SQLiteContext(IConfigurationRoot config) : base(config) { }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(config["ConnectionStrings:DefaultConnection"]);
        }
    }
}
