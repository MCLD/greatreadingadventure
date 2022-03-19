using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GRA.Abstract;
using GRA.Domain.Model;
using GRA.Domain.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace GRA.Domain.Service
{
    public class LanguageService : Abstract.BaseService<LanguageService>
    {
        private const int DefaultCacheHours = 4;

        private readonly IGraCache _cache;
        private readonly IOptions<RequestLocalizationOptions> _l10nOptions;
        private readonly ILanguageRepository _languageRepository;
        private readonly SiteLookupService _siteLookupService;

        public LanguageService(ILogger<LanguageService> logger,
            IDateTimeProvider dateTimeProvider,
            IGraCache cache,
            IOptions<RequestLocalizationOptions> l10nOptions,
            ILanguageRepository languageRepository,
            SiteLookupService siteLookupService) : base(logger, dateTimeProvider)
        {
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _l10nOptions = l10nOptions ?? throw new ArgumentNullException(nameof(l10nOptions));
            _languageRepository = languageRepository
                ?? throw new ArgumentNullException(nameof(languageRepository));
            _siteLookupService = siteLookupService
                ?? throw new ArgumentNullException(nameof(siteLookupService));
        }

        public async Task<ICollection<Language>> GetActiveAsync()
        {
            var languages = await _cache
                .GetObjectFromCacheAsync<ICollection<Language>>(CacheKey.ActiveLanguages);

            if (languages == null || languages.Count == 0)
            {
                languages = await _languageRepository.GetActiveAsync();
                await _cache.SaveToCacheAsync(CacheKey.ActiveLanguages,
                    languages,
                    DefaultCacheHours);
            }

            return languages;
        }

        public async Task<Language> GetActiveByIdAsync(int id)
        {
            var activeLanguages = await GetActiveAsync();
            return activeLanguages.SingleOrDefault(_ => _.Id == id);
        }

        public async Task<int> GetDefaultLanguageIdAsync()
        {
            var defaultLanguageId = await _cache.GetIntFromCacheAsync(CacheKey.DefaultLanguageId);
            if (!defaultLanguageId.HasValue)
            {
                defaultLanguageId = await _languageRepository.GetDefaultLanguageId();
                await _cache.SaveToCacheAsync(CacheKey.DefaultLanguageId,
                    defaultLanguageId.Value,
                    DefaultCacheHours);
            }
            return defaultLanguageId.Value;
        }

        public async Task<IDictionary<int, string>> GetIdDescriptionDictionaryAsync()
        {
            var languages = await GetActiveAsync();
            return languages.ToDictionary(k => k.Id, v => v.Description);
        }

        public async Task<int> GetLanguageIdAsync(string culture)
        {
            if (string.IsNullOrEmpty(culture))
            {
                return await GetDefaultLanguageIdAsync();
            }

            var cacheKey = GetCacheKey(CacheKey.LanguageId, culture);
            var languageId = await _cache.GetIntFromCacheAsync(cacheKey);
            if (!languageId.HasValue)
            {
                languageId = await _languageRepository.GetLanguageId(culture);
                await _cache.SaveToCacheAsync(cacheKey, languageId.Value, DefaultCacheHours);
            }
            return languageId.Value;
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
                    if (dbCulture.IsDefault
                        && dbCulture.Description != Culture.DefaultCulture.DisplayName)
                    {
                        dbCulture.Description = Culture.DefaultCulture.DisplayName;
                        doSave = true;
                    }
                    if (doSave)
                    {
                        await _languageRepository.UpdateSaveNoAuditAsync(dbCulture);
                    }
                }

                await _cache.RemoveAsync(GetCacheKey(CacheKey.LanguageId, dbCulture.Name));
            }

            var namesMissingFromDb = siteCultures
                .Select(_ => _.Name)
                .Except(databaseCultures.Select(_ => _.Name));

            int? systemUser = null;

            foreach (var missingCultureName in namesMissingFromDb)
            {
                if (systemUser == null)
                {
                    systemUser = await _siteLookupService.GetSystemUserId();
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
                await _cache.RemoveAsync(GetCacheKey(CacheKey.LanguageId, missingCultureName));
            }

            await _cache.RemoveAsync(CacheKey.DefaultLanguageId);
            await _cache.RemoveAsync(CacheKey.ActiveLanguages);
        }
    }
}
