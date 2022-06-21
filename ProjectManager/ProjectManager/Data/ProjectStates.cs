namespace ProjectManager.Data
{
    public class ProjectStates
    {
        public Guid Id { get; set; }
        public State State { get; set; }

        public Guid ProjectId { get; set; }
        public Project Project { get; set; }

        public string UserId { get; set; }
        public UserProfile User { get; set; }
    }

    public enum State
    {
        Unknown,
        Active,
        Archived,
        Ignored
    }
}

