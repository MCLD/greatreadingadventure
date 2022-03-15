using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using GRA.Abstract;
using GRA.Domain.Model;
using GRA.Domain.Repository;
using GRA.Domain.Service.Abstract;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace GRA.Domain.Service
{
    public class DefaultItemsService : BaseService<DefaultItemsService>
    {
        private readonly IConfiguration _config;
        private readonly IDirectEmailTemplateRepository _directEmailTemplateRepository;
        private readonly IEmailBaseRepository _emailBaseRepository;
        private readonly LanguageService _languageService;
        private readonly IUserRepository _userRepository;

        public DefaultItemsService(ILogger<DefaultItemsService> logger,
            IDateTimeProvider dateTimeProvider,
            IConfiguration config,
            IDirectEmailTemplateRepository directEmailTemplateRepository,
            IEmailBaseRepository emailBaseRepository,
            IUserRepository userRepository,
            LanguageService languageService) : base(logger, dateTimeProvider)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _directEmailTemplateRepository = directEmailTemplateRepository
                ?? throw new ArgumentNullException(nameof(directEmailTemplateRepository));
            _emailBaseRepository = emailBaseRepository
                ?? throw new ArgumentNullException(nameof(emailBaseRepository));
            _userRepository = userRepository
                ?? throw new ArgumentNullException(nameof(userRepository));
            _languageService = languageService
                ?? throw new ArgumentNullException(nameof(languageService));
        }

        public async Task EnsureDefaultItemsAsync()
        {
            var sw = Stopwatch.StartNew();
            int insertedItems = 0;

            string contentRoot = _config[ConfigurationKey.InternalContentPath];

            if (!Directory.Exists(contentRoot))
            {
                _logger.LogError("Missing internal content path configuration, please set {ConfigurationKey}",
                    ConfigurationKey.InternalContentPath);
                return;
            }

            var defaultItemsPath = Path.Combine(contentRoot, "assets", "defaultitems");

            if (!Directory.Exists(defaultItemsPath))
            {
                _logger.LogError("Could not insert default items: {SourcePath} does not exist",
                    defaultItemsPath);
                return;
            }

            var systemUserId = await _userRepository.GetSystemUserId();

            var languages = await _languageService.GetActiveAsync();
            var cultureLookup = languages.ToDictionary(k => k.Name, v => v.Id);

            var emailBasePath = Path.Combine(defaultItemsPath, "EmailBase");
            var emailBaseTextPath = Path.Combine(defaultItemsPath, "EmailBaseText");

            int? defaultBaseId = null;
            var defaultBase = await _emailBaseRepository.GetDefaultAsync();

            if (defaultBase != null)
            {
                defaultBaseId = defaultBase.Id;
            }

            if (Directory.Exists(emailBasePath) && Directory.Exists(emailBaseTextPath))
            {
                if (defaultBase == null)
                {
                    var defaultBasePath = Path.Combine(emailBasePath, "default.json");
                    using var emailBaseFileStream = File.OpenRead(defaultBasePath);
                    try
                    {
                        var insertEmailBase = await JsonSerializer
                            .DeserializeAsync<EmailBase>(emailBaseFileStream);
                        if (insertEmailBase != null)
                        {
                            insertEmailBase.IsDefault = true;
                            defaultBase = await _emailBaseRepository
                                .AddSaveAsync(systemUserId, insertEmailBase);
                            defaultBaseId = defaultBase.Id;
                            insertedItems++;
                        }
                    }
                    catch (JsonException jex)
                    {
                        _logger.LogError(jex,
                            "Unable to read JSON default EmailBase: {ErrorMessage}",
                            jex.Message);
                    }

                    if (!defaultBaseId.HasValue)
                    {
                        _logger.LogError("No default base present or inserted, unable to insert default base texts.");
                    }
                    else
                    {
                        foreach (var filePath in Directory
                            .EnumerateFiles(emailBaseTextPath, "*.json"))
                        {
                            using var emailBaseTextFileStream = File
                                .OpenRead(Path.Combine(emailBaseTextPath, filePath));
                            try
                            {
                                var insertEmailBaseText = await JsonSerializer
                                    .DeserializeAsync<EmailBaseText>(emailBaseTextFileStream);
                                if (insertEmailBaseText != null)
                                {
                                    if (string.IsNullOrEmpty(insertEmailBaseText.ImportCulture)
                                        || !cultureLookup
                                            .ContainsKey(insertEmailBaseText.ImportCulture))
                                    {
                                        _logger.LogError("Unable to import {filePath}, invalid or missing ImportCulture: {ImportCulture}",
                                            filePath,
                                            insertEmailBaseText.ImportCulture);
                                    }
                                    else
                                    {
                                        insertEmailBaseText.EmailBaseId = defaultBaseId.Value;
                                        insertEmailBaseText.LanguageId
                                            = cultureLookup[insertEmailBaseText.ImportCulture];
                                        await _emailBaseRepository
                                            .ImportSaveTextAsync(systemUserId, insertEmailBaseText);
                                        insertedItems++;
                                    }
                                }
                            }
                            catch (JsonException jex)
                            {
                                _logger.LogError(jex,
                                    "Unable to read JSON default EmailBaseText {Filename}: {ErrorMessage}",
                                    filePath,
                                    jex.Message);
                            }
                        }
                    }
                }
            }
            else
            {
                _logger.LogWarning("Unable to find default items in {EmailBasePath} or {EmailBaseTextPath}",
                    emailBasePath,
                    emailBaseTextPath);
            }

            if (!defaultBaseId.HasValue)
            {
                _logger.LogError("Unable to try to import direct email templates, no default email base.");
                return;
            }

            var directEmailTemplatePath = Path.Combine(defaultItemsPath, "DirectEmailTemplate");
            var directEmailTemplateTextPath = Path
                .Combine(defaultItemsPath, "DirectEmailTemplateText");

            if (Directory.Exists(directEmailTemplatePath)
                && Directory.Exists(directEmailTemplateTextPath))
            {
                // directemailtemplate items to potentially import
                foreach (var filePath in Directory
                    .EnumerateFiles(directEmailTemplatePath, "*.json"))
                {
                    using var fileStream = File.OpenRead(Path.Combine(emailBasePath, filePath));
                    try
                    {
                        var insertDirectEmailTemplate = await JsonSerializer
                            .DeserializeAsync<DirectEmailTemplate>(fileStream);

                        if (insertDirectEmailTemplate != null)
                        {
                            if (!string.IsNullOrEmpty(insertDirectEmailTemplate.SystemEmailId))
                            {
                                var exists = await _directEmailTemplateRepository
                                    .SystemEmailIdExistsAsync(insertDirectEmailTemplate
                                        .SystemEmailId);

                                if (!exists)
                                {
                                    insertDirectEmailTemplate.EmailBaseId = defaultBaseId.Value;
                                    await _directEmailTemplateRepository
                                        .AddSaveAsync(systemUserId, insertDirectEmailTemplate);
                                    insertedItems++;
                                }
                            }
                            else
                            {
                                _logger.LogError("Unable to import from file {Filename}: no SystemEmailId specified",
                                    filePath);
                            }
                        }
                    }
                    catch (JsonException jex)
                    {
                        _logger.LogError(jex,
                            "Unable to read JSON email template file {Filename}: {ErrorMessage}",
                            filePath,
                            jex.Message);
                    }
                }

                foreach (var filePath in Directory
                    .EnumerateFiles(directEmailTemplateTextPath, "*.json"))
                {
                    using var fileStream = File.OpenRead(Path.Combine(emailBasePath, filePath));
                    try
                    {
                        var insertText = await JsonSerializer
                            .DeserializeAsync<DirectEmailTemplateText>(fileStream);

                        if (insertText != null)
                        {
                            if (string.IsNullOrEmpty(insertText.ImportCulture)
                                || string.IsNullOrEmpty(insertText.ImportSystemEmailId))
                            {
                                _logger.LogError("Unable to import {Filename} - missing ImportCulture or SystemEmailId.");
                            }
                            else
                            {
                                try
                                {
                                    var (templateId, definedLanguages) = await _directEmailTemplateRepository
                                        .GetIdAndLanguagesBySystemIdAsync(insertText.ImportSystemEmailId);

                                    if (!cultureLookup.ContainsKey(insertText.ImportCulture))
                                    {
                                        _logger.LogError("Unable to find culture specified in {Filename}: {Culture}",
                                            filePath,
                                            insertText.ImportCulture);
                                    }
                                    else
                                    {
                                        var languageId = cultureLookup[insertText.ImportCulture];
                                        if (!definedLanguages.Contains(languageId))
                                        {
                                            insertText.LanguageId = languageId;
                                            insertText.DirectEmailTemplateId = templateId;
                                            await _directEmailTemplateRepository
                                                .ImportSaveTextAsync(systemUserId, insertText);
                                            insertedItems++;
                                        }
                                    }
                                }
                                catch (GraException gex)
                                {
                                    _logger.LogError(gex,
                                        "Could not map direct email template text to template: {ErrorMessage}",
                                        gex.Message);
                                }
                            }
                        }
                    }
                    catch (JsonException jex)
                    {
                        _logger.LogError(jex,
                            "Unable to read JSON email template file {Filename}: {ErrorMessage}",
                            filePath,
                            jex.Message);
                    }
                }
            }

            if (insertedItems > 0)
            {
                _logger.LogInformation("Inserted {InsertedItems} missing email templates in {Elapsed} ms",
                    insertedItems,
                    sw.ElapsedMilliseconds);
            }
            else
            {
                _logger.LogTrace("Email templates already present, checked in {Elapsed} ms",
                    sw.ElapsedMilliseconds);
            }
        }
    }
}
