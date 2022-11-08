using ProjectManager.Data;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace NativeBindings
{
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [ComVisible(true)]
    public class ProjectDocuments
    {
        public string[] rootFolders = new string[]
    {
        "N:\\Consulting",
        "M:\\Consulting",
        "P:\\SF Consulting",
    };


        public string[] GetProjectFolderPaths(string projectcode)
        {            
            List<string> foundPaths = new List<string>();
            Regex folderPattern = new Regex(@".*\(\d+.*");

            foreach (string path in rootFolders)
            {
                var foundDirectories = Directory.GetDirectories(path);
                foreach (var subDirectory in foundDirectories)
                {
                    if (folderPattern.IsMatch(subDirectory))
                    {
                        var jobFolders = Directory.GetDirectories(subDirectory);
                        foreach (var jobFolder in jobFolders)
                        {
                            if (jobFolder.StartsWith(projectcode))
                            {
                                foundPaths.Add(jobFolder);
                            }
                        }
                    }
                }
            }

            return foundPaths.ToArray();
        }

    }
}