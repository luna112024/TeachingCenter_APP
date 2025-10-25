using hongWenAPP.Helpers;
using hongWenAPP.Models;
using hongWenAPP.Models.StudentCourseModel.DTOs;
using hongWenAPP.Services;
using Microsoft.AspNetCore.Mvc;

namespace hongWenAPP.Controllers
{
    public class PromotionController : Controller
    {
        private readonly IPromotionService _promotionService;
        private readonly IStudentService _studentService;
        private readonly ICourseService _courseService;
        private readonly ITermService _termService;
        private readonly ReturnHelper _returnHelper;
        private readonly AuthenticationService _authService;

        public PromotionController(
            IPromotionService promotionService,
            IStudentService studentService,
            ICourseService courseService,
            ITermService termService,
            ReturnHelper returnHelper,
            AuthenticationService authService)
        {
            _promotionService = promotionService;
            _studentService = studentService;
            _courseService = courseService;
            _termService = termService;
            _returnHelper = returnHelper;
            _authService = authService;
        }

        // GET: Promotion/Index
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (!_authService.HasPermission("ViewPromotion"))
            {
                return PartialView("_AccessDenied");
            }

            // Get active terms and courses for selection
            ViewBag.ActiveTerms = await _termService.GetActiveTerm();
            ViewBag.ActiveCourses = await _courseService.GetAllCourses(status: "Active");

            return View();
        }

        // GET: Promotion/PromoteStudent
        [HttpGet]
        public async Task<IActionResult> PromoteStudent(Guid? studentId)
        {
            if (!_authService.HasPermission("ManagePromotion"))
            {
                return PartialView("_AccessDenied");
            }

            // Prepare dropdown data
            ViewBag.ActiveTerms = await _termService.GetActiveTerm();
            ViewBag.ActiveCourses = await _courseService.GetAllCourses(status: "Active");

            var model = new PromoteStudentDTO
            {
                StudentId = studentId ?? Guid.Empty,
                PromotionStatus = "Promoted"
            };

            return PartialView("_PromoteStudent", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PromoteStudent(PromoteStudentDTO dto)
        {
            try
            {
                if (!_authService.HasPermission("ManagePromotion"))
                {
                    return _returnHelper.ReturnNewResult(false, "Access Denied: You don't have permission to promote students.");
                }

                if (!ModelState.IsValid)
                {
                    return _returnHelper.ReturnNewResult(false, "Please fill all required fields.");
                }

                dto.CreatedBy = _authService.GetUserInfo()?.Username ?? "Unknown";

                var result = await _promotionService.PromoteStudent(dto);

                if (result.Flag)
                {
                    return _returnHelper.ReturnNewResult(true, "Student promoted successfully! Invoice generated with carryover balances if applicable.");
                }

                return _returnHelper.ReturnNewResult(false, result.Message);
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error: {ex.Message}");
            }
        }

        // POST: Promotion/PreviewPromotion (AJAX)
        [HttpPost]
        public async Task<IActionResult> PreviewPromotion([FromBody] PreviewPromotionDTO dto)
        {
            if (!_authService.HasPermission("ViewPromotion"))
            {
                return Json(new { success = false, message = "Access denied" });
            }

            try
            {
                var preview = await _promotionService.PreviewPromotion(dto);
                return Json(new { success = true, data = preview });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // GET: Promotion/BulkPromote
        [HttpGet]
        public async Task<IActionResult> BulkPromote()
        {
            if (!_authService.HasPermission("ManagePromotion"))
            {
                return PartialView("_AccessDenied");
            }

            // Prepare dropdown data
            ViewBag.ActiveTerms = await _termService.GetActiveTerm();
            ViewBag.ActiveCourses = await _courseService.GetAllCourses(status: "Active");

            var model = new BulkPromoteDTO
            {
                PromotionStatus = "Promoted"
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BulkPromote(BulkPromoteDTO dto)
        {
            try
            {
                if (!_authService.HasPermission("ManagePromotion"))
                {
                    return _returnHelper.ReturnNewResult(false, "Access Denied: You don't have permission to promote students.");
                }

                if (!ModelState.IsValid || !dto.StudentIds.Any())
                {
                    return _returnHelper.ReturnNewResult(false, "Please select at least one student.");
                }

                dto.CreatedBy = _authService.GetUserInfo()?.Username ?? "Unknown";

                var result = await _promotionService.BulkPromoteStudents(dto);

                return _returnHelper.ReturnNewResult(true, $"Bulk Promotion Complete - Total: {result.TotalStudents} | Successful: {result.Successful} | Failed: {result.Failed}");
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error: {ex.Message}");
            }
        }

        // GET: Promotion/History/{studentId}
        [HttpGet]
        public async Task<IActionResult> History(Guid studentId)
        {
            if (!_authService.HasPermission("ViewPromotion"))
            {
                return PartialView("_AccessDenied");
            }

            var history = await _promotionService.GetPromotionHistory(studentId);
            ViewBag.Student = await _studentService.GetStudent(studentId);

            return View(history);
        }
    }
}

