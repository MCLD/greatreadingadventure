using System.Linq;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Repository;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GRA.Data.Repository
{
    public class SegmentRepository
        : AuditingRepository<Model.Segment, Domain.Model.Segment>, ISegmentRepository
    {
        public SegmentRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<SegmentRepository> logger) : base(repositoryFacade, logger)
        {
        }

        public async Task<SegmentText> AddSaveTextAsync(int segmentId,
            int languageId,
            string text)
        {
            var segmentText = new Model.SegmentText
            {
                LanguageId = languageId,
                SegmentId = segmentId,
                Text = text
            };
            await _context.SegmentTexts.AddAsync(segmentText);
            await _context.SaveChangesAsync();
            return _mapper.Map<SegmentText>(segmentText);
        }

        public async Task<Segment> GetAsync(int segmentId)
        {
            return await _context.Segments
                .AsNoTracking()
                .Where(_ => _.Id == segmentId)
                .ProjectToType<Segment>()
                .SingleOrDefaultAsync();
        }

        public async Task<int[]> GetLanguagesAsync(int segmentId)
        {
            return await _context.SegmentTexts
                .AsNoTracking()
                .Where(_ => _.SegmentId == segmentId)
                .Select(_ => _.LanguageId)
                .ToArrayAsync();
        }

        public async Task<SegmentText> GetTextAsync(int segmentId, int languageId)
        {
            return await _context.SegmentTexts
                .AsNoTracking()
                .ProjectToType<SegmentText>()
                .SingleOrDefaultAsync(_ => _.LanguageId == languageId
                    && _.SegmentId == segmentId);
        }

        public async Task RemoveIfUnusedAsync(int segmentId)
        {
            var texts = _context.SegmentTexts
                .Any(_ => _.SegmentId == segmentId);

            if (!texts)
            {
                DbSet.Remove(await DbSet.SingleAsync(_ => _.Id == segmentId));
                await _context.SaveChangesAsync();
            }
        }

        public async Task RemoveTextAsync(int segmentId, int languageId)
        {
            var segmentText = _context.SegmentTexts
                .SingleOrDefault(_ => _.SegmentId == segmentId
                    && _.LanguageId == languageId)
                ?? throw new GraDbUpdateException($"Unable to find segment text for id {segmentId} in language {languageId}");
            _context.SegmentTexts.Remove(segmentText);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveTextsAsync(int segmentId)
        {
            _context.SegmentTexts.RemoveRange(_context
                .SegmentTexts
                .Where(_ => _.SegmentId == segmentId));
            await _context.SaveChangesAsync();
        }

        public async Task<bool> TextExistsAsync(int segmentId, int languageId)
        {
            return await _context.SegmentTexts
                .AsNoTracking()
                .AnyAsync(_ => _.SegmentId == segmentId
                    && _.LanguageId == languageId);
        }

        public async Task UpdateTextSaveAsync(int segmentId, int languageId, string text)
        {
            var segmentTexts = await _context.SegmentTexts
                .SingleOrDefaultAsync(_ => _.SegmentId == segmentId
                    && _.LanguageId == languageId)
                        ?? throw new GraDbUpdateException($"Unable to find SegmentText id {segmentId}, language id {languageId}");

            segmentTexts.Text = text;

            _context.Update(segmentTexts);

            await _context.SaveChangesAsync();
        }
    }
}
