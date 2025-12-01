using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

public class ExcelHelper
{
    private string _filePath;
    private List<string> _columns;

    public ExcelHelper(string filePath, List<string> columns)
    {
        _filePath = filePath;
        _columns = columns;
    }

    public void CreateFile()
    {
        using (SpreadsheetDocument document = SpreadsheetDocument.Create(_filePath, SpreadsheetDocumentType.Workbook))
        {
            WorkbookPart workbookPart = document.AddWorkbookPart();
            workbookPart.Workbook = new Workbook();

            WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
            worksheetPart.Worksheet = new Worksheet(new SheetData());

            Sheets sheets = document.WorkbookPart.Workbook.AppendChild(new Sheets());
            Sheet sheet = new Sheet()
            {
                Id = document.WorkbookPart.GetIdOfPart(worksheetPart),
                SheetId = 1,
                Name = "Data"
            };
            sheets.Append(sheet);

            SheetData sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();

            Row headerRow = new Row();
            foreach (var col in _columns)
            {
                headerRow.Append(
                    new Cell()
                    {
                        CellValue = new CellValue(col),
                        DataType = CellValues.String
                    });
            }

            sheetData.Append(headerRow);
        }
    }

    public void AddRow(List<string> values)
    {
        using (SpreadsheetDocument document = SpreadsheetDocument.Open(_filePath, true))
        {
            WorksheetPart worksheetPart = document.WorkbookPart.WorksheetParts.First();
            SheetData sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();

            Row newRow = new Row();

            for (int i = 0; i < _columns.Count; i++)
            {
                newRow.Append(new Cell()
                {
                    CellValue = new CellValue(values[i]),
                    DataType = CellValues.String
                });
            }

            sheetData.Append(newRow);
            worksheetPart.Worksheet.Save();
        }
    }

    public void DeleteRow(string columnName, string value)
    {
        int columnIndex = _columns.IndexOf(columnName);
        if (columnIndex < 0) throw new Exception("Column not found");

        using (SpreadsheetDocument document = SpreadsheetDocument.Open(_filePath, true))
        {
            WorksheetPart worksheetPart = document.WorkbookPart.WorksheetParts.First();
            SheetData sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();

            var rows = sheetData.Elements<Row>().Skip(1); // skip header

            foreach (var row in rows.ToList())
            {
                var cell = row.Elements<Cell>().ElementAt(columnIndex);
                string cellValue = cell.CellValue?.InnerText ?? "";

                if (cellValue == value)
                {
                    row.Remove();
                }
            }

            worksheetPart.Worksheet.Save();
        }
    }

    public string GetValue(string findColumn, string findValue, string returnColumn)
    {
        int findIndex = _columns.IndexOf(findColumn);
        int returnIndex = _columns.IndexOf(returnColumn);

        if (findIndex < 0 || returnIndex < 0)
            throw new Exception("Column not found");

        using (SpreadsheetDocument document = SpreadsheetDocument.Open(_filePath, false))
        {
            WorksheetPart worksheetPart = document.WorkbookPart.WorksheetParts.First();
            SheetData sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();

            var rows = sheetData.Elements<Row>().Skip(1); // skip header

            foreach (var row in rows)
            {
                var cell = row.Elements<Cell>().ElementAt(findIndex);
                string cellValue = cell.CellValue?.InnerText ?? "";

                if (cellValue == findValue)
                {
                    var targetCell = row.Elements<Cell>().ElementAt(returnIndex);
                    return targetCell.CellValue?.InnerText ?? "";
                }
            }
        }

        return null;
    }
}