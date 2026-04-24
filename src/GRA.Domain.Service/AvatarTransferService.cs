using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using GRA.Abstract;
using GRA.Domain.Model;
using GRA.Domain.Model.Utility;
using GRA.Domain.Repository;
using GRA.Domain.Service.Abstract;
using Microsoft.Extensions.Logging;

namespace GRA.Domain.Service
{
    public class AvatarTransferService : BaseUserService<AvatarTransferService>
    {
        private const string IconPng = "icon.png";
        private const string ItemPng = "item.png";
        private const string ThumbJpg = "thumbnail.jpg";

        private readonly IAvatarBundleRepository _avatarBundleRepository;
        private readonly IAvatarColorRepository _avatarColorRepository;
        private readonly IAvatarElementRepository _avatarElementRepository;
        private readonly IAvatarItemRepository _avatarItemRepository;
        private readonly IAvatarLayerRepository _avatarLayerRepository;
        private readonly AvatarService _avatarService;
        private readonly IAvatarTransferRepository _avatarTransferRepository;
        private readonly IJobRepository _jobRepository;
        private readonly LanguageService _languageService;
        private readonly IPathResolver _pathResolver;
        private readonly SiteLookupService _siteLookupService;

        public AvatarTransferService(ILogger<AvatarTransferService> logger,
            IDateTimeProvider dateTimeProvider,
            IUserContextProvider userContextProvider,
            AvatarService avatarService,
            IAvatarBundleRepository avatarBundleRepository,
            IAvatarColorRepository avatarColorRepository,
            IAvatarElementRepository avatarElementRepository,
            IAvatarItemRepository avatarItemRepository,
            IAvatarLayerRepository avatarLayerRepository,
            IAvatarTransferRepository avatarTransferRepository,
            IJobRepository jobRepository,
            IPathResolver pathResolver,
            LanguageService languageService,
            SiteLookupService siteLookupService)
            : base(logger, dateTimeProvider, userContextProvider)
        {
            ArgumentNullException.ThrowIfNull(avatarBundleRepository);
            ArgumentNullException.ThrowIfNull(avatarColorRepository);
            ArgumentNullException.ThrowIfNull(avatarElementRepository);
            ArgumentNullException.ThrowIfNull(avatarItemRepository);
            ArgumentNullException.ThrowIfNull(avatarLayerRepository);
            ArgumentNullException.ThrowIfNull(avatarService);
            ArgumentNullException.ThrowIfNull(avatarTransferRepository);
            ArgumentNullException.ThrowIfNull(jobRepository);
            ArgumentNullException.ThrowIfNull(languageService);
            ArgumentNullException.ThrowIfNull(pathResolver);
            ArgumentNullException.ThrowIfNull(siteLookupService);

            _avatarBundleRepository = avatarBundleRepository;
            _avatarColorRepository = avatarColorRepository;
            _avatarElementRepository = avatarElementRepository;
            _avatarItemRepository = avatarItemRepository;
            _avatarLayerRepository = avatarLayerRepository;
            _avatarService = avatarService;
            _avatarTransferRepository = avatarTransferRepository;
            _jobRepository = jobRepository;
            _languageService = languageService;
            _pathResolver = pathResolver;
            _siteLookupService = siteLookupService;
        }

        public async Task<AvatarTransfer> GetExportAsync(int id)
        {
            var export = await _avatarTransferRepository.GetByIdAsync(id);
            return export.TransferType == DataTransferType.Export ? export : null;
        }

        public async Task<ICollection<AvatarTransfer>> GetTransfers()
        {
            return await _avatarTransferRepository.GetAllAsync();
        }

        public async Task<JobStatus> TransferAvatarsAsync(JobMetadata metadata)
        {
            ArgumentNullException.ThrowIfNull(metadata);

            if (!HasPermission(Permission.ManageAvatars))
            {
                _logger.LogInformation("Avatar Transfer failed for {User} (id {UserId}): no manage avatar permission",
                    metadata.UserContactDetails.Username,
                    metadata.UserContactDetails.Id);

                return new JobStatus
                {
                    PercentComplete = 0,
                    Status = "Permission denied.",
                    Error = true,
                    Complete = true
                };
            }

            var job = await _jobRepository.GetByIdAsync(metadata.JobId);
            var jobDetails = JsonSerializer
                .Deserialize<JobDetailsAvatarTransfer>(job.SerializedParameters);

            _logger.LogInformation("Avatar {TransferType} initiated for {User} (id {UserId})",
                jobDetails.TransferType,
                metadata.UserContactDetails.Username,
                metadata.UserContactDetails.Id);

            await ReportJobStatusAsync(_jobRepository, metadata, new JobStatus
            {
                Status = $"Collecting data for avatar {jobDetails.TransferType}",
                Title = $"Avatar {jobDetails.TransferType}"
            });

            var sw = Stopwatch.StartNew();

            metadata.CancellationToken.Register(() =>
            {
                _logger.LogWarning("Avatar {TransferType} {Disposition} for {User} (id {UserId}) after {Elapsed:N1}",
                    jobDetails.TransferType,
                    "cancelled",
                    metadata.UserContactDetails.Username,
                    metadata.UserContactDetails.Id,
                    sw.Elapsed.TotalSeconds);
            });

            string assetPath = jobDetails.AssetPath;

            var transfer = new AvatarTransfer
            {
                CreatedBy = GetClaimId(ClaimType.UserId),
                JobId = metadata.JobId,
                TransferType = jobDetails.TransferType
            };

            TransferResult result = null;
            try
            {
                if (jobDetails.TransferType == DataTransferType.Export)
                {
                    result = await GenerateAvatarFileAsync(metadata, assetPath);
                    transfer.Filename = Path.GetFileName(result.Path);
                }
                else
                {
                    transfer.Filename = jobDetails.Filename;
                    transfer.FileKBytes = jobDetails.Filesize;
                    result = await ImportAvatarsAsync(metadata, jobDetails);
                }
            }
            catch (GraException gex)
            {
                sw.Stop();

                _logger.LogWarning("Avatar {TransferType} cancelled for {User} (id {UserId}) after {Elapsed:N1}: {Message}",
                    jobDetails.TransferType,
                    metadata.UserContactDetails.Username,
                    metadata.UserContactDetails.Id,
                    sw.Elapsed.TotalSeconds,
                    gex.Message);

                await ReportJobStatusAsync(_jobRepository, metadata, new JobStatus
                {
                    Complete = true,
                    Error = true,
                    Status = $"Avatar {jobDetails.TransferType} failed: {gex.Message}",
                });
            }

            await ReportJobStatusAsync(_jobRepository, metadata, new JobStatus
            {
                PercentComplete = 100,
                Status = $"Finalizing {jobDetails.TransferType} of {result.ItemCount} avatar items"
            });

            sw.Stop();

            _logger.LogInformation("Avatar {TransferType} completed for {User} (id {UserId}): {Items} in {Elapsed:N1} seconds.",
                jobDetails.TransferType,
                metadata.UserContactDetails.Username,
                metadata.UserContactDetails.Id,
                result.ItemCount,
                sw.Elapsed.TotalSeconds);

            await _avatarTransferRepository.AddSaveAsync(transfer.CreatedBy, transfer);

            string finalStatusMessage = null;

            if (jobDetails.TransferType == DataTransferType.Export)
            {
                finalStatusMessage = $"Avatar {jobDetails.TransferType}: {result.ItemCount} items to {new FileInfo(result.Path).Length / 1024:N0}KB ZIP in {sw.Elapsed.TotalSeconds:N1} seconds";
            }
            else if (jobDetails.Filesize.HasValue)
            {
                finalStatusMessage = $"Avatar {jobDetails.TransferType}: {result.ItemCount} items from {jobDetails.Filename} ({jobDetails.Filesize:N0}KB) in {sw.Elapsed.TotalSeconds:N1} seconds";
            }
            else
            {
                finalStatusMessage = $"Avatar {jobDetails.TransferType}: {result.ItemCount} items from {jobDetails.Filename} in {sw.Elapsed.TotalSeconds:N1} seconds";
            }

            var finalStatus = new JobStatus
            {
                Complete = true,
                Error = false,
                Status = finalStatusMessage
            };

            await ReportJobStatusAsync(_jobRepository, metadata, finalStatus);
            return finalStatus;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization",
            "CA1308:Normalize strings to uppercase",
            Justification = "Normalize filenames to lowercase")]
        private async Task<TransferResult> GenerateAvatarFileAsync(JobMetadata metadata,
            string assetPath)
        {
            int existingItemCount = 0;
            int totalItemsProcessed = 0;
            var lastLogAt = DateTime.MinValue;
            var siteBasePath = $"site{metadata.Site.Id}";
            var exportStamp = _dateTimeProvider.Now;

            // ensure we have a directory to copy files for zipping
            var dateString = exportStamp.ToString("yyyyMMdd", CultureInfo.CurrentCulture);
            string fullPath = Path.Join(assetPath, $"{dateString}-avatars.zip");
            int counter = 0;
            while (File.Exists(fullPath))
            {
                counter++;
                fullPath = Path.Join(assetPath, $"{dateString}-avatars-{counter:D2}.zip");
                if (counter > 99)
                {
                    fullPath = Path.Join(assetPath, $"{dateString}-avatars-{Guid.NewGuid():N}.zip");
                }
            }

            var zipPath = Path.Join(Path.GetDirectoryName(fullPath),
                Path.GetFileNameWithoutExtension(fullPath));

            if (Directory.Exists(zipPath))
            {
                new DirectoryInfo(zipPath).Delete(true);
            }
            Directory.CreateDirectory(zipPath);

            // copy background image
            File.Copy(_pathResolver.ResolveContentFilePath(Path.Join(siteBasePath,
                    "avatarbackgrounds",
                    "background.png")),
                Path.Join(zipPath, "background.png"));

            // copy bundle icons
            File.Copy(_pathResolver.ResolveContentFilePath(Path.Join(siteBasePath,
                    "avatarbundles",
                    IconPng)),
                Path.Join(zipPath, "bundleicon.png"));
            File.Copy(_pathResolver.ResolveContentFilePath(Path.Join(siteBasePath,
                    "avatarbundles",
                    "notif.png")),
                Path.Join(zipPath, "bundlenotif.png"));

            // load language data
            var defaultLanguage = await _languageService.GetDefaultLanguageIdAsync();
            var languageDictionary = (await _languageService.GetActiveAsync())
                .ToDictionary(k => k.Id, v => v.Name);

            await ReportJobStatusAsync(_jobRepository, metadata, new JobStatus
            {
                Status = "Loading layers and items"
            });

            // pull all layers, items, colors as one big object tree
            var layers = await _avatarLayerRepository.GetForExportAsync(GetCurrentSiteId());

            foreach (var layer in layers)
            {
                existingItemCount += Math.Max(layer.AvatarColors?.Count ?? 0, 1)
                    * Math.Max(layer.AvatarItems?.Count ?? 0, 1);
            }

            int layerCount = 0;
            var avBasePath = Path.Join(siteBasePath, "avatars");
            foreach (var layer in layers)
            {
                layerCount++;
                if (string.IsNullOrEmpty(layer.Name) && layer.Texts?.Count > 0)
                {
                    layer.Name = layer.Texts
                        .OrderByDescending(_ => _.LanguageName == languageDictionary[defaultLanguage])
                        .ThenBy(_ => _.LanguageName)
                        .FirstOrDefault()?
                        .Name;
                }

                await ReportJobStatusAsync(_jobRepository, metadata, new JobStatus
                {
                    Status = $"Processing layer {layer.Name} ({layerCount} of {layers.Count})"
                });

                var avLayerPath = Path.Join(avBasePath, $"layer{layer.Id}");

                var layerTransferPath = Path.Join(zipPath, layer.Name);
                Directory.CreateDirectory(layerTransferPath);

                File.Copy(_pathResolver.ResolveContentFilePath(Path.Join(avLayerPath, IconPng)),
                    Path.Join(layerTransferPath, IconPng));

                int itemCount = 0;
                foreach (var item in layer.AvatarItems)
                {
                    var itemPath = Path.Join(avLayerPath, $"item{item.Id}");
                    var itemTransferPath = Path.Join(layerTransferPath, item.Name);
                    Directory.CreateDirectory(itemTransferPath);

                    var thumbPath = Path.Join(avLayerPath, $"item{item.Id}", ThumbJpg);
                    File.Copy(_pathResolver.ResolveContentFilePath(thumbPath),
                        Path.Join(itemTransferPath, ThumbJpg));

                    if (layer.AvatarColors?.Count > 0)
                    {
                        foreach (var color in layer.AvatarColors)
                        {
                            File.Copy(_pathResolver.ResolveContentFilePath(Path.Join(itemPath,
                                    $"item_{color.Id}.png")),
                                Path.Join(itemTransferPath,
                                    $"{color.Color.ToLowerInvariant()}.png"));
                            totalItemsProcessed++;
                        }
                    }
                    else
                    {
                        File.Copy(_pathResolver.ResolveContentFilePath(Path.Join(itemPath,
                                ItemPng)),
                            Path.Join(itemTransferPath, ItemPng));
                        totalItemsProcessed++;
                    }

                    await ReportJobStatusAsync(_jobRepository, metadata, new JobStatus
                    {
                        PercentComplete = totalItemsProcessed * 100 / existingItemCount,
                        Status = $"Processing layer {layer.Name} ({layerCount} of {layers.Count}): item {++itemCount} of {layer.AvatarItems.Count} (overall item {totalItemsProcessed} of {existingItemCount})"
                    });

                    if (_dateTimeProvider.Now > lastLogAt.AddSeconds(10)
                        || totalItemsProcessed == existingItemCount)
                    {
                        _logger.LogInformation("Avatar {TransferType} for {Username} (id {UserId}): {CurrentItem}/{ItemCount} avatar assets",
                            "Export",
                            metadata.UserContactDetails.Username,
                            metadata.UserContactDetails.Id,
                            totalItemsProcessed,
                            existingItemCount);
                        lastLogAt = _dateTimeProvider.Now;
                    }
                }
            }

            await ReportJobStatusAsync(_jobRepository, metadata, new JobStatus
            {
                Status = "Writing out avatars.json index file"
            });

            var link = await _siteLookupService.GetSiteLinkAsync(metadata.Site.Id);

            await File.WriteAllTextAsync(Path.Join(zipPath, "avatars.json"),
                System.Text.Json.JsonSerializer.Serialize(new ListExport<AvatarLayerTransfer>
                {
                    Data = layers,
                    ExportedAt = exportStamp,
                    ExportedBy = metadata.UserExportDetails,
                    Source = $"{metadata.Site.Name} ({link})",
                    Type = nameof(AvatarLayerTransfer),
                    Version = 2
                }),
                metadata.CancellationToken);

            // generate bundles.json
            var bundles = await _avatarBundleRepository.GetForExportAsync(metadata.Site.Id);

            await File.WriteAllTextAsync(Path.Join(zipPath, "bundles.json"),
                System.Text.Json.JsonSerializer.Serialize(new ListExport<AvatarBundleTransfer>
                {
                    Data = bundles,
                    ExportedAt = exportStamp,
                    ExportedBy = metadata.UserExportDetails,
                    Source = $"{metadata.Site.Name} ({link})",
                    Type = nameof(AvatarBundleTransfer),
                    Version = 2
                }),
                metadata.CancellationToken);

            await ReportJobStatusAsync(_jobRepository, metadata, new JobStatus
            {
                Status = "Compressing ZIP archive"
            });

            _logger.LogInformation("Avatar {TransferType} for {Username} (id {UserId}): creating ZIP file {Filename}.zip",
                "Export",
                metadata.UserContactDetails.Username,
                metadata.UserContactDetails.Id,
                Path.GetFileName(zipPath));

            try
            {
                ZipFile.CreateFromDirectory(zipPath, fullPath, CompressionLevel.Optimal, false);
            }
            catch (Exception ex)
            {
                throw new GraException($"Error generating ZIP file: {ex.Message}", ex);
            }
            finally
            {
                await ReportJobStatusAsync(_jobRepository, metadata, new JobStatus
                {
                    Status = "Removing temporary files"
                });
                _logger.LogInformation("Avatar {TransferType} for {Username} (id {UserId}): removing temporary files",
                    "Export",
                    metadata.UserContactDetails.Username,
                    metadata.UserContactDetails.Id);
                new DirectoryInfo(zipPath).Delete(true);
            }

            return new TransferResult
            {
                ItemCount = totalItemsProcessed,
                Path = fullPath
            };
        }

        private async Task<TransferResult> ImportAvatarsAsync(JobMetadata metadata,
            JobDetailsAvatarTransfer jobDetails)
        {
            var sw = Stopwatch.StartNew();
            var requestingUser = metadata.UserContactDetails.Id;
            var assetPath = jobDetails.AssetPath;

            var ImportAvatarJson = jobDetails.Version == 1
                ? "default avatars.json"
                : "avatars.json";

            var ImportBundleJson = jobDetails.Version == 1
                ? "default bundles.json"
                : "bundles.json";

            Dictionary<string, int> LanguageNameIdMap = null;

            if (jobDetails.Version > 1)
            {
                LanguageNameIdMap = (await _languageService.GetActiveAsync())
                    .ToDictionary(k => k.Name, v => v.Id);
            }

            metadata.CancellationToken.Register(() =>
            {
                _logger.LogWarning("Import avatars for user {User} was cancelled after {Elapsed} ms.",
                    requestingUser,
                    sw?.ElapsedMilliseconds);
            });

            var jsonPath = Path.Combine(assetPath, ImportAvatarJson);

            if (!File.Exists(jsonPath))
            {
                _logger.LogError("Unable to find file {DefaultAvatarsJson}", jsonPath);
                throw new GraException($"Unable to find the default avatars.json file in {assetPath}.");
            }

            IEnumerable<AvatarLayer> avatarList = null;

            using (StreamReader file = File.OpenText(jsonPath))
            {
                var jsonString = await file.ReadToEndAsync();

                if (jobDetails.Version == 1)
                {
                    avatarList = Newtonsoft.Json.JsonConvert
                        .DeserializeObject<IEnumerable<AvatarLayer>>(jsonString);
                }
                else
                {
                    var import = JsonSerializer
                        .Deserialize<ListExport<AvatarLayer>>(jsonString);

                    avatarList = import.Data;
                }
            }

            var layerCount = avatarList?.Count();
            _logger.LogInformation("Found {Count} AvatarLayer objects in avatar JSON file",
                layerCount);

            // Layers + background/bundles
            var processingCount = layerCount + 1;
            var processedCount = 0;

            var bundleJsonPath = Path.Combine(assetPath, ImportBundleJson);
            var bundleJsonExists = File.Exists(bundleJsonPath);
            if (bundleJsonExists)
            {
                processingCount++;
            }

            var time = _dateTimeProvider.Now;
            int totalFilesCopied = 0;
            var siteId = GetCurrentSiteId();

            var destinationBase = Path.Combine($"site{siteId}", "avatars");
            var destinationBasePath = _pathResolver.ResolveContentFilePath(destinationBase);
            if (Directory.Exists(destinationBasePath))
            {
                _logger.LogWarning("Destination directory {Path} already exists, attempting to remove...",
                    destinationBasePath);
                Directory.Delete(destinationBasePath, true);
            }

            foreach (var layer in avatarList)
            {
                metadata.Progress?.Report(new JobStatus
                {
                    PercentComplete = processedCount * 100 / processingCount,
                    Status = $"Processing layer {layer.Name}",
                    Error = false
                });

                var colors = layer.AvatarColors;
                var items = layer.AvatarItems;
                var texts = layer.Texts;

                layer.AvatarColors = null;
                layer.AvatarItems = null;
                layer.Texts = null;

                var addedLayer = await _avatarService.AddLayerAsync(layer);

                var layerAssetPath = Path.Combine(assetPath, layer.Name);
                var destinationRoot = Path.Combine(destinationBase, $"layer{addedLayer.Id}");
                var destinationPath = _pathResolver.ResolveContentFilePath(destinationRoot);
                if (!Directory.Exists(destinationPath))
                {
                    Directory.CreateDirectory(destinationPath);
                }

                File.Copy(Path.Combine(layerAssetPath, IconPng),
                    Path.Combine(destinationPath, IconPng));

                await _avatarService.UpdateLayerAsync(addedLayer);

                int lastUpdateSent;

                if (texts?.Count > 0)
                {
                    await _avatarService.AddLayerTexts(addedLayer.Id, texts, jobDetails.Version);
                }

                if (colors?.Count > 0)
                {
                    metadata.Progress?.Report(new JobStatus
                    {
                        PercentComplete = processedCount * 100 / processingCount,
                        Status = $"Processing layer {layer.Name}: Adding colors...",
                        Error = false
                    });
                    lastUpdateSent = (int)sw.Elapsed.TotalSeconds;

                    var colorCount = colors.Count;
                    var currentColor = 1;
                    foreach (var color in colors)
                    {
                        var secondsFromLastUpdate =
                            (int)sw.Elapsed.TotalSeconds - lastUpdateSent;
                        if (secondsFromLastUpdate >= 5)
                        {
                            metadata.Progress?.Report(new JobStatus
                            {
                                PercentComplete = processedCount * 100 / processingCount,
                                Status = $"Processing layer {layer.Name}: Adding colors ({currentColor}/{colorCount})...",
                                Error = false
                            });
                            lastUpdateSent = (int)sw.Elapsed.TotalSeconds;
                        }

                        color.AvatarLayerId = addedLayer.Id;
                        color.CreatedAt = time;
                        color.CreatedBy = requestingUser;

                        if (jobDetails.Version > 1 && color.Texts != null)
                        {
                            foreach (var text in color.Texts)
                            {
                                if (LanguageNameIdMap.TryGetValue(text.LanguageName,
                                        out int languageId))
                                {
                                    text.LanguageId = languageId;
                                }
                            }
                        }
                        await _avatarColorRepository.AddAsync(requestingUser, color);

                        currentColor++;
                    }

                    await _avatarColorRepository.SaveAsync();
                    colors = await _avatarService.GetColorsByLayerAsync(addedLayer.Id);
                }

                metadata.Progress?.Report(new JobStatus
                {
                    PercentComplete = processedCount * 100 / processingCount,
                    Status = $"Processing layer {layer.Name}: Adding items...",
                    Error = false
                });
                lastUpdateSent = (int)sw.Elapsed.TotalSeconds;

                var itemCount = items.Count;
                var currentItem = 1;
                foreach (var item in items.OrderBy(_ => _.SortOrder))
                {
                    var secondsFromLastUpdate = (int)sw.Elapsed.TotalSeconds - lastUpdateSent;
                    if (secondsFromLastUpdate >= 5)
                    {
                        metadata.Progress?.Report(new JobStatus
                        {
                            PercentComplete = processedCount * 100 / processingCount,
                            Status = $"Processing layer {layer.Name}: Adding items ({currentItem}/{itemCount})...",
                            Error = false
                        });
                        lastUpdateSent = (int)sw.Elapsed.TotalSeconds;
                    }

                    item.AvatarLayerId = addedLayer.Id;
                    item.CreatedAt = time;
                    item.CreatedBy = requestingUser;

                    if (jobDetails.Version > 1 && item.Texts != null)
                    {
                        foreach (var text in item.Texts)
                        {
                            if (LanguageNameIdMap.TryGetValue(text.LanguageName,
                                    out int languageId))
                            {
                                text.LanguageId = languageId;
                            }
                        }
                    }
                    await _avatarItemRepository.AddAsync(requestingUser, item);

                    currentItem++;
                }
                await _avatarItemRepository.SaveAsync();
                items = await _avatarService.GetItemsByLayerAsync(addedLayer.Id);

                _logger.LogInformation("Processing {Count} items in {LayerName}",
                    items.Count,
                    layer.Name);

                metadata.Progress?.Report(new JobStatus
                {
                    PercentComplete = processedCount * 100 / processingCount,
                    Status = $"Processing layer {layer.Name}: Copying files...",
                    Error = false
                });
                lastUpdateSent = (int)sw.Elapsed.TotalSeconds;

                var elementCount = items.Count;
                if (colors?.Count > 0)
                {
                    elementCount *= colors.Count;
                }
                var currentElement = 1;
                foreach (var item in items)
                {
                    var secondsFromLastUpdate = (int)sw.Elapsed.TotalSeconds - lastUpdateSent;
                    if (secondsFromLastUpdate >= 5)
                    {
                        metadata.Progress?.Report(new JobStatus
                        {
                            PercentComplete = processedCount * 100 / processingCount,
                            Status = $"Processing layer {layer.Name}: Copying files ({currentElement}/{elementCount})...",
                            Error = false
                        });
                        lastUpdateSent = (int)sw.Elapsed.TotalSeconds;
                    }

                    if (currentElement % 500 == 0)
                    {
                        await _avatarElementRepository.SaveAsync();
                    }

                    var itemAssetPath = Path.Combine(layerAssetPath, item.Name);
                    var itemRoot = Path.Combine(destinationRoot, $"item{item.Id}");
                    var itemPath = Path.Combine(destinationPath, $"item{item.Id}");
                    if (!Directory.Exists(itemPath))
                    {
                        Directory.CreateDirectory(itemPath);
                    }
                    File.Copy(Path.Combine(itemAssetPath, ThumbJpg),
                        Path.Combine(itemPath, ThumbJpg));

                    if (colors?.Count > 0)
                    {
                        foreach (var color in colors)
                        {
                            var element = new AvatarElement
                            {
                                AvatarColorId = color.Id,
                                AvatarItemId = item.Id
                            };
                            await _avatarElementRepository.AddAsync(requestingUser, element);
                            File.Copy(
                                Path.Combine(itemAssetPath, $"{color.Color}.png"),
                                Path.Combine(itemPath, $"item_{color.Id}.png"));
                            currentElement++;
                        }
                    }
                    else
                    {
                        var element = new AvatarElement
                        {
                            AvatarItemId = item.Id,
                        };
                        await _avatarElementRepository.AddAsync(requestingUser, element);
                        File.Copy(Path.Combine(itemAssetPath, ItemPng),
                            Path.Combine(itemPath, ItemPng));
                        currentElement++;
                    }
                }

                await _avatarElementRepository.SaveAsync();
                totalFilesCopied += elementCount;
                _logger.LogInformation("Copied {Count} items for {LayerName}",
                    elementCount,
                    layer.Name);

                processedCount++;
            }

            metadata.Progress?.Report(new JobStatus
            {
                PercentComplete = processedCount * 100 / processingCount,
                Status = "Finishing avatar import...",
                Error = false
            });

            var backgroundRoot = Path.Combine($"site{siteId}", "avatarbackgrounds");
            var backgroundPath = _pathResolver.ResolveContentFilePath(backgroundRoot);
            if (Directory.Exists(backgroundPath))
            {
                Directory.Delete(backgroundPath, true);
            }
            Directory.CreateDirectory(backgroundPath);
            File.Copy(Path.Combine(assetPath, "background.png"),
                Path.Combine(backgroundPath, "background.png"));
            totalFilesCopied++;

            var bundleRoot = Path.Combine($"site{siteId}", "avatarbundles");
            var bundlePath = _pathResolver.ResolveContentFilePath(bundleRoot);
            if (Directory.Exists(bundlePath))
            {
                Directory.Delete(bundlePath, true);
            }
            Directory.CreateDirectory(bundlePath);
            File.Copy(Path.Combine(assetPath, "bundleicon.png"),
                Path.Combine(bundlePath, IconPng));
            totalFilesCopied++;
            File.Copy(Path.Combine(assetPath, "bundlenotif.png"),
                Path.Combine(bundlePath, "notif.png"));
            totalFilesCopied++;

            _logger.LogInformation("Copied {TotalFilesCopied} items for all layers.",
                totalFilesCopied);

            if (bundleJsonExists)
            {
                IEnumerable<AvatarBundle> bundleList;
                using (StreamReader file = File.OpenText(bundleJsonPath))
                {
                    var jsonString = await file.ReadToEndAsync();

                    if (jobDetails.Version == 1)
                    {
                        bundleList = Newtonsoft.Json.JsonConvert
                            .DeserializeObject<IEnumerable<AvatarBundle>>(jsonString);
                    }
                    else
                    {
                        var import = JsonSerializer
                            .Deserialize<ListExport<AvatarBundle>>(jsonString);

                        bundleList = import.Data;
                    }
                }

                foreach (var bundle in bundleList)
                {
                    _logger.LogInformation("Processing bundle {BundleName}", bundle.Name);
                    var items = new List<int>();
                    foreach (var bundleItem in bundle.AvatarItems)
                    {
                        var item = await _avatarService.GetItemByLayerPositionSortOrderAsync(
                            bundleItem.AvatarLayerPosition, bundleItem.SortOrder);
                        items.Add(item.Id);
                    }
                    bundle.AvatarItems = null;
                    await _avatarService.AddBundleAsync(bundle, items);
                }
            }

            var deleteIssues = false;

            if (jobDetails.UploadedFile)
            {
                _logger.LogInformation("Upload successful, clearing out uploaded files from {AssetPath}",
                    assetPath);
                var directoryInfo = new DirectoryInfo(assetPath);
                try
                {
                    directoryInfo.Delete(true);
                }
                catch (Exception ex) when (
                    ex is DirectoryNotFoundException
                    || ex is IOException
                    || ex is System.Security.SecurityException
                    || ex is UnauthorizedAccessException)
                {
                    deleteIssues = true;
                    _logger.LogWarning("Unable to delete uploaded files from {Path}: {Message}",
                        assetPath,
                        ex.Message);
                }
            }

            sw.Stop();
            _logger.LogInformation("Default avatars added in {TotalSeconds} seconds.",
                sw.Elapsed.TotalSeconds);

            var resultMessage = new StringBuilder("<strong>Import Complete</strong> in ")
                .Append(Convert.ToInt32(sw.Elapsed.TotalSeconds))
                .Append(" seconds");

            if (deleteIssues)
            {
                resultMessage.Append(" - could not delete all uploaded files");
            }

            return new TransferResult
            {
                ItemCount = totalFilesCopied
            };
        }

        private class TransferResult
        {
            public int ItemCount { get; set; }
            public string Path { get; set; }
        }
    }
}
