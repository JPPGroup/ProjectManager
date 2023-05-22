using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Office2016.Excel;
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
    public class VariationOrderRequestDocument : BaseExcelDocument
    {
        public string Reference
        {
            get
            {
                return ReadCell("F11-3 VOR", 5, "C");
            }
            set
            {
                UpdateCell("F11-3 VOR", value, 5, "C");
            }
        }

        public string Project
        {
            get
            {
                return ReadCell("F11-3 VOR", 10, "C");
            }
            set
            {
                UpdateCell("F11-3 VOR", value, 10, "C");
            }
        }

        public string ProjectReference
        {
            get
            {
                return ReadCell("F11-3 VOR", 12, "C");
            }
            set
            {
                UpdateCell("F11-3 VOR", value, 12, "C");
            }
        }

        public string Date
        {
            get
            {
                return ReadCell("F11-3 VOR", 10, "I");
            }
            set
            {
                UpdateCell("F11-3 VOR", value, 10, "I");
            }
        }

        public string Client
        {
            get
            {
                return ReadCell("F11-3 VOR", 12, "I");
            }
            set
            {
                UpdateCell("F11-3 VOR", value, 12, "I");
            }
        }

        public string Text
        {
            get
            {
                return ReadCell("F11-3 VOR", 23, "B");
            }
            set
            {
                UpdateCell("F11-3 VOR", value, 23, "B");
            }
        }

        public string Delay
        {
            get
            {
                return ReadCell("F11-3 VOR", 40, "G");
            }
            set
            {
                UpdateCell("F11-3 VOR", value, 40, "G");
            }
        }

        public string Cost
        {
            get
            {
                return ReadCell("F11-3 VOR", 42, "G");
            }
            set
            {
                UpdateCell("F11-3 VOR", value, 42, "G");
            }
        }


        public static VariationOrderRequestDocument NewVOR()
        {            
            MemoryStream stream = new MemoryStream();
            stream.Write(Resources.F11_3_Variation_Order_Request___VOR, 0, Resources.F11_3_Variation_Order_Request___VOR.Length);
            var document = new VariationOrderRequestDocument(stream);
            return document;
        }

        private VariationOrderRequestDocument(MemoryStream template) : base(template)
        {
        }

        public override string GetFilename()
        {
            return $"VOR-{ProjectReference}-{Reference}.xlsx";
        }
    }
}
