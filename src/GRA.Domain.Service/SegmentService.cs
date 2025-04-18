using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GRA.Abstract;
using GRA.Domain.Model;
using GRA.Domain.Repository;
using Microsoft.Extensions.Logging;

namespace GRA.Domain.Service
{
    public class SegmentService : Abstract.BaseService<SegmentService>
    {
        private readonly IGraCache _cache;
        private readonly LanguageService _languageService;
        private readonly ISegmentRepository _segmentRepository;
        private readonly int CacheSegmentsHours;

        public SegmentService(IDateTimeProvider dateTimeProvider,
            IGraCache cache,
            ILogger<SegmentService> logger,
            ISegmentRepository segmentRepository,
            LanguageService languageService) : base(logger, dateTimeProvider)
        {
            CacheSegmentsHours = 8;

            ArgumentNullException.ThrowIfNull(cache);
            ArgumentNullException.ThrowIfNull(languageService);
            ArgumentNullException.ThrowIfNull(segmentRepository);

            _cache = cache;
            _languageService = languageService;
            _segmentRepository = segmentRepository;
        }

        public async Task<SegmentText> AddTextAsync(int activeUserId,
            int languageId,
            string text,
            string name)
        {
            var segment = await _segmentRepository
                    .AddSaveAsync(activeUserId, new Segment
                    {
                        CreatedAt = _dateTimeProvider.Now,
                        CreatedBy = activeUserId,
                        Name = GetSegmentName(name)
                    });

            var segmentText = await _segmentRepository.AddSaveTextAsync(segment.Id,
                languageId,
                text);

            await _cache.RemoveAsync(GetCacheKey(CacheKey.SegmentText, segment.Id, languageId));

            return segmentText;
        }

        public async Task<IDictionary<int, int[]>> GetLanguageStatusAsync(int?[] segmentIds)
        {
            var languageStatus = new Dictionary<int, int[]>();

            foreach (var segmentId in segmentIds.Where(_ => _.HasValue))
            {
                languageStatus.Add(segmentId.Value,
                    await _segmentRepository.GetLanguagesAsync(segmentId.Value));
            }

            return languageStatus;
        }

        public async Task<string> GetNameAsync(int segmentId)
        {
            return await _segmentRepository.GetNameAsync(segmentId);
        }

        public async Task<string> GetTextAsync(int segmentId, int languageId, bool forceReload)
        {
            var segmentTextValue = await GetCacheTextAsync(segmentId, languageId, forceReload);

            if (string.IsNullOrEmpty(segmentTextValue))
            {
                var defaultLanguageId = await _languageService.GetDefaultLanguageIdAsync();
                segmentTextValue = await GetCacheTextAsync(segmentId,
                    defaultLanguageId,
                    forceReload);
            }

            return segmentTextValue;
        }

        public async Task<SegmentText> MCGetTextAsync(int segmentId, int languageId)
        {
            return await _segmentRepository.GetTextAsync(segmentId, languageId);
        }

        public async Task UpdateTextAsync(int segmentId,
            int languageId,
            string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                await _segmentRepository.RemoveTextAsync(segmentId, languageId);
                await _segmentRepository.RemoveIfUnusedAsync(segmentId);
            }
            else
            {
                if (await _segmentRepository.TextExistsAsync(segmentId, languageId))
                {
                    await _segmentRepository.UpdateTextSaveAsync(segmentId,
                        languageId,
                        text);
                }
                else
                {
                    await _segmentRepository.AddSaveTextAsync(segmentId,
                        languageId,
                        text);
                }
            }
            await _cache.RemoveAsync(GetCacheKey(CacheKey.SegmentText, segmentId, languageId));
        }

        private static string GetSegmentName(string item) => item switch
        {
            nameof(VendorCodeType.DonationSegmentId) => SegmentNames.VendorCodeDonation,
            nameof(VendorCodeType.EmailAwardSegmentId) => SegmentNames.VendorCodeEmailAward,
            _ => item
        };

        private async Task<string> GetCacheTextAsync(int segmentId,
            int languageId,
            bool forceReload)
        {
            var cacheKey = GetCacheKey(CacheKey.SegmentText, segmentId, languageId);

            string segmentTextValue = forceReload
                ? null
                : await _cache.GetStringFromCache(cacheKey);

            if (!string.IsNullOrEmpty(segmentTextValue))
            {
                return segmentTextValue;
            }

            var segmentText = await _segmentRepository.GetTextAsync(segmentId, languageId);

            if (segmentText != null && !string.IsNullOrEmpty(segmentText.Text))
            {
                await _cache.SaveToCacheAsync(cacheKey, segmentText.Text, CacheSegmentsHours);
            }

            return segmentText?.Text;
        }
    }
}
