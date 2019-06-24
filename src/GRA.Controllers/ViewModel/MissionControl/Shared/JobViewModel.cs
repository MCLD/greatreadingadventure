namespace GRA.Controllers.ViewModel.MissionControl.Shared
{
    public class JobViewModel
    {
        public JobViewModel()
        {
            CancelUrl = "/MissionControl/";
            CompleteButton = "Back to Mission Control";
            PingSeconds = 15;
            SuccessUrl = "/MissionControl/";
        }

        public int PingSeconds { get; set; }
        public string CancelUrl { get; set; }
        public string CompleteButton { get; set; }
        public string JobToken { get; set; }
        public string SuccessRedirectUrl { get; set; }
        public string SuccessUrl { get; set; }
        public string Title { get; set; }
    }
}
