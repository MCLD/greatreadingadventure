using System.IO;
using System.Threading.Tasks;
using GRA.Controllers.ViewModel.Share;
using Microsoft.AspNetCore.Mvc;

namespace GRA.Controllers
{
    public class ShareController : Base.UserController
    {
        public ShareController(ServiceFacade.Controller context) : base(context)
        {
            PageTitle = "Share";
        }

        public static string Name
        { get { return "Share"; } }

        public async Task<IActionResult> Avatar(string id)
        {
            if (await GetSiteSettingBoolAsync(SiteSettingKey.Avatars.DisableSharing))
            {
                return NotFound();
            }

            var site = await GetCurrentSiteAsync();

            string[] pathElements = [$"site{site.Id}",
                AvatarController.PathUserAvatars,
                $"{id}.png"];

            var filePath = _pathResolver.ResolveContentFilePath(Path.Combine(pathElements));
            if (System.IO.File.Exists(filePath))
            {
                var imageUrl = await _siteLookupService.GetSiteLinkAsync(
                    site.Id,
                    _pathResolver.ResolveContentPath(string.Join('/', pathElements))
                    );

                var viewModel = new ShareAvatarViewModel
                {
                    ImageUrl = imageUrl.ToString(),
                    Social = new Domain.Model.Social
                    {
                        Description = site.AvatarCardDescription,
                        ImageLink = imageUrl.ToString()
                    }
                };
                return View(viewModel);
            }
            else
            {
                return NotFound();
            }
        }
    }
}
