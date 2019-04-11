using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GRA.Abstract;
using GRA.Domain.Model;
using GRA.Domain.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace GRA.Domain.Service
{
    public class LanguageService : Abstract.BaseService<LanguageService>
    {
        private readonly IDistributedCache _cache;
        private readonly IOptions<RequestLocalizationOptions> _l10nOptions;
        private readonly ILanguageRepository _languageRepository;
        private readonly UserService _userService;

        public LanguageService(ILogger<LanguageService> logger,
            IDateTimeProvider dateTimeProvider,
            IDistributedCache cache,
            IOptions<RequestLocalizationOptions> l10nOptions,
            ILanguageRepository languageRepository,
            UserService userService) : base(logger, dateTimeProvider)
        {
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _l10nOptions = l10nOptions ?? throw new ArgumentNullException(nameof(l10nOptions));
            _languageRepository = languageRepository
                ?? throw new ArgumentNullException(nameof(languageRepository));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        public async Task SyncLanguagesAsync()
        {
            var siteCultures = _l10nOptions
                .Value
                .SupportedCultures;

            var databaseCultures = await _languageRepository.GetAllAsync();

            foreach (var dbCulture in databaseCultures)
            {
                var siteCulture = siteCultures.SingleOrDefault(_ => _.Name == dbCulture.Name);
                if (siteCulture == null && dbCulture.IsActive)
                {
                    // no longer active
                    _logger.LogInformation("Marking language {Name} inactive in the database.",
                        dbCulture.Name);
                    dbCulture.IsActive = false;
                    dbCulture.IsDefault = dbCulture.Name == Culture.DefaultName;
                    await _languageRepository.UpdateSaveNoAuditAsync(dbCulture);
                }
                else if (siteCulture != null && !dbCulture.IsActive)
                {
                    // valid but marked invalid in the database
                    _logger.LogInformation("Marking language {Name} as active in the database.",
                        dbCulture.Name);
                    dbCulture.IsActive = true;
                    dbCulture.IsDefault = dbCulture.Name == Culture.DefaultName;
                    await _languageRepository.UpdateSaveNoAuditAsync(dbCulture);
                }
                else
                {
                    bool doSave = false;
                    // ensure default is set properly
                    if ((dbCulture.IsDefault && dbCulture.Name != Culture.DefaultName)
                        || (!dbCulture.IsDefault && dbCulture.Name == Culture.DefaultName))
                    {
                        dbCulture.IsDefault = dbCulture.Name == Culture.DefaultName;
                        doSave = true;
                    }
                    if(dbCulture.IsDefault 
                        && dbCulture.Description != Culture.DefaultCulture.DisplayName)
                    {
                        dbCulture.Description = Culture.DefaultCulture.DisplayName;
                        doSave = true;
                    }
                    if(doSave)
                    {
                        await _languageRepository.UpdateSaveNoAuditAsync(dbCulture);
                    }
                }
                if (_cache.Get(string.Format(CacheKey.LanguageId, dbCulture.Name)) != null)
                {
                    _cache.Remove(string.Format(CacheKey.LanguageId, dbCulture.Name));
                }
            }

            var namesMissingFromDb = siteCultures
                .Select(_ => _.Name)
                .Except(databaseCultures.Select(_ => _.Name));

            int? systemUser = null;

            foreach (var missingCultureName in namesMissingFromDb)
            {
                if (systemUser == null)
                {
                    systemUser = await _userService.GetSystemUserId();
                }
                var culture = siteCultures.Single(_ => _.Name == missingCultureName);
                await _languageRepository.AddSaveNoAuditAsync(new Language
                {
                    CreatedAt = _dateTimeProvider.Now,
                    CreatedBy = (int)systemUser,
                    Description = culture.DisplayName,
                    IsActive = true,
                    IsDefault = culture.Name == Culture.DefaultName,
                    Name = culture.Name
                });
                if (_cache.Get(string.Format(CacheKey.LanguageId, missingCultureName)) != null)
                {
                    _cache.Remove(string.Format(CacheKey.LanguageId, missingCultureName));
                }
            }

            _cache.Remove(CacheKey.DefaultLanguageId);
        }

        public async Task<ICollection<Language>> GetActiveAsync()
        {
            return await _languageRepository.GetActiveAsync();
        }

        public async Task<Language> GetActiveByIdAsync(int id)
        {
            return await _languageRepository.GetActiveByIdAsync(id);
        }

        public async Task<int> GetDefaultLanguageIdAsync()
        {
            var cachedDefaultLanguageId = _cache.Get(CacheKey.DefaultLanguageId);
            if (cachedDefaultLanguageId == null)
            {
                var defaultLanguageId = await _languageRepository.GetDefaultLanguageId();
                cachedDefaultLanguageId = BitConverter.GetBytes(defaultLanguageId);
                _cache.Set(CacheKey.DefaultLanguageId, cachedDefaultLanguageId);
            }
            return BitConverter.ToInt32(cachedDefaultLanguageId, 0);
        }

        public async Task<int> GetLanguageIdAsync(string culture)
        {
            var key = string.Format(CacheKey.LanguageId, culture);
            var cachedLanguageId = _cache.Get(key);
            if (cachedLanguageId == null)
            {
                var languageId = await _languageRepository.GetLanguageId(culture);
                cachedLanguageId = BitConverter.GetBytes(languageId);
                _cache.Set(key, cachedLanguageId, new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = new TimeSpan(1, 0, 0)
                });
            }
            return BitConverter.ToInt32(cachedLanguageId, 0);
        }
    }
}
