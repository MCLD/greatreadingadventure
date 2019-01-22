using System;
using System.Collections.Generic;
using System.Text;
using GRA.Domain.Model;

namespace GRA.Controllers.ViewModel.MissionControl.News
{
    public class CategoryDetailViewModel
    {
        public NewsCategory Category { get; set; }
        public string Action { get; set; }
    }
}