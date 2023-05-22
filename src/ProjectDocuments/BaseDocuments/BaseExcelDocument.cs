using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.CustomProperties;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
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
    public abstract class BaseExcelDocument : IDisposable, IWritableDocument
    {
        protected SpreadsheetDocument _document;
        private MemoryStream _stream;

        public BaseExcelDocument(MemoryStream document)
        {
            _stream = document;
            _stream.Position = 0;
            _document = SpreadsheetDocument.Open(_stream, true);
        }

        protected void UpdateCell(string sheetname, string text, uint rowIndex, string columnName)
        {
            WorksheetPart worksheetPart = GetWorksheetPartByName(sheetname);

            if (worksheetPart == null)
                throw new ArgumentOutOfRangeException("Invalid worksheet name");
            
                Cell cell = GetCell(worksheetPart.Worksheet, columnName, rowIndex);
                int index = InsertSharedStringItem(text);

            cell.CellValue = new CellValue(index.ToString());
            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);

// Save the worksheet.
            worksheetPart.Worksheet.Save();
            
        }

        protected string ReadCell(string sheetname, uint rowIndex, string columnName)
        {
            WorksheetPart worksheetPart = GetWorksheetPartByName(sheetname);

            if (worksheetPart == null)
                throw new ArgumentOutOfRangeException("Invalid worksheet name");

            Cell cell = GetCell(worksheetPart.Worksheet, columnName, rowIndex);

            //TODO: Add proper string handling

            string value = cell.InnerText;

            switch (cell.DataType.Value)
            {
                case CellValues.SharedString:
                    value = GetSharedStringItem(int.Parse(cell.InnerText));
                    break;

                default:
                    throw new NotImplementedException("Cant handle cell value type");
            }
                
            return value;
        }

        protected WorksheetPart? GetWorksheetPartByName(string sheetName)
        {
            IEnumerable<Sheet> allSheets = _document.WorkbookPart.Workbook.GetFirstChild<Sheets>().Elements<Sheet>();
            IEnumerable<Sheet> sheets = allSheets.Where(s => s.Name == sheetName);

            if (!sheets.Any())
            {
                // The specified worksheet does not exist.
                return null;
            }

            string relationshipId = sheets.First().Id.Value;
            WorksheetPart worksheetPart = (WorksheetPart)_document.WorkbookPart.GetPartById(relationshipId);
            return worksheetPart;
        }

        private Cell GetCell(Worksheet worksheet, string columnName, uint rowIndex)
        {
            Row row = GetRow(worksheet, rowIndex);
            string cellReference = columnName + rowIndex;

            if (row == null)
                return null;

            
            if (row.Elements<Cell>().Where(c => c.CellReference.Value == cellReference).Count() > 0)
            {
                return row.Elements<Cell>().Where(c => c.CellReference.Value == cellReference).First();
            }
            else
            {
                // Cells must be in sequential order according to CellReference. Determine where to insert the new cell.
                Cell refCell = null;
                foreach (Cell cell in row.Elements<Cell>())
                {
                    if (cell.CellReference.Value.Length == cellReference.Length)
                    {
                        if (string.Compare(cell.CellReference.Value, cellReference, true) > 0)
                        {
                            refCell = cell;
                            break;
                        }
                    }
                }

                Cell newCell = new Cell() { CellReference = cellReference };
                row.InsertBefore(newCell, refCell);

                worksheet.Save();
                return newCell;
            }
        }


        // Given a worksheet and a row index, return the row.
        private Row GetRow(Worksheet worksheet, uint rowIndex)
        {

            SheetData sheetData = worksheet.GetFirstChild<SheetData>();
            Row row;
            if (sheetData.Elements<Row>().Where(r => r.RowIndex == rowIndex).Count() != 0)
            {
                row = sheetData.Elements<Row>().Where(r => r.RowIndex == rowIndex).First();
            }
            else
            {
                row = new Row() { RowIndex = rowIndex };
                sheetData.Append(row);
            }

            return row;
        }

        private string GetSharedStringItem(int text)
        {
            SharedStringTablePart shareStringPart = _document.WorkbookPart.GetPartsOfType<SharedStringTablePart>().First();
            return shareStringPart.SharedStringTable.ElementAt(text).InnerText;
        }


        private int InsertSharedStringItem(string text)
        {
            SharedStringTablePart shareStringPart;
            if (_document.WorkbookPart.GetPartsOfType<SharedStringTablePart>().Count() > 0)
            {
                shareStringPart = _document.WorkbookPart.GetPartsOfType<SharedStringTablePart>().First();
            }
            else
            {
                shareStringPart = _document.WorkbookPart.AddNewPart<SharedStringTablePart>();
            }

            // If the part does not contain a SharedStringTable, create one.
            if (shareStringPart.SharedStringTable == null)
            {
                shareStringPart.SharedStringTable = new SharedStringTable();
            }

            int i = 0;

            // Iterate through all the items in the SharedStringTable. If the text already exists, return its index.
            foreach (SharedStringItem item in shareStringPart.SharedStringTable.Elements<SharedStringItem>())
            {
                if (item.InnerText == text)
                {
                    return i;
                }

                i++;
            }

            // The text does not exist in the part. Create the SharedStringItem and return its index.
            shareStringPart.SharedStringTable.AppendChild(new SharedStringItem(new DocumentFormat.OpenXml.Spreadsheet.Text(text)));
            shareStringPart.SharedStringTable.Save();

            return i;
        }

        public MemoryStream Save()
        {
            //Guard this somehow
            _document.Save();
            _document.Close();            
            _stream.Position = 0;
            return _stream;
        }

        public abstract string GetFilename();

        public void Dispose()
        {
            _document.Dispose();
            _stream.Dispose();
        }
    }
}
