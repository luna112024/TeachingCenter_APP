using hongWenAPP.Helpers;
using hongWenAPP.Models;
using hongWenAPP.Models.StudentModel.DTOs;
using hongWenAPP.Services;
using Microsoft.AspNetCore.Mvc;

namespace hongWenAPP.Controllers
{
    public class StudentController : Controller
    {
        private readonly IStudentService _studentService;
        private readonly ILevelService _levelService;
        private readonly ReturnHelper _returnHelper;
        private readonly AuthenticationService _authService;

        public StudentController(IStudentService studentService, ILevelService levelService, ReturnHelper returnHelper, AuthenticationService authService)
        {
            _studentService = studentService;
            _levelService = levelService;
            _returnHelper = returnHelper;
            _authService = authService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(ListStudentDTOs model)
        {
            if (!_authService.HasPermission("ViewStudent"))
            {
                return PartialView("_AccessDenied");
            }
            var students = await _studentService.GetAllStudents();
            var list = new ListStudentDTOs
            {
                student = PageList<GetStudentDTO>.Create(students, model.Page, model.PageSize, "ListStudent")
            };

            return View(list);
        }

        [HttpGet]
        public async Task<IActionResult> ListStudent(ListStudentDTOs model)
        {
            if (!_authService.HasPermission("ViewStudent"))
            {
                return PartialView("_AccessDenied");
            }
            var students = await _studentService.GetAllStudents(search: model.SearchText);
            var list = PageList<GetStudentDTO>.Create(students, model.Page, model.PageSize, "ListStudent");
            return PartialView("_ListStudents", list);
        }

        [HttpGet]
        public async Task<IActionResult> FetchStudent(string i)
        {
            if (!_authService.HasPermission("ViewStudent"))
            {
                return PartialView("_AccessDenied");
            }
            var students = await _studentService.GetAllStudents(search: i);
            var result = students.Select(s => new
            {
                s.StudentId,
                s.StudentCode,
                StudentName = $"{s.FirstName} {s.LastName}",
                s.CurrentLevelName,
                s.StudentStatus,
                s.EnrollmentDate,
                s.GuardianName,
                s.GuardianPhone
            }).ToList();
            return Json(result);
        }

        [HttpGet]
        public async Task<IActionResult> AddStudent()
        {
            if (!_authService.HasPermission("ManageStudent"))
            {
                return PartialView("_AccessDenied");
            }
            
            // Get active levels for dropdown
            var activeLevels = await _levelService.GetActiveLevels();
            ViewBag.AvailableLevels = activeLevels;
            
            var model = new CreateStudentDTO
            {
                EnrollmentDate = DateTime.Today,
                StudentStatus = "Active",
                PreferredClassTime = "Flexible"
            };
            return PartialView("_addStudent", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddStudent(CreateStudentDTO createStudentDTO)
        {
            try
            {
                if (!_authService.HasPermission("ManageStudent"))
                {
                    return PartialView("_AccessDenied");
                }
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.ToDictionary(kvp => kvp.Key, kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray());
                    return Json(new { validationErrors = errors });
                }
                          
                var result = await _studentService.CreateStudent(createStudentDTO);
                return _returnHelper.ReturnNewResult(result.Flag, result.Message);
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditStudent(Guid id)
        {
            try
            {
                if (!_authService.HasPermission("ManageStudent"))
                {
                    return PartialView("_AccessDenied");
                }
                
                // Get active levels for dropdown
                var activeLevels = await _levelService.GetActiveLevels();
                ViewBag.AvailableLevels = activeLevels;
                
                var studentData = await _studentService.GetStudent(id);

                var studentUpdateDTO = new UpdateStudentDTO
                {
                    StudentId = studentData.StudentId,
                    StudentCode = studentData.StudentCode,
                    FirstName = studentData.FirstName,
                    LastName = studentData.LastName,
                    KhmerName = studentData.KhmerName,
                    ChineseName = studentData.ChineseName,
                    Gender = studentData.Gender,
                    DateOfBirth = studentData.DateOfBirth,
                    Nationality = studentData.Nationality,
                    IdCardNumber = studentData.IdCardNumber,
                    Phone = studentData.Phone,
                    Email = studentData.Email,
                    Address = studentData.Address,
                    City = studentData.City,
                    Province = studentData.Province,
                    PostalCode = studentData.PostalCode,
                    CurrentLevel = studentData.CurrentLevel,
                    LearningGoals = studentData.LearningGoals,
                    PreviousChineseStudy = studentData.PreviousChineseStudy,
                    StudyDurationYears = studentData.StudyDurationYears,
                    PreferredClassTime = studentData.PreferredClassTime,
                    EmergencyContactName = studentData.EmergencyContactName,
                    EmergencyContactPhone = studentData.EmergencyContactPhone,
                    EmergencyContactRelationship = studentData.EmergencyContactRelationship,
                    GuardianName = studentData.GuardianName,
                    GuardianPhone = studentData.GuardianPhone,
                    GuardianEmail = studentData.GuardianEmail,
                    GuardianRelationship = studentData.GuardianRelationship,
                    GuardianIdCard = studentData.GuardianIdCard,
                    GuardianOccupation = studentData.GuardianOccupation,
                    GuardianAddress = studentData.GuardianAddress,
                    EnrollmentDate = studentData.EnrollmentDate,
                    GraduationDate = studentData.GraduationDate,
                    ProfilePhoto = studentData.ProfilePhoto,
                    StudentStatus = studentData.StudentStatus,
                    Notes = studentData.Notes,
                    ModifiedBy = studentData.ModifiedBy
                };

                ViewBag.StudentId = id;
                return PartialView("_editStudent", studentUpdateDTO);
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error retrieving Student: {ex.Message}");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditStudent(UpdateStudentDTO model)
        {
            try
            {
                if (!_authService.HasPermission("ManageStudent"))
                {
                    return PartialView("_AccessDenied");
                }
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.ToDictionary(kvp => kvp.Key, kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray());
                    return Json(new { validationErrors = errors });
                }     
                
                var result = await _studentService.UpdateStudent(model);
                return _returnHelper.ReturnNewResult(result.Flag, result.Message ?? "Student updated successfully");
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error updating Student: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> DeleteStudent(Guid id)
        {
            try
            {
                if (!_authService.HasPermission("DeleteStudent"))
                {
                    return PartialView("_AccessDenied");
                }
                var student = await _studentService.GetStudent(id);
                if (student == null)
                {
                    return _returnHelper.ReturnNewResult(false, "Student not found.");
                }

                var studentToDelete = new UpdateStudentDTO
                {
                    StudentId = student.StudentId,
                    StudentCode = student.StudentCode,
                    StudentStatus = student.StudentStatus,
                    GuardianName = student.GuardianName
                };

                return PartialView("_deleteStudent", studentToDelete);
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error loading delete form: {ex.Message}");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteStudentConfirmed(UpdateStudentDTO model)
        {
            try
            {
                if (!_authService.HasPermission("DeleteStudent"))
                {
                    return PartialView("_AccessDenied");
                }
                var result = await _studentService.DeleteStudent(model.StudentId);
                return _returnHelper.ReturnNewResult(result.Flag, result.Message ?? "Student deleted successfully");
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error deleting student: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> DetailsStudent(Guid id)
        {
            try
            {
                if (!_authService.HasPermission("ViewStudent"))
                {
                    return PartialView("_AccessDenied");
                }
                var student = await _studentService.GetStudent(id);
                if (student == null)
                {
                    return _returnHelper.ReturnNewResult(false, "Student not found.");
                }

                return PartialView("_detailsStudent", student);
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error loading student details: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllStudents(string q = "")
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
                    id = s.StudentId,
                    text = $"{s.FirstName} {s.LastName}" ?? "Unknown",
                    studentCode = s.StudentCode
                }).ToList();

                // Filter by search query if provided
                if (!string.IsNullOrEmpty(q))
                {
                    result = result.Where(s => 
                        s.text.ToLower().Contains(q.ToLower()) || 
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
        public async Task<IActionResult> GetStudentsByStatus(string status)
        {
            if (!_authService.HasPermission("ViewStudent"))
            {
                return PartialView("_AccessDenied");
            }
            try
            {
                var students = await _studentService.GetStudentsByStatus(status);
                var result = students.Select(s => new
                {
                    id = s.StudentId,
                    text = $"{s.StudentCode} - {s.FirstName} {s.LastName}",
                    code = s.StudentCode,
                    status = s.StudentStatus,
                    enrollmentDate = s.EnrollmentDate.ToString("yyyy-MM-dd"),
                    studentName = $"{s.FirstName} {s.LastName}",
                    guardianName = s.GuardianName,
                    guardianPhone = s.GuardianPhone
                }).ToList();
                return Json(result);
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error loading students by status: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAcademicHistory(Guid id)
        {
            if (!_authService.HasPermission("ViewStudent"))
            {
                return PartialView("_AccessDenied");
            }
            try
            {
                ViewBag.StudentId = id;
                return PartialView("_academicHistory");
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error loading academic history: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAcademicHistoryData(Guid id)
        {
            if (!_authService.HasPermission("ViewStudent"))
            {
                return Json(new { error = "Access denied" });
            }
            try
            {
                var history = await _studentService.GetAcademicHistory(id);
                return Json(history);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetPaymentHistory(Guid id)
        {
            if (!_authService.HasPermission("ViewStudent"))
            {
                return PartialView("_AccessDenied");
            }
            try
            {
                ViewBag.StudentId = id;
                return PartialView("_paymentHistory");
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error loading payment history: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetPaymentHistoryData(Guid id)
        {
            if (!_authService.HasPermission("ViewStudent"))
            {
                return Json(new { error = "Access denied" });
            }
            try
            {
                var history = await _studentService.GetPaymentHistory(id);
                return Json(history);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateEmergencyContact(Guid id, string name, string phone, string relationship)
        {
            try
            {
                if (!_authService.HasPermission("ManageStudent"))
                {
                    return PartialView("_AccessDenied");
                }
                var result = await _studentService.UpdateEmergencyContact(id, name, phone, relationship);
                return _returnHelper.ReturnNewResult(result.Flag, result.Message ?? "Emergency contact updated successfully");
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error updating emergency contact: {ex.Message}");
            }
        }
    }
}
