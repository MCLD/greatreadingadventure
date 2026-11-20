using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GRA.Controllers.ViewModel
{
    public class JoinQuestionViewModel
    {
        public bool IsRequired { get; set; }
        public IEnumerable<SelectListItem> JoinQuestionOptions { get; set; }
        public int DisplayOrder { get; set; }
        public int JoinQuestionId { get; set; }
        public int JoinQuestionSelectedOption { get; set; }
        public string JoinQuestionText { get; set; }
    }
}
