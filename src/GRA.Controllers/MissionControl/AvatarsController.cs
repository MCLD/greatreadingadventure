using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
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
        private const string AvatarIndex = "default avatars.json";
        private const long MaxFileSize = 100L * 1024L * 1024L;
        private readonly AvatarService _avatarService;
        private readonly JobService _jobService;
        private readonly LanguageService _languageService;
        private readonly ILogger<AvatarsController> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AvatarsController(ILogger<AvatarsController> logger,
            ServiceFacade.Controller context,
            AvatarService avatarService,
            JobService jobService,
            LanguageService languageService,
            IWebHostEnvironment webHostEnvironment)
            : base(context)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(avatarService);
            ArgumentNullException.ThrowIfNull(jobService);
            ArgumentNullException.ThrowIfNull(languageService);
            ArgumentNullException.ThrowIfNull(webHostEnvironment);

            _logger = logger;
            _avatarService = avatarService;
            _jobService = jobService;
            _languageService = languageService;
            _webHostEnvironment = webHostEnvironment;

            PageTitle = "Avatars";
        }

        public static string Area
        { get { return nameof(MissionControl); } }

        public static string Name
        { get { return "Avatars"; } }

        public async Task<IActionResult> BundleCreate()
        {
            var viewModel = new BundlesDetailViewModel
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
            ArgumentNullException.ThrowIfNull(model);
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

        [HttpPost]
        public async Task<IActionResult> BundleDelete(int id)
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
            return RedirectToAction("Bundles");
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
            ArgumentNullException.ThrowIfNull(model);
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

        public async Task<IActionResult> Bundles(bool unlockable = true, int page = 1)
        {
            var filter = new AvatarFilter(page)
            {
                Unlockable = unlockable
            };

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
                Unlockable = unlockable
            };

            PageTitle = "Avatar Bundles";
            return View(viewModel);
        }
        
        public async Task<IActionResult> ColorTexts(int? page,
            bool? textMissing)
        {
            page = page ?? 1;
            textMissing = textMissing ?? true;

            var filter = new AvatarFilter(page)
            {
                TextMissing = textMissing
            };

            var colorList = await _avatarService.PageColorsAsync(filter);

            var viewModel = new ColorTextListViewModel
            {
                AltTextMaxLength = typeof(AvatarColorText)
                .GetProperty(nameof(AvatarColorText.AltText))
                .GetCustomAttribute<MaxLengthAttribute>()
                ?.Length,
                Colors = colorList.Data,
                CurrentPage = page.Value,
                ItemCount = colorList.Count,
                ItemsPerPage = filter.Take.Value,
                TextMissing = textMissing.Value
            };
            if (viewModel.PastMaxPage)
            {
                return RedirectToRoute(
                    new
                    {
                        page = viewModel.LastPage ?? 1
                    });
            }

            viewModel.Languages = await _languageService.GetActiveAsync();

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> ColorTexts(ColorTextListViewModel model)
        {
            ArgumentNullException.ThrowIfNull(model);

            try
            {
                await _avatarService.UpdateColorTextsAsync(model.Texts);
                ShowAlertSuccess("Avatar colors updated.");
            }
            catch (GraException gex)
            {
                ShowAlertDanger(gex.Message);
            }

            return RedirectToAction(nameof(ColorTexts), new
            {
                page = model.CurrentPage,
                textMissing = model.TextMissing
            });
        }

        [HttpPost]
        public async Task<IActionResult> DecreaseItemSort(ItemsListViewModel model)
        {
            ArgumentNullException.ThrowIfNull(model);
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
        public async Task<IActionResult> DeleteItem(ItemsListViewModel model)
        {
            ArgumentNullException.ThrowIfNull(model);
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
        public async Task<IActionResult> IncreaseItemSort(ItemsListViewModel model)
        {
            ArgumentNullException.ThrowIfNull(model);
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

        public async Task<IActionResult> ItemTexts(int? page,
            bool? textMissing)
        {
            page = page ?? 1;
            textMissing = textMissing ?? true;
            var filter = new AvatarFilter(page)
            {
                TextMissing = textMissing
            };

            var itemList = await _avatarService.PageItemsAsync(filter);

            var viewModel = new ItemTextListViewModel
            {
                AltTextMaxLength = typeof(AvatarItemText)
                    .GetProperty(nameof(AvatarItemText.AltText))
                    .GetCustomAttribute<MaxLengthAttribute>()
                    ?.Length,
                CurrentPage = page.Value,
                ItemCount = itemList.Count,
                Items = itemList.Data,
                ItemsPerPage = filter.Take.Value,
                TextMissing = textMissing.Value
            };
            if (viewModel.PastMaxPage)
            {
                return RedirectToRoute(
                    new
                    {
                        page = viewModel.LastPage ?? 1
                    });
            }

            foreach (var item in viewModel.Items)
            {
                item.AvatarLayerName = await _avatarService
                    .GetDefaultLayerNameByIdAsync(item.AvatarLayerId);
                item.Thumbnail = _pathResolver.ResolveContentPath(item.Thumbnail);
            }

            viewModel.Languages = await _languageService.GetActiveAsync();

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> ItemTexts(ItemTextListViewModel model)
        {
            ArgumentNullException.ThrowIfNull(model);

            var errorList = new List<string>();

            if (model.DeleteIds != null)
            {
                var deletedIds = new List<int>();
                foreach (var itemId in model.DeleteIds)
                {
                    try
                    {
                        await _avatarService.DeleteItemAsync(itemId);
                        deletedIds.Add(itemId);
                    }
                    catch (GraException gex)
                    {
                        var item = await _avatarService.GetItemByIdAsync(itemId);
                        errorList.Add($"Unable to delete item \"{item.Name}\": {gex.Message}");
                    }
                }

                model.Texts = model.Texts.Where(_ => !deletedIds.Contains(_.AvatarItemId));
            }
            try
            {
                await _avatarService.UpdateItemTextsAsync(model.Texts);
            }
            catch (GraException gex)
            {
                errorList.Add(gex.Message);
            }

            if (errorList.Count > 0)
            {
                var issues = new StringBuilder("Unable to update avatar items:<ul>");
                foreach (var error in errorList)
                {
                    issues.Append("<li>").Append(error).AppendLine("</li>");
                }
                ShowAlertDanger(issues.ToString());
            }
            else
            {
                ShowAlertSuccess("Avatar items updated.");
            }

            return RedirectToAction(nameof(ItemTexts), new
            {
                page = model.CurrentPage,
                textMissing = model.TextMissing
            });
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

            var avatarLayerName = await _avatarService.GetDefaultLayerNameByIdAsync(id);
            PageTitle = $"Avatar Items: {avatarLayerName}";

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> SetItemAvailable(ItemsListViewModel model)
        {
            ArgumentNullException.ThrowIfNull(model);
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
        public async Task<IActionResult> SetItemUnlockable(ItemsListViewModel model)
        {
            ArgumentNullException.ThrowIfNull(model);
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
            string assetPath = _pathResolver.ResolvePrivateFilePath();

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

            return await RunImportJob(assetPath, true);
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

        private static string LocateAvatarIndexPath(string assetPath)
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
            return await RunImportJob(assetPath, false);
        }

        private async Task<IActionResult> RunImportJob(string assetPath, bool uploadedFile)
        {
            var jobToken = await _jobService.CreateJobAsync(new Job
            {
                JobType = JobType.AvatarImport,
                SerializedParameters = JsonConvert
                    .SerializeObject(new JobDetailsAvatarImport
                    {
                        AssetPath = assetPath,
                        UploadedFile = uploadedFile
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
    }
}
