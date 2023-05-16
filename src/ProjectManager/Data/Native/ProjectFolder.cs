namespace ProjectManager.Data.Native
{
    public class ProjectFolder
    {
        public string ProjectCode { get; private set; }

        public string[] Paths { get; internal set; }
        public string PrimaryPath { get; set; }

        private NativeFiles _bridge;

        public ProjectFolder(string projectCode, NativeFiles bridge, string[] paths)
        {
            ProjectCode = projectCode;
            _bridge = bridge;
            Paths = paths;
            //TODO: Replace with a better system to determine
            PrimaryPath = Paths[0];
        }

        public async Task PersistQuoteDocument(string filename, MemoryStream data)
        {
            string path = Path.Combine(PrimaryPath, $"Financial\\Quotes\\{filename}");
            await _bridge.WriteToFile(path, data);
        }

        public async Task OpenFolder()
        {
            await _bridge.Open(PrimaryPath);
        }

        public async Task<IEnumerable<string>> GetIssuePaths()
        {
            string path = Path.Combine(PrimaryPath, $"Drawings\\JPP\\Issued");
            return await _bridge.GetSubFiles(path);
        }
    }
}
