namespace GRA.Controllers.ViewModel.Challenges
{
    public class TaskDetailViewModel
    {
        public int Id { get; set; }
        public bool IsCompleted { get; set; }
        public string TaskType { get; set; }
        public string BookCover { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
    }
}
