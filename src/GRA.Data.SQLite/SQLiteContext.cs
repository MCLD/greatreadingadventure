using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;

namespace GRA.Data.SQLite
{
    public class SQLiteContext : Context
    {
        private const string defaultDevCs = "Filename=./gra4.db";
        public SQLiteContext(IConfigurationRoot config) : base(config) {}
        internal SQLiteContext() : base(defaultDevCs) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!string.IsNullOrEmpty(devConnectionString))
            {
                optionsBuilder.UseSqlite(devConnectionString);
            }
            else
            {
                optionsBuilder.UseSqlite(config["ConnectionStrings:DefaultConnection"]);
            }
        }
    }
}
