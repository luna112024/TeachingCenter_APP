using hongWenAPP.Helpers;
using hongWenAPP.Models;
using hongWenAPP.Models.Common;
using hongWenAPP.Models.PaymentModel.DTOs;
using hongWenAPP.Services;
using Microsoft.AspNetCore.Mvc;

namespace hongWenAPP.Controllers
{
    public class PaymentController : Controller
    {
        private readonly IPaymentService _paymentService;
        private readonly IStudentService _studentService;
        private readonly ReturnHelper _returnHelper;
        private readonly AuthenticationService _authService;

        public PaymentController(IPaymentService paymentService, IStudentService studentService, ReturnHelper returnHelper, AuthenticationService authService)
        {
            _paymentService = paymentService;
            _studentService = studentService;
            _returnHelper = returnHelper;
            _authService = authService;
        }

        [HttpGet]
        public IActionResult Index(ListPaymentDTOs model)
        {
            // Redirect to NEW payment system
            TempData["InfoMessage"] = "Redirected to NEW Payment System. The old payment system is deprecated.";
            return RedirectToAction("Index", "PaymentNew");
        }

        public async Task<IActionResult> IndexOld(ListPaymentDTOs model)
        {
            if (!_authService.HasPermission("ViewPayment"))
            {
                return PartialView("_AccessDenied");
            }
            var payments = await _paymentService.GetPaymentsByDateRange(
                DateTime.Today.AddDays(-30), 
                DateTime.Today.AddDays(1));
            var list = new ListPaymentDTOs
            {
                Payments = PageList<GetPaymentDTO>.Create(payments, model.Page, model.PageSize, "ListPayment")
            };

            return View(list);
        }

        [HttpGet]
        public async Task<IActionResult> ListPayment(ListPaymentDTOs model)
        {
            if (!_authService.HasPermission("ViewPayment"))
            {
                return PartialView("_AccessDenied");
            }
            var payments = await _paymentService.GetPaymentsByDateRange(
                model.StartDate ?? DateTime.Today.AddDays(-30), 
                model.EndDate ?? DateTime.Today.AddDays(1));
            var list = PageList<GetPaymentDTO>.Create(payments, model.Page, model.PageSize, "ListPayment");
            return PartialView("_ListPayments", list);
        }

        [HttpGet]
        public async Task<IActionResult> FetchPayment(string i)
        {
            if (!_authService.HasPermission("ViewPayment"))
            {
                return PartialView("_AccessDenied");
            }
            var payments = await _paymentService.GetPaymentsByDateRange(
                DateTime.Today.AddDays(-30), 
                DateTime.Today.AddDays(1));
            var result = payments.Select(p => new
            {
                p.PaymentId,
                p.PaymentReference,
                p.StudentName,
                p.Amount,
                p.Currency,
                p.PaymentMethod,
                p.PaymentDate,
                p.Status
            }).ToList();
            return Json(result);
        }

        [HttpGet]
        public async Task<IActionResult> AddPayment()
        {
            if (!_authService.HasPermission("ManagePayment"))
            {
                return PartialView("_AccessDenied");
            }

            return PartialView("_addPayment");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddPayment(CreatePaymentDTO createPaymentDTO)
        {
            try
            {
                if (!_authService.HasPermission("ManagePayment"))
                {
                    return PartialView("_AccessDenied");
                }
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.ToDictionary(kvp => kvp.Key, kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray());
                    return Json(new { validationErrors = errors });
                }
                          
                var result = await _paymentService.CreatePayment(createPaymentDTO);
                return _returnHelper.ReturnNewResult(result.Flag, result.Message);
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetPayment(Guid id)
        {
            try
            {
                if (!_authService.HasPermission("ViewPayment"))
                {
                    return PartialView("_AccessDenied");
                }
                var payment = await _paymentService.GetPayment(id);
                if (payment == null)
                {
                    return _returnHelper.ReturnNewResult(false, "Payment not found.");
                }

                return PartialView("_PaymentDetails", payment);
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error loading payment details: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> VoidPayment(Guid id)
        {
            try
            {
                if (!_authService.HasPermission("ManagePayment"))
                {
                    return PartialView("_AccessDenied");
                }
                var payment = await _paymentService.GetPayment(id);
                if (payment == null)
                {
                    return _returnHelper.ReturnNewResult(false, "Payment not found.");
                }

                return PartialView("_voidPayment", payment);
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error loading void form: {ex.Message}");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VoidPaymentConfirmed(VoidPaymentDTO model)
        {
            try
            {
                if (!_authService.HasPermission("ManagePayment"))
                {
                    return PartialView("_AccessDenied");
                }
                var result = await _paymentService.VoidPayment(model);
                return _returnHelper.ReturnNewResult(result.Flag, result.Message ?? "Payment voided successfully");
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error voiding payment: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> DetailsPayment(Guid id)
        {
            try
            {
                if (!_authService.HasPermission("ViewPayment"))
                {
                    return PartialView("_AccessDenied");
                }
                var payment = await _paymentService.GetPayment(id);
                if (payment == null)
                {
                    return _returnHelper.ReturnNewResult(false, "Payment not found.");
                }

                return PartialView("_detailsPayment", payment);
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error loading payment details: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetStudents(string q = "")
        {
            if (!_authService.HasPermission("ViewStudent"))
            {
                return Json(new List<object>());
            }
            try
            {
                var students = await _studentService.GetAllStudents();
                var result = students.Select(s => new
                {
                    studentId = s.StudentId,
                    studentName = $"{s.FirstName} {s.LastName}" ?? "Unknown",
                    studentCode = s.StudentCode
                }).ToList();

                // Filter by search query if provided
                if (!string.IsNullOrEmpty(q))
                {
                    result = result.Where(s => 
                        s.studentName.ToLower().Contains(q.ToLower()) || 
                        s.studentCode.ToLower().Contains(q.ToLower())
                    ).ToList();
                }

                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new List<object>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetPaymentsByStudent(Guid studentId)
        {
            if (!_authService.HasPermission("ViewPayment"))
            {
                return PartialView("_AccessDenied");
            }
            try
            {
                var history = await _paymentService.GetStudentPaymentHistory(studentId);
                return PartialView("_StudentPaymentHistory", history);
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error loading payment history: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetPaymentHistoryData(Guid id)
        {
            if (!_authService.HasPermission("ViewPayment"))
            {
                return Json(new { error = "Access denied" });
            }
            try
            {
                var history = await _paymentService.GetStudentPaymentHistory(id);
                return Json(history);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }
    }
}

