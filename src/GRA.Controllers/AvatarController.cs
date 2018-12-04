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
        private readonly UserService _userService;

        public AvatarController(ILogger<AvatarController> logger,
            ServiceFacade.Controller context,
            AvatarService avatarService,
            SiteService siteService,
            UserService userService)
            : base(context)
        {
            _logger = Require.IsNotNull(logger, nameof(logger));
            _mapper = context.Mapper;
            _avatarService = Require.IsNotNull(avatarService,
                nameof(avatarService));
            _siteService = siteService ?? throw new ArgumentNullException(nameof(siteService));
            _userService = Require.IsNotNull(userService, nameof(userService));
            PageTitle = "Avatar";
        }

        public async Task<IActionResult> Index()
        {
            var userWardrobe = await _avatarService.GetUserWardrobeAsync();
            if (userWardrobe.Count == 0)
            {
                ShowAlertDanger("Avatars have not been set up.");
                return RedirectToAction("Index", "Home");
            }

            AvatarJsonModel model = new AvatarJsonModel();
            model.Layers = _mapper
                .Map<ICollection<AvatarJsonModel.AvatarLayer>>(userWardrobe);
            var layerGroupings = userWardrobe
                .GroupBy(_ => _.GroupId)
                .Select(_ => _.ToList())
                .ToList();
            AvatarViewModel viewModel = new AvatarViewModel()
            {
                LayerGroupings = layerGroupings,
                DefaultLayer = userWardrobe.Where(_ => _.DefaultLayer).First().Id,
                ImagePath = _pathResolver.ResolveContentPath($"site{GetCurrentSiteId()}/avatars/"),
                AvatarPiecesJson = Newtonsoft.Json.JsonConvert.SerializeObject(model)
            };

            var userAvatar = await _avatarService.GetUserAvatarAsync();
            if (userAvatar.Count == 0)
            {
                viewModel.NewAvatar = true;
            }
            return View(viewModel);
        }

        [HttpPost]
        public async Task<JsonResult> SaveAvatar(string selectionJson)
        {
            try
            {
                await UpdateAvatar(selectionJson);
                return Json(new { success = true });
            }
            catch (GraException gex)
            {
                return Json(new { success = false, message = gex.Message });
            }
        }

        public async Task<IActionResult> Share()
        {
            var userAvatar = await _avatarService.GetUserAvatarAsync();
            if (userAvatar != null && userAvatar.Count > 0)
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
                        using (Image<Rgba32> image = new Image<Rgba32>(1200, 630))
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
                        + "/" + filename;
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
            TempData[TempDataKey.AlertDanger] = "No avatar saved.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Share(string selectionJson)
        {
            if (!string.IsNullOrWhiteSpace(selectionJson))
            {
                try
                {
                    await UpdateAvatar(selectionJson);
                    ShowAlertSuccess("Avatar saved.");
                }
                catch (GraException gex)
                {
                    ShowAlertDanger($"Unable to save avater: {gex}");
                }
            }
            return RedirectToAction(nameof(Share));
        }

        private async Task UpdateAvatar(string selectionJson)
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

                using (Image<Rgba32> image = Image.Load(avatarPath))
                {
                    image.Mutate(_ => _.Resize(1080, 567));
                    image.Mutate(_ => _.Crop(new Rectangle(0, 0, 1080, 566)));
                    image.Save(igFilePath);
                }
                var imageBytes = System.IO.File.ReadAllBytes(igFilePath);
                return File(imageBytes, "image/png");
            }

            TempData[TempDataKey.AlertDanger] = "No avatar saved.";
            return RedirectToAction(nameof(Index));
        }
    }
}
