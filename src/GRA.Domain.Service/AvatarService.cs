using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GRA.Abstract;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Repository;
using GRA.Domain.Service.Abstract;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace GRA.Domain.Service
{
    public class AvatarService : BaseUserService<AvatarService>
    {
        private readonly IAvatarBundleRepository _avatarBundleRepository;
        private readonly IAvatarColorRepository _avatarColorRepository;
        private readonly IAvatarElementRepository _avatarElementRepository;
        private readonly IAvatarItemRepository _avatarItemRepository;
        private readonly IAvatarLayerRepository _avatarLayerRepository;
        private readonly IJobRepository _jobRepository;
        private readonly LanguageService _languageService;
        private readonly IPathResolver _pathResolver;
        private readonly ITriggerRepository _triggerRepository;

        public AvatarService(ILogger<AvatarService> logger,
            GRA.Abstract.IDateTimeProvider dateTimeProvider,
            IUserContextProvider userContextProvider,
            IAvatarBundleRepository avatarBundleRepository,
            IAvatarColorRepository avatarColorRepository,
            IAvatarElementRepository avatarElementRepository,
            IAvatarItemRepository avatarItemRepository,
            IAvatarLayerRepository avatarLayerRepository,
            IJobRepository jobRepository,
            LanguageService languageService,
            ITriggerRepository triggerRepository,
            IPathResolver pathResolver)
            : base(logger, dateTimeProvider, userContextProvider)
        {
            ArgumentNullException.ThrowIfNull(avatarBundleRepository);
            ArgumentNullException.ThrowIfNull(avatarColorRepository);
            ArgumentNullException.ThrowIfNull(avatarElementRepository);
            ArgumentNullException.ThrowIfNull(avatarItemRepository);
            ArgumentNullException.ThrowIfNull(avatarLayerRepository);
            ArgumentNullException.ThrowIfNull(jobRepository);
            ArgumentNullException.ThrowIfNull(languageService);
            ArgumentNullException.ThrowIfNull(pathResolver);
            ArgumentNullException.ThrowIfNull(triggerRepository);

            _avatarBundleRepository = avatarBundleRepository;
            _avatarColorRepository = avatarColorRepository;
            _avatarElementRepository = avatarElementRepository;
            _avatarItemRepository = avatarItemRepository;
            _avatarLayerRepository = avatarLayerRepository;
            _jobRepository = jobRepository;
            _languageService = languageService;
            _pathResolver = pathResolver;
            _triggerRepository = triggerRepository;

            SetManagementPermission(Permission.ManageAvatars);
        }

        public async Task<AvatarBundle> AddBundleAsync(AvatarBundle bundle, List<int> itemIds)
        {
            ArgumentNullException.ThrowIfNull(bundle);
            VerifyManagementPermission();
            var items = await _avatarItemRepository.GetByIdsAsync(itemIds);
            if (items.Any(_ => _.Unlockable != bundle.CanBeUnlocked))
            {
                throw new GraException($"Not all items are {(bundle.CanBeUnlocked ? "Unlockable" : "Available")}.");
            }

            if (!bundle.CanBeUnlocked
                && items.GroupBy(_ => _.AvatarLayerId).Any(_ => _.Skip(1).Any()))
            {
                throw new GraException("Default bundles cannot have multiple items per layer.");
            }

            bundle.SiteId = GetCurrentSiteId();
            var newBundle = await _avatarBundleRepository.AddSaveAsync(
                GetClaimId(ClaimType.UserId), bundle);

            await _avatarBundleRepository.AddItemsAsync(newBundle.Id, itemIds);

            return newBundle;
        }

        public async Task<AvatarLayer> AddLayerAsync(AvatarLayer layer)
        {
            ArgumentNullException.ThrowIfNull(layer);
            VerifyManagementPermission();
            layer.SiteId = GetCurrentSiteId();
            var currentLayer = await _avatarLayerRepository.AddSaveAsync(
                GetClaimId(ClaimType.UserId), layer);

            foreach (var text in layer.Texts)
            {
                var languageId = await _languageService.GetLanguageIdAsync(text.Language);
                if (languageId != default)
                {
                    await _avatarLayerRepository.AddAvatarLayerTextAsync(currentLayer.Id,
                        languageId,
                        text);
                }
            }
            await _avatarLayerRepository.SaveAsync();

            var layerData = _avatarLayerRepository.GetNameAndLabelByLanguageId(currentLayer.Id,
                await _languageService.GetDefaultLanguageIdAsync());
            currentLayer.Name = layerData["Name"];
            return currentLayer;
        }

        public async Task DeleteItemAsync(int id)
        {
            VerifyManagementPermission();
            if (await _avatarItemRepository.IsLastInRequiredLayer(id))
            {
                throw new GraException("Required layer needs an available item.");
            }
            await _avatarItemRepository.RemoveUserItemAsync(id);
            _avatarItemRepository.RemoveUserUnlockedItem(id);
            _avatarElementRepository.RemoveByItemId(id);
            _avatarBundleRepository.RemoveItemFromBundles(id);
            await _avatarItemRepository.RemoveAsync(GetClaimId(ClaimType.UserId), id);
            await _avatarItemRepository.SaveAsync();
        }

        public async Task DescreaseItemSortAsync(int id)
        {
            VerifyManagementPermission();
            await _avatarItemRepository.DecreaseSortPosition(GetCurrentSiteId(), id);
        }

        public async Task<AvatarBundle> EditBundleAsync(AvatarBundle bundle,
            List<int> itemIds)
        {
            ArgumentNullException.ThrowIfNull(bundle);
            VerifyManagementPermission();

            var currentBundle = await _avatarBundleRepository.GetByIdAsync(bundle.Id, false);
            if (currentBundle.HasBeenAwarded)
            {
                throw new GraException("This bundle has been awarded to a participant and can no longer be edited. ");
            }

            var items = await _avatarItemRepository.GetByIdsAsync(itemIds);
            if (items.Any(_ => _.Unlockable != currentBundle.CanBeUnlocked))
            {
                throw new GraException($"Not all items are {(bundle.CanBeUnlocked ? "Unlockable" : "Available")}.");
            }

            if (!currentBundle.CanBeUnlocked
                && items.GroupBy(_ => _.AvatarLayerId).Any(_ => _.Skip(1).Any()))
            {
                throw new GraException("Default bundles cannot have multiple items per layer.");
            }

            currentBundle.Name = bundle.Name;
            await _avatarBundleRepository.UpdateSaveAsync(GetClaimId(ClaimType.UserId),
                currentBundle);

            var currentItemIds = currentBundle.AvatarItems.Select(_ => _.Id).ToList();
            var itemsToRemove = currentItemIds.Except(itemIds).ToList();
            var itemsToAdd = itemIds.Except(currentItemIds).ToList();

            await _avatarBundleRepository.RemoveItemsAsync(currentBundle.Id, itemsToRemove);
            await _avatarBundleRepository.AddItemsAsync(currentBundle.Id, itemsToAdd);

            return currentBundle;
        }

        public async Task<ICollection<AvatarBundle>> GetAllBundlesAsync(
            bool? unlockable = null)
        {
            VerifyManagementPermission();
            return await _avatarBundleRepository.GetAllAsync(GetCurrentSiteId(), unlockable);
        }

        public async Task<AvatarBundle> GetBundleByIdAsync(int id, bool includeDeleted = false)
        {
            return await _avatarBundleRepository.GetByIdAsync(id, includeDeleted)
                ?? throw new GraException("The requested bundle could not be accessed or does not exist.");
        }

        public async Task<ICollection<AvatarColor>> GetColorsByLayerAsync(int layerId)
        {
            var currentCultureName = _userContextProvider.GetCurrentCulture()?.Name;
            var currentLanguageId =
                await _languageService.GetLanguageIdAsync(currentCultureName);

            return await _avatarColorRepository.GetByLayerAsync(layerId, currentLanguageId);
        }

        public async Task<string> GetDefaultLayerNameByIdAsync(int layerId)
        {
            VerifyManagementPermission();
            var languageId = await _languageService.GetDefaultLanguageIdAsync();
            return await _avatarLayerRepository.GetNameByLanguageIdAsync(layerId, languageId);
        }

        public async Task<AvatarItem> GetItemByLayerPositionSortOrderAsync(int layerPosition,
            int sortOrder)
        {
            VerifyManagementPermission();
            return await _avatarItemRepository.GetByLayerPositionSortOrderAsync(layerPosition,
                sortOrder);
        }

        public async Task<AvatarItem> GetItemByIdAsync(int id)
        {
            VerifyManagementPermission();
            return await _avatarItemRepository.GetByIdAsync(id);
        }

        public async Task<ICollection<AvatarItem>> GetItemsByIdsAsync(List<int> ids)
        {
            VerifyManagementPermission();
            return await _avatarItemRepository.GetByIdsAsync(ids);
        }

        public async Task<ICollection<AvatarItem>> GetItemsByLayerAsync(int layerId)
        {
            return await _avatarItemRepository.GetByLayerAsync(layerId);
        }

        public async Task<int> GetLayerAvailableItemCountAsync(int layerId)
        {
            VerifyManagementPermission();
            return await _avatarItemRepository.GetLayerAvailableItemCountAsync(layerId);
        }

        public async Task<IEnumerable<AvatarLayer>> GetLayersAsync()
        {
            var currentCultureName = _userContextProvider.GetCurrentCulture()?.Name;
            var currentLanguageId =
                await _languageService.GetLanguageIdAsync(currentCultureName);
            VerifyManagementPermission();
            var layers = await _avatarLayerRepository.GetAllAsync(GetCurrentSiteId());
            if (layers.Count > 0)
            {
                foreach (var layer in layers.ToList())
                {
                    var layerText = _avatarLayerRepository
                        .GetNameAndLabelByLanguageId(layer.Id, currentLanguageId);
                    layer.Name = layerText["Name"];
                    layer.RemoveLabel = layerText["RemoveLabel"];
                }
            }
            return layers;
        }

        public async Task<int> GetLayerUnavailableItemCountAsync(int layerId)
        {
            VerifyManagementPermission();
            return await _avatarItemRepository.GetLayerUnavailableItemCountAsync(layerId);
        }

        public async Task<int> GetLayerUnlockableItemCountAsync(int layerId)
        {
            VerifyManagementPermission();
            return await _avatarItemRepository.GetLayerUnlockableItemCountAsync(layerId);
        }

        public async Task<DataWithCount<ICollection<AvatarBundle>>>
            GetPaginatedBundleListAsync(AvatarFilter filter)
        {
            ArgumentNullException.ThrowIfNull(filter);
            VerifyManagementPermission();
            filter.SiteId = GetCurrentSiteId();
            return new DataWithCount<ICollection<AvatarBundle>>
            {
                Data = await _avatarBundleRepository.PageAsync(filter),
                Count = await _avatarBundleRepository.CountAsync(filter)
            };
        }

        public async Task<ICollection<Trigger>> GetTriggersAwardingBundleAsync(int id)
        {
            VerifyManagementPermission();
            return await _triggerRepository.GetTriggersAwardingBundleAsync(id);
        }

        public async Task<ICollection<AvatarElement>> GetUserAvatarAsync()
        {
            var currentCultureName = _userContextProvider.GetCurrentCulture()?.Name;
            var currentLanguageId =
                await _languageService.GetLanguageIdAsync(currentCultureName);
            return await _avatarElementRepository.GetUserAvatarAsync(GetActiveUserId(),
                currentLanguageId);
        }

        public async Task<ICollection<AvatarItem>> GetUsersItemsByLayerAsync(int layerId)
        {
            var currentCultureName = _userContextProvider.GetCurrentCulture()?.Name;
            var currentLanguageId =
                await _languageService.GetLanguageIdAsync(currentCultureName);
            var userId = GetClaimId(ClaimType.UserId);
            return await _avatarItemRepository.GetUserItemsByLayerAsync(userId, layerId,
                currentLanguageId);
        }

        public async Task<List<AvatarBundle>> GetUserUnlockBundlesAsync()
        {
            var userHistory = await _avatarBundleRepository.UserHistoryAsync(GetActiveUserId());
            var bundles = new List<AvatarBundle>();
            foreach (var historyItem in userHistory.Where(_ => _.AvatarBundleId.HasValue))
            {
                if (!bundles.Any(bundle => bundle.Id == historyItem.AvatarBundleId))
                {
                    var bundle = await GetBundleByIdAsync(historyItem.AvatarBundleId.Value);
                    bundle.HasBeenViewed = historyItem.HasBeenViewed ?? false;
                    bundles.Add(bundle);
                }
            }
            return bundles;
        }

        public async Task<ICollection<AvatarLayer>> GetUserWardrobeAsync()
        {
            var activeUserId = GetActiveUserId();
            var siteId = GetCurrentSiteId();
            var currentCultureName = _userContextProvider.GetCurrentCulture()?.Name;
            var currentLanguageId = await _languageService.GetLanguageIdAsync(currentCultureName);
            var layers = await _avatarLayerRepository.GetAllWithColorsAsync(siteId);

            if (layers.Count > 0)
            {
                var userAvatar = await _avatarElementRepository.GetUserAvatarAsync(activeUserId,
                    currentLanguageId);
                var bundleItems = new List<AvatarItem>();
                if (userAvatar.Count == 0)
                {
                    var randomBundle = await _avatarBundleRepository
                        .GetRandomDefaultBundleAsync(siteId);
                    bundleItems = randomBundle.ToList();
                }
                var filePath = _pathResolver.ResolveContentPath($"site{siteId}/avatars/");
                foreach (var layer in layers)
                {
                    var layerText = _avatarLayerRepository
                        .GetNameAndLabelByLanguageId(layer.Id, currentLanguageId);
                    layer.Name = layerText["Name"];
                    layer.RemoveLabel = layerText["RemoveLabel"];
                    layer.AvatarItems = await _avatarItemRepository
                               .GetUserItemsByLayerAsync(activeUserId, layer.Id, currentLanguageId);
                    layer.Icon = _pathResolver.ResolveContentPath(layer.Icon);

                    if (userAvatar.Count > 0)
                    {
                        var layerSelection = userAvatar.SingleOrDefault(_ =>
                            _.LayerId == layer.Id);
                        if (layerSelection != null)
                        {
                            layer.AltText = layerSelection.AltText;
                            layer.SelectedItem = layerSelection.AvatarItemId;
                            layer.SelectedColor = layerSelection.AvatarColorId;
                            layer.FilePath = _pathResolver
                                .ResolveContentPath(layerSelection.Filename);
                        }
                        else if (layer.AvatarColors.Count > 0)
                        {
                            layer.SelectedColor = layer.AvatarColors
                                .ElementAt(new Random().Next(0, layer.AvatarColors.Count)).Id;
                        }
                    }
                    else
                    {
                        if (layer.AvatarColors.Count > 0)
                        {
                            layer.SelectedColor = layer.AvatarColors
                                .ElementAt(new Random().Next(0, layer.AvatarColors.Count)).Id;
                        }

                        if (bundleItems.Count > 0)
                        {
                            var layerSelection = bundleItems.SingleOrDefault(_ =>
                                _.AvatarLayerId == layer.Id);
                            if (layerSelection != null)
                            {
                                layer.SelectedItem = layerSelection.Id;
                            }
                        }

                        if (!layer.SelectedItem.HasValue && !layer.CanBeEmpty)
                        {
                            layer.SelectedItem = layer.AvatarItems.First().Id;
                        }

                        if (layer.SelectedItem.HasValue)
                        {
                            var fileName = "item";
                            if (layer.SelectedColor.HasValue)
                            {
                                fileName += $"_{layer.SelectedColor}";
                            }
                            fileName += ".png";
                            layer.FilePath = Path.Combine(filePath,
                                $"layer{layer.Id}",
                                $"item{layer.SelectedItem}",
                                fileName);
                        }
                    }
                }
            }
            return layers.Where(_ => _.AvatarItems.Count > 0).ToList();
        }

        public async Task<JobStatus> ImportAvatarsAsync(int jobId,
            CancellationToken token,
            IProgress<JobStatus> progress = null)
        {
            var requestingUser = GetClaimId(ClaimType.UserId);

            if (HasPermission(Permission.ManageAvatars))
            {
                var sw = Stopwatch.StartNew();

                var job = await _jobRepository.GetByIdAsync(jobId);
                var jobDetails
                    = JsonConvert
                        .DeserializeObject<JobDetailsAvatarImport>(job.SerializedParameters);

                string assetPath = jobDetails.AssetPath;

                token.Register(() =>
                {
                    _logger.LogWarning("Import avatars for user {User} was cancelled after {Elapsed} ms.",
                        requestingUser,
                        sw?.ElapsedMilliseconds);
                });

                var jsonPath = Path.Combine(assetPath, "default avatars.json");

                if (!File.Exists(jsonPath))
                {
                    _logger.LogError("Unable to find file {DefaultAvatarsJson}", jsonPath);
                    return new JobStatus
                    {
                        PercentComplete = 0,
                        Complete = true,
                        Error = true,
                        Status = $"Unable to find the default avatars.json file in {assetPath}."
                    };
                }

                IEnumerable<AvatarLayer> avatarList;

                using (StreamReader file = File.OpenText(jsonPath))
                {
                    var jsonString = await file.ReadToEndAsync();
                    avatarList = JsonConvert
                        .DeserializeObject<IEnumerable<AvatarLayer>>(jsonString);
                }

                var layerCount = avatarList.Count();
                _logger.LogInformation("Found {Count} AvatarLayer objects in avatar JSON file",
                    layerCount);

                // Layers + background/bundles
                var processingCount = layerCount + 1;
                var processedCount = 0;

                var bundleJsonPath = Path.Combine(assetPath, "default bundles.json");
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
                    progress?.Report(new JobStatus
                    {
                        PercentComplete = processedCount * 100 / processingCount,
                        Status = $"Processing layer {layer.Name}",
                        Error = false
                    });

                    var colors = layer.AvatarColors;
                    var items = layer.AvatarItems;
                    layer.AvatarColors = null;
                    layer.AvatarItems = null;

                    var addedLayer = await AddLayerAsync(layer);

                    var layerAssetPath = Path.Combine(assetPath, layer.Name);
                    var destinationRoot = Path.Combine(destinationBase, $"layer{addedLayer.Id}");
                    var destinationPath = _pathResolver.ResolveContentFilePath(destinationRoot);
                    if (!Directory.Exists(destinationPath))
                    {
                        Directory.CreateDirectory(destinationPath);
                    }

                    addedLayer.Icon = Path.Combine(destinationRoot, "icon.png");
                    File.Copy(Path.Combine(layerAssetPath, "icon.png"),
                        Path.Combine(destinationPath, "icon.png"));

                    await UpdateLayerAsync(addedLayer);

                    int lastUpdateSent;
                    if (colors != null)
                    {
                        progress?.Report(new JobStatus
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
                                progress.Report(new JobStatus
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

                            await _avatarColorRepository.AddAsync(requestingUser, color);
                            currentColor++;
                        }

                        await _avatarColorRepository.SaveAsync();
                        colors = await GetColorsByLayerAsync(addedLayer.Id);
                    }

                    progress?.Report(new JobStatus
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
                            progress?.Report(new JobStatus
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

                        await _avatarItemRepository.AddAsync(requestingUser, item);
                        currentItem++;
                    }
                    await _avatarItemRepository.SaveAsync();
                    items = await GetItemsByLayerAsync(addedLayer.Id);

                    _logger.LogInformation("Processing {Count} items in {LayerName}",
                        items.Count,
                        layer.Name);

                    progress?.Report(new JobStatus
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
                            progress?.Report(new JobStatus
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
                        item.Thumbnail = Path.Combine(itemRoot, "thumbnail.jpg");
                        File.Copy(Path.Combine(itemAssetPath, "thumbnail.jpg"),
                            Path.Combine(itemPath, "thumbnail.jpg"));
                        await _avatarItemRepository.UpdateAsync(requestingUser, item);
                        if (colors != null)
                        {
                            foreach (var color in colors)
                            {
                                var element = new AvatarElement
                                {
                                    AvatarItemId = item.Id,
                                    AvatarColorId = color.Id,
                                    Filename = Path.Combine(itemRoot, $"item_{color.Id}.png")
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
                                Filename = Path.Combine(itemRoot, "item.png")
                            };
                            await _avatarElementRepository.AddAsync(requestingUser, element);
                            File.Copy(Path.Combine(itemAssetPath, "item.png"),
                                Path.Combine(itemPath, "item.png"));
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

                progress?.Report(new JobStatus
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
                    Path.Combine(bundlePath, "icon.png"));
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
                        bundleList = JsonConvert
                            .DeserializeObject<IEnumerable<AvatarBundle>>(jsonString);
                    }

                    foreach (var bundle in bundleList)
                    {
                        _logger.LogInformation("Processing bundle {BundleName}", bundle.Name);
                        var items = new List<int>();
                        foreach (var bundleItem in bundle.AvatarItems)
                        {
                            var item = await GetItemByLayerPositionSortOrderAsync(
                                bundleItem.AvatarLayerPosition, bundleItem.SortOrder);
                            items.Add(item.Id);
                        }
                        bundle.AvatarItems = null;
                        await AddBundleAsync(bundle, items);
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

                return new JobStatus
                {
                    PercentComplete = 100,
                    Complete = true,
                    Status = resultMessage.ToString()
                };
            }
            else
            {
                _logger.LogError("User {RequestingUser} doesn't have permission to import avatars.",
                    requestingUser);
                return new JobStatus
                {
                    PercentComplete = 0,
                    Status = "Permission denied.",
                    Error = true,
                    Complete = true
                };
            }
        }

        public async Task IncreaseItemSortAsync(int id)
        {
            VerifyManagementPermission();
            await _avatarItemRepository.IncreaseSortPosition(GetCurrentSiteId(), id);
        }

        public async Task<DataWithCount<ICollection<AvatarColor>>> PageColorsAsync(
            AvatarFilter filter)
        {
            ArgumentNullException.ThrowIfNull(filter);
            VerifyManagementPermission();
            filter.SiteId = GetCurrentSiteId();
            return await _avatarColorRepository.PageAsync(filter);
        }

        public async Task<DataWithCount<ICollection<AvatarItem>>> PageItemsAsync(
            AvatarFilter filter)
        {
            ArgumentNullException.ThrowIfNull(filter);
            VerifyManagementPermission();
            filter.SiteId = GetCurrentSiteId();
            return await _avatarItemRepository.PageAsync(filter);
        }

        public async Task RemoveBundleAsync(int id)
        {
            VerifyManagementPermission();
            if (await _triggerRepository.BundleIsInUseAsync(id))
            {
                throw new GraException("Bundle is currently being awarded by a trigger");
            }

            var bundle = await _avatarBundleRepository.GetByIdAsync(id, false);
            bundle.IsDeleted = true;
            await _avatarBundleRepository.UpdateSaveAsync(GetClaimId(ClaimType.UserId),
                bundle);
        }

        public async Task SetItemAvailableAsync(int id)
        {
            VerifyManagementPermission();
            if (await _avatarBundleRepository.IsItemInBundleAsync(id, true))
            {
                throw new GraException("Item is part of an unlockable bundle.");
            }

            var item = await _avatarItemRepository.GetByIdAsync(id);
            item.Unlockable = false;
            await _avatarItemRepository.UpdateSaveAsync(GetClaimId(ClaimType.UserId), item);
        }

        public async Task SetItemUnlockableAsync(int id)
        {
            VerifyManagementPermission();
            if (await _avatarItemRepository.IsLastInRequiredLayer(id))
            {
                throw new GraException("Required layer needs an available item.");
            }
            if (await _avatarBundleRepository.IsItemInBundleAsync(id, false))
            {
                throw new GraException("Item is part of a default bundle.");
            }
            if (await _avatarItemRepository.IsInUse(id, true))
            {
                throw new GraException("Item is in use by a participant.");
            }

            var item = await _avatarItemRepository.GetByIdAsync(id);
            item.Unlockable = true;
            await _avatarItemRepository.UpdateSaveAsync(GetClaimId(ClaimType.UserId), item);
        }

        public async Task UpdateBundleHasBeenViewedAsync(int bundleId)
        {
            await _avatarBundleRepository.UpdateHasBeenViewedAsync(GetActiveUserId(),
                bundleId);
        }

        public async Task UpdateColorTextsAsync(IEnumerable<AvatarColorText> colorTexts)
        {
            ArgumentNullException.ThrowIfNull(colorTexts);
            VerifyManagementPermission();

            var colorIds = colorTexts.Select(_ => _.AvatarColorId).Distinct();
            var currentTexts = await _avatarColorRepository.GetTextsByColorIdsAsync(colorIds);

            var textsToAdd = new List<AvatarColorText>();
            var textsToRemove = new List<AvatarColorText>();
            var textsToUpdate = new List<AvatarColorText>();

            foreach (var text in colorTexts)
            {
                text.AltText = text.AltText?.Trim();

                var currentText = currentTexts
                    .Where(_ => _.AvatarColorId == text.AvatarColorId
                        && _.LanguageId == text.LanguageId)
                    .SingleOrDefault();

                if (!string.IsNullOrWhiteSpace(text.AltText))
                {
                    if (currentText == null)
                    {

                        textsToAdd.Add(text);
                    }
                    else
                    {
                        currentText.AltText = text.AltText;
                        textsToUpdate.Add(currentText);
                    }
                }
                else if (currentText != null)
                {
                    textsToRemove.Add(currentText);
                }
            }

            try
            {
                if (textsToAdd.Count > 0 || textsToRemove.Count > 0 || textsToUpdate.Count > 0)
                {
                    if (textsToAdd.Count > 0)
                    {
                        await _avatarColorRepository.AddTextsAsync(textsToAdd);
                    }
                    if (textsToRemove.Count > 0)
                    {
                        _avatarColorRepository.RemoveTexts(textsToRemove);
                    }
                    if (textsToUpdate.Count > 0)
                    {
                        _avatarColorRepository.UpdateTexts(textsToUpdate);
                    }
                    await _avatarColorRepository.SaveAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unable to update avatar color texts: {ErrorMessage}",
                    ex.Message);
                throw new GraException($"Unable to update avatar color texts: {ex.Message}");
            }
        }

        public async Task UpdateItemTextsAsync(IEnumerable<AvatarItemText> itemTexts)
        {
            ArgumentNullException.ThrowIfNull(itemTexts);
            VerifyManagementPermission();

            var itemIds = itemTexts.Select(_ => _.AvatarItemId).Distinct();
            var currentTexts = await _avatarItemRepository.GetTextsByItemIdsAsync(itemIds);

            var textsToAdd = new List<AvatarItemText>();
            var textsToRemove = new List<AvatarItemText>();
            var textsToUpdate = new List<AvatarItemText>();

            foreach (var text in itemTexts)
            {
                text.AltText = text.AltText?.Trim();

                var currentText = currentTexts
                    .Where(_ => _.AvatarItemId == text.AvatarItemId
                        && _.LanguageId == text.LanguageId)
                    .SingleOrDefault();

                if (!string.IsNullOrWhiteSpace(text.AltText))
                {
                    if (currentText == null)
                    {

                        textsToAdd.Add(text);
                    }
                    else
                    {
                        currentText.AltText = text.AltText;
                        textsToUpdate.Add(currentText);
                    }
                }
                else if (currentText != null)
                {
                    textsToRemove.Add(currentText);
                }
            }

            try
            {
                if (textsToAdd.Count > 0 || textsToRemove.Count > 0 || textsToUpdate.Count > 0)
                {
                    if (textsToAdd.Count > 0)
                    {
                        await _avatarItemRepository.AddTextsAsync(textsToAdd);
                    }
                    if (textsToRemove.Count > 0)
                    {
                        _avatarItemRepository.RemoveTexts(textsToRemove);
                    }
                    if (textsToUpdate.Count > 0)
                    {
                        _avatarItemRepository.UpdateTexts(textsToUpdate);
                    }
                    await _avatarItemRepository.SaveAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unable to update avatar item texts: {ErrorMessage}",
                    ex.Message);
                throw new GraException($"Unable to update avatar item texts: {ex.Message}");
            }
        }

        public async Task<AvatarLayer> UpdateLayerAsync(AvatarLayer layer)
        {
            ArgumentNullException.ThrowIfNull(layer);
            VerifyManagementPermission();
            layer.SiteId = GetCurrentSiteId();
            return await _avatarLayerRepository.UpdateSaveAsync(
                GetClaimId(ClaimType.UserId), layer);
        }

        public async Task UpdateUserAvatarAsync(ICollection<AvatarLayer> selectionLayers)
        {
            var activeUserId = GetActiveUserId();
            var currentCultureName = _userContextProvider.GetCurrentCulture()?.Name;
            var currentLanguageId =
                await _languageService.GetLanguageIdAsync(currentCultureName);
            var layers = await _avatarLayerRepository.GetAllAsync(GetCurrentSiteId());
            if (layers.Count > 0)
            {
                foreach (var layer in layers.ToList())
                {
                    var layerText = _avatarLayerRepository
                        .GetNameAndLabelByLanguageId(layer.Id, currentLanguageId);
                    layer.Name = layerText["Name"];
                    layer.RemoveLabel = layerText["RemoveLabel"];
                }
            }
            var elementList = new List<int>();
            foreach (var layer in layers)
            {
                var selection = selectionLayers.FirstOrDefault(_ => _.Id == layer.Id);
                if (selection != default(AvatarLayer))
                {
                    var element = await _avatarElementRepository.GetByItemAndColorAsync(
                        selection.SelectedItem.Value, selection.SelectedColor);

                    if (element != default(AvatarElement)
                        && (!element.AvatarItem.Unlockable
                        || await _avatarItemRepository
                            .HasUserUnlockedItemAsync(activeUserId, element.AvatarItemId)))
                    {
                        elementList.Add(element.Id);
                    }
                    else
                    {
                        _logger.LogWarning("User {UserId} can't select item {ItemValue} and color {ItemColor} for layer {LayerId}.",
                            activeUserId,
                            selection.SelectedItem.Value,
                            selection.SelectedColor,
                            layer.Id);
                        throw new GraException($"Invalid selection for {layer.Name}");
                    }
                }
                else if (!layer.CanBeEmpty)
                {
                    _logger.LogWarning("User {UserId} can't have an empty selection for layer {LayerId}.",
                        activeUserId,
                        layer.Id);
                    throw new GraException($"A {layer.Name} must be selected");
                }
            }
            await _avatarElementRepository.SetUserAvatarAsync(activeUserId, elementList);
        }
    }
}
