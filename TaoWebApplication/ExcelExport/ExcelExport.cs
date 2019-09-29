using System.IO;
using System.Data;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Globalization;

namespace TaoWebApplication.ExcelExport
{
    public class ExcelExport
    {

        public static string GenerateExcelFile(DataSet tableData, string sheetName)
        {
            var file = Path.GetTempFileName();
            using (var workbook = SpreadsheetDocument.Create(file, DocumentFormat.OpenXml.SpreadsheetDocumentType.Workbook))
            {
                var workbookPart = workbook.AddWorkbookPart();

                workbook.WorkbookPart.Workbook = new DocumentFormat.OpenXml.Spreadsheet.Workbook();

                workbook.WorkbookPart.Workbook.Sheets = new DocumentFormat.OpenXml.Spreadsheet.Sheets();

                foreach (System.Data.DataTable table in tableData.Tables)
                {

                    var sheetPart = workbook.WorkbookPart.AddNewPart<WorksheetPart>();
                    var sheetData = new SheetData();
                    sheetPart.Worksheet = new Worksheet(sheetData);

                    Sheets sheets = workbook.WorkbookPart.Workbook.GetFirstChild<Sheets>();
                    string relationshipId = workbook.WorkbookPart.GetIdOfPart(sheetPart);

                    uint sheetId = 1;
                    if (sheets.Elements<Sheet>().Count() > 0)
                    {
                        sheetId = sheets.Elements<Sheet>().Select(s => s.SheetId.Value).Max() + 1;
                    }

                    Sheet sheet = new Sheet() { Id = relationshipId, SheetId = sheetId, Name = table.TableName };
                    sheets.Append(sheet);

                    Row headerRow = new Row();

                    List<string> columns = new List<string>();
                    foreach (System.Data.DataColumn column in table.Columns)
                    {
                        columns.Add(column.ColumnName);

                        Cell cell = new Cell();
                        cell.DataType = CellValues.String;
                        cell.CellValue = new CellValue(column.ColumnName);
                        headerRow.AppendChild(cell);
                    }


                    sheetData.AppendChild(headerRow);

                    foreach (System.Data.DataRow dsrow in table.Rows)
                    {
                        Row newRow = new Row();
                        foreach (String col in columns)
                        {
                            var cell = new Cell();

                            cell.DataType = GetDataType(dsrow[col]);
                            if (cell.DataType == CellValues.Number)
                            {
                                cell.CellValue = new CellValue(((decimal)dsrow[col]).ToString("G29", CultureInfo.InvariantCulture));
                            }
                            else
                            {
                                cell.CellValue = new CellValue(dsrow[col].ToString());
                            }
                            
                            newRow.AppendChild(cell);
                        }

                        sheetData.AppendChild(newRow);
                    }

                }
            }

            return file;
        }

        private static CellValues GetDataType(object value)
        {
            if (value.GetType() == typeof(System.DBNull))
                return CellValues.String; ;
            if (value.GetType() == typeof(bool))
                return CellValues.Boolean;
            if (value.GetType() == typeof(string))
                return CellValues.String;
            if (value.GetType() == typeof(DateTime) || value.GetType() == typeof(DateTimeOffset))
                return CellValues.Date;

            return CellValues.Number;
        }
    }
}