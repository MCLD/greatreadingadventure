using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GRA.Abstract;
using GRA.Domain.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace GRA.Domain.Service
{
    public class LanguageService : Abstract.BaseService<LanguageService>
    {
        private readonly IOptions<RequestLocalizationOptions> _l10nOptions;
        private readonly ILanguageRepository _languageRepository;

        public LanguageService(ILogger<LanguageService> logger,
            IDateTimeProvider dateTimeProvider,
            IOptions<RequestLocalizationOptions> l10nOptions,
            ILanguageRepository languageRepository) : base(logger, dateTimeProvider)
        {
            _l10nOptions = l10nOptions ?? throw new ArgumentNullException(nameof(l10nOptions));
            _languageRepository = languageRepository
                ?? throw new ArgumentNullException(nameof(languageRepository));
        }

        public async Task SyncLanguagesAsync(int currentUser)
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
                    // ensure default is set properly
                    if ((dbCulture.IsDefault && dbCulture.Name != Culture.DefaultName)
                        || (!dbCulture.IsDefault && dbCulture.Name == Culture.DefaultName))
                    {
                        dbCulture.IsDefault = dbCulture.Name == Culture.DefaultName;
                        await _languageRepository.UpdateSaveNoAuditAsync(dbCulture);
                    }
                }
            }

            var namesMissingFromDb = siteCultures
                .Select(_ => _.Name)
                .Except(databaseCultures.Select(_ => _.Name));

            foreach (var missingCultureName in namesMissingFromDb)
            {
                var culture = siteCultures.Single(_ => _.Name == missingCultureName);
                await _languageRepository.AddSaveNoAuditAsync(new Model.Language
                {
                    CreatedAt = _dateTimeProvider.Now,
                    CreatedBy = currentUser,
                    Description = culture.DisplayName,
                    IsActive = true,
                    IsDefault = culture.Name == Culture.DefaultName,
                    Name = culture.Name
                });
            }
        }

        public async Task<ICollection<Model.Language>> GetActiveAsync()
        {
            return await _languageRepository.GetActiveAsync();
        }
    }
}
