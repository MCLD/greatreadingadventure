using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Domain.Model
{
    public class Challenge
    {
        public ICollection<ChallengeTask> tasks { get; set; }

        public int Id { get; set; }
        [Required]
        public int SiteId { get; set; }
        [Required]
        public int RelatedSystemId { get; set; }
        [Required]
        public int RelatedBranchId { get; set; }

        [Required]
        public bool IsActive { get; set; }
        [Required]
        public bool IsDeleted { get; set; }
        [Required]
        public bool IsValid { get; set; }


        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        [Required]
        public int PointsAwarded { get; set; }
        [Required]
        public int TasksToComplete { get; set; }

        public void AddTask(int userId, ChallengeTask task)
        {
            if (tasks == null)
            {
                tasks = new List<ChallengeTask>();
            }
            tasks.Add(task);
        }

        public ICollection<ChallengeTask> GetTasks()
        {
            return tasks;
        }
    }
}
