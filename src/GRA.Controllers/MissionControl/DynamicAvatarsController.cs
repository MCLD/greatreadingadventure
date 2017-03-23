using GRA.Controllers.ViewModel.MissionControl.DynamicAvatars;
using GRA.Controllers.ViewModel.Shared;
using GRA.Domain.Model;
using GRA.Domain.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace GRA.Controllers.MissionControl
{
    [Area("MissionControl")]
    [Authorize(Policy = Policy.ManageAvatars)]
    public class DynamicAvatarsController : Base.MCController
    {
        private readonly ILogger<DynamicAvatarsController> _logger;
        private readonly DynamicAvatarService _avatarService;
        private readonly SiteService _siteService;
        public DynamicAvatarsController(ILogger<DynamicAvatarsController> logger,
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
    }
}