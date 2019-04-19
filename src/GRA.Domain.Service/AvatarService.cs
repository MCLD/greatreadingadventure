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
        private readonly ITriggerRepository _triggerRepository;
        private readonly IPathResolver _pathResolver;

        public AvatarService(ILogger<AvatarService> logger,
            GRA.Abstract.IDateTimeProvider dateTimeProvider,
            IUserContextProvider userContextProvider,
            IAvatarBundleRepository avatarBundleRepository,
            IAvatarColorRepository avatarColorRepository,
            IAvatarElementRepository avatarElementRepository,
            IAvatarItemRepository avatarItemRepository,
            IAvatarLayerRepository avatarLayerRepository,
            ITriggerRepository triggerRepository,
            IPathResolver pathResolver)
            : base(logger, dateTimeProvider, userContextProvider)
        {
            _avatarBundleRepository = Require.IsNotNull(avatarBundleRepository,
                nameof(avatarBundleRepository));
            _avatarColorRepository = Require.IsNotNull(avatarColorRepository,
                nameof(avatarColorRepository));
            _avatarElementRepository = Require.IsNotNull(avatarElementRepository,
                nameof(avatarElementRepository));
            _avatarItemRepository = Require.IsNotNull(avatarItemRepository,
                nameof(avatarItemRepository));
            _avatarLayerRepository = Require.IsNotNull(avatarLayerRepository,
                nameof(avatarLayerRepository));
            _triggerRepository = Require.IsNotNull(triggerRepository, nameof(triggerRepository));
            _pathResolver = Require.IsNotNull(pathResolver, nameof(pathResolver));

            SetManagementPermission(Permission.ManageAvatars);
        }

        public async Task<ICollection<AvatarLayer>> GetUserWardrobeAsync()
        {
            var activeUserId = GetActiveUserId();
            var siteId = GetCurrentSiteId();
            var layers = await _avatarLayerRepository.GetAllWithColorsAsync(
                siteId, activeUserId);

            if (layers.Count > 0)
            {
                var userAvatar = await _avatarElementRepository.GetUserAvatarAsync(activeUserId);
                var bundleItems = new List<AvatarItem>();
                if (userAvatar.Count == 0)
                {
                    bundleItems = (await _avatarBundleRepository.GetRandomDefaultBundleAsync(siteId)).ToList();
                }
                var filePath = _pathResolver.ResolveContentPath($"site{siteId}/avatars/");
                foreach (var layer in layers)
                {
                    layer.AvatarItems = await _avatarItemRepository
                               .GetUserItemsByLayerAsync(activeUserId, layer.Id);
                    layer.Icon = _pathResolver.ResolveContentPath(layer.Icon);

                    if (userAvatar.Count > 0)
                    {
                        var layerSelection = userAvatar.SingleOrDefault(_ =>
                            _.AvatarItem.AvatarLayerId == layer.Id);
                        if (layerSelection != null)
                        {
                            layer.SelectedItem = layerSelection.AvatarItemId;
                            layer.SelectedColor = layerSelection.AvatarColorId;
                            layer.FilePath = _pathResolver.ResolveContentPath(layerSelection.Filename);
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
                            layer.FilePath = Path.Combine(filePath, $"layer{layer.Id}", $"item{layer.SelectedItem}", fileName);
                        }
                    }
                }
            }
            return layers.Where(_ => _.AvatarItems.Count > 0).ToList();
        }

        public async Task<IEnumerable<AvatarLayer>> GetLayersAsync()
        {
            VerifyManagementPermission();
            return await _avatarLayerRepository.GetAllAsync(GetCurrentSiteId());
        }

        public async Task<int> GetLayerAvailableItemCountAsync(int layerId)
        {
            VerifyManagementPermission();
            return await _avatarItemRepository.GetLayerAvailableItemCountAsync(layerId);
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

        public async Task<AvatarLayer> AddLayerAsync(AvatarLayer layer)
        {
            VerifyManagementPermission();
            layer.SiteId = GetCurrentSiteId();
            return await _avatarLayerRepository.AddSaveAsync(
                GetClaimId(ClaimType.UserId), layer);
        }

        public async Task<AvatarLayer> UpdateLayerAsync(AvatarLayer layer)
        {
            VerifyManagementPermission();
            layer.SiteId = GetCurrentSiteId();
            return await _avatarLayerRepository.UpdateSaveAsync(
                GetClaimId(ClaimType.UserId), layer);
        }

        public async Task<ICollection<AvatarItem>> GetItemsByLayerAsync(int layerId)
        {
            return await _avatarItemRepository.GetByLayerAsync(layerId);
        }

        public async Task<AvatarItem> GetItemByLayerPositionSortOrderAsync(int layerPosition,
            int sortOrder)
        {
            VerifyManagementPermission();
            return await _avatarItemRepository.GetByLayerPositionSortOrderAsync(layerPosition,
                sortOrder);
        }

        public async Task<AvatarItem> AddItemAsync(AvatarItem item)
        {
            VerifyManagementPermission();
            return await _avatarItemRepository.AddSaveAsync(
                GetClaimId(ClaimType.UserId), item);
        }

        public async Task AddItemListAsync(ICollection<AvatarItem> itemList)
        {
            VerifyManagementPermission();
            var userId = GetClaimId(ClaimType.UserId);
            foreach (var item in itemList)
            {
                await _avatarItemRepository.AddAsync(userId, item);
            }
            await _avatarItemRepository.SaveAsync();
        }

        public async Task<AvatarItem> UpdateItemAsync(AvatarItem item)
        {
            VerifyManagementPermission();
            return await _avatarItemRepository.UpdateSaveAsync(
                GetClaimId(ClaimType.UserId), item);
        }

        public async Task UpdateItemListAsync(ICollection<AvatarItem> itemList)
        {
            VerifyManagementPermission();
            var userId = GetClaimId(ClaimType.UserId);
            foreach (var item in itemList)
            {
                await _avatarItemRepository.UpdateAsync(userId, item);
            }
            await _avatarItemRepository.SaveAsync();
        }

        public async Task DescreaseItemSortAsync(int id)
        {
            VerifyManagementPermission();
            await _avatarItemRepository.DecreaseSortPosition(GetCurrentSiteId(), id);
        }

        public async Task IncreaseItemSortAsync(int id)
        {
            VerifyManagementPermission();
            await _avatarItemRepository.IncreaseSortPosition(GetCurrentSiteId(), id);
        }

        public async Task SetItemUnlockableAsync(int id)
        {
            VerifyManagementPermission();
            if (await _avatarItemRepository.IsLastInRequiredLayer(id))
            {
                throw new GraException("Required layer needs an available item.");
            }
            if (await _avatarBundleRepository.IsItemInBundle(id, false))
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

        public async Task SetItemAvailableAsync(int id)
        {
            VerifyManagementPermission();
            if (await _avatarBundleRepository.IsItemInBundle(id, true))
            {
                throw new GraException("Item is part of an unlockable bundle.");
            }

            var item = await _avatarItemRepository.GetByIdAsync(id);
            item.Unlockable = false;
            await _avatarItemRepository.UpdateSaveAsync(GetClaimId(ClaimType.UserId), item);
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
            _avatarElementRepository.RemoveByItemIdAsync(id);
            _avatarBundleRepository.RemoveItemFromBundles(id);
            await _avatarItemRepository.RemoveAsync(GetClaimId(ClaimType.UserId), id);
            await _avatarItemRepository.SaveAsync();
        }

        public async Task<ICollection<AvatarColor>> GetColorsByLayerAsync(int layerId)
        {
            return await _avatarColorRepository.GetByLayerAsync(layerId);
        }

        public async Task<AvatarColor> AddColorAsync(AvatarColor color)
        {
            VerifyManagementPermission();
            return await _avatarColorRepository.AddSaveAsync(
                GetClaimId(ClaimType.UserId), color);
        }

        public async Task AddColorListAsync(ICollection<AvatarColor> colorList)
        {
            VerifyManagementPermission();
            var userId = GetClaimId(ClaimType.UserId);
            foreach (var color in colorList)
            {
                await _avatarColorRepository.AddAsync(userId, color);
            }
            await _avatarColorRepository.SaveAsync();
        }

        public async Task<AvatarColor> UpdateColorAsync(AvatarColor color)
        {
            VerifyManagementPermission();
            return await _avatarColorRepository.UpdateSaveAsync(
                GetClaimId(ClaimType.UserId), color);
        }

        public async Task<AvatarElement> AddElementAsync(AvatarElement element)
        {
            VerifyManagementPermission();
            return await _avatarElementRepository.AddSaveAsync(
                GetClaimId(ClaimType.UserId), element);
        }

        public async Task AddElementListAsync(List<AvatarElement> elementList)
        {
            VerifyManagementPermission();
            var userId = GetClaimId(ClaimType.UserId);
            var count = 0;
            foreach (var element in elementList)
            {
                await _avatarElementRepository.AddAsync(userId, element);
                count++;
                if (count % 500 == 0)
                {
                    await _avatarElementRepository.SaveAsync();
                }
            }
            if (count % 500 != 0)
            {
                await _avatarElementRepository.SaveAsync();
            }
        }

        public async Task<AvatarElement> UpdateElementAsync(AvatarElement element)
        {
            VerifyManagementPermission();
            return await _avatarElementRepository.UpdateSaveAsync(
                GetClaimId(ClaimType.UserId), element);
        }

        public async Task<AvatarBundle> AddBundleAsync(AvatarBundle bundle,
            List<int> itemIds)
        {
            VerifyManagementPermission();
            var items = await _avatarItemRepository.GetByIdsAsync(itemIds);
            if (items.Any(_ => _.Unlockable != bundle.CanBeUnlocked))
            {
                throw new GraException($"Not all items are {(bundle.CanBeUnlocked ? "Unlockable" : "Available")}.");
            }

            if (!bundle.CanBeUnlocked
                && items.GroupBy(_ => _.AvatarLayerId).Any(_ => _.Skip(1).Any()))
            {
                throw new GraException($"Default bundles cannot have multiple items per layer.");
            }

            bundle.SiteId = GetCurrentSiteId();
            var newBundle = await _avatarBundleRepository.AddSaveAsync(
                GetClaimId(ClaimType.UserId), bundle);

            await _avatarBundleRepository.AddItemsAsync(newBundle.Id, itemIds);

            return newBundle;
        }

        public async Task<AvatarBundle> EditBundleAsync(AvatarBundle bundle,
            List<int> itemIds)
        {
            VerifyManagementPermission();

            var currentBundle = await _avatarBundleRepository.GetByIdAsync(bundle.Id, false);
            if (currentBundle.HasBeenAwarded)
            {
                throw new GraException($"This bundle has been awarded to a participant and can no longer be edited. ");
            }

            var items = await _avatarItemRepository.GetByIdsAsync(itemIds);
            if (items.Any(_ => _.Unlockable != currentBundle.CanBeUnlocked))
            {
                throw new GraException($"Not all items are {(bundle.CanBeUnlocked ? "Unlockable" : "Available")}.");
            }

            if (!currentBundle.CanBeUnlocked
                && items.GroupBy(_ => _.AvatarLayerId).Any(_ => _.Skip(1).Any()))
            {
                throw new GraException($"Default bundles cannot have multiple items per layer.");
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

        public async Task<ICollection<AvatarElement>> GetUserAvatarAsync()
        {
            return await _avatarElementRepository.GetUserAvatarAsync(GetActiveUserId());
        }

        public async Task UpdateUserAvatarAsync(ICollection<AvatarLayer> selectionLayers)
        {
            var activeUserId = GetActiveUserId();
            var layers = await _avatarLayerRepository.GetAllAsync(GetCurrentSiteId());
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
                        _logger.LogWarning($"User {activeUserId} can't select item {selection.SelectedItem.Value} and color {selection.SelectedColor} for layer {layer.Id}.");
                        throw new GraException($"Invalid selection for {layer.Name}");
                    }
                }
                else if (!layer.CanBeEmpty)
                {
                    _logger.LogWarning($"User {activeUserId} can't have an empty selection for layer {layer.Id}.");
                    throw new GraException($"A {layer.Name} must be selected");
                }
            }
            await _avatarElementRepository.SetUserAvatarAsync(activeUserId, elementList);
        }

        public async Task<ICollection<AvatarBundle>> GetAllBundlesAsync(
            bool? unlockable = null)
        {
            VerifyManagementPermission();
            return await _avatarBundleRepository.GetAllAsync(GetCurrentSiteId(), unlockable);
        }

        public async Task<AvatarBundle> GetBundleByIdAsync(int id, bool includeDeleted = false)
        {
            var bundle = await _avatarBundleRepository.GetByIdAsync(id, includeDeleted);
            if (bundle == null)
            {
                throw new GraException("The requested bundle could not be accessed or does not exist.");
            }
            return bundle;
        }

        public async Task<DataWithCount<ICollection<AvatarBundle>>>
            GetPaginatedBundleListAsync(AvatarFilter filter)
        {
            VerifyManagementPermission();
            filter.SiteId = GetCurrentSiteId();
            return new DataWithCount<ICollection<AvatarBundle>>
            {
                Data = await _avatarBundleRepository.PageAsync(filter),
                Count = await _avatarBundleRepository.CountAsync(filter)
            };
        }

        public async Task<DataWithCount<ICollection<AvatarItem>>> PageItemsAsync(
            AvatarFilter filter)
        {
            VerifyManagementPermission();
            filter.SiteId = GetCurrentSiteId();
            return new DataWithCount<ICollection<AvatarItem>>
            {
                Data = await _avatarItemRepository.PageAsync(filter),
                Count = await _avatarItemRepository.CountAsync(filter)
            };
        }

        public async Task<ICollection<AvatarItem>> GetItemsByIdsAsync(List<int> ids)
        {
            VerifyManagementPermission();
            return await _avatarItemRepository.GetByIdsAsync(ids);
        }

        public async Task<ICollection<Trigger>> GetTriggersAwardingBundleAsync(int id)
        {
            VerifyManagementPermission();
            return await _triggerRepository.GetTriggersAwardingBundleAsync(id);
        }
    }
}
