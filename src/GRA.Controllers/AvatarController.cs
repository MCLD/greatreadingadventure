using GRA.Controllers.ViewModel.Avatar;
using GRA.Domain.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Controllers
{
    [Authorize]
    public class AvatarController : Base.UserController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly StaticAvatarService _staticAvatarService;
        private readonly UserService _userService;

        public AvatarController(ILogger<HomeController> logger,
            ServiceFacade.Controller context,
            StaticAvatarService staticAvatarService,
            UserService userService)
            : base(context)
        {
            _logger = Require.IsNotNull(logger, nameof(logger));
            _staticAvatarService = Require.IsNotNull(staticAvatarService,
                nameof(staticAvatarService));
            _userService = Require.IsNotNull(userService, nameof(userService));
            PageTitle = "Avatar";
        }

        public async Task<IActionResult> Index(int? id)
        {
            var avatarList = (await _staticAvatarService.GetAvartarListAsync()).ToList();

            if (avatarList.Count() > 0)
            {
                if (id.HasValue)
                {
                    if (!avatarList.Any(_ => _.Id == id.Value))
                    {
                        id = null;
                    }
                }

                var user = await _userService.GetDetails(GetActiveUserId());
                var viewingAvatarId = id ?? user.AvatarId ?? avatarList.FirstOrDefault().Id;
                var avatar = avatarList.FirstOrDefault(_ => _.Id == viewingAvatarId);
                avatar.Filename = ResolveContentPath(avatar.Filename);

                var currentIndex = avatarList.FindIndex(_ => _.Id == viewingAvatarId);
                int previousAvatarId;
                int nextAvatarId;
                if (currentIndex == 0)
                {
                    previousAvatarId = avatarList.Last().Id;
                }
                else
                {
                    previousAvatarId = avatarList[currentIndex - 1].Id;
                }

                if (currentIndex == avatarList.Count - 1)
                {
                    nextAvatarId = avatarList.First().Id;
                }
                else
                {
                    nextAvatarId = avatarList[currentIndex + 1].Id;
                }

                AvatarSelectionViewModel viewModel = new AvatarSelectionViewModel()
                {
                    Avatar = avatarList.FirstOrDefault(_ => _.Id == viewingAvatarId),
                    PreviousAvatarId = previousAvatarId,
                    NextAvatarId = nextAvatarId
                };

                return View(viewModel);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Index (AvatarSelectionViewModel model)
        {
            try
            {
                var avatar = await _staticAvatarService.GetByIdAsync(model.Avatar.Id);
                var user = await _userService.GetDetails(GetActiveUserId());
                user.AvatarId = avatar.Id;
                await _userService.Update(user);
                AlertSuccess = "Your avatar has been updated";
                return RedirectToAction("Index", "Home");
            }
            catch (GraException gex)
            {
                AlertInfo = gex.Message;
                return RedirectToAction("Index");
            }
        }
    }
}
