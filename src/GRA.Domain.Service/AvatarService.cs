using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GRA.Abstract;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Repository;
using GRA.Domain.Service.Abstract;
using Microsoft.Extensions.Logging;

namespace GRA.Domain.Service
{
    public class AvatarService : BaseUserService<AvatarService>
    {
        private readonly IAvatarBundleRepository _avatarBundleRepository;
        private readonly IAvatarColorRepository _avatarColorRepository;
        private readonly IAvatarElementRepository _avatarElementRepository;
        private readonly IAvatarItemRepository _avatarItemRepository;
        private readonly IAvatarLayerRepository _avatarLayerRepository;
        private readonly LanguageService _languageService;
        private readonly IPathResolver _pathResolver;
        private readonly ITriggerRepository _triggerRepository;

        public AvatarService(ILogger<AvatarService> logger,
            IDateTimeProvider dateTimeProvider,
            IUserContextProvider userContextProvider,
            IAvatarBundleRepository avatarBundleRepository,
            IAvatarColorRepository avatarColorRepository,
            IAvatarElementRepository avatarElementRepository,
            IAvatarItemRepository avatarItemRepository,
            IAvatarLayerRepository avatarLayerRepository,
            IPathResolver pathResolver,
            ITriggerRepository triggerRepository,
            LanguageService languageService)
            : base(logger, dateTimeProvider, userContextProvider)
        {
            ArgumentNullException.ThrowIfNull(avatarBundleRepository);
            ArgumentNullException.ThrowIfNull(avatarColorRepository);
            ArgumentNullException.ThrowIfNull(avatarElementRepository);
            ArgumentNullException.ThrowIfNull(avatarItemRepository);
            ArgumentNullException.ThrowIfNull(avatarLayerRepository);
            ArgumentNullException.ThrowIfNull(languageService);
            ArgumentNullException.ThrowIfNull(pathResolver);
            ArgumentNullException.ThrowIfNull(triggerRepository);

            _avatarBundleRepository = avatarBundleRepository;
            _avatarColorRepository = avatarColorRepository;
            _avatarElementRepository = avatarElementRepository;
            _avatarItemRepository = avatarItemRepository;
            _avatarLayerRepository = avatarLayerRepository;
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

            await _avatarLayerRepository.SaveAsync();

            return currentLayer;
        }

        public async Task AddLayerTexts(int layerId,
                    ICollection<AvatarLayerText> texts,
            int version)
        {
            ArgumentNullException.ThrowIfNull(texts);
            foreach (var text in texts)
            {
                var languageId = await _languageService
                    .GetLanguageIdAsync(version == 1 ? text.Language : text.LanguageName);
                if (languageId != default)
                {
                    await _avatarLayerRepository.AddAvatarLayerTextAsync(layerId, languageId, text);
                }
            }
            await _avatarLayerRepository.SaveAsync();
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

        public async Task<AvatarBundle> EditBundleAsync(AvatarBundle bundle, List<int> itemIds)
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

        public async Task<ICollection<AvatarBundle>> GetAllBundlesAsync(bool? unlockable = null)
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

        public async Task<AvatarItem> GetItemByIdAsync(int id)
        {
            VerifyManagementPermission();
            return await _avatarItemRepository.GetByIdAsync(id);
        }

        public async Task<AvatarItem> GetItemByLayerPositionSortOrderAsync(int layerPosition,
            int sortOrder)
        {
            VerifyManagementPermission();
            return await _avatarItemRepository.GetByLayerPositionSortOrderAsync(layerPosition,
                sortOrder);
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
                    bundleItems = [.. randomBundle];
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
            return [.. layers.Where(_ => _.AvatarItems.Count > 0)];
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
                    .SingleOrDefault(_ => _.AvatarColorId == text.AvatarColorId
                        && _.LanguageId == text.LanguageId);

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
                    .SingleOrDefault(_ => _.AvatarItemId == text.AvatarItemId
                        && _.LanguageId == text.LanguageId);

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
