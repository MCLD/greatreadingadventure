using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GRA
{
    public static class DefaultConnectionString
    {
        public const string SqlServer = @"Server=(localdb)\mssqllocaldb;Database=gra4;Trusted_Connection=True;MultipleActiveResultSets=true";
        public const string SQLite = @"Filename=./gra4.db";
    }
}
