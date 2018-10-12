using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace GRA.Data.SQLite
{
    public class SQLiteContext : Context
    {
        public SQLiteContext(DbContextOptions<SQLiteContext> options) : base(options) { }
    }
}
