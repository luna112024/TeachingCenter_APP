using hongWenAPP.Helpers;
using hongWenAPP.Models;
using hongWenAPP.Models.FeeModel.DTOs;
using hongWenAPP.Services;
using Microsoft.AspNetCore.Mvc;

namespace hongWenAPP.Controllers
{
    public class StudentFeeController : Controller
    {
        private readonly IFeeService _feeService;
        private readonly IStudentService _studentService;
        private readonly IEnrollmentService _enrollmentService;
        private readonly ReturnHelper _returnHelper;
        private readonly AuthenticationService _authService;

        public StudentFeeController(IFeeService feeService, IStudentService studentService, IEnrollmentService enrollmentService, ReturnHelper returnHelper, AuthenticationService authService)
        {
            _feeService = feeService;
            _studentService = studentService;
            _enrollmentService = enrollmentService;
            _returnHelper = returnHelper;
            _authService = authService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(ListStudentFeeDTOs model)
        {
            if (!_authService.HasPermission("ViewFee"))
            {
                return PartialView("_AccessDenied");
            }
            var studentFees = await _feeService.GetPendingFees();
            var list = new ListStudentFeeDTOs
            {
                studentFee = PageList<GetStudentFeeDTO>.Create(studentFees, model.Page, model.PageSize, "ListStudentFee")
            };

            return View(list);
        }

        [HttpGet]
        public async Task<IActionResult> ListStudentFee(ListStudentFeeDTOs model)
        {
            if (!_authService.HasPermission("ViewFee"))
            {
                return PartialView("_AccessDenied");
            }
            var studentFees = await _feeService.GetPendingFees();
            var list = PageList<GetStudentFeeDTO>.Create(studentFees, model.Page, model.PageSize, "ListStudentFee");
            return PartialView("_ListStudentFees", list);
        }

        [HttpGet]
        public async Task<IActionResult> FetchStudentFee(string i)
        {
            if (!_authService.HasPermission("ViewFee"))
            {
                return PartialView("_AccessDenied");
            }
            var studentFees = await _feeService.GetPendingFees();
            var result = studentFees.Select(sf => new
            {
                sf.StudentFeeId,
                sf.StudentName,
                sf.StudentCode,
                sf.FeeName,
                sf.FeeType,
                sf.FinalAmount,
                sf.Currency,
                sf.DueDate,
                sf.Status
            }).ToList();
            return Json(result);
        }

        [HttpGet]
        public async Task<IActionResult> AssignFeeToStudent()
        {
            if (!_authService.HasPermission("ManageFee"))
            {
                return PartialView("_AccessDenied");
            }
            
            // Get students and fee templates for dropdowns
            var students = await _studentService.GetAllStudents();
            var templates = await _feeService.GetAllFeeTemplates();
            
            ViewBag.AvailableStudents = students;
            ViewBag.AvailableTemplates = templates;
            
            var model = new AssignFeeToStudentDTO
            {
                FeeName = "",
                FeeType = "Tuition",
                DueDate = DateTime.Today.AddDays(7),
                Status = "Pending",
                Currency = "USD"
            };
            return PartialView("_assignFeeToStudent", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignFeeToStudent(AssignFeeToStudentDTO assignFeeToStudentDTO)
        {
            try
            {
                if (!_authService.HasPermission("ManageFee"))
                {
                    return PartialView("_AccessDenied");
                }
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.ToDictionary(kvp => kvp.Key, kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray());
                    return Json(new { validationErrors = errors });
                }  
                var result = await _feeService.AssignFeeToStudent(assignFeeToStudentDTO);
                return _returnHelper.ReturnNewResult(result.Flag, result.Message);
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditStudentFee(Guid id)
        {
            try
            {
                if (!_authService.HasPermission("ManageFee"))
                {
                    return PartialView("_AccessDenied");
                }
                
                var studentFeeData = await _feeService.GetStudentFee(id);

                var studentFeeUpdateDTO = new UpdateStudentFeeDTO
                {
                    StudentFeeId = studentFeeData.StudentFeeId,
                    StudentId = studentFeeData.StudentId,
                    EnrollmentId = studentFeeData.EnrollmentId,
                    TemplateId = studentFeeData.TemplateId,
                    FeeName = studentFeeData.FeeName,
                    FeeType = studentFeeData.FeeType,
                    OriginalAmount = studentFeeData.OriginalAmount,
                    DiscountAmount = studentFeeData.DiscountAmount,
                    DiscountReason = studentFeeData.DiscountReason,
                    FinalAmount = studentFeeData.FinalAmount,
                    Currency = studentFeeData.Currency,
                    DueDate = studentFeeData.DueDate,
                    GracePeriodDays = studentFeeData.GracePeriodDays,
                    LateFeeApplied = studentFeeData.LateFeeApplied,
                    Status = studentFeeData.Status,
                    AmountPaid = studentFeeData.AmountPaid,
                    AmountOutstanding = studentFeeData.AmountOutstanding,
                    InvoiceNumber = studentFeeData.InvoiceNumber,
                    Notes = studentFeeData.Notes,
                    ModifiedBy = studentFeeData.ModifiedBy
                };

                ViewBag.StudentFeeId = id;
                ViewBag.StudentName = studentFeeData.StudentName;
                ViewBag.StudentCode = studentFeeData.StudentCode;
                return PartialView("_editStudentFee", studentFeeUpdateDTO);
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error retrieving Student Fee: {ex.Message}");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditStudentFee(UpdateStudentFeeDTO model)
        {
            try
            {
                if (!_authService.HasPermission("ManageFee"))
                {
                    return PartialView("_AccessDenied");
                }
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.ToDictionary(kvp => kvp.Key, kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray());
                    return Json(new { validationErrors = errors });
                }

                var result = await _feeService.UpdateStudentFee(model);
                return _returnHelper.ReturnNewResult(result.Flag, result.Message ?? "Student Fee updated successfully");
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error updating Student Fee: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> DeleteStudentFee(Guid id)
        {
            try
            {
                if (!_authService.HasPermission("DeleteFee"))
                {
                    return PartialView("_AccessDenied");
                }
                var studentFee = await _feeService.GetStudentFee(id);
                if (studentFee == null)
                {
                    return _returnHelper.ReturnNewResult(false, "Student Fee not found.");
                }

                return PartialView("_deleteStudentFee", studentFee);
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error loading delete form: {ex.Message}");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteStudentFeeConfirmed(Guid StudentFeeId)
        {
            try
            {
                if (!_authService.HasPermission("DeleteFee"))
                {
                    return PartialView("_AccessDenied");
                }
                var result = await _feeService.DeleteStudentFee(StudentFeeId);
                return _returnHelper.ReturnNewResult(result.Flag, result.Message ?? "Student Fee deleted successfully");
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error deleting student fee: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> DetailsStudentFee(Guid id)
        {
            try
            {
                if (!_authService.HasPermission("ViewFee"))
                {
                    return PartialView("_AccessDenied");
                }
                var studentFee = await _feeService.GetStudentFee(id);
                if (studentFee == null)
                {
                    return _returnHelper.ReturnNewResult(false, "Student Fee not found.");
                }

                return PartialView("_detailsStudentFee", studentFee);
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error loading student fee details: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> WaiveStudentFee(Guid id)
        {
            if (!_authService.HasPermission("ManageFee"))
            {
                return PartialView("_AccessDenied");
            }
            try
            {
                ViewBag.StudentFeeId = id;
                var model = new WaiveStudentFeeDTO
                {
                    StudentFeeId = id
                };
                return PartialView("_waiveStudentFee", model);
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error loading waive form: {ex.Message}");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> WaiveStudentFee(WaiveStudentFeeDTO model)
        {
            try
            {
                if (!_authService.HasPermission("ManageFee"))
                {
                    return PartialView("_AccessDenied");
                }
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.ToDictionary(kvp => kvp.Key, kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray());
                    return Json(new { validationErrors = errors });
                }
                
                var result = await _feeService.WaiveStudentFee(model);
                return _returnHelper.ReturnNewResult(result.Flag, result.Message ?? "Student fee waived successfully");
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error waiving student fee: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetStudentFeesByStudent(Guid studentId)
        {
            if (!_authService.HasPermission("ViewFee"))
            {
                return PartialView("_AccessDenied");
            }
            try
            {
                var studentFees = await _feeService.GetStudentFees(studentId);
                var result = studentFees.Select(sf => new
                {
                    id = sf.StudentFeeId,
                    text = $"{sf.FeeName} - {sf.FinalAmount:C}",
                    feeName = sf.FeeName,
                    feeType = sf.FeeType,
                    finalAmount = sf.FinalAmount,
                    currency = sf.Currency,
                    dueDate = sf.DueDate.ToString("yyyy-MM-dd"),
                    status = sf.Status
                }).ToList();
                return Json(result);
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error loading student fees by student: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetOverdueFees()
        {
            if (!_authService.HasPermission("ViewFee"))
            {
                return PartialView("_AccessDenied");
            }
            try
            {
                var overdueFees = await _feeService.GetOverdueFees();
                var result = overdueFees.Select(sf => new
                {
                    sf.StudentFeeId,
                    sf.StudentName,
                    sf.StudentCode,
                    sf.FeeName,
                    sf.FinalAmount,
                    sf.Currency,
                    sf.DueDate,
                    sf.Status
                }).ToList();
                return Json(result);
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error loading overdue fees: {ex.Message}");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CalculateLateFees()
        {
            try
            {
                if (!_authService.HasPermission("ManageFee"))
                {
                    return PartialView("_AccessDenied");
                }
                
                var result = await _feeService.CalculateLateFees();
                return _returnHelper.ReturnNewResult(result.Flag, result.Message ?? "Late fees calculated successfully");
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error calculating late fees: {ex.Message}");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GenerateInvoices(Guid? studentId = null, DateTime? dueDate = null)
        {
            try
            {
                if (!_authService.HasPermission("ManageFee"))
                {
                    return PartialView("_AccessDenied");
                }
                
                var result = await _feeService.GenerateInvoices(studentId, dueDate);
                return _returnHelper.ReturnNewResult(result.Flag, result.Message ?? "Invoices generated successfully");
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error generating invoices: {ex.Message}");
            }
        }
    }
}
