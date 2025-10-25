using hongWenAPP.Helpers;
using hongWenAPP.Models;
using hongWenAPP.Models.InvoiceModel.DTOs;
using hongWenAPP.Services;
using Microsoft.AspNetCore.Mvc;

namespace hongWenAPP.Controllers
{
    public class InvoiceController : Controller
    {
        private readonly IInvoiceService _invoiceService;
        private readonly IStudentService _studentService;
        private readonly ISupplyService _supplyService;
        private readonly ReturnHelper _returnHelper;
        private readonly AuthenticationService _authService;

        public InvoiceController(
            IInvoiceService invoiceService,
            IStudentService studentService,
            ISupplyService supplyService,
            ReturnHelper returnHelper,
            AuthenticationService authService)
        {
            _invoiceService = invoiceService;
            _studentService = studentService;
            _supplyService = supplyService;
            _returnHelper = returnHelper;
            _authService = authService;
        }

        // GET: Invoice/Index
        [HttpGet]
        public IActionResult Index()
        {
            if (!_authService.HasPermission("ViewInvoice"))
            {
                return PartialView("_AccessDenied");
            }

            return View();
        }

        // GET: Invoice/ListInvoices
        [HttpGet]
        public async Task<IActionResult> ListInvoices(string? studentId, string? status, string? invoiceType)
        {
            if (!_authService.HasPermission("ViewInvoice"))
            {
                return PartialView("_AccessDenied");
            }

            Guid? studentGuid = null;
            if (!string.IsNullOrEmpty(studentId) && Guid.TryParse(studentId, out var parsedId))
            {
                studentGuid = parsedId;
            }

            var invoices = await _invoiceService.GetAllInvoices(studentGuid, status, invoiceType);
            return PartialView("_ListInvoices", invoices);
        }

        // GET: Invoice/Details/{id}
        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            if (!_authService.HasPermission("ViewInvoice"))
            {
                return PartialView("_AccessDenied");
            }

            try
            {
                var invoice = await _invoiceService.GetInvoiceById(id);
                if (invoice == null)
                {
                    TempData["ErrorMessage"] = "Invoice not found";
                    return RedirectToAction("Index");
                }

                return View(invoice);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Index");
            }
        }

        // GET: Invoice/StudentInvoices/{studentId}
        [HttpGet]
        public async Task<IActionResult> StudentInvoices(Guid studentId)
        {
            if (!_authService.HasPermission("ViewInvoice"))
            {
                return PartialView("_AccessDenied");
            }

            var student = await _studentService.GetStudent(studentId);
            var invoices = await _invoiceService.GetStudentInvoices(studentId);

            ViewBag.Student = student;
            ViewBag.StudentId = studentId;

            return View(invoices);
        }

        // GET: Invoice/Outstanding
        [HttpGet]
        public async Task<IActionResult> Outstanding(Guid? studentId)
        {
            if (!_authService.HasPermission("ViewInvoice"))
            {
                return PartialView("_AccessDenied");
            }

            var invoices = await _invoiceService.GetOutstandingInvoices(studentId);
            
            if (studentId.HasValue)
            {
                ViewBag.Student = await _studentService.GetStudent(studentId.Value);
            }

            return View(invoices);
        }

        // GET: Invoice/Overdue
        [HttpGet]
        public async Task<IActionResult> Overdue()
        {
            if (!_authService.HasPermission("ViewInvoice"))
            {
                return PartialView("_AccessDenied");
            }

            var invoices = await _invoiceService.GetOverdueInvoices();
            return View(invoices);
        }

        // GET: Invoice/AddLineItem/{invoiceId}
        [HttpGet]
        public async Task<IActionResult> AddLineItem(Guid invoiceId)
        {
            if (!_authService.HasPermission("ManageInvoice"))
            {
                return PartialView("_AccessDenied");
            }

            ViewBag.InvoiceId = invoiceId;
            ViewBag.Supplies = await _supplyService.GetAllSupplies(status: "Active");

            var model = new AddInvoiceLineItemDTO();
            return PartialView("_AddLineItem", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddLineItem(Guid invoiceId, AddInvoiceLineItemDTO dto)
        {
            try
            {
                if (!_authService.HasPermission("ManageInvoice"))
                {
                    return _returnHelper.ReturnNewResult(false, "Access Denied: You don't have permission to modify invoices.");
                }

                if (!ModelState.IsValid)
                {
                    return _returnHelper.ReturnNewResult(false, "Please fill all required fields.");
                }

                var result = await _invoiceService.AddLineItem(invoiceId, dto);

                return _returnHelper.ReturnNewResult(result.Flag, result.Message ?? "Line item added to invoice.");
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error: {ex.Message}");
            }
        }

        // GET: Invoice/ApplyDiscount/{invoiceId}
        [HttpGet]
        public IActionResult ApplyDiscount(Guid invoiceId)
        {
            if (!_authService.HasPermission("ManageInvoice"))
            {
                return PartialView("_AccessDenied");
            }

            ViewBag.InvoiceId = invoiceId;
            var model = new ApplyDiscountDTO();
            return PartialView("_ApplyDiscount", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApplyDiscount(Guid invoiceId, ApplyDiscountDTO dto)
        {
            try
            {
                if (!_authService.HasPermission("ManageInvoice"))
                {
                    return _returnHelper.ReturnNewResult(false, "Access Denied: You don't have permission to apply discounts.");
                }

                dto.ModifiedBy = _authService.GetUserInfo()?.Username ?? "Unknown";

                var result = await _invoiceService.ApplyDiscount(invoiceId, dto);

                return _returnHelper.ReturnNewResult(result.Flag, result.Message ?? "Discount applied successfully.");
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error: {ex.Message}");
            }
        }

        // API Endpoint: Get Outstanding Balance
        [HttpGet]
        public async Task<IActionResult> GetOutstandingBalance(Guid studentId)
        {
            if (!_authService.HasPermission("ViewInvoice"))
            {
                return Json(new { success = false, message = "Access denied" });
            }

            try
            {
                var balance = await _invoiceService.GetStudentOutstandingBalance(studentId);
                return Json(new { success = true, data = balance });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}

