using GRA.Domain.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRA.Data.Repository
{
    public class SystemInformationRepository : ISystemInformationRepository
    {
        private readonly Context _context;
        public SystemInformationRepository(Context context)
        {
            _context = Require.IsNotNull(context, nameof(context));
        }
        public async Task<string> GetCurrentMigrationAsync()
        {
            return await _context.GetCurrentMigrationAsync();
        }

        public async Task<IEnumerable<string>> GetMigrationsListAsync()
        {
            return await _context.GetMigrationsListAsync();
        }
    }
}
