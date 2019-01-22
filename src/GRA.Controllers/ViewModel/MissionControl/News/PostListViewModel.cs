using System.Collections.Generic;
using GRA.Controllers.ViewModel.Shared;
using GRA.Domain.Model;

namespace GRA.Controllers.ViewModel.MissionControl.News
{
    public class PostListViewModel
    {
        public IEnumerable<NewsPost> Posts { get; set; }
        public PaginateViewModel PaginateModel { get; set; }
        public string Search { get; set; }
        public int? CategoryId { get; set; }
        public NewsCategory Category { get; set; }
        public NewsPost Post { get; set; }

        public IEnumerable<NewsCategory> CategoryList { get; set; }
    }
}
