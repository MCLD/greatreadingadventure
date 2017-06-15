using GRA.Controllers.ViewModel.MissionControl.Avatar;
using GRA.Controllers.ViewModel.Shared;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace GRA.Controllers.MissionControl
{

    [Area("MissionControl")]
    [Authorize(Policy = Policy.ManageAvatars)]
    public class AvatarsController : Base.MCController
    {
        private readonly ILogger<AvatarsController> _logger;
        private readonly DynamicAvatarService _avatarService;
        private readonly SiteService _siteService;
        public AvatarsController(ILogger<AvatarsController> logger,
            ServiceFacade.Controller context,
            DynamicAvatarService avatarService,
            SiteService siteService)
            : base(context)
        {
            _logger = Require.IsNotNull(logger, nameof(logger));
            _avatarService = Require.IsNotNull(avatarService, nameof(avatarService));
            _siteService = Require.IsNotNull(siteService, nameof(SiteService));

            PageTitle = "Avatars";
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
                model.Bundle.DynamicAvatarItems = await _avatarService.GetItemsByIdsAsync(itemList);
                foreach (var item in model.Bundle.DynamicAvatarItems)
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
            var bundle = new Domain.Model.DynamicAvatarBundle();
            try
            {
                bundle = await _avatarService.GetBundleByIdAsync(id);
            }
            catch (GraException gex)
            {
                ShowAlertWarning("Unable to view bundle: ", gex);
                return RedirectToAction("Bundles");
            }
            foreach (var item in bundle.DynamicAvatarItems)
            {
                item.Thumbnail = _pathResolver.ResolveContentPath(item.Thumbnail);
            }

            var viewModel = new BundlesDetailViewModel()
            {
                Bundle = bundle,
                Action = "Edit",
                ItemsList = string.Join(",", bundle.DynamicAvatarItems.Select(_ => _.Id)),
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
                model.Bundle.DynamicAvatarItems = await _avatarService.GetItemsByIdsAsync(itemList);
                foreach (var item in model.Bundle.DynamicAvatarItems)
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
            bool unlockable,
            int page = 1)
        {
            var filter = new AvatarFilter(page, 10)
            {
                Search = search,
                LayerId = layerId,
                Unlockable = unlockable
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

        /*
        public async Task<IActionResult> Index(string Search, int page = 1)
        {
            var viewModel = await GetAvatarList(Search, page);

            if (viewModel.PaginateModel.MaxPage > 0
                && viewModel.PaginateModel.CurrentPage > viewModel.PaginateModel.MaxPage)
            {
                return RedirectToRoute(
                    new
                    {
                        page = viewModel.PaginateModel.LastPage ?? 1
                    });
            }

            return View("Index", viewModel);
        }

        private async Task<AvatarsListViewModel> GetAvatarList(string search, int page)
        {
            int take = 15;
            int skip = take * (page - 1);

            var avatarList = await _avatarService.GetPaginatedAvatarListAsync(skip, take, search);

            PaginateViewModel paginateModel = new PaginateViewModel()
            {
                ItemCount = avatarList.Count(),
                CurrentPage = page,
                ItemsPerPage = take
            };

            var systemList = (await _siteService.GetSystemList())
                .OrderByDescending(_ => _.Id == GetId(ClaimType.SystemId)).ThenBy(_ => _.Name);

            AvatarsListViewModel viewModel = new AvatarsListViewModel()
            {
                Avatars = avatarList,
                PaginateModel = paginateModel,
                Search = search,
                CanAddAvatars = true,
                CanDeleteAvatars = true,
                CanEditAvatars = true,
                SystemList = systemList
            };

            return viewModel;
        }

        public async Task<IActionResult> Create(int? id)
        {
            PageTitle = "Create Avatar";

            var viewModel = new AvatarsDetailViewModel();

            if (id.HasValue)
            {
                var graAvatar = await _avatarService.GetAvatarDetailsAsync(id.Value);
                viewModel.Avatar = graAvatar;
            }

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(AvatarsDetailViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var graAvatar = viewModel.Avatar;

                    await _avatarService.AddAvatarAsync(graAvatar);
                    ShowAlertSuccess($"Avatar '{graAvatar.Name}' created.");
                    return RedirectToAction("Index");
                }
                catch (GraException gex)
                {
                    ShowAlertWarning("Could not create Avatar: ", gex.Message);
                }
            }

            PageTitle = "Create Avatar";
            return View(viewModel);
        }

        public async Task<IActionResult> Edit(int id)
        {
            PageTitle = "Edit Avatar";

            var avatarRoot = Path.Combine($"site{GetCurrentSiteId()}", "dynamicavatars");

            var avatar = await _avatarService.GetAvatarDetailsAsync(id);
            var layerList = await _avatarService.GetAllLayersAsync();

            var elementViewModels = new List<AvatarsElementDetailViewModel>();

            foreach (var layer in layerList)
            {
                var element = avatar.Elements.Where(_ => _.DynamicAvatarLayerId == layer.Id).FirstOrDefault();

                var newElement = new AvatarsElementDetailViewModel()
                {
                    AvatarId = avatar.Id,
                    Element = element,
                    Create = element == null,
                    Layer = layer,
                    BaseAvatarUrl = avatarRoot
                };
                elementViewModels.Add(newElement);
            }

            var viewModel = new AvatarsDetailViewModel()
            {
                Avatar = avatar,
                Elements = elementViewModels
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(AvatarsDetailViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var graAvatar = viewModel.Avatar;

                    await _avatarService.EditAvatarAsync(graAvatar);
                    ShowAlertSuccess($"Avatar '{graAvatar.Name}' edited.");
                    return RedirectToAction("Index");
                }
                catch (GraException gex)
                {
                    ShowAlertWarning("Could not edit avatar: ", gex.Message);
                }
            }
            PageTitle = "Edit Avatar";
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                // delete all avatar files
                var avatar = await _avatarService.GetAvatarDetailsAsync(id);

                foreach (var element in avatar.Elements)
                {
                    _avatarService.DeleteElementFile(element);
                }

                // remove from database
                await _avatarService.RemoveAvatarAsync(id);
                ShowAlertSuccess("Avatar deleted.");
            }
            catch (GraException gex)
            {
                ShowAlertWarning("Unable to delete avatar: ", gex.Message);
            }

            return RedirectToAction("Index");
        }


        [HttpPost]
        public async Task<IActionResult> EditElement(AvatarsElementDetailViewModel viewModel)
        {
            var element = viewModel.Element;
            element.DynamicAvatarId = viewModel.AvatarId;
            element.DynamicAvatarLayerId = viewModel.Layer.Id;

            if (viewModel.Create || element == null)
            {
                element = await _avatarService.AddElementAsync(element);
            }
            else
            {
                element = await _avatarService.EditElementAsync(element);
            }

            if (viewModel.UploadImage != null)
            {
                byte[] avatarBytes;

                using (var fileStream = viewModel.UploadImage.OpenReadStream())
                {
                    using (var ms = new MemoryStream())
                    {
                        fileStream.CopyTo(ms);
                        avatarBytes = ms.ToArray();
                    }
                }

                _avatarService.WriteElementFile(element, avatarBytes);
            }

            return RedirectToAction("Edit", new { id = viewModel.AvatarId });
        }
        */
    }
}