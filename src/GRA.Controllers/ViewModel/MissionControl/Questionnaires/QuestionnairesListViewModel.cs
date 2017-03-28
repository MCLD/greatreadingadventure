using GRA.Controllers.ViewModel.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Controllers.ViewModel.MissionControl.Questionnaires
{
    public class QuestionnairesListViewModel
    {
        public IEnumerable<GRA.Domain.Model.Questionnaire> Questionnaires { get; set; }
        public PaginateViewModel PaginateModel { get; set; }
    }
}
