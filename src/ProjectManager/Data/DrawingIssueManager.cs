using ProjectDocuments;
using ProjectManagerContext.Data;
using System.Globalization;
using System.Text.RegularExpressions;

namespace ProjectManager.Data
{
    public partial class DrawingIssueManager
    {
        private List<IDwgNamingConvention> nameSchemes = new List<IDwgNamingConvention>();
        private Project _project;
        private ApplicationDbContext _context;

        public List<DwgName> Names { get; set; }
        public Dictionary<string, DrawingIssue> Issue { get; set; }

        public List<string> Errors { get; set; }

        public DrawingIssueManager(Project project, ApplicationDbContext context)
        {
            nameSchemes.Add(new JppConvention());
            nameSchemes.Add(new SfConvention2());
            nameSchemes.Add(new SfConvention());
            Names = new List<DwgName>();
            Issue = new Dictionary<string, DrawingIssue>();
            _project = project;
            _context = context;
            Errors = new List<string>();
        }

        public void PopulateFromCache()
        {
            _context.Entry(_project).Collection(p => p.DrawingIssues).Load();
            foreach (DrawingIssue di in _project.DrawingIssues)
            {
                foreach (ProjectManagerContext.Data.DrawingIssueEntry drawingIssueEntry in di.Entries)
                {
                    ProcessDrawingEntry(new DwgName()
                    {
                        Name = drawingIssueEntry.Title,
                        Number = drawingIssueEntry.Number,
                        Revision = drawingIssueEntry.Revision,
                        ProjectCode = _project.ProjectId
                    });
                }
                Issue.Add(di.Name, di);
            }
        }

        public void AddIssue(DrawingIssue issue)
        {
            Issue.Add(issue.Name, issue);
            _project.DrawingIssues.Add(issue);
        }

        public void PopulateFromFileSystem(IEnumerable<string> files)
        {
            foreach (string file in files)
            {
                if (Path.GetExtension(file) == ".pdf")
                {
                    if (Path.GetFileNameWithoutExtension(file).Contains("Drawing Register"))
                        continue;

                    DwgName? name = null;

                    foreach (IDwgNamingConvention convention in nameSchemes)
                    {
                        try
                        {
                            name = convention.Parse(file);
                        }
                        catch
                        {

                        }
                    }

                    if (name == null)
                    {
                        Errors.Add($"{file} was not in a recognized format.");
                    }
                    else
                    {
                        //Get issue date and name
                        string date = ExtractDateRegex().Match(file).Groups[1].Value;
                        string issuename = ExtractDateRegex().Match(file).Groups[2].Value;

                        if (!string.IsNullOrEmpty(date) && !string.IsNullOrEmpty(issuename))
                        {
                            if (!Issue.ContainsKey(issuename))
                            {
                                AddIssue(new DrawingIssue()
                                {
                                    Date = DateTime.ParseExact(date, "yyMMdd", CultureInfo.InvariantCulture),
                                    Name = issuename
                                });

                            }

                            Issue[issuename].SetEntry(name.Number, name.Revision, file, name.Name);
                        }

                        //Validate
                        ProcessDrawingEntry(name);
                    }
                }
            }

            OrderDrawings();
            //TODO: Await
            _context.SaveChangesAsync();
        }

        private void ProcessDrawingEntry(DwgName name)
        {
            //Search for same name
            var matches = Names.Where(n => n.Number == name.Number);
            if (matches.Any())
            {
                foreach (var match in matches)
                {
                    if (name.Name != match.Name)
                    {
                        Errors.Add($"For {name.Number}, a different title of {name.Name} was found");
                    }

                    //Update latest revision
                    match.Revision = GetLatestRevision(match.Revision, name.Revision);
                }
            }
            else
            {
                Names.Add(name);
            }
        }

        private void OrderDrawings()
        {
            Names = Names.OrderBy(n => n.Number).ToList();
        }


        private string GetLatestRevision(string rev1, string rev2)
        {
            if (rev1.StartsWith("AB"))
            {
                if (rev2.StartsWith("AB"))
                {
                    int num1 = int.Parse(rev1.Substring(2));
                    int num2 = int.Parse(rev2.Substring(2));

                    if (num1 > num2)
                    {
                        return rev1;
                    }
                    else
                    {
                        return rev2;
                    }
                }
                else
                {
                    return rev1;
                }
            }
            if (rev1.StartsWith("C"))
            {
                if (rev2.StartsWith("C"))
                {
                    int num1 = int.Parse(rev1.Substring(1));
                    int num2 = int.Parse(rev2.Substring(1));

                    if (num1 > num2)
                    {
                        return rev1;
                    }
                    else
                    {
                        return rev2;
                    }
                }
                else
                {
                    return rev1;
                }
            }
            if (rev1.StartsWith("T"))
            {
                if (rev2.StartsWith("T"))
                {
                    int num1 = int.Parse(rev1.Substring(1));
                    int num2 = int.Parse(rev2.Substring(1));

                    if (num1 > num2)
                    {
                        return rev1;
                    }
                    else
                    {
                        return rev2;
                    }
                }
                else
                {
                    return rev1;
                }
            }
            if (rev1.StartsWith("P"))
            {
                if (rev2.StartsWith("P"))
                {
                    int num1 = int.Parse(rev1.Substring(1));
                    int num2 = int.Parse(rev2.Substring(1));

                    if (num1 > num2)
                    {
                        return rev1;
                    }
                    else
                    {
                        return rev2;
                    }
                }
                else
                {
                    return rev1;
                }
            }

            if (rev1.StartsWith("WIP"))
            {
                return rev2;
            }

            return rev1;
            //Shouldnt get to here
            throw new ArgumentOutOfRangeException($"Revison not recognised, {rev1} and {rev2}");
        }

        [GeneratedRegex(".*\\\\(\\d{6})\\W*(.*)\\\\\\d+.*[.].{3}", RegexOptions.CultureInvariant, matchTimeoutMilliseconds: 1000)]
        private static partial Regex ExtractDateRegex();
    }

    public static class DrawingIssueManagerExtensions
    {
        public static DrawingIssueManager GetDrawingIssueManager(this Project project, ApplicationDbContext context)
        {
            DrawingIssueManager drawingIssueManager = new DrawingIssueManager(project, context);
            drawingIssueManager.PopulateFromCache();
            return drawingIssueManager;
        }
    }
}
