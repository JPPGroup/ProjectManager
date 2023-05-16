using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectDocuments
{
    public interface IDwgNamingConvention
    {
        DwgName Parse(string filename);

        string GetFilename(DwgName name);
    }

    public class DwgName
    {
        public string ProjectCode { get; set; }
        public string Number { get; set; }
        public string Revision { get; set; }
        public string Name { get; set; }
    }
}
