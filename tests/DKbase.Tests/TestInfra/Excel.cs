using System.IO;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace DKbase.Tests.TestInfra
{
    /// <summary>Genera un .xlsx mínimo en memoria. Cada celda: número (CellValues.Number), texto compartido ("s:..."), o texto inline ("i:...").</summary>
    public static class Excel
    {
        public static byte[] Crear(params string[][] filas)
        {
            using var ms = new MemoryStream();
            using (var doc = SpreadsheetDocument.Create(ms, SpreadsheetDocumentType.Workbook))
            {
                var wb = doc.AddWorkbookPart();
                wb.Workbook = new Workbook();
                var ws = wb.AddNewPart<WorksheetPart>();
                var sheetData = new SheetData();
                ws.Worksheet = new Worksheet(sheetData);
                var sst = wb.AddNewPart<SharedStringTablePart>();
                sst.SharedStringTable = new SharedStringTable();
                int idx = 0;
                foreach (var fila in filas)
                {
                    var row = new Row();
                    foreach (var v in fila)
                    {
                        Cell c;
                        if (v.StartsWith("s:"))
                        {
                            sst.SharedStringTable.AppendChild(new SharedStringItem(new Text(v.Substring(2))));
                            c = new Cell { DataType = CellValues.SharedString, CellValue = new CellValue((idx++).ToString()) };
                        }
                        else if (v.StartsWith("i:"))
                            c = new Cell { DataType = CellValues.InlineString, InlineString = new InlineString(new Text(v.Substring(2))) };
                        else if (v.StartsWith("n:"))
                            c = new Cell { DataType = CellValues.Number, CellValue = new CellValue(v.Substring(2)) };
                        else
                            c = new Cell { CellValue = new CellValue(v) };
                        row.AppendChild(c);
                    }
                    sheetData.AppendChild(row);
                }
                var sheets = wb.Workbook.AppendChild(new Sheets());
                sheets.AppendChild(new Sheet { Id = wb.GetIdOfPart(ws), SheetId = 1, Name = "Hoja1" });
                wb.Workbook.Save();
            }
            return ms.ToArray();
        }
    }
}
