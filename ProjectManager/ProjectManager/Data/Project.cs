namespace ProjectManager.Data
{
    public class Project
    {
        public Guid Id { get; set; }
        public string ProjectId { get; set; } = null!;
        public string Name { get; set; } = null!;

        public List<ProjectStates> States { get; set; }
        public List<ProjectTask> Tasks { get; set; }

        public Project()
        {
            States = new List<ProjectStates>();
            Tasks = new List<ProjectTask>();    
        }
    }
}
