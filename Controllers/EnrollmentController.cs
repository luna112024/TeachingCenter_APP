using hongWenAPP.Helpers;
using hongWenAPP.Models;
using hongWenAPP.Models.EnrollmentModel.DTOs;
using hongWenAPP.Services;
using Microsoft.AspNetCore.Mvc;

namespace hongWenAPP.Controllers
{
    public class EnrollmentController : Controller
    {
        private readonly IEnrollmentService _enrollmentService;
        private readonly IStudentService _studentService;
        private readonly IClassSectionService _classSectionService;
        private readonly ReturnHelper _returnHelper;
        private readonly AuthenticationService _authService;

        public EnrollmentController(IEnrollmentService enrollmentService, IStudentService studentService, IClassSectionService classSectionService, ReturnHelper returnHelper, AuthenticationService authService)
        {
            _enrollmentService = enrollmentService;
            _studentService = studentService;
            _classSectionService = classSectionService;
            _returnHelper = returnHelper;
            _authService = authService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(ListEnrollmentDTOs model)
        {
            if (!_authService.HasPermission("ViewEnrollments"))
            {
                return PartialView("_AccessDenied");
            }
            var enrollments = await _enrollmentService.GetAllEnrollments();
            var list = new ListEnrollmentDTOs
            {
                enrollment = PageList<GetEnrollmentDTO>.Create(enrollments, model.Page, model.PageSize, "ListEnrollment")
            };

            return View(list);
        }

        [HttpGet]
        public async Task<IActionResult> ListEnrollment(ListEnrollmentDTOs model)
        {
            if (!_authService.HasPermission("ViewEnrollments"))
            {
                return PartialView("_AccessDenied");
            }
            var enrollments = await _enrollmentService.GetAllEnrollments(status: model.SearchText);
            var list = PageList<GetEnrollmentDTO>.Create(enrollments, model.Page, model.PageSize, "ListEnrollment");
            return PartialView("_ListEnrollments", list);
        }

        [HttpGet]
        public async Task<IActionResult> FetchEnrollment(string i)
        {
            if (!_authService.HasPermission("ViewEnrollments"))
            {
                return PartialView("_AccessDenied");
            }
            var enrollments = await _enrollmentService.GetAllEnrollments(status: i);
            var result = enrollments.Select(e => new
            {
                e.EnrollmentId,
                e.StudentCode,
                e.StudentName,
                e.CourseName,
                e.SectionName,
                e.Status,
                e.EnrollmentDate,
                e.TeacherName
            }).ToList();
            return Json(result);
        }

        [HttpGet]
        public async Task<IActionResult> AddEnrollment()
        {
            if (!_authService.HasPermission("ManageEnrollments"))
            {
                return PartialView("_AccessDenied");
            }
            
            // Get active students and sections for dropdowns
            var students = await _studentService.GetAllStudents();
            var sections = await _classSectionService.GetAllClassSections();
            
            ViewBag.AvailableStudents = students;
            ViewBag.AvailableSections = sections;
            
            var model = new CreateEnrollmentDTO
            {
                EnrollmentDate = DateTime.Today,
                Status = "Enrolled",
                EnrollmentType = "Regular"
            };
            return PartialView("_addEnrollment", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEnrollment(CreateEnrollmentDTO createEnrollmentDTO)
        {
            try
            {
                if (!_authService.HasPermission("ManageEnrollments"))
                {
                    return PartialView("_AccessDenied");
                }
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.ToDictionary(kvp => kvp.Key, kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray());
                    return Json(new { validationErrors = errors });
                }  
                var result = await _enrollmentService.CreateEnrollment(createEnrollmentDTO);
                return _returnHelper.ReturnNewResult(result.Flag, result.Message);
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditEnrollment(Guid id)
        {
            try
            {
                if (!_authService.HasPermission("ManageEnrollments"))
                {
                    return PartialView("_AccessDenied");
                }
                
                // Get active students and sections for dropdowns
                var students = await _studentService.GetAllStudents();
                var sections = await _classSectionService.GetAllClassSections();
                
                ViewBag.AvailableStudents = students;
                ViewBag.AvailableSections = sections;
                
                var enrollmentData = await _enrollmentService.GetEnrollment(id);

                var enrollmentUpdateDTO = new UpdateEnrollmentDTO
                {
                    EnrollmentId = enrollmentData.EnrollmentId,
                    StudentId = enrollmentData.StudentId,
                    SectionId = enrollmentData.SectionId,
                    EnrollmentDate = enrollmentData.EnrollmentDate,
                    EnrollmentType = enrollmentData.EnrollmentType,
                    Status = enrollmentData.Status,
                    StartDate = enrollmentData.StartDate,
                    CompletionDate = enrollmentData.CompletionDate,
                    FinalGrade = enrollmentData.FinalGrade,
                    FinalScore = enrollmentData.FinalScore,
                    AttendanceRate = enrollmentData.AttendanceRate,
                    DiscountPercentage = enrollmentData.DiscountPercentage,
                    DiscountReason = enrollmentData.DiscountReason,
                    SpecialNeeds = enrollmentData.SpecialNeeds,
                    Notes = enrollmentData.Notes,
                    ModifiedBy = enrollmentData.ModifiedBy
                };

                ViewBag.EnrollmentId = id;
                return PartialView("_editEnrollment", enrollmentUpdateDTO);
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error retrieving Enrollment: {ex.Message}");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditEnrollment(UpdateEnrollmentDTO model)
        {
            try
            {
                if (!_authService.HasPermission("ManageEnrollments"))
                {
                    return PartialView("_AccessDenied");
                }
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.ToDictionary(kvp => kvp.Key, kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray());
                    return Json(new { validationErrors = errors });
                }

                
                var result = await _enrollmentService.UpdateEnrollment(model);
                return _returnHelper.ReturnNewResult(result.Flag, result.Message ?? "Enrollment updated successfully");
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error updating Enrollment: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> DeleteEnrollment(Guid id)
        {
            try
            {
                if (!_authService.HasPermission("DeleteEnrollments"))
                {
                    return PartialView("_AccessDenied");
                }
                var enrollment = await _enrollmentService.GetEnrollment(id);
                if (enrollment == null)
                {
                    return _returnHelper.ReturnNewResult(false, "Enrollment not found.");
                }

                return PartialView("_deleteEnrollment", enrollment);
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error loading delete form: {ex.Message}");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteEnrollmentConfirmed(Guid EnrollmentId)
        {
            try
            {
                if (!_authService.HasPermission("DeleteEnrollments"))
                {
                    return PartialView("_AccessDenied");
                }
                var result = await _enrollmentService.DeleteEnrollment(EnrollmentId);
                return _returnHelper.ReturnNewResult(result.Flag, result.Message ?? "Enrollment deleted successfully");
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error deleting enrollment: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> DetailsEnrollment(Guid id)
        {
            try
            {
                if (!_authService.HasPermission("ViewEnrollments"))
                {
                    return PartialView("_AccessDenied");
                }
                var enrollment = await _enrollmentService.GetEnrollment(id);
                if (enrollment == null)
                {
                    return _returnHelper.ReturnNewResult(false, "Enrollment not found.");
                }

                return PartialView("_detailsEnrollment", enrollment);
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error loading enrollment details: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetEnrollmentsByStudent(Guid studentId)
        {
            if (!_authService.HasPermission("ViewEnrollments"))
            {
                return PartialView("_AccessDenied");
            }
            try
            {
                var enrollments = await _enrollmentService.GetEnrollmentsByStudent(studentId);
                var result = enrollments.Select(e => new
                {
                    id = e.EnrollmentId,
                    text = $"{e.CourseName} - {e.SectionName}",
                    courseName = e.CourseName,
                    sectionName = e.SectionName,
                    status = e.Status,
                    enrollmentDate = e.EnrollmentDate.ToString("yyyy-MM-dd"),
                    teacherName = e.TeacherName
                }).ToList();
                return Json(result);
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error loading enrollments by student: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetEnrollmentsBySection(Guid sectionId)
        {
            if (!_authService.HasPermission("ViewEnrollments"))
            {
                return PartialView("_AccessDenied");
            }
            try
            {
                var enrollments = await _enrollmentService.GetEnrollmentsBySection(sectionId);
                var result = enrollments.Select(e => new
                {
                    id = e.EnrollmentId,
                    text = $"{e.StudentName} ({e.StudentCode})",
                    studentName = e.StudentName,
                    studentCode = e.StudentCode,
                    status = e.Status,
                    enrollmentDate = e.EnrollmentDate.ToString("yyyy-MM-dd")
                }).ToList();
                return Json(result);
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error loading enrollments by section: {ex.Message}");
            }
        }

        [HttpGet]
        public IActionResult TransferEnrollment(Guid id)
        {
            if (!_authService.HasPermission("ManageEnrollments"))
            {
                return PartialView("_AccessDenied");
            }
            try
            {
                ViewBag.EnrollmentId = id;
                var model = new TransferEnrollmentDTO
                {
                    TransferDate = DateTime.Today
                };
                return PartialView("_transferEnrollment", model);
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error loading transfer form: {ex.Message}");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TransferEnrollment(Guid id, TransferEnrollmentDTO model)
        {
            try
            {
                if (!_authService.HasPermission("ManageEnrollments"))
                {
                    return PartialView("_AccessDenied");
                }
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.ToDictionary(kvp => kvp.Key, kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray());
                    return Json(new { validationErrors = errors });
                }
                
                
                
                var result = await _enrollmentService.TransferEnrollment(id, model);
                return _returnHelper.ReturnNewResult(result.Flag, result.Message ?? "Enrollment transferred successfully");
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error transferring enrollment: {ex.Message}");
            }
        }
    }
}
