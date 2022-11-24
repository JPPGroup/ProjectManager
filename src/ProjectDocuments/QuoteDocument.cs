using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Wordprocessing;
using ProjectDocuments.BaseDocuments;
using ProjectDocuments.Properties;
using ProjectManager.Data;
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

        public void UpdateTaskTable(IEnumerable<QuoteEntry> entries)
        {
            IEnumerable<Table> tables = _document.MainDocumentPart.Document.Body.Elements<Table>();
            var taskTable = tables.ElementAt(1);

            var rows = taskTable.Descendants<TableRow>();

            /*for(int i = 2; i < rows.Count(); i++)
            {
                var row = rows.ElementAt(i);
                foreach (var cell in row.Descendants<TableCell>())
                {
                    foreach (var para in cell.Descendants<Paragraph>())
                    {
                        
                    }                    
                }                
            }*/
            //Clear current entries
            for (int i = rows.Count() - 1; i > 1; i--)
            {
                var row = rows.ElementAt(i);
                row.Remove();
            }

            foreach(var entry in entries)
            {                
                TableRow tr = new TableRow();                               
                TableCell tc1 = new TableCell();                
                //tc1.Append(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "2400" }));
                                
                tc1.Append(ConvertFromHtml(entry.Item));                
                tr.Append(tc1);

                TableCell tc2 = new TableCell();
                tc2.Append(ConvertFromHtml(entry.Description));
                tr.Append(tc2);

                TableCell tc3 = new TableCell();
                tc3.Append(ConvertFromHtml(entry.Exclusions));
                tr.Append(tc3);

                taskTable.Append(tr);
            }

            _document.MainDocumentPart.Document.Save();
        }
    }
}
