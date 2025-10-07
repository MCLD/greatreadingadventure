using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;

namespace GRA.Domain.Repository
{
    public interface IAvatarColorRepository : IRepository<AvatarColor>
    {
        Task AddTextsAsync(IEnumerable<AvatarColorText> texts);
        Task<ICollection<AvatarColor>> GetByLayerAsync(int layerId, int languageId);
        Task<IEnumerable<AvatarColorText>> GetTextsByColorIdsAsync(
            IEnumerable<int> colorIds);
        Task<DataWithCount<ICollection<AvatarColor>>> PageAsync(AvatarFilter filter);
        void RemoveTexts(IEnumerable<AvatarColorText> texts);
        void UpdateTexts(IEnumerable<AvatarColorText> texts);
    }
}
