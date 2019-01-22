using System;
using System.Collections.Generic;
using System.Text;
using GRA.Domain.Model;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GRA.Controllers.ViewModel.MissionControl.News
{
    public class PostDetailViewModel
    {
        public NewsPost Post { get; set; }
        public string Action { get; set; }
        public bool Publish { get; set; }
        public SelectList Categories { get; set; }
    }
}
