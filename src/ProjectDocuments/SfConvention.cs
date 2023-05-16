using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ProjectDocuments
{
    public class SfConvention : IDwgNamingConvention
    {
        public DwgName Parse(string filename)
        {
            DwgName name = new DwgName();

            string fileName = Path.GetFileNameWithoutExtension(filename);
            var parts = fileName.Split('-');

            int offset = 0;

            if (parts.Length > 5)
            {
                offset = parts.Length - 5;
            }

            name.ProjectCode = parts[0];
            for (int i = 1; i <= 2 + offset; i++)
            {
                name.Number += parts[i];
            }

            name.Revision = parts[3 + offset];
            name.Name = parts[4 +offset].Trim();

            return name;
        }

        public string GetFilename(DwgName name)
        {
            throw new NotImplementedException();
        }
    }
}
