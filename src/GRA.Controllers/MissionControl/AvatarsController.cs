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
        private const string AvatarExportDirectory = "avatar-export";
        private const string AvatarImportDirectory = "avatar-import";
        private const string AvatarIndexV1 = "default avatars.json";
        private const string AvatarIndexV2 = "avatars.json";
        private const string DefaultAvatars = "defaultavatars";
        private const string DirAssets = "assets";
        private const long MaxFileSize = 100L * 1024L * 1024L;
        private const string ZipAvatars = "avatars.zip";
        private readonly AvatarService _avatarService;
        private readonly AvatarTransferService _avatarTransferService;
        private readonly JobService _jobService;
        private readonly LanguageService _languageService;
        private readonly ILogger<AvatarsController> _logger;
        private readonly UserService _userService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AvatarsController(ILogger<AvatarsController> logger,
            AvatarService avatarService,
            AvatarTransferService avatarTransferService,
            IWebHostEnvironment webHostEnvironment,
            JobService jobService,
            LanguageService languageService,
            ServiceFacade.Controller context,
            UserService userService)
            : base(context)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(avatarTransferService);
            ArgumentNullException.ThrowIfNull(avatarService);
            ArgumentNullException.ThrowIfNull(jobService);
            ArgumentNullException.ThrowIfNull(languageService);
            ArgumentNullException.ThrowIfNull(webHostEnvironment);
            ArgumentNullException.ThrowIfNull(userService);

            _avatarService = avatarService;
            _avatarTransferService = avatarTransferService;
            _jobService = jobService;
            _languageService = languageService;
            _logger = logger;
            _userService = userService;
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
                itemList = [.. model.ItemsList
                    .Split(',')
                    .Where(_ => !string.IsNullOrWhiteSpace(_))
                    .Select(int.Parse)];
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
                itemList = [.. model.ItemsList
                    .Split(',')
                    .Where(_ => !string.IsNullOrWhiteSpace(_))
                    .Select(int.Parse)];
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

        public async Task<IActionResult> ColorTexts(int? page, bool? textMissing, int? language)
        {
            page ??= 1;
            textMissing ??= true;

            var filter = new AvatarFilter(page)
            {
                LanguageId = language,
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
            if (language.HasValue)
            {
                viewModel.SelectedLanguage = viewModel.Languages
                    .FirstOrDefault(_ => _.Id == language.Value);
            }

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
                textMissing = model.TextMissing,
                language = model.SelectedLanguage?.Id,
                page = model.CurrentPage
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

        [HttpPost]
        public async Task<IActionResult> GenerateExport()
        {
            var jobToken = await _jobService.CreateJobAsync(new Job
            {
                JobType = JobType.AvatarExport,
                SerializedParameters = JsonConvert
                    .SerializeObject(new JobDetailsAvatarTransfer
                    {
                        AssetPath = _pathResolver.ResolvePrivateFilePath(AvatarExportDirectory),
                        TransferType = DataTransferType.Export
                    })
            });

            return View("Job", new ViewModel.MissionControl.Shared.JobViewModel
            {
                CancelUrl = Url.Action(nameof(Transfers)),
                JobToken = jobToken.ToString(),
                PingSeconds = 5,
                SuccessUrl = Url.Action(nameof(Transfers)),
                Title = "Starting avatar export"
            });
        }

        [HttpGet]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability",
            "CA2000:Dispose objects before losing scope",
            Justification = "File() method calls Dispose for us")]
        public async Task<IActionResult> GetExport(int id)
        {
            var export = await _avatarTransferService.GetExportAsync(id);
            var path = _pathResolver
                .ResolvePrivateFilePath(Path.Combine(AvatarExportDirectory, export.Filename));
            return File(new FileStream(path, FileMode.Open), "application/zip", export.Filename);
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
                filter.ItemIds = [.. itemIds.Split(',')
                    .Where(_ => !string.IsNullOrWhiteSpace(_))
                    .Select(int.Parse)];
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

            if (layers?.Count() > 1)
            {
                foreach (var layer in layers)
                {
                    layer.Icon = _pathResolver.ResolveContentPath(layer.Icon);

                    layer.AvailableItems = await _avatarService
                        .GetLayerAvailableItemCountAsync(layer.Id);
                    layer.UnavailableItems = await _avatarService
                        .GetLayerUnavailableItemCountAsync(layer.Id);
                    layer.UnlockableItems = await _avatarService
                        .GetLayerUnlockableItemCountAsync(layer.Id);
                }

                return View(new AvatarIndexViewModel
                {
                    Layers = layers,
                });
            }
            else
            {
                return RedirectToAction(nameof(Transfers));
            }
        }

        public async Task<IActionResult> ItemTexts(int? page,
            bool? textMissing,
            int? language)
        {
            page ??= 1;
            textMissing ??= true;

            var filter = new AvatarFilter(page)
            {
                LanguageId = language,
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
            if (language.HasValue)
            {
                viewModel.SelectedLanguage = viewModel.Languages
                    .FirstOrDefault(_ => _.Id == language.Value);
            }

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
                textMissing = model.TextMissing,
                language = model.SelectedLanguage?.Id,
                page = model.CurrentPage
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
        public async Task<IActionResult> SetupAvatars(TransfersViewModel viewModel)
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

            assetPath = Path.Combine(assetPath, AvatarImportDirectory);
            if (Directory.Exists(assetPath))
            {
                Directory.Delete(assetPath, true);
            }

            Directory.CreateDirectory(assetPath);

            try
            {
                using var archive = new ZipArchive(viewModel.UploadedFile.OpenReadStream());
                archive.ExtractToDirectory(assetPath);
            }
            catch (Exception ex) when (ex is SystemException || ex is FileNotFoundException)
            {
                ShowAlertDanger($"Error with avatar .zip file: {ex.Message}");
                return RedirectToAction(nameof(AvatarsController.Index));
            }

            var avatarIndex = FindAvatarIndex(assetPath);

            if (avatarIndex == null)
            {
                ShowAlertDanger("Could not find avatar index in ZIP file.");
                return RedirectToAction(nameof(AvatarsController.Transfers));
            }

            return await RunImportJob(avatarIndex,
                true,
                viewModel.UploadedFile.FileName,
                viewModel.UploadedFile.Length / 1024);
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

            var avatarZip = _pathResolver.ResolvePrivateFilePath(ZipAvatars);

            if (!System.IO.File.Exists(avatarZip))
            {
                AlertDanger = "The avatars.zip cannot be found in the shared/private folder.";
                return RedirectToAction(nameof(Index));
            }

            string assetPath = Path.Combine(
                Directory.GetParent(_webHostEnvironment.WebRootPath).FullName, DirAssets);

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
            }
            catch (Exception ex) when (ex is SystemException || ex is FileNotFoundException)
            {
                ShowAlertDanger($"Error with avatar .zip file: {ex.Message}");
                return RedirectToAction(nameof(AvatarsController.Index));
            }

            var avatarIndex = FindAvatarIndex(assetPath);

            if (avatarIndex == null)
            {
                ShowAlertDanger("Could not find avatar index in ZIP file.");
                return RedirectToAction(nameof(AvatarsController.Transfers));
            }

            return await RunImportJob(avatarIndex,
                false,
                Path.GetFileName(avatarZip),
                new FileInfo(avatarZip).Length / 1024);
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
                Directory.GetParent(_webHostEnvironment.WebRootPath).FullName, DirAssets);

            if (!Directory.Exists(assetPath))
            {
                AlertDanger = $"Asset directory not found at: {assetPath}";
                return RedirectToAction(nameof(Index));
            }

            assetPath = Path.Combine(assetPath, DefaultAvatars);
            if (!Directory.Exists(assetPath))
            {
                AlertDanger = $"Asset directory not found at: {assetPath}";
                return RedirectToAction(nameof(Index));
            }

            var avatarIndex = FindAvatarIndex(assetPath);

            if (avatarIndex == null)
            {
                ShowAlertDanger("Could not find avatar index in ZIP file.");
                return RedirectToAction(nameof(AvatarsController.Transfers));
            }

            return await RunImportJob(avatarIndex, false, DefaultAvatars, null);
        }

        [HttpGet]
        public async Task<IActionResult> Transfers()
        {
            var defaultAvatarPath = Path.Combine(
                Directory.GetParent(_webHostEnvironment.WebRootPath).FullName,
                DirAssets,
                DefaultAvatars);

            var avatarZip = _pathResolver.ResolvePrivateFilePath(ZipAvatars);

            var layers = await _avatarService.GetLayersAsync();
            var viewModel = new TransfersViewModel
            {
                LayersPresent = layers?.Count() > 0,
                DefaultAvatarsPresent = Directory.Exists(defaultAvatarPath),
                AvatarZipPresent = System.IO.File.Exists(avatarZip)
            };

            ((List<AvatarTransfer>)viewModel.Transfers)
                .AddRange(await _avatarTransferService.GetTransfers());

            foreach (var transfer in viewModel.Transfers)
            {
                transfer.CreatedByName
                    = await _userService.GetUsersNameByIdAsync(transfer.CreatedBy);

                if (!viewModel.Jobs.ContainsKey(transfer.JobId))
                {
                    viewModel.Jobs.Add(transfer.JobId,
                        await _jobService.GetJobAsync(transfer.JobId));
                }

                var path = _pathResolver.ResolvePrivateFilePath(Path.Combine(AvatarExportDirectory,
                    transfer.Filename));

                if (transfer.TransferType == DataTransferType.Export && System.IO.File.Exists(path))
                {
                    transfer.FileKBytes = new FileInfo(path).Length / 1024;
                }
            }

            PageTitle = "Avatar Transfers";
            return View(viewModel);
        }

        private static AvatarIndex FindAvatarIndex(string path)
        {
            var avatarIndex = IndexVersion(path);

            if (avatarIndex == null)
            {
                foreach (var directory in Directory.GetDirectories(path))
                {
                    avatarIndex = IndexVersion(directory);
                    if (avatarIndex != null)
                    {
                        break;
                    }
                }
            }

            return avatarIndex;
        }

        private static AvatarIndex IndexVersion(string path)
        {
            return System.IO.File.Exists(Path.Join(path, AvatarIndexV2))
                ? new AvatarIndex { Version = 2, Path = path }
                : System.IO.File.Exists(Path.Combine(path, AvatarIndexV1))
                    ? new AvatarIndex { Version = 1, Path = path }
                    : null;
        }

        private async Task<IActionResult> RunImportJob(AvatarIndex avatarIndex,
            bool uploadedFile,
            string fileName,
            long? fileSize)
        {
            var jobToken = await _jobService.CreateJobAsync(new Job
            {
                JobType = JobType.AvatarImport,
                SerializedParameters = JsonConvert
                    .SerializeObject(new JobDetailsAvatarTransfer
                    {
                        AssetPath = avatarIndex.Path,
                        Filename = fileName,
                        Filesize = fileSize,
                        TransferType = DataTransferType.Import,
                        UploadedFile = uploadedFile,
                        Version = avatarIndex.Version
                    })
            });

            return View("Job", new ViewModel.MissionControl.Shared.JobViewModel
            {
                CancelUrl = Url.Action(nameof(Transfers)),
                JobToken = jobToken.ToString(),
                PingSeconds = 5,
                SuccessRedirectUrl = "",
                SuccessUrl = Url.Action(nameof(Transfers)),
                Title = "Starting avatar import"
            });
        }

        private class AvatarIndex
        {
            public string Path { get; set; }
            public int Version { get; set; }
        }
    }
}
