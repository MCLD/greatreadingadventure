using Microsoft.EntityFrameworkCore;

namespace GRA.Data.SqlServer
{
    public class SqlServerContext : Context
    {
        public SqlServerContext(DbContextOptions<SqlServerContext> options) : base(options) { }
    }
}
