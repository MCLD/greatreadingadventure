using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using GRA.Controllers.ViewModel.MissionControl.Avatar;
using GRA.Controllers.ViewModel.Shared;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
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
        private const long MaxFileSize = 100L * 1024L * 1024L;
        private const string AvatarIndex = "default avatars.json";

        private readonly ILogger<AvatarsController> _logger;
        private readonly AvatarService _avatarService;
        private readonly JobService _jobService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AvatarsController(ILogger<AvatarsController> logger,
            ServiceFacade.Controller context,
            AvatarService avatarService,
            JobService jobService,
            IWebHostEnvironment webHostEnvironment)
            : base(context)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _avatarService = avatarService ?? throw new ArgumentNullException(nameof(avatarService));
            _jobService = jobService ?? throw new ArgumentNullException(nameof(jobService));
            _webHostEnvironment = webHostEnvironment ?? throw new ArgumentNullException(nameof(webHostEnvironment));
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

            var defaultAvatarPath = Path.Combine(
                Directory.GetParent(_webHostEnvironment.WebRootPath).FullName,
                "assets",
                "defaultavatars");

            var avatarZip = _pathResolver.ResolvePrivateFilePath("avatars.zip");

            return View(new AvatarIndexViewModel
            {
                Layers = layers,
                DefaultAvatarsPresent = Directory.Exists(defaultAvatarPath),
                AvatarZipPresent = System.IO.File.Exists(avatarZip)
            });
        }

        [HttpPost]
        public async Task<IActionResult> SetupDefaultAvatars()
        {
            var layers = await _avatarService.GetLayersAsync();
            if (layers.Any())
            {
                AlertDanger = "Avatars have already been set up";
                return RedirectToAction(nameof(Index));
            }

            string assetPath = Path.Combine(
                Directory.GetParent(_webHostEnvironment.WebRootPath).FullName, "assets");

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

            return await RunImportJob(assetPath);
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
            if (paginateModel.PastMaxPage)
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

        public async Task<IActionResult> GetLayersItems(
    string type, int layerId, int selectedItemId, int bundleId, int[] selectedItemIds)
        {
            try
            {
                var model = new PreconfiguredAvatarsViewModel
                {
                    ItemPath = _pathResolver.ResolveContentPath($"site{GetCurrentSiteId()}/avatars/")
                };
                if (type == "Item")
                {
                    model.LayerItems = await _avatarService.GetUsersItemsByLayerAsync(layerId);
                    model.SelectedItemId = selectedItemId;
                    model.ItemPath = _pathResolver.ResolveContentPath($"site{GetCurrentSiteId()}/avatars/");
                    model.LayerId = layerId;
                }
                else if (type == "Color")
                {
                    model.LayerColors = await _avatarService.GetColorsByLayerAsync(layerId);
                    model.SelectedItemId = selectedItemId;
                    model.LayerId = layerId;
                }
                else
                {
                    model.Bundle = await _avatarService.GetBundleByIdAsync(bundleId, false, true);
                    model.SelectedItemIds = selectedItemIds;
                }
                Response.StatusCode = StatusCodes.Status200OK;
                return PartialView("_SlickPartial", model);
            }
            catch (GraException gex)
            {
                _logger.LogError(gex,
                    "Could not retrieve layer items for layer id {layerId}: {Message}",
                    layerId,
                    gex.Message);
                Response.StatusCode = StatusCodes.Status400BadRequest;
                return Json(new
                {
                    success = false
                });
            }
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

        public async Task<IActionResult> Bundles(string search = null, int page = 1)
        {
            var filter = new AvatarFilter(page);
            if (search == "Unlockable")
            {
                filter.Unlockable = true;
            }
            else if (search == "Preconfigured")
            {
                filter.Preconfigured = true;
            }
            else
            {
                filter.Unlockable = false;
                search = "Default";
            }
            var bundleList = await _avatarService.GetPaginatedBundleListAsync(filter);

            var paginateModel = new PaginateViewModel
            {
                ItemCount = bundleList.Count,
                CurrentPage = page,
                ItemsPerPage = filter.Take.Value
            };
            if (paginateModel.PastMaxPage)
            {
                return RedirectToRoute(
                    new
                    {
                        page = paginateModel.LastPage ?? 1
                    });
            }

            var viewModel = new BundlesListViewModel
            {
                Bundles = bundleList.Data,
                PaginateModel = paginateModel,
                Search = search
            };

            PageTitle = "Avatar Bundles";
            if (search == "Preconfigured")
            {
                return View("Preconfigured", viewModel);
            }
            return View(viewModel);
        }

        public async Task<IActionResult> BundleCreate(string search = "Default")
        {
            var viewModel = new BundlesDetailViewModel
            {
                Action = "Create",
                Search = search,
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
        public async Task<IActionResult> PreconfiguredDetails(int? bundleId = null)
        {
            var selectedIds = new List<int>();
            var mannequin = await _avatarService.GetRandomMannequinAsync();
            var allBundles = await _avatarService.GetAllPreconfiguredParentBundlesAsync();

            var viewModel = new PreconfiguredDetailsViewModel
            {
                Bundles = allBundles.Where(_ => _.Description == null).ToList(),
                ImagePath = _pathResolver.ResolveContentPath($"site{GetCurrentSiteId()}/avatars/")
            };
            if (bundleId.HasValue)
            {
                viewModel.Bundle = await _avatarService.GetBundleByIdAsync(bundleId.Value);
                viewModel.AssociatedBundleId = viewModel.Bundle.AssociatedBundleId.Value;
                viewModel.SelectedItems = await _avatarService.GetBundleItemsAsync(bundleId.Value);
                if (viewModel.SelectedItems.Count > 0)
                {
                    selectedIds = viewModel.SelectedItems.Select(_ => _.Id).ToList();
                }
                selectedIds.Add(mannequin.Id);
                viewModel.LayerGroupings = await _avatarService.GetWardrobe(selectedIds);
                viewModel.SelectedItemIds = selectedIds;
            }
            else
            {
                selectedIds.Add(mannequin.Id);
                viewModel.LayerGroupings = await _avatarService.GetWardrobe(selectedIds);
                viewModel.Bundle = new AvatarBundle();
            }
            PageTitle = !bundleId.HasValue ? "Create Preconfigured Avatar" : "Edit Preconfigured Avatar";
            viewModel.NewAvatar = !bundleId.HasValue;
            return View("PreconfiguredDetails", viewModel);
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
        public async Task<IActionResult> BundleDelete(int id, string search = "Default")
        {
            try
            {
                await _avatarService.RemoveBundleAsync(id);
                ShowAlertSuccess("Bundle successfully deleted!");
            }
            catch (GraException gex)
            {
                ShowAlertWarning("Unable to delete bundle: ", gex.Message);
            }
            return RedirectToAction(nameof(Bundles), new { search });
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

        [HttpPost]
        [RequestSizeLimit(MaxFileSize)]
        [RequestFormLimits(MultipartBodyLengthLimit = MaxFileSize)]
        public async Task<IActionResult> SetupAvatars(AvatarIndexViewModel viewModel)
        {
            var layers = await _avatarService.GetLayersAsync();
            if (layers.Any())
            {
                AlertDanger = "Avatars have already been set up";
                return RedirectToAction(nameof(Index));
            }

            if (viewModel?.UploadedFile == null)
            {
                ShowAlertDanger("You must upload an avatar .zip file.");
                ModelState.AddModelError(nameof(viewModel.UploadedFile),
                    "An avatar .zip file is required.");
                return RedirectToAction(nameof(AvatarsController.Index));
            }

            string assetPath = Path.Combine(
                Directory.GetParent(_webHostEnvironment.WebRootPath).FullName, "assets");

            if (!Directory.Exists(assetPath))
            {
                Directory.CreateDirectory(assetPath);
            }

            assetPath = Path.Combine(assetPath, "uploadedavatars");
            if (Directory.Exists(assetPath))
            {
                Directory.Delete(assetPath, true);
            }

            Directory.CreateDirectory(assetPath);

            try
            {
                using var archive = new ZipArchive(viewModel.UploadedFile.OpenReadStream());
                archive.ExtractToDirectory(assetPath);
                assetPath = LocateAvatarIndexPath(assetPath);
            }
            catch (FileNotFoundException ex)
            {
                ShowAlertDanger($"{ex.Message}: {ex.FileName} in ZIP file.");
                return RedirectToAction(nameof(AvatarsController.Index));
            }
            catch (Exception ex)
            {
                ShowAlertDanger($"Error with avatar .zip file: {ex.Message}");
                return RedirectToAction(nameof(AvatarsController.Index));
            }

            return await RunImportJob(assetPath);
        }

        [HttpPost]
        public async Task<IActionResult> SetupAvatarZip()
        {
            var layers = await _avatarService.GetLayersAsync();
            if (layers.Any())
            {
                AlertDanger = "Avatars have already been set up";
                return RedirectToAction(nameof(Index));
            }

            var avatarZip = _pathResolver.ResolvePrivateFilePath("avatars.zip");

            if (!System.IO.File.Exists(avatarZip))
            {
                AlertDanger = "The avatars.zip cannot be found in the shared/private folder.";
                return RedirectToAction(nameof(Index));
            }

            string assetPath = Path.Combine(
                Directory.GetParent(_webHostEnvironment.WebRootPath).FullName, "assets");

            if (!Directory.Exists(assetPath))
            {
                Directory.CreateDirectory(assetPath);
            }

            assetPath = Path.Combine(assetPath, "avatarzip");

            if (Directory.Exists(assetPath))
            {
                Directory.Delete(assetPath, true);
            }

            Directory.CreateDirectory(assetPath);

            try
            {
                ZipFile.ExtractToDirectory(avatarZip, assetPath);
                assetPath = LocateAvatarIndexPath(assetPath);
            }
            catch (FileNotFoundException ex)
            {
                ShowAlertDanger($"{ex.Message}: {ex.FileName} in ZIP file.");
                return RedirectToAction(nameof(AvatarsController.Index));
            }
            catch (Exception ex)
            {
                ShowAlertDanger($"Error with avatar .zip file: {ex.Message}");
                return RedirectToAction(nameof(AvatarsController.Index));
            }

            return await RunImportJob(assetPath);
        }

        private string LocateAvatarIndexPath(string assetPath)
        {
            if (!System.IO.File.Exists(Path.Combine(assetPath, AvatarIndex)))
            {
                foreach (var directory in Directory.GetDirectories(assetPath))
                {
                    if (System.IO.File.Exists(Path.Combine(directory, AvatarIndex)))
                    {
                        return directory;
                    }
                }
            }

            if (!System.IO.File.Exists(Path.Combine(assetPath, AvatarIndex)))
            {
                throw new FileNotFoundException("Not able to find avatar index file",
                    AvatarIndex);
            }

            return assetPath;
        }

        private async Task<IActionResult> RunImportJob(string assetPath)
        {
            var jobToken = await _jobService.CreateJobAsync(new Job
            {
                JobType = JobType.AvatarImport,
                SerializedParameters = JsonConvert
                    .SerializeObject(new JobDetailsAvatarImport
                    {
                        AssetPath = assetPath
                    })
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
        }

        public async Task<IActionResult> CreatePreconfiguredAvatar([FromBody]PreconfiguredDetailsViewModel model)
        {
            var bundle = new AvatarBundle
            {
                Name = model.Name,
                Description = model.Description,
                AssociatedBundleId = model.AssociatedBundleId,
            };
            if (string.IsNullOrEmpty(bundle.Name))
            {
                ModelState.AddModelError("Bundle.Name",
                    _sharedLocalizer[ErrorMessages.Field, nameof(AvatarBundle.Name)]);
            }
            if (string.IsNullOrEmpty(bundle.Description))
            {
                ModelState.AddModelError("Bundle.Description",
                    _sharedLocalizer[ErrorMessages.Field, nameof(AvatarBundle.Description)]);
            }
            if (bundle.AssociatedBundleId == 0 || model.SelectedItemIds.Count < 1)
            {
                ModelState.AddModelError("SelectedItemIds", "A bundle and items must be selected.");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    var newPreconfigured = await _avatarService.AddBundleAsync(bundle, model.SelectedItemIds);
                    ShowAlertSuccess($"Created '{newPreconfigured.Name}' pre-configured avatar.");
                    RedirectToAction(nameof(PreconfiguredDetails), new { id = newPreconfigured.Id});
                }
                catch (GraException gex)
                {
                    ShowAlertDanger(gex.Message);
                }
            }
            else
            {
                ShowAlertDanger("Could not create pre-configured avatar.");
            }
            var mannequin = await _avatarService.GetRandomMannequinAsync();
            model.SelectedItemIds.Add(mannequin.Id);
            if (model.SelectedItemIds.Count > 0)
            {
                model.LayerGroupings = await _avatarService.GetWardrobe(model.SelectedItemIds);
            }
            else
            {
                var wardrobe = await _avatarService.GetLayersAsync();

                model.LayerGroupings = wardrobe
                    .GroupBy(_ => _.GroupId)
                    .Select(_ => _.ToList())
                    .ToList();
            }
            model.Bundles = await _avatarService.GetAllPreconfiguredParentBundlesAsync();
            model.ImagePath = _pathResolver.ResolveContentPath($"site{GetCurrentSiteId()}/avatars/");
            model.Bundle = bundle;
            model.NewAvatar = true;
            PageTitle = "Create Preconfigured Avatar";
            return View("PreconfiguredDetails", model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePreconfiguredAvatar([FromBody]PreconfiguredDetailsViewModel model)
        {
            var bundle = new AvatarBundle
            {
                Id = model.BundleId,
                Name = model.Name,
                Description = model.Description,
                AssociatedBundleId = model.AssociatedBundleId,
            };
            if (string.IsNullOrEmpty(bundle.Name))
            {
                ModelState.AddModelError("Bundle.Name",
                    _sharedLocalizer[ErrorMessages.Field, nameof(AvatarBundle.Name)]);
            }
            if (string.IsNullOrEmpty(bundle.Description))
            {
                ModelState.AddModelError("Bundle.Description",
                    _sharedLocalizer[ErrorMessages.Field, nameof(AvatarBundle.Description)]);
            }
            if (bundle.AssociatedBundleId == 0 || model.SelectedItemIds.Count < 1)
            {
                ModelState.AddModelError("SelectedItemIds", "A bundle and items must be selected.");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    var newPreconfigured = await _avatarService.EditBundleAsync(bundle, model.SelectedItemIds);
                    ShowAlertSuccess($"Updated '{newPreconfigured.Name}' pre-configured avatar.");
                    RedirectToAction(nameof(PreconfiguredDetails), new { id = newPreconfigured.Id });
                }
                catch (GraException gex)
                {
                    ShowAlertDanger(gex.Message);
                }
            }
            else
            {
                ShowAlertDanger("Could not update pre-configured avatar.");
            }
            var mannequin = await _avatarService.GetRandomMannequinAsync();
            model.SelectedItemIds.Add(mannequin.Id);
            if (model.SelectedItemIds.Count > 0)
            {
                model.LayerGroupings = await _avatarService.GetWardrobe(model.SelectedItemIds);
            }
            else
            {
                var wardrobe = await _avatarService.GetLayersAsync();

                model.LayerGroupings = wardrobe
                    .GroupBy(_ => _.GroupId)
                    .Select(_ => _.ToList())
                    .ToList();
            }
            model.Bundles = await _avatarService.GetAllPreconfiguredParentBundlesAsync();
            model.ImagePath = _pathResolver.ResolveContentPath($"site{GetCurrentSiteId()}/avatars/");

            model.Bundle = bundle;
            model.NewAvatar = false;
            PageTitle = "Edit Preconfigured Avatar";
            return View("PreconfiguredDetails", model);
        }
    }
}