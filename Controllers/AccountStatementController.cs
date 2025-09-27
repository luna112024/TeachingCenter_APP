using Microsoft.AspNetCore.Mvc;
using hongWenAPP.Models.ViewStatement.DTOs;
using hongWenAPP.Models;
using hongWenAPP.Services;
using System.Globalization;

namespace hongWenAPP.Controllers
{
    public class AccountStatementController : Controller
    {
        private readonly ReportExportService _reportExportService;
        private readonly AuthenticationService _authService;
        private readonly IAccountStatementService _accountStatementService;
        private readonly ILogger<AccountStatementController> _logger;
        private readonly IIdentityService _identityService;

        public AccountStatementController(ReportExportService reportExportService, 
            AuthenticationService authService, IAccountStatementService accountStatementService,
            ILogger<AccountStatementController> logger, IIdentityService identityService)
        {
            _reportExportService = reportExportService;
            _authService = authService;
            _accountStatementService = accountStatementService;
            _logger = logger;
            _identityService = identityService;
        }

        public IActionResult ViewStatement(ListAccountStatementModel model)
        {
            // Check permission for viewing account statements
            if (!_authService.HasPermission("ViewAccountStatements"))
            {
                return View("AccessDenied");
            }

            var data = new List<AccountStatementModel>();
            var list = new ListAccountStatementModel
            {
                transactions = PageList<AccountStatementModel>.Create(data, model.Page, model.PageSize, "ListStatement"),
            };
            return View(list);
        }

        [HttpGet]
        public async Task<IActionResult> SearchAccounts(string term = "")
        {
            try
            {
                if (!_authService.IsAuthenticated())
                {
                    return Unauthorized(new { error = "User not authenticated" });
                }

                var accountInfo = await _identityService.GetUserAccountInfo();
                
                if (accountInfo == null)
                {
                    _logger.LogWarning("No account information found for user");
                    return Json(new object[0]); 
                }
                
                _logger.LogInformation("Retrieved account info via IdentityService. Company: {CompanyName}, Accounts: {AccountCount}", 
                    accountInfo.CompanyName, accountInfo.Accounts?.Count ?? 0);

                var accounts = accountInfo.Accounts
                    .Where(a => a.IsActive) 
                    .Select(a => new
                    {
                        AccountNo = a.AccountNumber,
                        Currency = a.Currency ?? "",
                        CompanyName = accountInfo.CompanyName,
                        AccID = a.AccID
                    });

                if (!string.IsNullOrEmpty(term))
                {
                    accounts = accounts.Where(a => 
                        a.AccountNo.Contains(term, StringComparison.OrdinalIgnoreCase) || 
                        a.CompanyName.Contains(term, StringComparison.OrdinalIgnoreCase));
                }

                return Json(accounts.ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching accounts with term: {Term}", term);
                return StatusCode(500, new { error = "Failed to search accounts" });
            }
        }



        private (string fromDate, string toDate)? ParseDateRange(string dateRange)
        {
            try
            {
                if (string.IsNullOrEmpty(dateRange))
                    return null;

                _logger.LogInformation("Parsing date range: {DateRange}", dateRange);

                // Expected format: "01/08/2022 - 31/08/2022" (DD/MM/YYYY format from AIA date picker)
                var dates = dateRange.Split(new[] { " - " }, StringSplitOptions.RemoveEmptyEntries);
                if (dates.Length != 2)
                    return null;

                // Try parsing with multiple date formats
                var dateFormats = new[] { "dd/MM/yyyy", "MM/dd/yyyy", "dd/MMM/yyyy" };
                
                DateTime fromDate, toDate;
                bool fromDateParsed = DateTime.TryParseExact(dates[0].Trim(), dateFormats, 
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out fromDate);
                bool toDateParsed = DateTime.TryParseExact(dates[1].Trim(), dateFormats, 
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out toDate);

                // Fallback to general parsing if exact format fails
                if (!fromDateParsed) fromDateParsed = DateTime.TryParse(dates[0].Trim(), out fromDate);
                if (!toDateParsed) toDateParsed = DateTime.TryParse(dates[1].Trim(), out toDate);

                if (fromDateParsed && toDateParsed)
                {
                    // Validate date range is not more than 1 month (31 days)
                    var daysDifference = (toDate - fromDate).TotalDays;
                    if (daysDifference > 31)
                    {
                        _logger.LogWarning("Date range exceeds 1 month limit: {Days} days", daysDifference);
                        throw new ArgumentException("Date range cannot exceed 1 month (31 days). Please select a shorter period.");
                    }

                    if (daysDifference < 0)
                    {
                        _logger.LogWarning("Invalid date range: From date is after To date");
                        throw new ArgumentException("From date cannot be after To date.");
                    }

                    var fromDateFormatted = fromDate.ToString("yyyyMMdd");
                    var toDateFormatted = toDate.ToString("yyyyMMdd");
                    
                    _logger.LogInformation("Converted dates - From: {FromDate}, To: {ToDate}, Days: {Days}", 
                        fromDateFormatted, toDateFormatted, daysDifference);
                    
                    return (fromDateFormatted, toDateFormatted);
                }
                else
                {
                    _logger.LogWarning("Failed to parse dates: {FromDate} and {ToDate}", dates[0], dates[1]);
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Date parsing error for range: {DateRange}", dateRange);
                return null;
            }
        }

        private async Task<List<AccountStatementModel>> GetStatementDataFromAPI(string accountNo, string fromDate, string toDate, string companyName = null)
        {
            var request = new AccountStatementRequest
            {
                AccountNo = accountNo,
                FromDate = fromDate,
                ToDate = toDate,
                CompanyName = companyName ?? "" 
            };

            var response = await _accountStatementService.GetAccountStatementDataAsync(request);

            if (!response.Success)
            {
                throw new Exception(response.Message);
            }

            return response.Data;
        }

        private async Task<List<AccountStatementModel>> GetTransactionsForExport(string accountNo, string daterange, string companyName = null)
        {
            if (string.IsNullOrEmpty(accountNo) || string.IsNullOrEmpty(daterange))
            {
                return new List<AccountStatementModel>();
            }

            var dates = ParseDateRange(daterange);
            if (!dates.HasValue)
            {
                return new List<AccountStatementModel>();
            }

            return await GetStatementDataFromAPI(accountNo, dates.Value.fromDate, dates.Value.toDate, companyName);
        }

        private IActionResult HandleExportError(string exportType, Exception ex, string accountNo, string daterange)
        {
            _logger.LogError(ex, "Error exporting {ExportType} for account {AccountNo}, date range {DateRange}", 
                exportType, accountNo, daterange);

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return StatusCode(500, new { 
                    error = $"Error generating {exportType}: {ex.Message}",
                    exportType = exportType
                });
            }
            else
            {
                TempData["Error"] = $"Error generating {exportType}: {ex.Message}";
                return RedirectToAction("ViewStatement");
            }
        }

        private IActionResult HandlePermissionDenied(string action)
        {
            var message = $"You don't have permission to {action}.";
            
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return StatusCode(403, new { error = message });
            }
            else
            {
                TempData["Error"] = message;
                return RedirectToAction("ViewStatement");
            }
        }

        private IActionResult HandleNoDataFound(string exportType)
        {
            var message = "No transactions found for the specified criteria.";
            
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return BadRequest(new { error = message, exportType = exportType });
            }
            else
            {
                TempData["Error"] = message;
                return RedirectToAction("ViewStatement");
            }
        }

        // Export to PDF using the new ReportExportService
        [HttpPost]
        public async Task<IActionResult> ExportStatementPDF(string accountNo, string daterange, string companyName = null)
        {
            _logger.LogInformation("ExportStatementPDF called with AccountNo: {AccountNo}, DateRange: {DateRange}", accountNo, daterange);
            
            // Check permission for exporting reports
            if (!_authService.HasAnyPermission("ExportReports", "ExportAccountStatements"))
            {
                return HandlePermissionDenied("export reports");
            }

            try
            {
                var transactions = await GetTransactionsForExport(accountNo, daterange, companyName);
                if (!transactions.Any())
                {
                    return HandleNoDataFound("PDF");
                }

                var result = transactions.ExportAs(ExportFormat.PDF, _reportExportService, 
                    "rptCustomerStatement.rdlc", "DataSet", null, "CustomerStatement");

                if (result.Success)
                {
                    _logger.LogInformation("PDF export successful for account {AccountNo}", accountNo);
                    return File(result.FileData, result.ContentType, result.FileName);
                }
                else
                {
                    throw new Exception(result.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                return HandleExportError("PDF", ex, accountNo, daterange);
            }
        }

        // Export to Excel using the new ReportExportService
        [HttpPost]
        public async Task<IActionResult> ExportStatementExcel(string accountNo, string daterange)
        {
            _logger.LogInformation("ExportStatementExcel called with AccountNo: {AccountNo}, DateRange: {DateRange}", accountNo, daterange);
            
            if (!_authService.HasAnyPermission("ExportReports", "ExportAccountStatements"))
            {
                return HandlePermissionDenied("export reports");
            }

            try
            {
                var transactions = await GetTransactionsForExport(accountNo, daterange);
                if (!transactions.Any())
                {
                    return HandleNoDataFound("Excel (RDLC)");
                }

                var result = transactions.ExportAs(ExportFormat.Excel, _reportExportService, 
                    "rptCustomerStatement.rdlc", "DataSet", null, "CustomerStatement");

                if (result.Success)
                {
                    _logger.LogInformation("Excel export successful for account {AccountNo}", accountNo);
                    return File(result.FileData, result.ContentType, result.FileName);
                }
                else
                {
                    throw new Exception(result.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                return HandleExportError("Excel (RDLC)", ex, accountNo, daterange);
            }
        }

        // Export to Word using the new ReportExportService
        [HttpPost]
        public async Task<IActionResult> ExportStatementWord(string accountNo, string daterange)
        {
            _logger.LogInformation("ExportStatementWord called with AccountNo: {AccountNo}, DateRange: {DateRange}", accountNo, daterange);
            
            // Check permission for exporting reports
            if (!_authService.HasAnyPermission("ExportReports", "ExportAccountStatements"))
            {
                return HandlePermissionDenied("export reports");
            }

            try
            {
                var transactions = await GetTransactionsForExport(accountNo, daterange);
                if (!transactions.Any())
                {
                    return HandleNoDataFound("Word");
                }

                var parameters = new Dictionary<string, object>
                {
                    {"Account Number", transactions.FirstOrDefault()?.AccountNo ?? ""},
                    {"Account Holder", transactions.FirstOrDefault()?.NameLatin ?? ""},
                    {"Branch", transactions.FirstOrDefault()?.Branch ?? ""},
                    {"Currency", transactions.FirstOrDefault()?.Currency ?? ""},
                    {"Statement Date", DateTime.Now.ToString("yyyy-MM-dd")}
                };

                var result = transactions.ExportAs(ExportFormat.Word, _reportExportService, 
                    null, "DataSet", parameters, "Account Statement");

                if (result.Success)
                {
                    _logger.LogInformation("Word export successful for account {AccountNo}", accountNo);
                    return File(result.FileData, result.ContentType, result.FileName);
                }
                else
                {
                    throw new Exception(result.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                return HandleExportError("Word", ex, accountNo, daterange);
            }
        }

        // Export to Native Excel using the new ReportExportService
        [HttpPost]
        public async Task<IActionResult> ExportStatementNativeExcel(string accountNo, string daterange)
        {
            _logger.LogInformation("ExportStatementNativeExcel called with AccountNo: {AccountNo}, DateRange: {DateRange}", accountNo, daterange);
            
            // Check permission for exporting reports
            if (!_authService.HasAnyPermission("ExportReports", "ExportAccountStatements"))
            {
                return HandlePermissionDenied("export reports");
            }

            try
            {
                var transactions = await GetTransactionsForExport(accountNo, daterange);
                if (!transactions.Any())
                {
                    return HandleNoDataFound("Excel (Native)");
                }

                var parameters = new Dictionary<string, object>
                {
                    {"Account Number", transactions.FirstOrDefault()?.AccountNo ?? ""},
                    {"Account Holder", transactions.FirstOrDefault()?.NameLatin ?? ""},
                    {"Branch", transactions.FirstOrDefault()?.Branch ?? ""},
                    {"Currency", transactions.FirstOrDefault()?.Currency ?? ""},
                    {"Statement Date", DateTime.Now.ToString("yyyy-MM-dd")}
                };

                var result = transactions.ExportAs(ExportFormat.NativeExcel, _reportExportService, 
                    null, "DataSet", parameters, "Account Statement");

                if (result.Success)
                {
                    _logger.LogInformation("Native Excel export successful for account {AccountNo}", accountNo);
                    return File(result.FileData, result.ContentType, result.FileName);
                }
                else
                {
                    throw new Exception(result.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                return HandleExportError("Excel (Native)", ex, accountNo, daterange);
            }
        }

        // Print Statement using the new ReportExportService
        [HttpPost]
        public async Task<IActionResult> PrintStatement(string accountNo, string daterange)
        {
            _logger.LogInformation("PrintStatement called with AccountNo: {AccountNo}, DateRange: {DateRange}", accountNo, daterange);
            
            // Check permission for printing reports
            if (!_authService.HasAnyPermission("PrintReports", "PrintAccountStatements"))
            {
                return HandlePermissionDenied("print reports");
            }

            try
            {
                var transactions = await GetTransactionsForExport(accountNo, daterange);
                if (!transactions.Any())
                {
                    return HandleNoDataFound("Print");
                }

                var result = transactions.ExportAs(ExportFormat.Print, _reportExportService, 
                    "rptCustomerStatement.rdlc", "DataSet", null, "CustomerStatement_Print");

                if (result.Success)
                {
                    _logger.LogInformation("Print export successful for account {AccountNo}", accountNo);
                    return File(result.FileData, result.ContentType, result.FileName);
                }
                else
                {
                    throw new Exception(result.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                return HandleExportError("Print", ex, accountNo, daterange);
            }
        }
    }
}
