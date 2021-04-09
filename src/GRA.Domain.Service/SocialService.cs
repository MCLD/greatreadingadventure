using System;
using System.Text.Json;
using System.Threading.Tasks;
using GRA.Abstract;
using GRA.Domain.Model;
using GRA.Domain.Repository;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace GRA.Domain.Service
{
    public class SocialService : Abstract.BaseService<SocialService>
    {
        // if this is updated here, SocialManagementService must be updated as well
        private const int CacheInHours = 4;

        private readonly IDistributedCache _cache;
        private readonly LanguageService _languageService;
        private readonly ISocialHeaderRepository _socialHeaderRepository;
        private readonly ISocialRepository _socialRepository;
        public SocialService(ILogger<SocialService> logger,
            IDateTimeProvider dateTimeProvider,
            IDistributedCache cache,
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
            var socialHeadingCache = await _cache.GetAsync(CacheKey.SocialHeader);
            var expiration = DateTime.Now.AddHours(CacheInHours).Ticks;

            int? headerId;

            if (socialHeadingCache != null)
            {
                headerId = BitConverter.ToInt32(socialHeadingCache);
            }
            else
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

                await _cache.SetAsync(CacheKey.SocialHeader,
                    BitConverter.GetBytes(headerIdRecord.Id),
                    new DistributedCacheEntryOptions
                    {
                        AbsoluteExpiration = new DateTime(expiration)
                    });

                headerId = headerIdRecord.Id;
            }

            if (!headerId.HasValue)
            {
                return null;
            }

            var languageId = await _languageService.GetLanguageIdAsync(culture);

            Social social = null;

            var cacheKey = GetCacheKey(CacheKey.Social, headerId.Value, languageId);

            var socialCache = await _cache.GetStringAsync(cacheKey);

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

                await _cache.SetStringAsync(cacheKey,
                    JsonSerializer.Serialize(social),
                    new DistributedCacheEntryOptions
                    {
                        AbsoluteExpiration = new DateTime(expiration)
                    });
            }

            return social;
        }
    }
}