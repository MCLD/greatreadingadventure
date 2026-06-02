using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using GRA.Controllers.Attributes;
using GRA.Controllers.ViewModel.Avatar;
using GRA.Domain.Model;
using GRA.Domain.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace GRA.Controllers
{
    [Authorize]
    public class AvatarController : Base.UserController
    {
        public static readonly string PathUserAvatars = "useravatars";
        private const string PathAvatars = "avatars";
        private const string PathIgAvatars = "igavatars";
        private readonly AvatarService _avatarService;
        private readonly ILogger<AvatarController> _logger;
        private readonly SiteService _siteService;

        public AvatarController(ILogger<AvatarController> logger,
            ServiceFacade.Controller context,
            AvatarService avatarService,
            SiteService siteService)
            : base(context)
        {
            ArgumentNullException.ThrowIfNull(avatarService);
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(siteService);

            _avatarService = avatarService;
            _logger = logger;
            _siteService = siteService;

            PageTitle = _sharedLocalizer[Annotations.Title.Avatar];
        }

        public static string Name
        { get { return "Avatar"; } }

        public async Task<IActionResult> Download(string id)
        {
            if (await GetSiteSettingBoolAsync(SiteSettingKey.Avatars.DisableSharing))
            {
                return RedirectToAction(nameof(Index));
            }

            var siteId = GetCurrentSiteId();

            // check if the file exists first
            var igFilePath = _pathResolver
                .ResolveContentFilePath(Path.Combine($"site{siteId}", PathIgAvatars, $"{id}.png"));
            if (!System.IO.File.Exists(igFilePath))
            {
                // if not check if the user has a saved avatar
                var avatarPath = _pathResolver.ResolveContentFilePath(Path.Combine(
                    $"site{siteId}",
                    PathUserAvatars,
                    $"{id}.png"));
                if (!System.IO.File.Exists(avatarPath))
                {
                    TempData[TempDataKey.AlertDanger]
                        = _sharedLocalizer[Annotations.Validate.CustomizeAvatarFirst];
                    return RedirectToAction(nameof(Index));
                }

                var directory = _pathResolver
                    .ResolveContentFilePath(Path.Combine($"site{siteId}", PathIgAvatars));
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                // create an image with the preferred dimensions for a landscape Instagram post
                // https://www.adobe.com/express/discover/sizes/instagram

                var timer = Stopwatch.StartNew();
                using var image = await Image.LoadAsync(avatarPath);
                image.Mutate(_ => _.Resize(1080, 567));
                image.Mutate(_ => _.Crop(new Rectangle(0, 0, 1080, 566)));
                await image.SaveAsPngAsync(igFilePath, new PngEncoder
                {
                    CompressionLevel = PngCompressionLevel.BestCompression
                });
                _logger.LogInformation(
                    "Generated avatar share {Type} id {Id} ({Size:F2} kb) in {ElapsedMs} ms",
                    "export",
                    id,
                    new FileInfo(igFilePath).Length / 1024,
                    timer.ElapsedMilliseconds);
            }

            return File(await System.IO.File.ReadAllBytesAsync(igFilePath),
                MediaTypeNames.Image.Png);
        }

        [PreventAjaxRedirect]
        public async Task<IActionResult> GetLayersItems(string type,
            int layerId,
            int selectedItemId,
            int bundleId,
            int[] selectedItemIds)
        {
            try
            {
                var siteId = GetCurrentSiteId();
                var layeritems = await _avatarService.GetUsersItemsByLayerAsync(layerId);
                var model = new AvatarViewModel
                {
                    ItemPath = _pathResolver.ResolveContentPath($"site{siteId}/{PathAvatars}/")
                };
                switch (type?.ToUpperInvariant())
                {
                    case "ITEM":
                        model.LayerItems = layeritems;
                        model.SelectedItemId = selectedItemId;
                        model.ItemPath = _pathResolver
                            .ResolveContentPath($"site{siteId}/{PathAvatars}/");
                        model.LayerId = layerId;
                        break;

                    case "COLOR":
                        model.LayerColors = await _avatarService.GetColorsByLayerAsync(layerId);
                        model.SelectedItemId = selectedItemId;
                        model.LayerId = layerId;
                        break;

                    default:
                        model.Bundle = await _avatarService.GetBundleByIdAsync(bundleId);
                        model.SelectedItemIds = selectedItemIds;
                        break;
                }
                if (model.Bundle != null)
                {
                    var lookupIndex = model.SelectedItemIds
                        .Select(_ => model.SelectedItemIds
                            .ToList()
                            .IndexOf(_))
                        .FirstOrDefault(_ => _ != -1);
                    model.SelectedItemIndex = lookupIndex == -1 ? 0 : lookupIndex;
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
                return Json(new { success = false });
            }
        }

        public async Task<IActionResult> Index()
        {
            var userWardrobe = await _avatarService.GetUserWardrobeAsync();
            if (userWardrobe.Count == 0)
            {
                ShowAlertDanger("Avatars have not been configured.");
                _logger.LogError(
                    "User {id} tried to customize their avatar but avatars are not configured!",
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
                ImagePath = _pathResolver.ResolveContentPath($"site{GetCurrentSiteId()}/avatars/"),
                SharingEnabled =
                    !await GetSiteSettingBoolAsync(SiteSettingKey.Avatars.DisableSharing)
            };
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

        public async Task<IActionResult> Share()
        {
            if (await GetSiteSettingBoolAsync(SiteSettingKey.Avatars.DisableSharing))
            {
                return RedirectToAction(nameof(Index));
            }

            PageTitle = _sharedLocalizer[Annotations.Title.ShareYourAvatar];
            var userAvatar = await _avatarService.GetUserAvatarAsync();
            if (userAvatar?.Count > 0)
            {
                var site = await GetCurrentSiteAsync();

                // ensure avatar sharing directory exists
                var avatarPath = new List<string> { $"site{site.Id}", PathUserAvatars };
                var filePath = _pathResolver.ResolveContentFilePath(Path.Combine([.. avatarPath]));
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }

                var viewModel = new ShareViewModel();
                userAvatar = [.. userAvatar.OrderBy(_ => _.LayerPosition)];
                var avatarBytes = userAvatar.Select(_ => _.Id)
                        .SelectMany(BitConverter.GetBytes)
                        .ToArray();
                var encoded = MD5.HashData(avatarBytes);
                viewModel.AvatarId = Convert.ToBase64String(encoded)
                    .TrimEnd('=')
                    .Replace('+', '-')
                    .Replace('/', '-');
                avatarPath.Add($"{viewModel.AvatarId}.png");

                filePath = _pathResolver.ResolveContentFilePath(Path.Combine([.. avatarPath]));
                if (!System.IO.File.Exists(filePath))
                {

                    // create an image with the preferred dimensions for Facebook link sharing
                    // https://www.adobe.com/express/discover/sizes/facebook

                    var timer = Stopwatch.StartNew();
                    using var image = new Image<Rgba32>(1200, 630);
                    var background = _pathResolver.ResolveContentFilePath(
                        Path.Combine($"site{site.Id}", "avatarbackgrounds", "background.png"));
                    var backgroundImage = await Image.LoadAsync(background);
                    image.Mutate(_ => _.DrawImage(backgroundImage, 1));

                    var avatarPoint = new Point(660, 60);
                    foreach (var element in userAvatar)
                    {
                        var fileName = _pathResolver.ResolveContentFilePath(element
                            .GetFilenameLink(site.Id, element.LayerId));
                        var avatarPartImage = await Image.LoadAsync(fileName);
                        image.Mutate(_ => _.DrawImage(avatarPartImage, avatarPoint, 1));
                    }

                    await image.SaveAsPngAsync(filePath, new PngEncoder
                    {
                        CompressionLevel = PngCompressionLevel.BestCompression
                    });

                    _logger.LogInformation(
                        "Generated avatar share {Type} id {Id} ({Size:F2} kb) in {ElapsedMs} ms",
                        "image",
                        viewModel.AvatarId,
                        new FileInfo(filePath).Length / 1024,
                        timer.ElapsedMilliseconds);
                }
                viewModel.AvatarImageUrl = await _siteLookupService.GetSiteLinkAsync(
                    site.Id,
                    _pathResolver.ResolveContentPath(string.Join('/', avatarPath))
                    );

                var shareUrl = await _siteLookupService.GetSiteLinkAsync(
                    site.Id,
                    Url.Action(nameof(ShareController.Avatar),
                        ShareController.Name,
                        new { id = viewModel.AvatarId })
                    );

                viewModel.FacebookShareUrl = new Uri(QueryHelpers.AddQueryString(
                    "https://www.facebook.com/sharer/sharer.php",
                    new Dictionary<string, string?> { { "u", shareUrl.ToString() } }
                    ));

                viewModel.TwitterShareUrl = new Uri(QueryHelpers.AddQueryString(
                    "https://twitter.com/intent/tweet",
                    new Dictionary<string, string>
                    {
                        { "url", shareUrl.ToString() },
                        { "hashtags", site.TwitterAvatarHashtags },
                        { "text", site.TwitterAvatarMessage },
                        { "via", site.TwitterUsername }
                    }
                    ));

                return View(viewModel);
            }
            TempData[TempDataKey.AlertDanger]
                = _sharedLocalizer[Annotations.Validate.CustomizeAvatarFirst].ToString();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Share(string selectionJson)
        {
            if (await GetSiteSettingBoolAsync(SiteSettingKey.Avatars.DisableSharing))
            {
                return RedirectToAction(nameof(Index));
            }

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

        private async Task UpdateAvatarAsync(string selectionJson)
        {
            var selection = Newtonsoft.Json.JsonConvert
                        .DeserializeObject<ICollection<AvatarLayer>>(selectionJson);
            selection = [.. selection.Where(_ => _.SelectedItem.HasValue)];
            await _avatarService.UpdateUserAvatarAsync(selection);
        }
    }
}
