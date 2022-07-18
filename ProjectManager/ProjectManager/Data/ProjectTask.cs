using System.ComponentModel.DataAnnotations;

namespace ProjectManager.Data
{
    public class ProjectTask
    {
        public Guid Id { get; set; }

        public Guid ProjectId { get; set; }
        public Project Project { get; set; } = null!;

        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        public ProjectTaskStatus Status { get; set; }
        [Required]
        public DateTime Due { get; set; }
        [Required]
        public int EstimatedDuration { get; set; }
        
        public string? AssignedUserId { get; set; }
        public UserProfile? AssignedUser { get; set; }

        public string CreatedUserId { get; set; }
        public UserProfile CreatedUser { get; set; }
    }

    public enum ProjectTaskStatus
    {
        OnHold,
        InProgress,
        Complete
    }
}
