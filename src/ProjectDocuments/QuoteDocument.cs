using ProjectDocuments.BaseDocuments;
using ProjectDocuments.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace ProjectDocuments
{
    public class QuoteDocument : BaseWordDocument
    {
        public static QuoteDocument NewQuote()
        {
            MemoryStream stream = new MemoryStream(Resources.F05_12_Fee_and_Task_Schedule_JPP);
            return new QuoteDocument(stream);
        }

        private QuoteDocument(MemoryStream template) : base(template)
        {
        }

        public MemoryStream GetStream()
        {
            return new MemoryStream(Resources.F05_12_Fee_and_Task_Schedule_JPP);
        }


    }
}
