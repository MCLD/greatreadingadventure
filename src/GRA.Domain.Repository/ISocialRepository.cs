using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Domain.Model;

namespace GRA.Domain.Repository
{
    public interface ISocialRepository
    {
        public Task<Social> AddSaveAsync(Social social);

        public Task<ICollection<Social>> GetByHeaderIdsAsync(IEnumerable<int> headerIds);

        public Task<Social> GetByHeaderLanguageAsync(int headerId, int languageId);

        public Task RemoveSaveAsync(int headerId, int languageId);

        public Task<Social> UpdateSaveAsync(Social social);
    }
}