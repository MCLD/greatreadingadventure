using GRA.Abstract;
using GRA.Domain.Model;
using GRA.Domain.Repository;
using GRA.Domain.Service.Abstract;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace GRA.Domain.Service
{
    public class DynamicAvatarService : BaseUserService<DynamicAvatarService>
    {
        private readonly IDynamicAvatarBundleRepository _dynamicAvatarBundleRepository;
        private readonly IDynamicAvatarColorRepository _dynamicAvatarColorRepository;
        private readonly IDynamicAvatarElementRepository _dynamicAvatarElementRepository;
        private readonly IDynamicAvatarItemRepository _dynamicAvatarItemRepository;
        private readonly IDynamicAvatarLayerRepository _dynamicAvatarLayerRepository;
        private readonly IPathResolver _pathResolver;
        public DynamicAvatarService(ILogger<DynamicAvatarService> logger,
            GRA.Abstract.IDateTimeProvider dateTimeProvider,
            IUserContextProvider userContextProvider,
            IDynamicAvatarBundleRepository dynamicAvatarBundleRepository,
            IDynamicAvatarColorRepository dynamicAvatarColorRepository,
            IDynamicAvatarElementRepository dynamicAvatarElementRepository,
            IDynamicAvatarItemRepository dynamicAvatarItemRepository,
            IDynamicAvatarLayerRepository dynamicAvatarLayerRepository,
            IPathResolver pathResolver)
            : base(logger, dateTimeProvider, userContextProvider)
        {
            _dynamicAvatarBundleRepository = Require.IsNotNull(dynamicAvatarBundleRepository,
                nameof(dynamicAvatarBundleRepository));
            _dynamicAvatarColorRepository = Require.IsNotNull(dynamicAvatarColorRepository,
                nameof(dynamicAvatarColorRepository));
            _dynamicAvatarElementRepository = Require.IsNotNull(dynamicAvatarElementRepository,
                nameof(dynamicAvatarElementRepository));
            _dynamicAvatarItemRepository = Require.IsNotNull(dynamicAvatarItemRepository,
                nameof(dynamicAvatarItemRepository));
            _dynamicAvatarLayerRepository = Require.IsNotNull(dynamicAvatarLayerRepository,
                nameof(dynamicAvatarLayerRepository));
            _pathResolver = Require.IsNotNull(pathResolver, nameof(pathResolver));

            SetManagementPermission(Permission.ManageAvatars);
        }

        public async Task<ICollection<DynamicAvatarLayer>> GetUserWardrobeAsync()
        {
            var activeUserId = GetActiveUserId();
            var siteId = GetCurrentSiteId();
            var layers = await _dynamicAvatarLayerRepository.GetAllWithColorsAsync(
                siteId, activeUserId);

            if (layers.Count > 0)
            {
                var userAvatar = await _dynamicAvatarElementRepository.GetUserAvatarAsync(activeUserId);
                var bundleItems = new List<DynamicAvatarItem>();
                if (userAvatar.Count == 0)
                {
                    bundleItems = (await _dynamicAvatarBundleRepository.GetRandomDefaultBundleAsync(siteId)).ToList();
                }
                foreach (var layer in layers)
                {
                    layer.DynamicAvatarItems = await _dynamicAvatarItemRepository
                               .GetUserItemsByLayerAsync(activeUserId, layer.Id);

                    if (userAvatar.Count > 0)
                    {
                        var layerSelection = userAvatar.Where(_ =>
                        _.DynamicAvatarItem.DynamicAvatarLayerId == layer.Id).SingleOrDefault();
                        if (layerSelection != null)
                        {
                            layer.SelectedItem = layerSelection.DynamicAvatarItemId;
                            layer.SelectedColor = layerSelection.DynamicAvatarColorId;
                            layer.FilePath = _pathResolver.ResolveContentPath(layerSelection.Filename);
                        }
                        else if (layer.DynamicAvatarColors.Count > 0)
                        {
                            layer.SelectedColor = layer.DynamicAvatarColors
                                .ElementAt(new Random().Next(0, layer.DynamicAvatarColors.Count)).Id;
                        }
                    }
                    else
                    {
                        if (layer.DynamicAvatarColors.Count > 0)
                        {
                            layer.SelectedColor = layer.DynamicAvatarColors
                                .ElementAt(new Random().Next(0, layer.DynamicAvatarColors.Count)).Id;
                        }

                        if (bundleItems.Count > 0)
                        {
                            var layerSelection = bundleItems.Where(_ =>
                            _.DynamicAvatarLayerId == layer.Id).SingleOrDefault();
                            if (layerSelection != null)
                            {
                                layer.SelectedItem = layerSelection.Id;
                            }
                        }

                        if (!layer.SelectedItem.HasValue && !layer.CanBeEmpty)
                        {
                            layer.SelectedItem = layer.DynamicAvatarItems.First().Id;
                        }

                        if (layer.SelectedItem.HasValue)
                        {
                            var filePath = _pathResolver.ResolveContentPath($"site{siteId}/dynamicavatars/");
                            var fileName = layer.SelectedItem.ToString();
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
            return layers;
        }

        public async Task<DynamicAvatarLayer> AddLayerAsync(DynamicAvatarLayer layer)
        {
            VerifyManagementPermission();
            layer.SiteId = GetCurrentSiteId();
            return await _dynamicAvatarLayerRepository.AddSaveAsync(
                GetClaimId(ClaimType.UserId), layer);
        }

        public async Task<DynamicAvatarLayer> UpdateLayerAsync(DynamicAvatarLayer layer)
        {
            VerifyManagementPermission();
            layer.SiteId = GetCurrentSiteId();
            return await _dynamicAvatarLayerRepository.UpdateSaveAsync(
                GetClaimId(ClaimType.UserId), layer);
        }

        public async Task<DynamicAvatarItem> AddItemAsync(DynamicAvatarItem item)
        {
            VerifyManagementPermission();
            return await _dynamicAvatarItemRepository.AddSaveAsync(
                GetClaimId(ClaimType.UserId), item);
        }

        public async Task<DynamicAvatarItem> UpdateItemAsync(DynamicAvatarItem item)
        {
            VerifyManagementPermission();
            return await _dynamicAvatarItemRepository.UpdateSaveAsync(
                GetClaimId(ClaimType.UserId), item);
        }

        public async Task<DynamicAvatarColor> AddColorAsync(DynamicAvatarColor color)
        {
            VerifyManagementPermission();
            return await _dynamicAvatarColorRepository.AddSaveAsync(
                GetClaimId(ClaimType.UserId), color);
        }

        public async Task<DynamicAvatarColor> UpdateColorAsync(DynamicAvatarColor color)
        {
            VerifyManagementPermission();
            return await _dynamicAvatarColorRepository.UpdateSaveAsync(
                GetClaimId(ClaimType.UserId), color);
        }

        public async Task<DynamicAvatarElement> AddElementAsync(DynamicAvatarElement element)
        {
            VerifyManagementPermission();
            return await _dynamicAvatarElementRepository.AddSaveAsync(
                GetClaimId(ClaimType.UserId), element);
        }

        public async Task AddElementListAsync(List<DynamicAvatarElement> elementList)
        {
            VerifyManagementPermission();
            var userId = GetClaimId(ClaimType.UserId);
            var count = 0;
            foreach (var element in elementList)
            {
                await _dynamicAvatarElementRepository.AddAsync(userId, element);
                count++;
                if (count % 500 == 0)
                {
                    await _dynamicAvatarElementRepository.SaveAsync();
                }
            }
            if (count % 500 != 0)
            {
                await _dynamicAvatarElementRepository.SaveAsync();
            }
        }

        public async Task<DynamicAvatarElement> UpdateElementAsync(DynamicAvatarElement element)
        {
            VerifyManagementPermission();
            return await _dynamicAvatarElementRepository.UpdateSaveAsync(
                GetClaimId(ClaimType.UserId), element);
        }

        public async Task<DynamicAvatarBundle> AddBundleAsync(DynamicAvatarBundle bundle)
        {
            VerifyManagementPermission();
            bundle.SiteId = GetCurrentSiteId();
            return await _dynamicAvatarBundleRepository.AddSaveAsync(
                GetClaimId(ClaimType.UserId), bundle);
        }

        public async Task AddBundleItemAsync(int bundleId, int itemId)
        {
            VerifyManagementPermission();
            await _dynamicAvatarBundleRepository.AddItemAsync(bundleId, itemId);
        }

        public async Task<ICollection<DynamicAvatarElement>> GetUserAvatarAsync()
        {
            return await _dynamicAvatarElementRepository.GetUserAvatarAsync(GetActiveUserId());
        }

        public async Task UpdateUserAvatarAsync(ICollection<DynamicAvatarLayer> selectionLayers)
        {
            var activeUserId = GetActiveUserId();
            var layers = await _dynamicAvatarLayerRepository.GetAllAsync(GetCurrentSiteId());
            var elementList = new List<int>();
            foreach (var layer in layers)
            {
                var selection = selectionLayers.Where(_ => _.Id == layer.Id).FirstOrDefault();
                if (selection != default(DynamicAvatarLayer))
                {
                    var element = await _dynamicAvatarElementRepository.GetByItemAndColorAsync(
                        selection.SelectedItem.Value, selection.SelectedColor);
                    if (element != default(DynamicAvatarElement) && element.DynamicAvatarItem.Unlockable == false)
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
            await _dynamicAvatarElementRepository.SetUserAvatarAsync(activeUserId, elementList);
        }
    }
}
