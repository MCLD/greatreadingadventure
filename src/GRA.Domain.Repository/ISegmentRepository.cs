using System.Threading.Tasks;
using GRA.Domain.Model;

namespace GRA.Domain.Repository
{
    public interface ISegmentRepository : IRepository<Segment>
    {
        public Task<SegmentText> AddSaveTextAsync(int segmentId,
            int languageId,
            string text);

        public Task<Segment> GetAsync(int segmentId);

        public Task<int[]> GetLanguagesAsync(int segmentId);

        public Task<SegmentText> GetTextAsync(int segmentId, int languageId);

        public Task RemoveIfUnusedAsync(int segmentId);

        public Task RemoveTextAsync(int segmentId, int languageId);

        public Task RemoveTextsAsync(int segmentId);

        public Task<bool> TextExistsAsync(int segmentId, int languageId);

        public Task UpdateTextSaveAsync(int segmentId, int languageId, string text);
    }
}
