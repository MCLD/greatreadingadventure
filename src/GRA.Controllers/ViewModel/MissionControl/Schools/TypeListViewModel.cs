using GRA.Controllers.ViewModel.Shared;
using System.Collections.Generic;

namespace GRA.Controllers.ViewModel.MissionControl.Schools
{
    public class TypeListViewModel
    {
        public List<GRA.Domain.Model.SchoolType> SchoolTypes { get; set; }
        public PaginateViewModel PaginateModel { get; set; }
        public GRA.Domain.Model.SchoolType Type { get; set; }
        public string Search { get; set; }
    }
}
