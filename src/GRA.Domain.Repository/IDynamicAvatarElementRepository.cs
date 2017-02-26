using GRA.Domain.Model;
using System.Threading.Tasks;

namespace GRA.Domain.Repository
{
    public interface IDynamicAvatarElementRepository : IRepository<Model.DynamicAvatarElement>
    {
        Task<bool> ExistsAsync(int dynamicAvatarLayerId, int id);
        Task<int> GetFirstElement(int dynamicAvatarLayerId);
        Task<int> GetIdByLayerIdAsync(int dynamicAvatarLayerId);
        Task<int> GetLastElement(int dynamicAvatarLayerId);
        Task<int?> GetNextElement(int dynamicAvatarLayerId, int elementId);
        Task<int?> GetPreviousElement(int dynamicAvatarLayerId, int elementId);
    }
}
