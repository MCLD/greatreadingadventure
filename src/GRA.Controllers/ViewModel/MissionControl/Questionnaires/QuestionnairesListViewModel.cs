using System.Collections.Generic;
using GRA.Controllers.ViewModel.Shared;

namespace GRA.Controllers.ViewModel.MissionControl.Questionnaires
{
    public class QuestionnairesListViewModel
    {
        public IEnumerable<GRA.Domain.Model.Questionnaire> Questionnaires { get; set; }
        public PaginateViewModel PaginateModel { get; set; }
    }
}
