using hongWenAPP.Helpers;
using hongWenAPP.Models;
using hongWenAPP.Models.PaymentNewModel.DTOs;
using hongWenAPP.Services;
using Microsoft.AspNetCore.Mvc;

namespace hongWenAPP.Controllers
{
    public class PaymentNewController : Controller
    {
        private readonly IPaymentNewService _paymentService;
        private readonly IInvoiceService _invoiceService;
        private readonly IStudentService _studentService;
        private readonly IIdentityService _identityService;
        private readonly ReturnHelper _returnHelper;
        private readonly AuthenticationService _authService;

        public PaymentNewController(
            IPaymentNewService paymentService,
            IInvoiceService invoiceService,
            IStudentService studentService,
            IIdentityService identityService,
            ReturnHelper returnHelper,
            AuthenticationService authService)
        {
            _paymentService = paymentService;
            _invoiceService = invoiceService;
            _studentService = studentService;
            _identityService = identityService;
            _returnHelper = returnHelper;
            _authService = authService;
        }

        // GET: PaymentNew/Index
        [HttpGet]
        public IActionResult Index()
        {
            if (!_authService.HasPermission("ViewPayment"))
            {
                return PartialView("_AccessDenied");
            }

            return View();
        }

        // GET: PaymentNew/RecordPayment/{invoiceId}
        [HttpGet]
        public async Task<IActionResult> RecordPayment(Guid invoiceId)
        {
            if (!_authService.HasPermission("ManagePayment"))
            {
                return PartialView("_AccessDenied");
            }

            // Get invoice details
            var invoice = await _invoiceService.GetInvoiceById(invoiceId);
            if (invoice == null)
            {
                TempData["ErrorMessage"] = "Invoice not found";
                    return RedirectToAction("Index");
            }

            // Get current user for ReceivedBy
            var currentUserId = _authService.GetCurrentUserId() ?? Guid.Empty;

            var model = new CreatePaymentNewDTO
            {
                InvoiceId = invoiceId,
                StudentId = invoice.StudentId,
                Amount = invoice.AmountOutstanding,
                PaymentDate = DateTime.Now,
                PaymentMethod = "Cash",
                ReceivedBy = currentUserId
            };

            ViewBag.Invoice = invoice;
            ViewBag.PaymentMethods = new List<string> { "Cash", "BankTransfer", "ABA", "Wing", "TrueMoney", "Check" };

            return PartialView("_RecordPayment", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RecordPayment(CreatePaymentNewDTO dto)
        {
            try
            {
                if (!_authService.HasPermission("ManagePayment"))
                {
                    return _returnHelper.ReturnNewResult(false, "Access Denied: You don't have permission to record payments.");
                }

                if (!ModelState.IsValid)
                {
                    return _returnHelper.ReturnNewResult(false, "Please fill all required fields.");
                }

                dto.CreatedBy = _authService.GetUserInfo()?.Username ?? "Unknown";

                var result = await _paymentService.RecordPayment(dto);

                if (result.Flag)
                {
                    return _returnHelper.ReturnNewResult(true, "Payment recorded successfully. Status: Pending - Awaiting confirmation.");
                }

                return _returnHelper.ReturnNewResult(false, result.Message);
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error: {ex.Message}");
            }
        }

        // GET: PaymentNew/Details/{paymentId}
        [HttpGet]
        public async Task<IActionResult> Details(Guid paymentId)
        {
            if (!_authService.HasPermission("ViewPayment"))
            {
                return PartialView("_AccessDenied");
            }

            try
            {
                var payment = await _paymentService.GetPaymentById(paymentId);
                if (payment == null)
                {
                    TempData["ErrorMessage"] = "Payment not found";
                    return RedirectToAction("Index");
                }

                // Get audit trail
                ViewBag.AuditTrail = await _paymentService.GetPaymentAudit(paymentId);

                return View(payment);
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error: {ex.Message}");
            }
        }

        // POST: PaymentNew/Confirm/{paymentId}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmPayment(Guid paymentId)
        {
            try
            {
                if (!_authService.HasPermission("ManagePayment"))
                {
                    return _returnHelper.ReturnNewResult(false, "You don't have permission to confirm payments.");
                }

                var currentUserId = _authService.GetCurrentUserId() ?? Guid.Empty;

                var dto = new ConfirmPaymentDTO
                {
                    ConfirmedBy = currentUserId
                };

                var result = await _paymentService.ConfirmPayment(paymentId, dto);

                if (result.Flag)
                {
                    return _returnHelper.ReturnNewResult(true, 
                        "Payment confirmed and LOCKED. It can no longer be edited.");
                }

                return _returnHelper.ReturnNewResult(false, result.Message);
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error: {ex.Message}");
            }
        }

        // GET: PaymentNew/StudentPayments/{studentId}
        [HttpGet]
        public async Task<IActionResult> StudentPayments(Guid studentId)
        {
            if (!_authService.HasPermission("ViewPayment"))
            {
                return PartialView("_AccessDenied");
            }

            var student = await _studentService.GetStudent(studentId);
            var payments = await _paymentService.GetStudentPaymentHistory(studentId);

            ViewBag.Student = student;

            return View(payments);
        }

        // POST: PaymentNew/AddNote/{paymentId}
        [HttpGet]
        public IActionResult AddNote(Guid paymentId)
        {
            if (!_authService.HasPermission("ManagePayment"))
            {
                return PartialView("_AccessDenied");
            }

            ViewBag.PaymentId = paymentId;
            var model = new AddPaymentNoteDTO();
            return PartialView("_AddNote", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddNote(Guid paymentId, AddPaymentNoteDTO dto)
        {
            try
            {
                if (!_authService.HasPermission("ManagePayment"))
                {
                    return _returnHelper.ReturnNewResult(false, "You don't have permission to add notes.");
                }

                dto.ModifiedBy = _authService.GetUserInfo()?.Username ?? "Unknown";

                var result = await _paymentService.AddNoteToPayment(paymentId, dto);

                if (result.Flag)
                {
                    return _returnHelper.ReturnNewResult(true, "Note added to payment.");
                }

                return _returnHelper.ReturnNewResult(false, result.Message);
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error: {ex.Message}");
            }
        }

        // GET: PaymentNew/CreateAdjustment/{originalPaymentId}
        [HttpGet]
        public async Task<IActionResult> CreateAdjustment(Guid originalPaymentId)
        {
            if (!_authService.HasPermission("ManagePayment"))
            {
                return PartialView("_AccessDenied");
            }

            // Only admins can create adjustments - check role
            if (!(_authService.GetUserInfo()?.Roles?.Contains("Admin") ?? false))
            {
                return PartialView("_AccessDenied");
            }

            var originalPayment = await _paymentService.GetPaymentById(originalPaymentId);
            if (originalPayment == null)
            {
                TempData["ErrorMessage"] = "Payment not found";
                    return RedirectToAction("Index");
            }

            var currentUserId = _authService.GetCurrentUserId() ?? Guid.Empty;

            var model = new CreatePaymentAdjustmentDTO
            {
                OriginalPaymentId = originalPaymentId,
                AdjustedBy = currentUserId
            };

            ViewBag.OriginalPayment = originalPayment;

            return PartialView("_CreateAdjustment", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAdjustment(CreatePaymentAdjustmentDTO dto)
        {
            try
            {
                if (!_authService.HasPermission("ManagePayment") || !(_authService.GetUserInfo()?.Roles?.Contains("Admin") ?? false))
                {
                    return _returnHelper.ReturnNewResult(false, "Only admins can create payment adjustments.");
                }

                if (!ModelState.IsValid)
                {
                    return _returnHelper.ReturnNewResult(false, "Please fill all required fields.");
                }

                dto.CreatedBy = _authService.GetUserInfo()?.Username ?? "Unknown";

                var result = await _paymentService.CreatePaymentAdjustment(dto);

                if (result.Flag)
                {
                    return _returnHelper.ReturnNewResult(true, 
                        "Payment adjustment created. Original payment remains unchanged for audit trail.");
                }

                return _returnHelper.ReturnNewResult(false, result.Message);
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error: {ex.Message}");
            }
        }

        // API: Search Payment by Reference
        [HttpGet]
        public async Task<IActionResult> SearchByReference(string reference)
        {
            if (!_authService.HasPermission("ViewPayment"))
            {
                return Json(new { success = false, message = "Access denied" });
            }

            try
            {
                var payment = await _paymentService.GetPaymentByReference(reference);
                return Json(new { success = true, data = payment });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}

