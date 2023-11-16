using System;

namespace GRA.Controllers.ViewModel.Events
{
    public class StreamViewModel
    {
        public string Embed { get; set; }
        public DateTime? EndDate { get; set; }
        public string EventName { get; set; }
        public string SecretCode { get; set; }
    }
}
