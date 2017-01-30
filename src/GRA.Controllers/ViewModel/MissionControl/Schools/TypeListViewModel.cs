using GRA.Controllers.ViewModel.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Controllers.ViewModel.MissionControl.Schools
{
    public class TypeListViewModel
    {
        public List<GRA.Domain.Model.SchoolType> SchoolTypes { get; set; }
        public PaginateViewModel PaginateModel { get; set; }
        public GRA.Domain.Model.SchoolType Type { get; set; }
    }
}
