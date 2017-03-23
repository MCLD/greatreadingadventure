using GRA.Domain.Model;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace GRA.Controllers.ViewModel.MissionControl.DynamicAvatars
{
    public class AvatarsDetailViewModel
    {
        public DynamicAvatar Avatar { get; set; }
        public List<AvatarsElementDetailViewModel> Elements { get; set; }
    }
}
