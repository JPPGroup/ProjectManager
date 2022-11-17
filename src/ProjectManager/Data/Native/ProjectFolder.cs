namespace ProjectManager.Data.Native
{
    public class ProjectFolder
    {
        public string ProjectCode { get; private set; }

        public string[] Paths { get; internal set; }
        public string PrimaryPath { get; set; }

        private NativeFiles _bridge;

        public ProjectFolder(string projectCode, NativeFiles bridge)
        {
            ProjectCode = projectCode;
            _bridge = bridge;
        }

        public async Task PersistQuoteDocument(string filename, MemoryStream data)
        {
            string path = Path.Combine(PrimaryPath, $"Financial\\QUotes\\{filename}");
            await _bridge.WriteToFile(path, data);
        }
    }
}
