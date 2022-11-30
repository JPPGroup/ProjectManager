using ProjectManager.Data;
using System.Diagnostics;
using System.IO;
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
        //"N:\\Consulting",
        //"M:\\Consulting",
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
                        List<string> jobFolders = new List<string>();
                         jobFolders.AddRange(Directory.GetDirectories(subDirectory));

                        string abandoned = Path.Combine(subDirectory, "0-Abandoned");
                        if (Directory.Exists(abandoned))
                        {
                            jobFolders.AddRange(Directory.GetDirectories(abandoned));
                        }

                        string completed = Path.Combine(subDirectory, "0-Completed");
                        if (Directory.Exists(completed))
                        {
                            jobFolders.AddRange(Directory.GetDirectories(completed));
                        }

                        string enquiries = Path.Combine(subDirectory, "0-Enquiries");
                        if (Directory.Exists(enquiries))
                        {
                            jobFolders.AddRange(Directory.GetDirectories(enquiries));
                        }

                        foreach (var jobFolder in jobFolders)
                        {                            
                            var dirName = new DirectoryInfo(jobFolder).Name;
                            if (dirName.StartsWith(projectcode))
                            {
                                foundPaths.Add(jobFolder);
                            }
                        }
                    }
                }
            }

            return foundPaths.ToArray();
        }

        public void WriteToFile(string path, string datastring)
        {
            MemoryStream data = new MemoryStream(Convert.FromBase64String(datastring));
            using var fileStream = File.Open(path, FileMode.CreateNew);
            data.CopyTo(fileStream);
        }

        public void Open(string path)
        {
            if (Directory.Exists(path))
            {

                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    Arguments = "\"" + path + "\"",
                    FileName = "explorer.exe"
                };
                Process.Start(startInfo);
            }
        }
    }
}