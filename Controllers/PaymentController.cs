using hongWenAPP.Helpers;
using hongWenAPP.Models;
using hongWenAPP.Models.Common;
using hongWenAPP.Models.PaymentModel.DTOs;
using hongWenAPP.Services;
using Microsoft.AspNetCore.Mvc;

namespace hongWenAPP.Controllers
{
    /// <summary>
    /// Simple Payment Controller - Cambodia Standard Practice
    /// - Immutable payments (NO editing after creation)
    /// - Void only for mistakes (original stays in system)
    /// - Support: Cash, Bank Transfer, ABA, Wing, TrueMoney
    /// </summary>
    public class PaymentController : Controller
    {
        private readonly IPaymentService _paymentService;
        private readonly IStudentService _studentService;
        private readonly ReturnHelper _returnHelper;
        private readonly AuthenticationService _authService;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(
            IPaymentService paymentService, 
            IStudentService studentService, 
            ReturnHelper returnHelper, 
            AuthenticationService authService,
            ILogger<PaymentController> logger)
        {
            _paymentService = paymentService;
            _studentService = studentService;
            _returnHelper = returnHelper;
            _authService = authService;
            _logger = logger;
        }

        // ========================================
        // MAIN PAGE & LISTING
        // ========================================
        
        [HttpGet]
        public async Task<IActionResult> Index(ListPaymentDTOs model)
        {
            if (!_authService.HasPermission("ViewPayment"))
            {
                return PartialView("_AccessDenied");
            }

            try
            {
                _logger.LogInformation("Loading payments index page");
                var payments = await _paymentService.GetPaymentsByDateRange(
                    DateTime.Today.AddDays(-30), 
                    DateTime.Today.AddDays(1));

                var list = new ListPaymentDTOs
                {
                    Payments = PageList<GetPaymentDTO>.Create(payments, model.Page, model.PageSize, "ListPayment")
                };

                return View(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load payments index page");
                return View("Error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> ListPayment(ListPaymentDTOs model)
        {
            if (!_authService.HasPermission("ViewPayment"))
            {
                return PartialView("_AccessDenied");
            }

            try
            {
                _logger.LogInformation("Listing payments");
                var payments = await _paymentService.GetPaymentsByDateRange(
                    model.StartDate ?? DateTime.Today.AddDays(-30), 
                    model.EndDate ?? DateTime.Today.AddDays(1));

                var list = PageList<GetPaymentDTO>.Create(payments, model.Page, model.PageSize, "ListPayment");
                return PartialView("_ListPayments", list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to list payments");
                return PartialView("_Error");
            }
        }

        // ========================================
        // VIEW PAYMENT DETAILS
        // ========================================
        
        [HttpGet]
        public async Task<IActionResult> GetPayment(Guid paymentId)
        {
            if (!_authService.HasPermission("ViewPayment"))
            {
                return PartialView("_AccessDenied");
            }

            try
            {
                _logger.LogInformation("Retrieving payment {PaymentId}", paymentId);
                var payment = await _paymentService.GetPayment(paymentId);
                return PartialView("_PaymentDetails", payment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve payment {PaymentId}", paymentId);
                return PartialView("_Error");
            }
        }

        // ========================================
        // STUDENT PAYMENT HISTORY
        // ========================================
        
        [HttpGet]
        public async Task<IActionResult> GetPaymentsByStudent(Guid studentId)
        {
            if (!_authService.HasPermission("ViewPayment"))
            {
                return PartialView("_AccessDenied");
            }

            try
            {
                _logger.LogInformation("Retrieving payment history for student {StudentId}", studentId);
                var history = await _paymentService.GetStudentPaymentHistory(studentId);
                return PartialView("_StudentPaymentHistory", history);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve payment history for student {StudentId}", studentId);
                return PartialView("_Error");
            }
        }

        // ========================================
        // RECORD NEW PAYMENT (IMMUTABLE)
        // ========================================
        
        [HttpGet]
        public async Task<IActionResult> AddPayment(Guid? studentId = null)
        {
            if (!_authService.HasPermission("ManagePayment"))
            {
                return PartialView("_AccessDenied");
            }

            try
            {
                _logger.LogInformation("Loading add payment form for student {StudentId}", studentId);
                ViewBag.StudentId = studentId;
                return PartialView("_addPayment");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load add payment form");
                return PartialView("_Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddPayment(CreatePaymentDTO paymentDto)
        {
            if (!_authService.HasPermission("ManagePayment"))
            {
                return Json(new Response { Flag = false, Message = "Access denied" });
            }

            if (!ModelState.IsValid)
            {
                return Json(new Response { Flag = false, Message = "Invalid payment data" });
            }

            try
            {

                _logger.LogInformation("Recording payment for student {StudentId}, Amount: {Amount} {Currency} by {Username}", 
                    paymentDto.StudentId, paymentDto.Amount, paymentDto.Currency, paymentDto.CreatedBy);

                var response = await _paymentService.CreatePayment(paymentDto);
                
                if (response.Flag)
                {
                    _logger.LogInformation("Payment recorded successfully: {Message}", response.Message);
                }
                else
                {
                    _logger.LogWarning("Payment recording failed: {Message}", response.Message);
                }

                return Json(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to record payment for student {StudentId}", paymentDto.StudentId);
                return Json(new Response { Flag = false, Message = "Failed to record payment" });
            }
        }

        // ========================================
        // VOID PAYMENT (For Mistakes Only)
        // ========================================
        
        [HttpGet]
        public async Task<IActionResult> VoidPayment(Guid paymentId)
        {
            if (!_authService.HasPermission("ManagePayment"))
            {
                return PartialView("_AccessDenied");
            }

            try
            {
                _logger.LogInformation("Loading void payment form for payment {PaymentId}", paymentId);
                var payment = await _paymentService.GetPayment(paymentId);
                
                if (payment.Status == "Voided")
                {
                    return PartialView("_Error", "This payment is already voided");
                }

                return PartialView("_voidPayment", payment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load void payment form for payment {PaymentId}", paymentId);
                return PartialView("_Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> VoidPayment(VoidPaymentDTO voidDto)
        {
            if (!_authService.HasPermission("ManagePayment"))
            {
                return Json(new Response { Flag = false, Message = "Access denied" });
            }

            if (!ModelState.IsValid)
            {
                return Json(new Response { Flag = false, Message = "Invalid void data" });
            }

            try
            {

                _logger.LogWarning("Voiding payment {PaymentId} by {Username}. Reason: {Reason}", 
                    voidDto.PaymentId, voidDto.VoidedBy, voidDto.VoidReason);

                var response = await _paymentService.VoidPayment(voidDto);
                
                if (response.Flag)
                {
                    _logger.LogWarning("Payment {PaymentId} voided successfully by {Username}", 
                        voidDto.PaymentId, voidDto.VoidedBy);
                }
                else
                {
                    _logger.LogWarning("Void payment failed: {Message}", response.Message);
                }

                return Json(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to void payment {PaymentId}", voidDto.PaymentId);
                return Json(new Response { Flag = false, Message = "Failed to void payment" });
            }
        }

        // ========================================
        // PAYMENT REPORTS
        // ========================================
        
        [HttpGet]
        public async Task<IActionResult> DailyReport(DateTime? reportDate = null)
        {
            if (!_authService.HasPermission("ViewPayment"))
            {
                return PartialView("_AccessDenied");
            }

            try
            {
                var date = reportDate ?? DateTime.Today;
                _logger.LogInformation("Generating daily payment report for {ReportDate}", date);
                
                var report = await _paymentService.GetDailyReport(date);
                return PartialView("_DailyPaymentReport", report);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to generate daily payment report");
                return PartialView("_Error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> DateRangeReport(DateTime? startDate = null, DateTime? endDate = null)
        {
            if (!_authService.HasPermission("ViewPayment"))
            {
                return PartialView("_AccessDenied");
            }

            try
            {
                var start = startDate ?? DateTime.Today.AddDays(-30);
                var end = endDate ?? DateTime.Today;

                _logger.LogInformation("Generating payment report from {StartDate} to {EndDate}", start, end);
                var payments = await _paymentService.GetPaymentsByDateRange(start, end);
                
                return PartialView("_PaymentReport", payments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to generate payment report");
                return PartialView("_Error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> RevenueReport(DateTime? startDate = null, DateTime? endDate = null)
        {
            if (!_authService.HasPermission("ViewPayment"))
            {
                return Json(new { TotalRevenue = 0, Error = "Access denied" });
            }

            try
            {
                _logger.LogInformation("Retrieving revenue report from {StartDate} to {EndDate}", startDate, endDate);
                var revenue = await _paymentService.GetTotalRevenue(startDate, endDate);
                return Json(new { TotalRevenue = revenue });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve revenue report");
                return Json(new { TotalRevenue = 0, Error = "Failed to retrieve revenue" });
            }
        }

        // ========================================
        // JSON HELPERS FOR DROPDOWNS
        // ========================================
        
        [HttpGet]
        public async Task<IActionResult> GetStudents(string q = "")
        {
            try
            {
                _logger.LogInformation("Retrieving students for dropdown with query: {Query}", q);
                var students = await _studentService.GetAllStudents();
                
                if (!string.IsNullOrEmpty(q))
                {
                    students = students.Where(s => 
                        s.FirstName.Contains(q, StringComparison.OrdinalIgnoreCase) ||
                        s.LastName.Contains(q, StringComparison.OrdinalIgnoreCase) ||
                        s.StudentCode.Contains(q, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                }

                return Json(students.Select(s => new { 
                    StudentId = s.StudentId, 
                    StudentName = $"{s.FirstName} {s.LastName}",
                    StudentCode = s.StudentCode
                }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve students for dropdown");
                return Json(new List<object>());
            }
        }
    }
}
