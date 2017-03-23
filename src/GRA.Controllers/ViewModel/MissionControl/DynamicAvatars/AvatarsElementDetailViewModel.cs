using GRA.Domain.Model;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace GRA.Controllers.ViewModel.MissionControl.DynamicAvatars
{
    public class AvatarsElementDetailViewModel
    {
        public int AvatarId { get; set; }
        public DynamicAvatarLayer Layer { get; set; }
        public DynamicAvatarElement Element { get; set; }
        public bool Create { get; set; }
        public IFormFile UploadImage { get; set; }
        public string BaseAvatarUrl { get; set; }
    }
}
