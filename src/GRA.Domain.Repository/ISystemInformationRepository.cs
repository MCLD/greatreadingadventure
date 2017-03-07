using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRA.Domain.Repository
{
    public interface ISystemInformationRepository
    {
        Task<string> GetCurrentMigrationAsync();
        Task<IEnumerable<string>> GetMigrationsListAsync();
    }
}
