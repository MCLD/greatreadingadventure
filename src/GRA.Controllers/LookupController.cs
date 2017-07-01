using GRA.Domain.Model;
using GRA.Domain.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Controllers
{
    public class LookupController : Base.Controller
    {
        private readonly ILogger<LookupController> _logger;
        private readonly DynamicAvatarService _dynamicAvatarService;
        private readonly SchoolService _schoolService;
        private readonly SiteService _siteService;
        private readonly UserService _userService;

        public LookupController(ILogger<LookupController> logger,
             ServiceFacade.Controller context,
             DynamicAvatarService dynamicAvatarService,
             SchoolService schoolService,
            SiteService siteService,
            UserService userService) : base(context)
        {
            _logger = Require.IsNotNull(logger, nameof(logger));
            _dynamicAvatarService = Require.IsNotNull(dynamicAvatarService, 
                nameof(dynamicAvatarService));
            _schoolService = Require.IsNotNull(schoolService, nameof(schoolService));
            _siteService = Require.IsNotNull(siteService, nameof(siteService));
            _userService = Require.IsNotNull(userService, nameof(userService));
        }

        public async Task<JsonResult> GetBranches(int? systemId,
            int? branchId,
            bool listAll = false,
            bool prioritize = false)
        {
            var branchList = new List<Branch>();

            if (systemId.HasValue)
            {
                branchList = (await _siteService.GetBranches(systemId.Value)).OrderBy(_ => _.Name)
                    .ToList();
            }
            else if (listAll)
            {
                branchList = (await _siteService.GetAllBranches()).OrderBy(_ => _.Name)
                    .ToList();
            }

            if (prioritize)
            {
                branchList = branchList.OrderByDescending(_ => _.Id == GetId(ClaimType.BranchId))
                    .ToList();
            }

            return Json(new SelectList(branchList, "Id", "Name", branchId));
        }

        public async Task<JsonResult> GetSchoolTypes(int? districtId, int? typeId)
        {
            var schoolTypeList = await _schoolService.GetTypesAsync(districtId);
            return Json(new SelectList(schoolTypeList, "Id", "Name", typeId));
        }

        public async Task<JsonResult> GetSchools(int? districtId, int? typeId, int? schoolId, string schoolName = null)
        {
            var schoolList = await _schoolService.GetSchoolsAsync(districtId, typeId);
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

        public async Task<JsonResult> GetItemsInBundleAsync(int id)
        {
            var bundle = await _dynamicAvatarService.GetBundleByIdAsync(id, true);
            var thumbnailList = bundle.DynamicAvatarItems
                .Select(_ => _pathResolver.ResolveContentPath(_.Thumbnail))
                .ToList();

            return Json(thumbnailList);
        }
    }
}
