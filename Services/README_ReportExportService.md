# ReportExportService Class Library

## Overview
The `ReportExportService` is a reusable class library that provides easy export functionality for reports in multiple formats: PDF, Excel (RDLC), Excel (Native), Word, and Print.

## Features
- **PDF Export**: Uses RDLC reports for professional PDF generation
- **Excel Export**: Two options available:
  - RDLC-based Excel export for formatted reports
  - Native Excel export using OpenXML for better compatibility
- **Word Export**: Creates Word documents using OpenXML
- **Print Export**: PDF optimized for printing
- **Generic Support**: Works with any `List<T>` data type
- **Parameter Support**: Allows passing custom parameters to reports
- **Extension Methods**: Provides fluent API for easy usage

## Required NuGet Packages
```xml
<PackageReference Include="Microsoft.ReportingServices.ReportViewerControl.Winforms" Version="150.1652.0" />
<PackageReference Include="DocumentFormat.OpenXml" Version="2.20.0" />
```

## Registration (Program.cs)
```csharp
// Register Report Export Service
builder.Services.AddScoped<ReportExportService>();
```

## Usage Examples

### 1. Basic PDF Export (RDLC)
```csharp
public IActionResult ExportToPDF()
{
    var data = GetSampleData();
    var result = data.ExportAs(ExportFormat.PDF, _reportExportService, 
        "MyReport.rdlc", "DataSet", null, "MyReport");
    
    if (result.Success)
        return File(result.FileData, result.ContentType, result.FileName);
    
    TempData["Error"] = result.ErrorMessage;
    return RedirectToAction("Index");
}
```

### 2. Excel Export with Parameters
```csharp
public IActionResult ExportToExcel()
{
    var data = GetSampleData();
    var parameters = new Dictionary<string, object>
    {
        {"ReportTitle", "Monthly Statement"},
        {"GeneratedDate", DateTime.Now.ToString("yyyy-MM-dd")},
        {"UserName", "John Doe"}
    };
    
    var result = data.ExportAs(ExportFormat.Excel, _reportExportService, 
        "MyReport.rdlc", "DataSet", parameters, "MonthlyStatement");
    
    return File(result.FileData, result.ContentType, result.FileName);
}
```

### 3. Native Excel Export (No RDLC Required)
```csharp
public IActionResult ExportToNativeExcel()
{
    var data = GetSampleData();
    var parameters = new Dictionary<string, object>
    {
        {"Report Title", "Account Statement"},
        {"Generated On", DateTime.Now}
    };
    
    var result = data.ExportAs(ExportFormat.NativeExcel, _reportExportService, 
        null, "DataSet", parameters, "AccountStatement");
    
    return File(result.FileData, result.ContentType, result.FileName);
}
```

### 4. Word Document Export
```csharp
public IActionResult ExportToWord()
{
    var data = GetSampleData();
    var parameters = new Dictionary<string, object>
    {
        {"Document Title", "Account Statement Report"},
        {"Account Number", "4001234567890123"},
        {"Statement Period", "January 2024"}
    };
    
    var result = data.ExportAs(ExportFormat.Word, _reportExportService, 
        null, "DataSet", parameters, "Account Statement");
    
    return File(result.FileData, result.ContentType, result.FileName);
}
```

### 5. Direct Service Usage (Alternative Method)
```csharp
public IActionResult CustomExport()
{
    var data = GetSampleData();
    
    // Direct service method calls
    var pdfData = _reportExportService.ExportToPdf(data, "report.rdlc", "DataSet");
    var excelData = _reportExportService.ExportToNativeExcel(data, "Sheet1");
    var wordData = _reportExportService.ExportToWord(data, "My Report");
    
    return File(pdfData, "application/pdf", "report.pdf");
}
```

## Controller Implementation Example

```csharp
public class ReportsController : Controller
{
    private readonly ReportExportService _reportExportService;
    
    public ReportsController(ReportExportService reportExportService)
    {
        _reportExportService = reportExportService;
    }
    
    public IActionResult ExportData(string format)
    {
        var data = GetData(); // Your data source
        
        ExportFormat exportFormat = format.ToLower() switch
        {
            "pdf" => ExportFormat.PDF,
            "excel" => ExportFormat.Excel,
            "nativeexcel" => ExportFormat.NativeExcel,
            "word" => ExportFormat.Word,
            "print" => ExportFormat.Print,
            _ => ExportFormat.PDF
        };
        
        var result = data.ExportAs(exportFormat, _reportExportService, 
            "MyReport.rdlc", "DataSet", null, "MyReport");
        
        if (result.Success)
            return File(result.FileData, result.ContentType, result.FileName);
        
        return BadRequest(result.ErrorMessage);
    }
}
```

## Available Export Formats

| Format | Description | Requires RDLC | Output Extension |
|--------|-------------|---------------|------------------|
| `ExportFormat.PDF` | Professional PDF using RDLC | Yes | .pdf |
| `ExportFormat.Excel` | Excel using RDLC formatting | Yes | .xls |
| `ExportFormat.NativeExcel` | Native Excel using OpenXML | No | .xlsx |
| `ExportFormat.Word` | Word document using OpenXML | No | .docx |
| `ExportFormat.Print` | Print-optimized PDF | Yes | .pdf |

## Error Handling
The service returns an `ExportResult` object that contains:
- `Success`: Boolean indicating if export was successful
- `FileData`: Byte array of the exported file
- `ContentType`: MIME type for the file
- `FileName`: Suggested filename with timestamp
- `ErrorMessage`: Error details if export failed

## Best Practices
1. **RDLC Reports**: Place your .rdlc files in `wwwroot/Reports/` directory
2. **Error Handling**: Always check `result.Success` before returning files
3. **Memory Management**: The service handles memory cleanup automatically
4. **Parameters**: Use parameters for dynamic report content
5. **File Names**: The service automatically appends timestamps to prevent conflicts

## Troubleshooting
- **RDLC Not Found**: Ensure .rdlc files are in `wwwroot/Reports/` directory
- **OpenXML Errors**: Verify DocumentFormat.OpenXml package is installed
- **Memory Issues**: For large datasets, consider pagination or streaming
- **Permission Errors**: Ensure proper file system permissions for temp directories 