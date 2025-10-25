using hongWenAPP.Helpers;
using hongWenAPP.Models;
using hongWenAPP.Models.StudentCourseModel.DTOs;
using hongWenAPP.Services;
using Microsoft.AspNetCore.Mvc;

namespace hongWenAPP.Controllers
{
    public class StudentCourseController : Controller
    {
        private readonly IStudentCourseService _studentCourseService;
        private readonly IStudentService _studentService;
        private readonly ICourseService _courseService;
        private readonly ITermService _termService;
        private readonly ReturnHelper _returnHelper;
        private readonly AuthenticationService _authService;

        public StudentCourseController(
            IStudentCourseService studentCourseService,
            IStudentService studentService,
            ICourseService courseService,
            ITermService termService,
            ReturnHelper returnHelper,
            AuthenticationService authService)
        {
            _studentCourseService = studentCourseService;
            _studentService = studentService;
            _courseService = courseService;
            _termService = termService;
            _returnHelper = returnHelper;
            _authService = authService;
        }

        // GET: StudentCourse/Index
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (!_authService.HasPermission("ViewStudentCourse"))
            {
                return PartialView("_AccessDenied");
            }

            // Get active terms and courses for filters
            ViewBag.ActiveTerms = await _termService.GetActiveTerm();
            ViewBag.ActiveCourses = await _courseService.GetAllCourses(status: "Active");

            return View();
        }

        // POST: StudentCourse/AssignStudent
        [HttpGet]
        public async Task<IActionResult> AssignStudent(Guid? studentId)
        {
            if (!_authService.HasPermission("ManageStudentCourse"))
            {
                return PartialView("_AccessDenied");
            }

            // Prepare dropdown data
            ViewBag.ActiveTerms = await _termService.GetActiveTerm();
            ViewBag.ActiveCourses = await _courseService.GetAllCourses(status: "Active");

            var model = new AssignStudentToCourseDTO
            {
                StudentId = studentId ?? Guid.Empty,
                StartDate = DateTime.Now
            };

            return PartialView("_AssignStudentToCourse", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignStudent(AssignStudentToCourseDTO dto)
        {
            try
            {
                if (!_authService.HasPermission("ManageStudentCourse"))
                {
                    return _returnHelper.ReturnNewResult(false, "Access Denied: You don't have permission to assign students to courses.");
                }

                if (!ModelState.IsValid)
                {
                    return _returnHelper.ReturnNewResult(false, "Please fill all required fields.");
                }

                dto.CreatedBy = _authService.GetUserInfo()?.Username ?? "Unknown";

                var result = await _studentCourseService.AssignStudentToCourse(dto);

                return _returnHelper.ReturnNewResult(result.Flag, 
                    result.Flag ? "Student assigned to course successfully. Invoice has been generated." : result.Message);
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error: {ex.Message}");
            }
        }

        // GET: StudentCourse/History/{studentId}
        [HttpGet]
        public async Task<IActionResult> History(Guid studentId)
        {
            if (!_authService.HasPermission("ViewStudentCourse"))
            {
                return PartialView("_AccessDenied");
            }

            var history = await _studentCourseService.GetStudentCourseHistory(studentId);
            ViewBag.Student = await _studentService.GetStudent(studentId);
            
            return View(history);
        }

        // GET: StudentCourse/CurrentCourse/{studentId}
        [HttpGet]
        public async Task<IActionResult> GetCurrentCourse(Guid studentId)
        {
            if (!_authService.HasPermission("ViewStudentCourse"))
            {
                return Json(new { success = false, message = "Access denied" });
            }

            try
            {
                var currentCourse = await _studentCourseService.GetCurrentCourseAssignment(studentId);
                return Json(new { success = true, data = currentCourse });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // GET: StudentCourse/StudentsInCourse
        [HttpGet]
        public async Task<IActionResult> StudentsInCourse(Guid courseId, Guid termId)
        {
            if (!_authService.HasPermission("ViewStudentCourse"))
            {
                return PartialView("_AccessDenied");
            }

            var students = await _studentCourseService.GetStudentsInCourse(courseId, termId);
            ViewBag.Course = await _courseService.GetCourse(courseId);
            ViewBag.Term = await _termService.GetTerm(termId);

            return View(students);
        }
    }
}

