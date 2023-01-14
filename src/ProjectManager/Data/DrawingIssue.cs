namespace ProjectManager.Data
{
    public class DrawingIssue
    {
        public DateTime Date { get; set; }
        public string Name { get; set; }

        public Dictionary<string, DrawingEntry> Entries { get; set; }

        public DrawingIssue()
        {
            Entries = new Dictionary<string, DrawingEntry>();
        }

        public void SetEntry(string Number, string Revision, string Path)
        {
            if (Entries.ContainsKey(Number))
                throw new ArgumentException("Drawing entry already found");

            Entries.Add(Number, new DrawingEntry()
            {
                Number = Number,
                Revision = Revision,
                Path = Path
            });
        }
    }

    public class DrawingEntry
    {
        public string Number { get; set; }
        public string Revision { get; set; }
        public string Path { get; set; }
    }
}
