using Microsoft.AspNetCore.Mvc;
using hongWenAPP.Models.ViewStatement.DTOs;
using hongWenAPP.Services;

namespace hongWenAPP.Controllers
{
    public class AIAStatementController : Controller
    {
        private readonly ReportExportService _reportExportService;
        private readonly AuthenticationService _authService;
        private readonly IAIAStatementService _aiaStatementService;
        private readonly ILogger<AIAStatementController> _logger;

        public AIAStatementController(ReportExportService reportExportService,
            AuthenticationService authService, IAIAStatementService aiaStatementService,
            ILogger<AIAStatementController> logger)
        {
            _reportExportService = reportExportService;
            _authService = authService;
            _aiaStatementService = aiaStatementService;
            _logger = logger;
        }

        public IActionResult Index()
        {
            // Check permission for viewing account statements
            if (!_authService.HasPermission("ViewAIAReports"))
            {
                return View("AccessDenied");
            }

            return View();
        }

        // AIA Export to Excel
        public async Task<IActionResult> ExportAIAStatementExcel(string periodDate, string transactionType, string accountNo = null)
        {
            // Check if this is an AJAX request
            bool isAjaxRequest = Request.Headers.XRequestedWith == "XMLHttpRequest" || 
                               Request.Headers.Accept.ToString().Contains("application/json");

            // Check permission
            if (!_authService.HasAnyPermission("ExportReports", "ExportAIAReports"))
            {
                var errorMessage = "You don't have permission to export reports.";
                if (isAjaxRequest)
                {
                    return Json(new { success = false, message = errorMessage });
                }
                TempData["Error"] = errorMessage;
                return RedirectToAction("Index");
            }

            try
            {
                _logger.LogInformation("Starting AIA Statement Excel export - PeriodDate: {PeriodDate}, TransactionType: {TransactionType}, AccountNo: {AccountNo}", 
                    periodDate, transactionType, accountNo);

                // Validate required parameters
                if (string.IsNullOrEmpty(periodDate) || string.IsNullOrEmpty(transactionType))
                {
                    var errorMessage = "Period date and transaction type are required.";
                    if (isAjaxRequest)
                    {
                        return Json(new { success = false, message = errorMessage });
                    }
                    TempData["Error"] = errorMessage;
                    return RedirectToAction("Index");
                }

                var model = new AIAStatementModel
                {
                    PeriodDate = periodDate,
                    TransactionType = transactionType,
                    AccountNo = accountNo
                };

                _logger.LogInformation("Calling AIA statement service...");
                var response = await _aiaStatementService.GetAIAStatementDataAsync(model);
                _logger.LogInformation("AIA service response - Success: {Success}, Message: {Message}, DataCount: {Count}", 
                    response.Success, response.Message, response.Data?.Count ?? 0);

                if (!response.Success)
                {
                    var errorMessage = $"Service error: {response.Message}";
                    _logger.LogWarning("AIA service failed: {Message}", errorMessage);
                    
                    if (isAjaxRequest)
                {
                        return Json(new { success = false, message = errorMessage });
                    }
                    TempData["Error"] = errorMessage;
                    return RedirectToAction("Index");
                }

                if (!response.Data.Any())
                {
                    var errorMessage = "No data found for the specified criteria.";
                    _logger.LogWarning("No data returned from AIA service");
                    
                    if (isAjaxRequest)
                    {
                        return Json(new { success = false, message = errorMessage });
                    }
                    TempData["Error"] = errorMessage;
                    return RedirectToAction("Index");
                }

                // Convert dynamic data to a format suitable for Excel export
                var exportData = response.Data.Select(item => item.Properties).ToList();
                _logger.LogInformation("Converting {Count} records for Excel export", exportData.Count);

                // Create parameters for the export
                var parameters = new Dictionary<string, object>
                {
                    {"Period Date", periodDate},
                    {"Transaction Type", transactionType},
                    {"Generated Date", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")},
                    {"Total Records", response.Data.Count}
                };

                if (!string.IsNullOrEmpty(accountNo))
                {
                    parameters.Add("Account Number", accountNo);
                }

                _logger.LogInformation("Calling ReportExportService to generate Excel...");
                // Use the specialized dynamic data export method
                var fileData = _reportExportService.ExportDynamicDataToNativeExcel(exportData, $"AIA_{transactionType}", parameters);
                var fileName = $"AIA_Statement_{transactionType}_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                _logger.LogInformation("AIA Excel export completed successfully - FileName: {FileName}, DataSize: {DataSize} bytes", 
                    fileName, fileData?.Length ?? 0);

                if (isAjaxRequest)
                {
                    // For AJAX requests, return success with download URL
                    var downloadUrl = Url.Action("DownloadAIAExcel", new { 
                        periodDate, transactionType, accountNo 
                    });
                    return Json(new { 
                        success = true, 
                        message = "Excel export completed successfully",
                        downloadUrl = downloadUrl,
                        fileName = fileName
                    });
                }
                else
                {
                    // For regular requests, return file directly
                    return File(fileData, contentType, fileName);
                }
            }
            catch (Exception ex)
            {
                var errorMessage = $"Error generating Excel: {ex.Message}";
                _logger.LogError(ex, "Error during AIA Excel export - StackTrace: {StackTrace}", ex.StackTrace);
                
                if (isAjaxRequest)
                {
                    return Json(new { success = false, message = errorMessage });
                }
                TempData["Error"] = errorMessage;
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public async Task<IActionResult> DownloadAIAExcel(string periodDate, string transactionType, string accountNo = null)
        {
            try
            {
                var model = new AIAStatementModel
                {
                    PeriodDate = periodDate,
                    TransactionType = transactionType,
                    AccountNo = accountNo
                };

                var response = await _aiaStatementService.GetAIAStatementDataAsync(model);

                if (!response.Success || !response.Data.Any())
                {
                    return NotFound("No data found for download.");
                }

                var exportData = response.Data.Select(item => item.Properties).ToList();

                var parameters = new Dictionary<string, object>
                {
                    {"Period Date", periodDate},
                    {"Transaction Type", transactionType},
                    {"Generated Date", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")},
                    {"Total Records", response.Data.Count}
                };

                if (!string.IsNullOrEmpty(accountNo))
                {
                    parameters.Add("Account Number", accountNo);
                }

                var fileData = _reportExportService.ExportDynamicDataToNativeExcel(exportData, $"AIA_{transactionType}", parameters);
                var fileName = $"AIA_Statement_{transactionType}_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                return File(fileData, contentType, fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during AIA Excel download");
                return BadRequest("Error downloading file.");
            }
        }
    }
}
