using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GRA.Controllers.ViewModel.MissionControl.Avatar;
using GRA.Controllers.ViewModel.Shared;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace GRA.Controllers.MissionControl
{
    [Area("MissionControl")]
    [Authorize(Policy = Policy.ManageAvatars)]
    public class AvatarsController : Base.MCController
    {
        private readonly ILogger<AvatarsController> _logger;
        private readonly AvatarService _avatarService;
        private readonly JobService _jobService;
        private readonly IHostingEnvironment _hostingEnvironment;

        public AvatarsController(ILogger<AvatarsController> logger,
            ServiceFacade.Controller context,
            AvatarService avatarService,
            IHostingEnvironment hostingEnvironment)
            : base(context)
        {
            _logger = Require.IsNotNull(logger, nameof(logger));
            _avatarService = Require.IsNotNull(avatarService, nameof(avatarService));
            _hostingEnvironment = Require.IsNotNull(hostingEnvironment, nameof(hostingEnvironment));
            PageTitle = "Avatars";
        }

        public async Task<IActionResult> Index()
        {
            var layers = await _avatarService.GetLayersAsync();
            foreach (var layer in layers)
            {
                layer.Icon = _pathResolver.ResolveContentPath(layer.Icon);

                layer.AvailableItems = await _avatarService.GetLayerAvailableItemCountAsync(
                    layer.Id);
                layer.UnavailableItems = await _avatarService.GetLayerUnavailableItemCountAsync(
                    layer.Id);
                layer.UnlockableItems = await _avatarService.GetLayerUnlockableItemCountAsync(
                    layer.Id);
            }
            return View(layers);
        }

        [HttpPost]
        public async Task<IActionResult> SetupDefaultAvatars()
        {
            var layers = await _avatarService.GetLayersAsync();
            if (layers.Any())
            {
                AlertDanger = $"Avatars have already been set up";
                return RedirectToAction(nameof(Index));
            }

            int siteId = GetCurrentSiteId();

            string assetPath = Path.Combine(
                Directory.GetParent(_hostingEnvironment.WebRootPath).FullName, "assets");

            if (!Directory.Exists(assetPath))
            {
                AlertDanger = $"Asset directory not found at: {assetPath}";
                return RedirectToAction(nameof(Index));
            }

            assetPath = Path.Combine(assetPath, "defaultavatars");
            if (!Directory.Exists(assetPath))
            {
                AlertDanger = $"Asset directory not found at: {assetPath}";
                return RedirectToAction(nameof(Index));
            }

            var jobToken = await _jobService.CreateJobAsync(new Job
            {
                JobType = JobType.AvatarImport
            });

            return View("Job", new ViewModel.MissionControl.Shared.JobViewModel
            {
                CancelUrl = Url.Action(nameof(Index)),
                JobToken = jobToken.ToString(),
                PingSeconds = 5,
                SuccessRedirectUrl = "",
                SuccessUrl = Url.Action(nameof(Index)),
                Title = "Importing avatars..."
            });

            IEnumerable<AvatarLayer> avatarList;
            var jsonPath = Path.Combine(assetPath, "default avatars.json");
            using (StreamReader file = System.IO.File.OpenText(jsonPath))
            {
                var jsonString = await file.ReadToEndAsync();
                avatarList = JsonConvert.DeserializeObject<IEnumerable<AvatarLayer>>(jsonString);
            }

            _logger.LogInformation($"Found {avatarList.Count()} AvatarLayer objects in avatar file");

            var time = _dateTimeProvider.Now;
            int totalFilesCopied = 0;
            var userId = GetId(ClaimType.UserId);

            var backgroundRoot = Path.Combine($"site{siteId}", "avatarbackgrounds");
            var backgroundPath = _pathResolver.ResolveContentFilePath(backgroundRoot);
            if (!Directory.Exists(backgroundPath))
            {
                Directory.CreateDirectory(backgroundPath);
            }
            System.IO.File.Copy(Path.Combine(assetPath, "background.png"),
                Path.Combine(backgroundPath, "background.png"));
            totalFilesCopied++;

            foreach (var layer in avatarList)
            {
                int layerFilesCopied = 0;

                var colors = layer.AvatarColors;
                var items = layer.AvatarItems;
                layer.AvatarColors = null;
                layer.AvatarItems = null;

                var addedLayer = await _avatarService.AddLayerAsync(layer);

                var layerAssetPath = Path.Combine(assetPath, addedLayer.Name);
                var destinationRoot = Path.Combine($"site{siteId}", "avatars", $"layer{addedLayer.Id}");
                var destinationPath = _pathResolver.ResolveContentFilePath(destinationRoot);
                if (!Directory.Exists(destinationPath))
                {
                    Directory.CreateDirectory(destinationPath);
                }

                addedLayer.Icon = Path.Combine(destinationRoot, "icon.png");
                System.IO.File.Copy(Path.Combine(layerAssetPath, "icon.png"),
                    Path.Combine(destinationPath, "icon.png"));

                await _avatarService.UpdateLayerAsync(addedLayer);

                if (colors != null)
                {
                    foreach (var color in colors)
                    {
                        color.AvatarLayerId = addedLayer.Id;
                        color.CreatedAt = time;
                        color.CreatedBy = userId;
                    }
                    await _avatarService.AddColorListAsync(colors);
                    colors = await _avatarService.GetColorsByLayerAsync(addedLayer.Id);
                }
                foreach (var item in items)
                {
                    item.AvatarLayerId = addedLayer.Id;
                    item.CreatedAt = time;
                    item.CreatedBy = userId;
                }
                await _avatarService.AddItemListAsync(items);
                items = await _avatarService.GetItemsByLayerAsync(addedLayer.Id);

                var elementList = new List<AvatarElement>();
                _logger.LogInformation($"Processing {items.Count} items in {addedLayer.Name}...");

                foreach (var item in items)
                {
                    var itemAssetPath = Path.Combine(layerAssetPath, item.Name);
                    var itemRoot = Path.Combine(destinationRoot, $"item{item.Id}");
                    var itemPath = Path.Combine(destinationPath, $"item{item.Id}");
                    if (!Directory.Exists(itemPath))
                    {
                        Directory.CreateDirectory(itemPath);
                    }
                    item.Thumbnail = Path.Combine(itemRoot, "thumbnail.jpg");
                    System.IO.File.Copy(Path.Combine(itemAssetPath, "thumbnail.jpg"),
                        Path.Combine(itemPath, "thumbnail.jpg"));
                    if (colors != null)
                    {
                        foreach (var color in colors)
                        {
                            var element = new AvatarElement()
                            {
                                AvatarItemId = item.Id,
                                AvatarColorId = color.Id,
                                Filename = Path.Combine(itemRoot, $"item_{color.Id}.png")
                            };
                            elementList.Add(element);
                            System.IO.File.Copy(
                                Path.Combine(itemAssetPath, $"{color.Color}.png"),
                                Path.Combine(itemPath, $"item_{color.Id}.png"));
                            layerFilesCopied++;
                        }
                    }
                    else
                    {
                        var element = new AvatarElement()
                        {
                            AvatarItemId = item.Id,
                            Filename = Path.Combine(itemRoot, "item.png")
                        };
                        elementList.Add(element);
                        System.IO.File.Copy(Path.Combine(itemAssetPath, "item.png"),
                            Path.Combine(itemPath, "item.png"));
                        layerFilesCopied++;
                    }
                }

                await _avatarService.UpdateItemListAsync(items);
                await _avatarService.AddElementListAsync(elementList);
                totalFilesCopied += layerFilesCopied;
                _logger.LogInformation($"Copied {layerFilesCopied} items for {layer.Name}");
            }
            _logger.LogInformation($"Copied {totalFilesCopied} items for all layers.");

            var bundleJsonPath = Path.Combine(assetPath, "default bundles.json");
            if (System.IO.File.Exists(bundleJsonPath))
            {
                IEnumerable<AvatarBundle> bundleList;
                using (StreamReader file = System.IO.File.OpenText(bundleJsonPath))
                {
                    var jsonString = await file.ReadToEndAsync();
                    bundleList = JsonConvert.DeserializeObject<IEnumerable<AvatarBundle>>(jsonString);
                }

                foreach (var bundle in bundleList)
                {
                    _logger.LogInformation($"Processing bundle {bundle.Name}...");
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

            string loaded = $"Default avatars added in {5} seconds.";
            _logger.LogInformation(loaded);
            ShowAlertSuccess(loaded);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Layer(int id,
            string search,
            bool available = false,
            bool unavailable = false,
            bool unlockable = false,
            int page = 1)
        {
            var computedAvailable = available;
            var computedUnavailable = unavailable;
            var computedUnlockable = unlockable;

            var filter = new AvatarFilter(page, 12)
            {
                LayerId = id,
                Search = search
            };
            if (computedAvailable)
            {
                filter.Available = true;
                computedUnavailable = false;
                computedUnlockable = false;
            }
            else if (computedUnavailable)
            {
                filter.Unavailable = true;
                computedUnlockable = false;
            }
            else if (computedUnlockable)
            {
                filter.Unlockable = true;
            }

            var itemList = await _avatarService.PageItemsAsync(filter);

            var paginateModel = new PaginateViewModel
            {
                ItemCount = itemList.Count,
                CurrentPage = page,
                ItemsPerPage = filter.Take.Value
            };
            if (paginateModel.MaxPage > 0 && paginateModel.CurrentPage > paginateModel.MaxPage)
            {
                return RedirectToRoute(
                    new
                    {
                        page = paginateModel.LastPage ?? 1
                    });
            }

            foreach (var item in itemList.Data)
            {
                item.Thumbnail = _pathResolver.ResolveContentPath(item.Thumbnail);
            }

            if (itemList.Data.Count > 0)
            {
                PageTitle = $"Avatar Items: {itemList.Data.First().AvatarLayerName}";
            }

            var viewModel = new ItemsListViewModel
            {
                Items = itemList.Data,
                PaginateModel = paginateModel,
                Id = id,
                Search = search,
                Available = computedAvailable,
                Unavailable = computedUnavailable,
                Unlockable = computedUnlockable
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> DecreaseItemSort(ItemsListViewModel model)
        {
            await _avatarService.DescreaseItemSortAsync(model.ItemId);
            return RedirectToAction(nameof(Layer), new
            {
                id = model.Id,
                search = model.Search,
                available = model.Available,
                unavailable = model.Unavailable,
                unlockable = model.Unlockable,
                page = model.PaginateModel.CurrentPage
            });
        }

        [HttpPost]
        public async Task<IActionResult> IncreaseItemSort(ItemsListViewModel model)
        {
            await _avatarService.IncreaseItemSortAsync(model.ItemId);
            return RedirectToAction(nameof(Layer), new
            {
                id = model.Id,
                search = model.Search,
                available = model.Available,
                unavailable = model.Unavailable,
                unlockable = model.Unlockable,
                page = model.PaginateModel.CurrentPage
            });
        }

        [HttpPost]
        public async Task<IActionResult> SetItemUnlockable(ItemsListViewModel model)
        {
            try
            {
                await _avatarService.SetItemUnlockableAsync(model.ItemId);
                ShowAlertSuccess("Item has been set to be unlockable.");
            }
            catch (GraException gex)
            {
                ShowAlertDanger("Unable to set item to be unlockable: ", gex);
            }
            return RedirectToAction(nameof(Layer), new
            {
                id = model.Id,
                search = model.Search,
                available = model.Available,
                unavailable = model.Unavailable,
                unlockable = model.Unlockable,
                page = model.PaginateModel.CurrentPage
            });
        }

        [HttpPost]
        public async Task<IActionResult> SetItemAvailable(ItemsListViewModel model)
        {
            try
            {
                await _avatarService.SetItemAvailableAsync(model.ItemId);
                ShowAlertSuccess("Item has been set to be available.");
            }
            catch (GraException gex)
            {
                ShowAlertDanger("Unable to set item to be available: ", gex);
            }
            return RedirectToAction(nameof(Layer), new
            {
                id = model.Id,
                search = model.Search,
                available = model.Available,
                unavailable = model.Unavailable,
                unlockable = model.Unlockable,
                page = model.PaginateModel.CurrentPage
            });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteItem(ItemsListViewModel model)
        {
            try
            {
                await _avatarService.DeleteItemAsync(model.ItemId);
                ShowAlertSuccess("Item has been deleted.");
            }
            catch (GraException gex)
            {
                ShowAlertDanger("Unable to delete item: ", gex);
            }
            return RedirectToAction(nameof(Layer), new
            {
                id = model.Id,
                search = model.Search,
                available = model.Available,
                unavailable = model.Unavailable,
                unlockable = model.Unlockable,
                page = model.PaginateModel.CurrentPage
            });
        }

        public async Task<IActionResult> Bundles(bool unlockable = true, int page = 1)
        {
            var filter = new AvatarFilter(page)
            {
                Unlockable = unlockable
            };

            var bundleList = await _avatarService.GetPaginatedBundleListAsync(filter);

            var paginateModel = new PaginateViewModel()
            {
                ItemCount = bundleList.Count,
                CurrentPage = page,
                ItemsPerPage = filter.Take.Value
            };
            if (paginateModel.MaxPage > 0 && paginateModel.CurrentPage > paginateModel.MaxPage)
            {
                return RedirectToRoute(
                    new
                    {
                        page = paginateModel.LastPage ?? 1
                    });
            }

            var viewModel = new BundlesListViewModel()
            {
                Bundles = bundleList.Data,
                PaginateModel = paginateModel,
                Unlockable = unlockable
            };

            PageTitle = "Avatar Bundles";
            return View(viewModel);
        }

        public async Task<IActionResult> BundleCreate()
        {
            var viewModel = new BundlesDetailViewModel()
            {
                Action = "Create",
                Layers = new SelectList(await _avatarService.GetLayersAsync(), "Id", "Name")
            };

            PageTitle = "Create Bundle";
            return View("BundleDetail", viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> BundleCreate(BundlesDetailViewModel model)
        {
            var itemList = new List<int>();
            if (!string.IsNullOrWhiteSpace(model.ItemsList))
            {
                itemList = model.ItemsList
                    .Split(',')
                    .Where(_ => !string.IsNullOrWhiteSpace(_))
                    .Select(int.Parse)
                    .ToList();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var bundle = await _avatarService.AddBundleAsync(model.Bundle, itemList);
                    ShowAlertSuccess($"Bundle '<strong>{bundle.Name}</strong>' successfully created!");
                    return RedirectToAction("Bundles");
                }
                catch (GraException gex)
                {
                    ShowAlertDanger("Unable to create bundle: ", gex);
                }
            }

            if (itemList.Count > 0)
            {
                model.Bundle.AvatarItems = await _avatarService.GetItemsByIdsAsync(itemList);
                foreach (var item in model.Bundle.AvatarItems)
                {
                    item.Thumbnail = _pathResolver.ResolveContentPath(item.Thumbnail);
                }
            }
            model.Layers = new SelectList(await _avatarService.GetLayersAsync(), "Id", "Name");
            PageTitle = "Create Bundle";
            return View("BundleDetail", model);
        }

        public async Task<IActionResult> BundleEdit(int id)
        {
            AvatarBundle bundle = null;
            try
            {
                bundle = await _avatarService.GetBundleByIdAsync(id);
            }
            catch (GraException gex)
            {
                ShowAlertWarning("Unable to view bundle: ", gex);
                return RedirectToAction("Bundles");
            }
            foreach (var item in bundle.AvatarItems)
            {
                item.Thumbnail = _pathResolver.ResolveContentPath(item.Thumbnail);
            }

            var viewModel = new BundlesDetailViewModel()
            {
                Bundle = bundle,
                Action = "Edit",
                ItemsList = string.Join(",", bundle.AvatarItems.Select(_ => _.Id)),
                Layers = new SelectList(await _avatarService.GetLayersAsync(), "Id", "Name")
            };
            if (bundle.CanBeUnlocked)
            {
                viewModel.TriggersAwardingBundle = await _avatarService
                    .GetTriggersAwardingBundleAsync(id);
            }

            PageTitle = "Edit Bundle";
            return View("BundleDetail", viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> BundleEdit(BundlesDetailViewModel model)
        {
            var itemList = new List<int>();
            if (!string.IsNullOrWhiteSpace(model.ItemsList))
            {
                itemList = model.ItemsList
                    .Split(',')
                    .Where(_ => !string.IsNullOrWhiteSpace(_))
                    .Select(int.Parse)
                    .ToList();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var bundle = await _avatarService.EditBundleAsync(model.Bundle, itemList);
                    ShowAlertSuccess($"Bundle '<strong>{bundle.Name}</strong>' successfully edited!");
                    return RedirectToAction("Bundles");
                }
                catch (GraException gex)
                {
                    ShowAlertDanger("Unable to edit bundle: ", gex);
                }
            }

            if (itemList.Count > 0)
            {
                model.Bundle.AvatarItems = await _avatarService.GetItemsByIdsAsync(itemList);
                foreach (var item in model.Bundle.AvatarItems)
                {
                    item.Thumbnail = _pathResolver.ResolveContentPath(item.Thumbnail);
                }
            }
            model.Layers = new SelectList(await _avatarService.GetLayersAsync(), "Id", "Name");
            PageTitle = "Edit Bundle";
            return View("BundleDetail", model);
        }

        [HttpPost]
        public async Task<IActionResult> BundleDelete(int id)
        {
            try
            {
                await _avatarService.RemoveBundleAsync(id);
                ShowAlertSuccess($"Bundle successfully deleted!");
            }
            catch (GraException gex)
            {
                ShowAlertWarning("Unable to delete bundle: ", gex.Message);
            }
            return RedirectToAction("Bundles");
        }

        public async Task<IActionResult> GetItemsList(string itemIds,
            int? layerId,
            string search,
            bool canBeUnlocked,
            int page = 1)
        {
            var filter = new AvatarFilter(page, 10)
            {
                Search = search,
                LayerId = layerId,
                CanBeUnlocked = canBeUnlocked
            };

            if (!string.IsNullOrWhiteSpace(itemIds))
            {
                filter.ItemIds = itemIds.Split(',')
                    .Where(_ => !string.IsNullOrWhiteSpace(_))
                    .Select(int.Parse)
                    .ToList();
            }

            var items = await _avatarService.PageItemsAsync(filter);
            var paginateModel = new PaginateViewModel
            {
                ItemCount = items.Count,
                CurrentPage = page,
                ItemsPerPage = filter.Take.Value
            };

            foreach (var item in items.Data)
            {
                if (!string.IsNullOrWhiteSpace(item.Thumbnail))
                {
                    item.Thumbnail = _pathResolver.ResolveContentPath(item.Thumbnail);
                }
            }

            var viewModel = new ItemsListViewModel
            {
                Items = items.Data,
                PaginateModel = paginateModel
            };

            return PartialView("_ItemsPartial", viewModel);
        }
    }
}