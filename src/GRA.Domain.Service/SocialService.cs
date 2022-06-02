using System;
using System.Text.Json;
using System.Threading.Tasks;
using GRA.Abstract;
using GRA.Domain.Model;
using GRA.Domain.Repository;
using Microsoft.Extensions.Logging;

namespace GRA.Domain.Service
{
    public class SocialService : Abstract.BaseService<SocialService>
    {
        // if this is updated here, SocialManagementService must be updated as well
        private const int CacheInHours = 4;

        private readonly IGraCache _cache;
        private readonly LanguageService _languageService;
        private readonly ISocialHeaderRepository _socialHeaderRepository;
        private readonly ISocialRepository _socialRepository;

        public SocialService(ILogger<SocialService> logger,
            IDateTimeProvider dateTimeProvider,
            IGraCache cache,
            ISocialRepository socialRepository,
            ISocialHeaderRepository socialHeaderRepository,
            LanguageService languageService) : base(logger, dateTimeProvider)
        {
            _languageService = languageService
                ?? throw new ArgumentNullException(nameof(languageService));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _socialRepository = socialRepository
                ?? throw new ArgumentNullException(nameof(socialRepository));
            _socialHeaderRepository = socialHeaderRepository
                ?? throw new ArgumentNullException(nameof(socialHeaderRepository));
        }

        public async Task<Social> GetAsync(string culture)
        {
            var expiration = DateTime.Now.AddHours(CacheInHours).Ticks;

            int? headerId = await _cache.GetIntFromCacheAsync(CacheKey.SocialHeader);

            if (!headerId.HasValue)
            {
                var headerIdRecord = await _socialHeaderRepository
                    .GetByDateAsync(_dateTimeProvider.Now);

                if (headerIdRecord == null)
                {
                    return null;
                }

                if (headerIdRecord.NextStartDate.HasValue
                    && headerIdRecord.NextStartDate.Value != default)
                {
                    expiration = Math.Min(expiration,
                        headerIdRecord.NextStartDate.Value.Ticks);
                }

                await _cache.SaveToCacheAsync(CacheKey.SocialHeader,
                    headerIdRecord.Id,
                    TimeSpan.FromTicks(expiration));

                headerId = headerIdRecord.Id;
            }

            if (!headerId.HasValue)
            {
                return null;
            }

            var languageId = await _languageService.GetLanguageIdAsync(culture);

            Social social = null;

            var cacheKey = GetCacheKey(CacheKey.Social, headerId.Value, languageId);

            var socialCache = await _cache.GetStringFromCache(cacheKey);

            if (!string.IsNullOrEmpty(socialCache))
            {
                try
                {
                    social = JsonSerializer.Deserialize<Social>(socialCache);
                }
                catch (JsonException jex)
                {
                    _logger.LogError("Unable to deserialize social object: {ErrorMessage}",
                        jex.Message);
                }
            }

            if (social == null)
            {
                social = await _socialRepository.GetByHeaderLanguageAsync(headerId.Value, languageId);

                await _cache.SaveToCacheAsync(cacheKey,
                    JsonSerializer.Serialize(social),
                    TimeSpan.FromTicks(expiration));
            }

            return social;
        }
    }
}
