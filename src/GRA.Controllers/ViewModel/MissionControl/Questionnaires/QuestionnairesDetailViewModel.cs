using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Controllers.ViewModel.MissionControl.Questionnaires
{
    public class QuestionnairesDetailViewModel
    {
        public GRA.Domain.Model.Questionnaire Questionnaire { get; set; }
        public string QuestionSortOrder { get; set; }
        public GRA.Domain.Model.Question Question { get; set; }
        public string AnswerSortOrder { get; set; }
    }
}
