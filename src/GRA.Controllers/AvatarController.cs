using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using GRA.Controllers.ViewModel.Avatar;
using GRA.Domain.Model;
using GRA.Domain.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.Primitives;

namespace GRA.Controllers
{
    [Authorize]
    public class AvatarController : Base.UserController
    {
        private readonly ILogger<AvatarController> _logger;
        private readonly AutoMapper.IMapper _mapper;
        private readonly AvatarService _avatarService;
        private readonly SiteService _siteService;

        public static string Name { get { return "Avatar"; } }

        public AvatarController(ILogger<AvatarController> logger,
            ServiceFacade.Controller context,
            AvatarService avatarService,
            SiteService siteService)
            : base(context)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = context.Mapper;
            _avatarService = avatarService
                ?? throw new ArgumentNullException(nameof(avatarService));
            _siteService = siteService ?? throw new ArgumentNullException(nameof(siteService));
            PageTitle = _sharedLocalizer[Annotations.Title.Avatar];
        }

        public async Task<IActionResult> Index()
        {
            var userWardrobe = await _avatarService.GetUserWardrobeAsync();
            if (userWardrobe.Count == 0)
            {
                ShowAlertDanger("Avatars have not been configured.");
                _logger.LogError("User {id} tried to customize their avatar but avatars have not been configured!",
                    GetActiveUserId());
                return RedirectToAction(nameof(HomeController.Index), HomeController.Name);
            }

            var layerGroupings = userWardrobe
                .GroupBy(_ => _.GroupId)
                .Select(_ => _.ToList())
                .ToList();

            var usersresult = await _avatarService.GetUserUnlockBundlesAsync();
            var viewModel = new AvatarViewModel
            {
                LayerGroupings = layerGroupings,
                Bundles = usersresult,
                DefaultLayer = userWardrobe.First(_ => _.DefaultLayer).Id,
                ImagePath = _pathResolver.ResolveContentPath($"site{GetCurrentSiteId()}/avatars/")
            };
            var currentCulture = _userContextProvider.GetCurrentCulture();
            viewModel.UseEnglish = currentCulture.Name == Culture.EnglishUS;
            var userAvatar = await _avatarService.GetUserAvatarAsync();
            viewModel.NewAvatar = userAvatar.Count == 0;
            return View(viewModel);
        }

        [HttpPost]
        public async Task<JsonResult> SaveAvatar(string selectionJson)
        {
            try
            {
                await UpdateAvatarAsync(selectionJson);
                return Json(new { success = true });
            }
            catch (GraException gex)
            {
                return Json(new { success = false, message = gex.Message });
            }
        }

        [HttpPost]
        public async Task<JsonResult> UpdateBundleHasBeenViewed(int bundleId)
        {
            try
            {
                await _avatarService.UpdateBundleHasBeenViewedAsync(bundleId);
                return Json(new { success = true });
            }
            catch (GraException gex)
            {
                return Json(new { success = false, message = gex.Message });
            }
        }

        public async Task<IActionResult> Share()
        {
            PageTitle = _sharedLocalizer[Annotations.Title.ShareYourAvatar];
            var userAvatar = await _avatarService.GetUserAvatarAsync();
            if (userAvatar?.Count > 0)
            {
                var site = await GetCurrentSiteAsync();
                var directory = _pathResolver
                        .ResolveContentFilePath($"site{site.Id}/useravatars/");
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                var viewModel = new ShareViewModel();
                userAvatar = userAvatar.OrderBy(_ => _.AvatarItem.AvatarLayerPosition).ToList();
                using (var md5er = MD5.Create())
                {
                    var avatarBytes = userAvatar.Select(_ => _.Id)
                            .SelectMany(BitConverter.GetBytes)
                            .ToArray();
                    var encoded = md5er.ComputeHash(avatarBytes);
                    var filename = Convert.ToBase64String(encoded).TrimEnd('=').Replace('+', '-')
                        .Replace('/', '-');
                    viewModel.AvatarId = filename;
                    var path = $"site{site.Id}/useravatars/{filename}.png";
                    var filePath = _pathResolver.ResolveContentFilePath(path);
                    if (!System.IO.File.Exists(filePath))
                    {
                        using (var image = new Image<Rgba32>(1200, 630))
                        {
                            var background = _pathResolver
                                .ResolveContentFilePath($"site{site.Id}/avatarbackgrounds/background.png");
                            image.Mutate(_ => _.DrawImage(Image.Load(background), 1));

                            var avatarPoint = new Point(660, 60);
                            foreach (var element in userAvatar)
                            {
                                var file = _pathResolver.ResolveContentFilePath(element.Filename);
                                image.Mutate(_ => _.DrawImage(Image.Load(file), 1, avatarPoint));
                            }

                            image.Save(filePath);
                        }
                    }
                    var siteUrl = await _siteService.GetBaseUrl(Request.Scheme,
                                Request.Host.Value);
                    var contentPath = _pathResolver.ResolveContentPath(path);
                    viewModel.AvatarImageUrl = Path.Combine(siteUrl, contentPath)
                        .Replace("\\", "/");

                    var shareUrl = siteUrl + Url.Action(nameof(ShareController.Avatar), "Share")
                        + $"/{filename}";
                    var facebookShareUrl = $"https://www.facebook.com/sharer/sharer.php?u={shareUrl}";
                    var twitterShareUrl = $"https://twitter.com/intent/tweet?url={shareUrl}";
                    if (!string.IsNullOrWhiteSpace(site.TwitterAvatarMessage))
                    {
                        twitterShareUrl += $"&text={site.TwitterAvatarMessage}";
                    }
                    if (!string.IsNullOrWhiteSpace(site.TwitterAvatarHashtags))
                    {
                        twitterShareUrl += $"&hashtags={site.TwitterAvatarHashtags}";
                    }
                    if (!string.IsNullOrWhiteSpace(site.TwitterUsername))
                    {
                        twitterShareUrl += $"&via{site.TwitterUsername}";
                    }
                    viewModel.FacebookShareUrl = facebookShareUrl;
                    viewModel.TwitterShareUrl = twitterShareUrl;
                }
                return View(viewModel);
            }
            TempData[TempDataKey.AlertDanger]
                = _sharedLocalizer[Annotations.Validate.CustomizeAvatarFirst];
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Share(string selectionJson)
        {
            if (!string.IsNullOrWhiteSpace(selectionJson))
            {
                try
                {
                    await UpdateAvatarAsync(selectionJson);
                    ShowAlertSuccess(_sharedLocalizer[Annotations.Interface.AvatarSaved]);
                }
                catch (GraException gex)
                {
                    _logger.LogError(gex,
                        "Could not save avatar for sharing: {Message}",
                        gex.Message);
                    ShowAlertDanger(_sharedLocalizer[Annotations.Validate.CouldNotSaveAvatarReason,
                        gex.Message]);
                }
            }
            return RedirectToAction(nameof(Share));
        }

        public async Task<IActionResult> GetLayersItems(
            string type, int layerId, int selectedItemId, int bundleId, int[] selectedItemIds)
        {
            bool user = !AuthUser.Identity.IsAuthenticated;
            if (user)
            {
                ShowAlertDanger(_sharedLocalizer[Annotations.Validate.SessionExpired].ToString());
                return NotFound();
            }
            try
            {
                var layeritems = await _avatarService.GetItemsByLayerAsync(layerId);
                var model = new AvatarViewModel
                {
                    ItemPath = _pathResolver.ResolveContentPath($"site{GetCurrentSiteId()}/avatars/")
                };
                if (type == "Item")
                {

                    model.LayerItems = layeritems;
                    model.SelectedItemId = selectedItemId;
                    model.ItemPath = _pathResolver.ResolveContentPath($"site{GetCurrentSiteId()}/avatars/");
                    model.LayerId = layerId;
                }
                else if(type == "Color")
                {
                    model.LayerColors = await _avatarService.GetColorsByLayerAsync(layerId);
                    model.SelectedItemId = selectedItemId;
                    model.LayerId = layerId;
                }
                else
                {
                    model.Bundle = await _avatarService.GetBundleByIdAsync(bundleId);
                    model.SelectedItemIds = selectedItemIds;
                }
                return PartialView("_SlickPartial", model);
            }
            catch(GraException gex)
            {
                _logger.LogError(gex,
                    "Could not retrieve layer items for layer id {layerId}: {Message}",
                    layerId,
                    gex.Message);
                ShowAlertDanger(_sharedLocalizer[Annotations.Validate.SomethingWentWrong].ToString());
                return NotFound();
            }
        }

        private async Task UpdateAvatarAsync(string selectionJson)
        {
            var selection = Newtonsoft.Json.JsonConvert
                        .DeserializeObject<ICollection<AvatarLayer>>(selectionJson);
            selection = selection.Where(_ => _.SelectedItem.HasValue).ToList();
            await _avatarService.UpdateUserAvatarAsync(selection);
        }

        public IActionResult InstagramImage(string id)
        {
            var siteId = GetCurrentSiteId();

            var igFilePath = _pathResolver.ResolveContentFilePath($"site{siteId}/igavatars/{id}.png");
            if (System.IO.File.Exists(igFilePath))
            {
                var imageBytes = System.IO.File.ReadAllBytes(igFilePath);
                return File(imageBytes, "image/png");
            }

            var avatarPath = _pathResolver.ResolveContentFilePath($"site{siteId}/useravatars/{id}.png");
            if (System.IO.File.Exists(avatarPath))
            {
                var directory = _pathResolver.ResolveContentFilePath($"site{siteId}/igavatars/");
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                using (var image = Image.Load(avatarPath))
                {
                    image.Mutate(_ => _.Resize(1080, 567));
                    image.Mutate(_ => _.Crop(new Rectangle(0, 0, 1080, 566)));
                    image.Save(igFilePath);
                }
                var imageBytes = System.IO.File.ReadAllBytes(igFilePath);
                return File(imageBytes, "image/png");
            }

            TempData[TempDataKey.AlertDanger]
                = _sharedLocalizer[Annotations.Validate.CustomizeAvatarFirst];
            return RedirectToAction(nameof(Index));
        }
    }
}
