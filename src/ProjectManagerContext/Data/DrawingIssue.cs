using ProjectManager.Data;

namespace ProjectManagerContext.Data
{
    public class DrawingIssue
    {
        public Guid Id { get; set; }
        public required DateTime Date { get; set; }
        public required string Name { get; set; }

        public DrawingIssue()
        {
            Entries = new List<DrawingIssueEntry>();
        }
        
        public Guid ProjectId { get; set; }
        public virtual Project Project { get; set; }

        public DateTime IssueDate { get; set; }
        public virtual ICollection<Contact> Contacts { get; set; }
        public virtual List<DrawingIssueEntry> Entries { get; set; }

        public void SetEntry(string Number, string Revision, string Path, string Title)
        {
            if (Entries.Any(e => e.Number == Number))
                throw new ArgumentException("Drawing entry already found");

            Entries.Add(new DrawingIssueEntry()
            {
                Number = Number,
                Revision = Revision,
                Path = Path,
                Title = Title
            });
        }

        public DrawingIssueEntry GetEntry(string Number)
        {
            return Entries.First(di => di.Number == Number);
        }
    }

    public class DrawingIssueEntry
    {
        public Guid Id { get; set; }

        public string Number { get; set; }
        public string Title { get; set; }
        public string Revision { get; set; }
        public string Path { get; set; }

        public Guid DrawingIssueId { get; set; }
        public virtual DrawingIssue Issue { get; set; } = null!;
    }
}
