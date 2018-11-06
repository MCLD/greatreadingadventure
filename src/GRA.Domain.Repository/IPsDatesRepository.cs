using System.Threading.Tasks;
using GRA.Domain.Model;

namespace GRA.Domain.Repository
{
    public interface IPsSettingsRepository : IRepository<PsSettings>
    {
        Task<PsSettings> GetBySiteIdAsync(int siteId);
    }
}
