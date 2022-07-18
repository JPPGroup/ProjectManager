namespace ProjectManager.Data
{
    public class ProjectStates
    {
        public Guid Id { get; set; }
        public State State { get; set; }

        public Guid ProjectId { get; set; }
        public Project Project { get; set; } = null!;

        public string UserId { get; set; } = null!;
        public UserProfile User { get; set; } = null!;
    }

    public enum State
    {
        Unknown,
        Active,
        Archived,
        Ignored
    }
}

