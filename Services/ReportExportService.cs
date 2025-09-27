using System.Data;
using Microsoft.Reporting.NETCore;
using System.Reflection;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml.Spreadsheet;

namespace hongWenAPP.Services
{
    public class ReportExportService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ReportExportService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        /// <summary>
        /// Export data to PDF using RDLC report
        /// </summary>
        public byte[] ExportToPdf<T>(List<T> data, string reportFileName, string dataSetName, Dictionary<string, object> parameters = null)
        {
            var reportPath = Path.Combine(_webHostEnvironment.WebRootPath, "Reports", reportFileName);
            
            using (var report = new LocalReport())
            {
                report.ReportPath = reportPath;

                // Convert to DataTable
                var dataTable = ConvertToDataTable(data);
                var dataSource = new ReportDataSource(dataSetName, dataTable);
                report.DataSources.Add(dataSource);

                // Add parameters if provided
                if (parameters != null)
                {
                    foreach (var param in parameters)
                    {
                        report.SetParameters(new ReportParameter(param.Key, param.Value?.ToString()));
                    }
                }

                return report.Render("PDF");
            }
        }

        /// <summary>
        /// Export data to Excel using RDLC report
        /// </summary>
        public byte[] ExportToExcel<T>(List<T> data, string reportFileName, string dataSetName, Dictionary<string, object> parameters = null)
        {
            var reportPath = Path.Combine(_webHostEnvironment.WebRootPath, "Reports", reportFileName);
            
            using (var report = new LocalReport())
            {
                report.ReportPath = reportPath;

                // Convert to DataTable
                var dataTable = ConvertToDataTable(data);
                var dataSource = new ReportDataSource(dataSetName, dataTable);
                report.DataSources.Add(dataSource);

                // Add parameters if provided
                if (parameters != null)
                {
                    foreach (var param in parameters)
                    {
                        report.SetParameters(new ReportParameter(param.Key, param.Value?.ToString()));
                    }
                }

                return report.Render("EXCEL");
            }
        }

        /// <summary>
        /// Export data to Word document using OpenXML
        /// </summary>
        public byte[] ExportToWord<T>(List<T> data, string title = "Report", Dictionary<string, object> parameters = null)
        {
            using (var stream = new MemoryStream())
            {
                using (var document = WordprocessingDocument.Create(stream, WordprocessingDocumentType.Document))
                {
                    var mainPart = document.AddMainDocumentPart();
                    mainPart.Document = new Document();
                    var body = mainPart.Document.AppendChild(new Body());

                    // Add title
                    var titleParagraph = new Paragraph(new DocumentFormat.OpenXml.Wordprocessing.Run(new DocumentFormat.OpenXml.Wordprocessing.Text(title)));
                    titleParagraph.ParagraphProperties = new ParagraphProperties(
                        new Justification() { Val = JustificationValues.Center },
                        new ParagraphStyleId() { Val = "Heading1" }
                    );
                    body.AppendChild(titleParagraph);

                    // Add parameters if provided
                    if (parameters != null)
                    {
                        foreach (var param in parameters)
                        {
                            var paramParagraph = new Paragraph(new DocumentFormat.OpenXml.Wordprocessing.Run(new DocumentFormat.OpenXml.Wordprocessing.Text($"{param.Key}: {param.Value}")));
                            body.AppendChild(paramParagraph);
                        }
                        body.AppendChild(new Paragraph(new DocumentFormat.OpenXml.Wordprocessing.Run(new DocumentFormat.OpenXml.Wordprocessing.Text(""))));
                    }

                    // Create table
                    var table = new DocumentFormat.OpenXml.Wordprocessing.Table();
                    
                    // Table properties
                    var tableProperties = new TableProperties(
                        new TableBorders(
                            new DocumentFormat.OpenXml.Wordprocessing.TopBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 },
                            new DocumentFormat.OpenXml.Wordprocessing.BottomBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 },
                            new DocumentFormat.OpenXml.Wordprocessing.LeftBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 },
                            new DocumentFormat.OpenXml.Wordprocessing.RightBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 },
                            new InsideHorizontalBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 },
                            new InsideVerticalBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 }
                        )
                    );
                    table.AppendChild(tableProperties);

                    if (data.Any())
                    {
                        // Header row
                        var headerRow = new TableRow();
                        var properties = typeof(T).GetProperties();
                        
                        foreach (var prop in properties)
                        {
                            var cell = new TableCell();
                            cell.Append(new Paragraph(new DocumentFormat.OpenXml.Wordprocessing.Run(new DocumentFormat.OpenXml.Wordprocessing.Text(prop.Name))));
                            headerRow.Append(cell);
                        }
                        table.Append(headerRow);

                        // Data rows
                        foreach (var item in data)
                        {
                            var dataRow = new TableRow();
                            foreach (var prop in properties)
                            {
                                var cell = new TableCell();
                                var value = prop.GetValue(item)?.ToString() ?? "";
                                cell.Append(new Paragraph(new DocumentFormat.OpenXml.Wordprocessing.Run(new DocumentFormat.OpenXml.Wordprocessing.Text(value))));
                                dataRow.Append(cell);
                            }
                            table.Append(dataRow);
                        }
                    }

                    body.Append(table);
                    document.Save();
                }

                return stream.ToArray();
            }
        }

        /// <summary>
        /// Export data to native Excel format using OpenXML
        /// </summary>
        public byte[] ExportToNativeExcel<T>(List<T> data, string sheetName = "Sheet1", Dictionary<string, object> parameters = null)
        {
            using (var stream = new MemoryStream())
            {
                using (var document = SpreadsheetDocument.Create(stream, SpreadsheetDocumentType.Workbook))
                {
                    var workbookPart = document.AddWorkbookPart();
                    workbookPart.Workbook = new Workbook();

                    var worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                    worksheetPart.Worksheet = new Worksheet(new SheetData());

                    var sheets = document.WorkbookPart.Workbook.AppendChild(new Sheets());
                    var sheet = new Sheet() { Id = document.WorkbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = sheetName };
                    sheets.Append(sheet);

                    var sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();
                    uint rowIndex = 1;

                    // Add parameters if provided
                    if (parameters != null)
                    {
                        foreach (var param in parameters)
                        {
                            var paramRow = new Row() { RowIndex = rowIndex++ };
                            paramRow.Append(CreateCell($"{param.Key}: {param.Value}", CellValues.String));
                            sheetData.Append(paramRow);
                        }
                        rowIndex++; // Empty row
                    }

                    if (data.Any())
                    {
                        var properties = typeof(T).GetProperties();

                        // Header row
                        var headerRow = new Row() { RowIndex = rowIndex++ };
                        foreach (var prop in properties)
                        {
                            headerRow.Append(CreateCell(prop.Name, CellValues.String));
                        }
                        sheetData.Append(headerRow);

                        // Data rows
                        foreach (var item in data)
                        {
                            var dataRow = new Row() { RowIndex = rowIndex++ };
                            foreach (var prop in properties)
                            {
                                var value = prop.GetValue(item)?.ToString() ?? "";
                                dataRow.Append(CreateCell(value, CellValues.String));
                            }
                            sheetData.Append(dataRow);
                        }
                    }

                    workbookPart.Workbook.Save();
                }

                return stream.ToArray();
            }
        }

        /// <summary>
        /// Export for printing (returns PDF optimized for printing)
        /// </summary>
        public byte[] ExportForPrint<T>(List<T> data, string reportFileName, string dataSetName, Dictionary<string, object> parameters = null)
        {
            // For print, we use PDF format with print-optimized settings
            return ExportToPdf(data, reportFileName, dataSetName, parameters);
        }

        /// <summary>
        /// Generic method to convert any List<T> to DataTable
        /// </summary>
        private DataTable ConvertToDataTable<T>(List<T> data)
        {
            var dataTable = new DataTable();
            var properties = typeof(T).GetProperties();

            // Add columns
            foreach (var prop in properties)
            {
                var type = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                dataTable.Columns.Add(prop.Name, type);
            }

            // Add rows
            foreach (var item in data)
            {
                var row = dataTable.NewRow();
                foreach (var prop in properties)
                {
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                }
                dataTable.Rows.Add(row);
            }

            return dataTable;
        }

        /// <summary>
        /// Export dynamic data (Dictionary<string, object>) to native Excel format
        /// Specifically designed for AIA API responses
        /// </summary>
        public byte[] ExportDynamicDataToNativeExcel(List<Dictionary<string, object>> data, string sheetName = "Sheet1", Dictionary<string, object> parameters = null)
        {
            using (var stream = new MemoryStream())
            {
                using (var document = SpreadsheetDocument.Create(stream, SpreadsheetDocumentType.Workbook))
                {
                    var workbookPart = document.AddWorkbookPart();
                    workbookPart.Workbook = new Workbook();

                    var worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                    worksheetPart.Worksheet = new Worksheet(new SheetData());

                    var sheets = document.WorkbookPart.Workbook.AppendChild(new Sheets());
                    var sheet = new Sheet() { Id = document.WorkbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = sheetName };
                    sheets.Append(sheet);

                    var sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();
                    uint rowIndex = 1;

                    // Add parameters if provided
                    if (parameters != null)
                    {
                        foreach (var param in parameters)
                        {
                            var paramRow = new Row() { RowIndex = rowIndex++ };
                            paramRow.Append(CreateCell($"{param.Key}: {param.Value}", CellValues.String));
                            sheetData.Append(paramRow);
                        }
                        rowIndex++; // Empty row
                    }

                    if (data.Any())
                    {
                        // Get all unique column names from all data items
                        var allColumns = data.SelectMany(item => item.Keys).Distinct().ToList();

                        // Header row
                        var headerRow = new Row() { RowIndex = rowIndex++ };
                        foreach (var column in allColumns)
                        {
                            headerRow.Append(CreateCell(column, CellValues.String));
                        }
                        sheetData.Append(headerRow);

                        // Data rows
                        foreach (var item in data)
                        {
                            var dataRow = new Row() { RowIndex = rowIndex++ };
                            foreach (var column in allColumns)
                            {
                                var value = item.ContainsKey(column) ? item[column]?.ToString() ?? "" : "";
                                dataRow.Append(CreateCell(value, CellValues.String));
                            }
                            sheetData.Append(dataRow);
                        }
                    }

                    workbookPart.Workbook.Save();
                }

                return stream.ToArray();
            }
        }

        /// <summary>
        /// Helper method to create Excel cells
        /// </summary>
        private Cell CreateCell(string value, CellValues dataType)
        {
            return new Cell()
            {
                CellValue = new CellValue(value),
                DataType = new EnumValue<CellValues>(dataType)
            };
        }
    }

    /// <summary>
    /// Export format enumeration
    /// </summary>
    public enum ExportFormat
    {
        PDF,
        Excel,
        Word,
        NativeExcel,
        Print
    }

    /// <summary>
    /// Export result model
    /// </summary>
    public class ExportResult
    {
        public byte[] FileData { get; set; }
        public string ContentType { get; set; }
        public string FileName { get; set; }
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }

        public static ExportResult CreateSuccess(byte[] fileData, string contentType, string fileName)
        {
            return new ExportResult
            {
                FileData = fileData,
                ContentType = contentType,
                FileName = fileName,
                Success = true
            };
        }

        public static ExportResult CreateError(string errorMessage)
        {
            return new ExportResult
            {
                Success = false,
                ErrorMessage = errorMessage
            };
        }
    }

    /// <summary>
    /// Extension methods for easy export
    /// </summary>
    public static class ReportExportExtensions
    {
        public static ExportResult ExportAs<T>(this List<T> data, ExportFormat format, ReportExportService service, 
            string reportFileName = null, string dataSetName = "DataSet", Dictionary<string, object> parameters = null, string title = "Report")
        {
            try
            {
                byte[] fileData;
                string contentType;
                string fileName;

                switch (format)
                {
                    case ExportFormat.PDF:
                        fileData = service.ExportToPdf(data, reportFileName, dataSetName, parameters);
                        contentType = "application/pdf";
                        fileName = $"{title}_{DateTime.Now:yyyyMMdd}.pdf";
                        break;

                    case ExportFormat.Excel:
                        fileData = service.ExportToExcel(data, reportFileName, dataSetName, parameters);
                        contentType = "application/vnd.ms-excel";
                        fileName = $"{title}_{DateTime.Now:yyyyMMdd}.xls";
                        break;

                    case ExportFormat.NativeExcel:
                        fileData = service.ExportToNativeExcel(data, title, parameters);
                        contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        fileName = $"{title}_{DateTime.Now:yyyyMMdd}.xlsx";
                        break;

                    case ExportFormat.Word:
                        fileData = service.ExportToWord(data, title, parameters);
                        contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                        fileName = $"{title}_{DateTime.Now:yyyyMMdd}.docx";
                        break;

                    case ExportFormat.Print:
                        fileData = service.ExportForPrint(data, reportFileName, dataSetName, parameters);
                        contentType = "application/pdf";
                        fileName = $"{title}_Print_{DateTime.Now:yyyyMMdd}.pdf";
                        break;

                    default:
                        return ExportResult.CreateError("Unsupported export format");
                }

                return ExportResult.CreateSuccess(fileData, contentType, fileName);
            }
            catch (Exception ex)
            {
                return ExportResult.CreateError($"Error during export: {ex.Message}");
            }
        }
    }
} 