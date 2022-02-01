using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

namespace GRA.Controllers
{
    public class LookupController : Base.Controller
    {
        private readonly ILogger<LookupController> _logger;
        private readonly AvatarService _avatarService;
        private readonly SchoolService _schoolService;
        private readonly SiteService _siteService;
        private readonly UserService _userService;

        public static string Name { get { return "Lookup"; } }

        public LookupController(ILogger<LookupController> logger,
             ServiceFacade.Controller context,
             AvatarService avatarService,
             SchoolService schoolService,
            SiteService siteService,
            UserService userService) : base(context)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _avatarService = avatarService 
                ?? throw new ArgumentNullException(nameof(avatarService));
            _schoolService = schoolService 
                ?? throw new ArgumentNullException(nameof(schoolService));
            _siteService = siteService 
                ?? throw new ArgumentNullException(nameof(siteService));
            _userService = userService 
                ?? throw new ArgumentNullException(nameof(userService));
        }

        public async Task<JsonResult> GetBranches(int? systemId,
            int? branchId,
            bool listAll = false,
            bool prioritize = false,
            bool includeSystemName = false)
        {
            var branchList = new List<Branch>();

            if (systemId.HasValue)
            {
                branchList = (await _siteService.GetBranches(systemId.Value)).OrderBy(_ => _.Name)
                    .ToList();
            }
            else if (listAll)
            {
                branchList = (await _siteService.GetAllBranches(includeSystemName))
                    .OrderBy(_ => _.Name)
                    .ToList();
            }

            if (prioritize)
            {
                branchList = branchList.OrderByDescending(_ => _.Id == GetId(ClaimType.BranchId))
                    .ToList();
            }

            return Json(new SelectList(branchList, "Id", "Name", branchId));
        }

        public async Task<JsonResult> GetSchools(int? districtId, int? schoolId,
            string schoolName = null)
        {
            var schoolList = await _schoolService.GetSchoolsAsync(districtId);

            if (!string.IsNullOrWhiteSpace(schoolName))
            {
                foreach (var school in schoolList)
                {
                    if (string.Equals(schoolName.Trim(), school.Name, StringComparison.OrdinalIgnoreCase))
                    {
                        schoolId = school.Id;
                        break;
                    }
                }
            }
            return Json(new SelectList(schoolList, "Id", "Name", schoolId));
        }

        [HttpPost]
        public async Task<JsonResult> UsernameInUse(string username)
        {
            return Json(await _userService.UsernameInUseAsync(username));
        }

        public async Task<JsonResult> GetItemsInBundle(int id)
        {
            var bundle = await _avatarService.GetBundleByIdAsync(id, true);
            var thumbnailList = bundle.AvatarItems
                .Select(_ => _pathResolver.ResolveContentPath(_.Thumbnail))
                .ToList();

            return Json(thumbnailList);
        }
    }
}
