using System.ComponentModel;
using GRA.Domain.Model;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GRA.Controllers.ViewModel.MissionControl.News
{
    public class PostDetailViewModel
    {
        public string Action { get; set; }
        public SelectList Categories { get; set; }

        [DisplayName("Show this post as updated and sort it by today's date")]
        public bool MarkUpdated { get; set; }

        public SelectList NoYes { get; set; }
        public NewsPost Post { get; set; }
        public bool Publish { get; set; }
    }
}
