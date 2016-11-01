using Microsoft.EntityFrameworkCore.Infrastructure;

namespace GRA.Data.SqlServer
{
    internal class SqlServerContextFactory : IDbContextFactory<SqlServerContext>
    {
        SqlServerContext IDbContextFactory<SqlServerContext>.Create(DbContextFactoryOptions options)
        {
            return new SqlServerContext();
        }
    }
}
