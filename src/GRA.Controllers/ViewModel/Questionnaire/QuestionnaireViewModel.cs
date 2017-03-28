using System;
using System.Collections.Generic;
using System.Text;
using GRA.Domain.Model;

namespace GRA.Controllers.ViewModel.Questionnaire
{
    public class QuestionnaireViewModel
    {
        public int QuestionnaireId { get; set; }
        public IList<Question> Questions { get; set; }
    }
}
