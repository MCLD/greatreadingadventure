using GRA.Controllers.ViewModel.Avatar;
using GRA.Domain.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using GRA.Domain.Model;

namespace GRA.Controllers
{
    [Authorize]
    public class AvatarController : Base.UserController
    {
        private readonly ILogger<AvatarController> _logger;
        private readonly AutoMapper.IMapper _mapper;
        private readonly DynamicAvatarService _dynamicAvatarService;
        private readonly UserService _userService;

        public AvatarController(ILogger<AvatarController> logger,
            ServiceFacade.Controller context,
            DynamicAvatarService dynamicAvatarService,
            UserService userService)
            : base(context)
        {
            _logger = Require.IsNotNull(logger, nameof(logger));
            _mapper = context.Mapper;
            _dynamicAvatarService = Require.IsNotNull(dynamicAvatarService,
                nameof(dynamicAvatarService));
            _userService = Require.IsNotNull(userService, nameof(userService));
            PageTitle = "Avatar";
        }

        public async Task<IActionResult> Index(int? id)
        {
            var currentSite = await GetCurrentSiteAsync();
            var userWardrobe = await _dynamicAvatarService.GetUserWardrobeAsync();
            if (userWardrobe?.Count > 0)
            {
                DynamicAvatarJsonModel model = new DynamicAvatarJsonModel();
                model.Layers = _mapper
                    .Map<ICollection<DynamicAvatarJsonModel.DynamicAvatarLayer>>(userWardrobe);
                DynamicAvatarViewModel viewModel = new DynamicAvatarViewModel()
                {
                    Layers = userWardrobe,
                    GroupIds = userWardrobe.Select(_ => _.GroupId).Distinct(),
                    DefaultLayer = userWardrobe.Where(_ => _.DefaultLayer).Select(_ => _.Id).First(),
                    ImagePath = _pathResolver.ResolveContentPath($"site{currentSite.Id}/dynamicavatars/"),
                    AvatarPiecesJson = Newtonsoft.Json.JsonConvert.SerializeObject(model)
                };
                return View("DynamicIndex", viewModel);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public async Task<JsonResult> SaveAvatar(string selectionJson)
        {
            try
            {
                var selection = Newtonsoft.Json.JsonConvert
                    .DeserializeObject<ICollection<DynamicAvatarLayer>>(selectionJson);
                selection = selection.Where(_ => _.SelectedItem.HasValue).ToList();
                await _dynamicAvatarService.UpdateUserAvatarAsync(selection);
                return Json(new { success = true });
            }
            catch (GraException gex)
            {
                return Json(new { success = false, message = gex.Message });
            }
        }
    }
}
