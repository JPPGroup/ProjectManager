using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ProjectDocuments
{
    //Unhyphenated name version
    public class SfConvention2 : IDwgNamingConvention
    {
        public DwgName Parse(string filename)
        {
            DwgName name = new DwgName();

            string fileName = Path.GetFileNameWithoutExtension(filename);
            var parts = fileName.Split('-');

            int offset = 0;

            if (parts.Length > 4)
            {
                offset = parts.Length - 4;
            }

            name.ProjectCode = parts[0];
            for (int i = 1; i <= 2 + offset; i++)
            {
                name.Number += parts[i];
            }

            var endToken = parts[3 + offset];
            int index = endToken.IndexOf(' ');

            name.Revision = endToken.Substring(0, index);
            
            name.Name = endToken.Substring(index).Trim();

            return name;
        }

        public string GetFilename(DwgName name)
        {
            throw new NotImplementedException();
        }
    }
}
