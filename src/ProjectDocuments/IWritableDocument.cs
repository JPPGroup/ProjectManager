using System.IO;

namespace ProjectDocuments
{
    public interface IWritableDocument
    {
        MemoryStream Save();

        string GetFilename();
    }
}
