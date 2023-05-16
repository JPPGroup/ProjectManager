using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ProjectDocuments
{
    public class JppConvention : IDwgNamingConvention
    {
        public DwgName Parse(string filename)
        {
            DwgName name = new DwgName();

            string fileName = Path.GetFileNameWithoutExtension(filename);
            var parts = fileName.Split('-');

            name.ProjectCode = parts[0];
            name.Number = parts[1].Substring(0, parts[1].Length - 3);
            name.Revision = parts[1].Substring(parts[1].Length - 3);
            name.Name = parts[2];

            return name;
        }

        public string GetFilename(DwgName name)
        {
            return $"{name.ProjectCode} - {name.Number}{name.Revision} - {name.Name}";
        }
    }
}
