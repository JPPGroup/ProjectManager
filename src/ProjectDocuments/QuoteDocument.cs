using DocumentFormat.OpenXml.Wordprocessing;
using ProjectDocuments.BaseDocuments;
using ProjectDocuments.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Xml.Linq;

namespace ProjectDocuments
{
    public class QuoteDocument : BaseWordDocument
    {
        public string ProjectReference
        {
            get
            {
                return _projectReference;
            }
            set
            {
                _projectReference = value;
                SetStringProperty("ProjectReference", value);
            }
        }

        public string Title
        {
            get
            { 
                return _title; 
            }
            set
            {
                _title = value;
                SetStringProperty("Title", value);
            }
        }

        public string Revision
        {
            get
            {
                return _revision;
            }
            set
            {
                _revision = value;
                SetStringProperty("Revision", value);
            }
        }

        public string Introduction
        {
            get
            {
                return _introduction;
            }
            set
            {
                _introduction = value;
                SetIntroductionText();
            }
        }        

        private string _projectReference, _title, _revision, _introduction;

        public static QuoteDocument NewQuote()
        {            
            MemoryStream stream = new MemoryStream();
            stream.Write(Resources.F05_12_Fee_and_Task_Schedule_JPP, 0, Resources.F05_12_Fee_and_Task_Schedule_JPP.Length);
            var document = new QuoteDocument(stream);
            document.Revision = "0";
            return document;
        }

        private QuoteDocument(MemoryStream template) : base(template)
        {
        }

        public string GetFilename()
        {
            return $"Q-{ProjectReference}-{Revision} - Fee and Task Schedule.docm";
        }

        private void SetIntroductionText()
        {
            IEnumerable<Paragraph> paras = _document.MainDocumentPart.Document.Body.Descendants<Paragraph>();

            int startIndex = -1;
            int endIndex = -1;

            for (int i = 0; i < paras.Count(); i++)
            {
                if(paras.ElementAt(i).InnerText == "Introduction")
                {
                    startIndex = i;
                    
                }
                if (paras.ElementAt(i).InnerText == "Service Provided")
                {                    
                    endIndex = i - 2;
                }
            }

            if (startIndex == -1 || endIndex == -1)
                throw new InvalidOperationException("Intorudction paragraph not found");

            //Remove existing
            for (int i = endIndex - 1; i > startIndex; i--)
            {
                paras.ElementAt(i).Remove();
            }

            Paragraph lastPara = paras.ElementAt(startIndex);

            ConvertFromHtml(lastPara, _introduction);            

            _document.MainDocumentPart.Document.Save();
        }


    }
}
