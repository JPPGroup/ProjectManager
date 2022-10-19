using DocumentFormat.OpenXml.CustomProperties;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.VariantTypes;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace ProjectDocuments.BaseDocuments
{
    public class BaseWordDocument : IDisposable
    {
        private WordprocessingDocument _document;
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
            //Does this break stuff??
            _stream.Position = 0;
            return _stream;
        }

        public void Dispose()
        {
            _document.Dispose();
            _stream.Dispose();
        }
    }
}
