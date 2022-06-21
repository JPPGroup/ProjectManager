namespace ProjectManager.Data
{
    public class ProjectTask
    {
        public Guid Id { get; set; }
        
        public Guid ProjectId { get; set; }
        public Project Project { get; set; }
    }
}
