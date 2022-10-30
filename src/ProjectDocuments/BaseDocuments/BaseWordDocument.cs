using DocumentFormat.OpenXml.CustomProperties;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.VariantTypes;
using DocumentFormat.OpenXml.Vml;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace ProjectDocuments.BaseDocuments
{
    public class BaseWordDocument : IDisposable
    {
        protected WordprocessingDocument _document;
        private MemoryStream _stream;

        public BaseWordDocument(MemoryStream document)
        {
            _stream = document;
            _stream.Position = 0;
            _document = WordprocessingDocument.Open(_stream, true);
        }

        public void SetStringProperty(string propertyName, string propertyValue)
        {
            //https://learn.microsoft.com/en-us/office/open-xml/how-to-set-a-custom-property-in-a-word-processing-document

            var customProps = _document.CustomFilePropertiesPart;
            var props = customProps.Properties;

            var prop = props.Where(p => ((CustomDocumentProperty)p).Name.Value == propertyName).FirstOrDefault();
                        
            if (prop != null)
            {                
                prop.Remove();
            }

            var newProp = new CustomDocumentProperty();
            newProp.VTLPWSTR = new VTLPWSTR(propertyValue.ToString());
            newProp.FormatId = "{D5CDD505-2E9C-101B-9397-08002B2CF9AE}";
            newProp.Name = propertyName;

            props.AppendChild(newProp);
            int pid = 2;
            foreach (CustomDocumentProperty item in props)
            {
                item.PropertyId = pid++;
            }
            props.Save();            
        }

        public MemoryStream Save()
        {
            DocumentSettingsPart settingsPart = _document.MainDocumentPart.GetPartsOfType<DocumentSettingsPart>().First();            
            UpdateFieldsOnOpen updateFields = new UpdateFieldsOnOpen();
            updateFields.Val = new DocumentFormat.OpenXml.OnOffValue(true);            
            settingsPart.Settings.PrependChild<UpdateFieldsOnOpen>(updateFields);
            settingsPart.Settings.Save();

            //Guard this somehow
            _document.Save();
            _document.Close();            
            _stream.Position = 0;
            return _stream;
        }

        public void ConvertFromHtml(Paragraph insertAfter, string content)
        {
            Paragraph lastPara = insertAfter;

            var paragraphs = ConvertFromHtml(content);

            foreach (var para in paragraphs)
            {
                _document.MainDocumentPart.Document.Body.InsertAfter<Paragraph>(para, lastPara);
                lastPara = para;
            }
        }

        public IEnumerable<Paragraph> ConvertFromHtml(string content)
        {         
            List<Paragraph> paragraphs = new List<Paragraph>();

            content = $"<root>{content}</root>";
            content = content.Replace("<br>", "<br/>");
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(content);
            var root = xmlDocument.ChildNodes[0];

            foreach (XmlNode xnode in root.ChildNodes)
            {
                string name = xnode.Name;
                string value = xnode.InnerText;
                switch (name)
                {
                    case "p":
                        if (!string.IsNullOrEmpty(value))
                        {
                            Paragraph para = new Paragraph();
                            Run run = para.AppendChild(new Run());
                            run.AppendChild(new Text(value));

                            paragraphs.Add(para);                            
                        }

                        break;

                    default:
                        throw new InvalidOperationException("unknown root element");
                }

            }

            return paragraphs;
        }

        public void Dispose()
        {
            _document.Dispose();
            _stream.Dispose();
        }
    }
}
