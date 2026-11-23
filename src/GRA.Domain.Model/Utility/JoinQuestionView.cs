using System.Collections.Generic;

namespace GRA.Domain.Model.Utility
{
    public class JoinQuestionView
    {
        public JoinQuestionView()
        {
            JoinQuestionOptions = new Dictionary<string, string>();
        }
        public bool IsRequired { get; set; }
        public IDictionary<string, string> JoinQuestionOptions { get; set; }
        public int DisplayOrder { get; set; }
        public int JoinQuestionId { get; set; }
        public int JoinQuestionSelectedOption { get; set; }
        public string JoinQuestionText { get; set; }
        public string FieldName { get { return $"gra-join-q-{JoinQuestionId}"; } }
    }
}
