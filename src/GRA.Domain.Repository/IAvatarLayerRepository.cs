using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Domain.Model;

namespace GRA.Domain.Repository
{
    public interface IAvatarLayerRepository : IRepository<AvatarLayer>
    {
        Task<ICollection<AvatarLayer>> GetAllAsync(int siteId);
        Task<ICollection<AvatarLayer>> GetAllWithColorsAsync(int siteId);
        Dictionary<string, string> GetNameAndLabelByLanguageId(int layerId, int languageId);
        Task<string> GetNameByLanguageIdAsync(int layerId, int languageId);
        Task AddAvatarLayerTextAsync(int layerId, int languageId, AvatarLayerText text);
    }
}
