using ProjectManager.Data;

namespace ProjectManagerContext.Data
{
    public class DrawingIssue
    {
        public Guid Id { get; set; }
        
        public Guid ProjectId { get; set; }
        public virtual Project Project { get; set; }

        public DateTime IssueDate { get; set; }
        public virtual ICollection<Contact> Contacts { get; set; }
        public virtual List<DrawingIssueEntry> Drawings { get; set; }
    }

    public class DrawingIssueEntry
    {
        public Guid Id { get; set; }

        public string DrawingNumber { get; set; }
        public string DrawingTitle { get; set; }
        public string Revision { get; set; }

        public Guid DrawingIssueId { get; set; }
        public virtual DrawingIssue Issue { get; set; }
    }
}
