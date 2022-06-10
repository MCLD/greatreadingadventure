using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using GRA.Abstract;
using GRA.Domain.Model;
using GRA.Domain.Model.Utility;
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
                            .DeserializeAsync<ItemImport<EmailBase>>(emailBaseFileStream);
                        if (insertEmailBase.Data != null)
                        {
                            insertEmailBase.Data.IsDefault = true;
                            defaultBase = await _emailBaseRepository
                                .AddSaveAsync(systemUserId, insertEmailBase.Data);
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
                            try
                            {
                                string fullPath = Path.Combine(emailBaseTextPath, filePath);
                                insertedItems += await ImportBaseTextAsync(fullPath,
                                        cultureLookup,
                                        systemUserId,
                                        defaultBaseId.Value);
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
                    var fullPath = Path.Combine(emailBasePath, filePath);
                    try
                    {
                        insertedItems += await ImportDirectEmailTemplateAsync(fullPath,
                            systemUserId,
                            defaultBaseId.Value);
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
                    var fullPath = Path.Combine(emailBasePath, filePath);
                    try
                    {
                        insertedItems += await ImportDirectEmailTextAsync(fullPath,
                            cultureLookup,
                            systemUserId);
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

        private async Task<int> ImportBaseTextAsync(string path,
            IDictionary<string, int> cultureLookup,
            int systemUserId,
            int defaultBaseId)
        {
            using var emailBaseTextFileStream = File.OpenRead(path);
            var insertEmailBaseText = await JsonSerializer
                .DeserializeAsync<ItemImport<EmailBaseText>>(emailBaseTextFileStream);

            if (insertEmailBaseText != null)
            {
                if (string.IsNullOrEmpty(insertEmailBaseText.Data.ImportCulture)
                    || !cultureLookup.ContainsKey(insertEmailBaseText.Data.ImportCulture))
                {
                    _logger.LogError("Unable to import {Path}, invalid or missing ImportCulture: {ImportCulture}",
                        path,
                        insertEmailBaseText.Data.ImportCulture);
                }
                else
                {
                    insertEmailBaseText.Data.EmailBaseId = defaultBaseId;
                    insertEmailBaseText.Data.LanguageId
                        = cultureLookup[insertEmailBaseText.Data.ImportCulture];
                    await _emailBaseRepository
                        .ImportSaveTextAsync(systemUserId, insertEmailBaseText.Data);
                    return 1;
                }
            }

            return 0;
        }

        private async Task<int> ImportDirectEmailTemplateAsync(string path,
                    int systemUserId,
            int defaultBaseId)
        {
            using var fileStream = File.OpenRead(path);

            var insertDirectEmailTemplate = await JsonSerializer
                .DeserializeAsync<ItemImport<DirectEmailTemplate>>(fileStream);

            if (insertDirectEmailTemplate != null)
            {
                if (!string.IsNullOrEmpty(insertDirectEmailTemplate.Data.SystemEmailId))
                {
                    var exists = await _directEmailTemplateRepository
                        .SystemEmailIdExistsAsync(insertDirectEmailTemplate
                            .Data.SystemEmailId);

                    if (!exists)
                    {
                        insertDirectEmailTemplate.Data.EmailBaseId = defaultBaseId;
                        await _directEmailTemplateRepository
                            .AddSaveAsync(systemUserId, insertDirectEmailTemplate.Data);
                        return 1;
                    }
                }
                else
                {
                    _logger.LogError("Unable to import from file {Path}: no SystemEmailId specified",
                        path);
                }
            }

            return 0;
        }

        private async Task<int> ImportDirectEmailTextAsync(string path,
            IDictionary<string, int> cultureLookup,
            int systemUserId)
        {
            using var fileStream = File.OpenRead(path);
            var insertText = await JsonSerializer
                .DeserializeAsync<ItemImport<DirectEmailTemplateText>>(fileStream);

            if (insertText != null)
            {
                if (string.IsNullOrEmpty(insertText.Data.ImportCulture)
                    || string.IsNullOrEmpty(insertText.Data.ImportSystemEmailId))
                {
                    _logger.LogError("Unable to import {Path} - missing ImportCulture or SystemEmailId.",
                        path);
                }
                else
                {
                    try
                    {
                        var (templateId, definedLanguages) = await _directEmailTemplateRepository
                            .GetIdAndLanguagesBySystemIdAsync(insertText.Data.ImportSystemEmailId);

                        if (!cultureLookup.ContainsKey(insertText.Data.ImportCulture))
                        {
                            _logger.LogError("Unable to find culture specified in {Path}: {Culture}",
                                path,
                                insertText.Data.ImportCulture);
                        }
                        else
                        {
                            var languageId = cultureLookup[insertText.Data.ImportCulture];
                            if (!definedLanguages.Contains(languageId))
                            {
                                insertText.Data.LanguageId = languageId;
                                insertText.Data.DirectEmailTemplateId = templateId;
                                await _directEmailTemplateRepository
                                    .ImportSaveTextAsync(systemUserId, insertText.Data);
                                return 1;
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

            return 0;
        }
    }
}
