using hongWenAPP.Helpers;
using hongWenAPP.Models;
using hongWenAPP.Models.TeacherModel.DTOs;
using hongWenAPP.Services;
using Microsoft.AspNetCore.Mvc;

namespace hongWenAPP.Controllers
{
    public class TeacherController : Controller
    {
        private readonly ITeacherService _teacherService;
        private readonly IIdentityService _identityService;
        private readonly ReturnHelper _returnHelper;
        private readonly AuthenticationService _authService;

        public TeacherController(ITeacherService teacherService, IIdentityService identityService, ReturnHelper returnHelper, AuthenticationService authService)
        {
            _teacherService = teacherService;
            _identityService = identityService;
            _returnHelper = returnHelper;
            _authService = authService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(ListTeacherDTOs model)
        {
            if (!_authService.HasPermission("ViewTeacher"))
            {
                return PartialView("_AccessDenied");
            }
            var teachers = await _teacherService.GetAllTeachers();
            var list = new ListTeacherDTOs
            {
                teacher = PageList<GetTeacherDTO>.Create(teachers, model.Page, model.PageSize, "ListTeacher")
            };

            return View(list);
        }

        [HttpGet]
        public async Task<IActionResult> ListTeacher(ListTeacherDTOs model)
        {
            if (!_authService.HasPermission("ViewTeacher"))
            {
                return PartialView("_AccessDenied");
            }
            var teachers = await _teacherService.GetAllTeachers(name: model.SearchText);
            var list = PageList<GetTeacherDTO>.Create(teachers, model.Page, model.PageSize, "ListTeacher");
            return PartialView("_ListTeachers", list);
        }

        [HttpGet]
        public async Task<IActionResult> FetchTeacher(string i)
        {
            if (!_authService.HasPermission("ViewTeacher"))
            {
                return PartialView("_AccessDenied");
            }
            var teachers = await _teacherService.GetAllTeachers(name: i);
            var result = teachers.Select(t => new
            {
                t.TeacherId,
                t.TeacherCode,
                t.UserName,
                t.UserEmail,
                t.ChineseProficiency,
                t.ExperienceYears,
                t.EmploymentStatus
            }).ToList();
            return Json(result);
        }

        [HttpGet]
        public async Task<IActionResult> AddTeacher()
        {
            if (!_authService.HasPermission("ManageTeacher"))
            {
                return PartialView("_AccessDenied");
            }
            
            // Get all users with Teacher role
            var allUsers = await _identityService.GetUsers();
            var usersWithTeacherRole = allUsers.Where(u => u.Roles.Contains("Teacher")).ToList();
            
            // Get existing teachers to exclude them
            var existingTeachers = await _teacherService.GetAllTeachers();
            var existingTeacherUserIds = existingTeachers.Select(t => t.UserId).ToHashSet();
            
            // Filter users who have Teacher role but don't have teacher profiles
            var availableUsers = usersWithTeacherRole.Where(u => !existingTeacherUserIds.Contains(u.UserId)).ToList();
            
            var model = new CreateTeacherDTO
            {
                HireDate = DateTime.Today
            };
            
            ViewBag.AvailableUsers = availableUsers;
            return PartialView("_addTeacher", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddTeacher(CreateTeacherDTO createTeacherDTO)
        {
            try
            {
                if (!_authService.HasPermission("ManageTeacher"))
                {
                    return PartialView("_AccessDenied");
                }
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.ToDictionary(kvp => kvp.Key, kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray());
                    return Json(new { validationErrors = errors });
                }
                var result = await _teacherService.CreateTeacher(createTeacherDTO);
                return _returnHelper.ReturnNewResult(result.Flag, result.Message);
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditTeacher(Guid id)
        {
            try
            {
                if (!_authService.HasPermission("ManageTeacher"))
                {
                    return PartialView("_AccessDenied");
                }
                var teacherData = await _teacherService.GetTeacher(id);

                var teacherUpdateDTO = new UpdateTeacherDTO
                {
                    TeacherId = teacherData.TeacherId,
                    UserId = teacherData.UserId,
                    TeacherCode = teacherData.TeacherCode,
                    Specializations = teacherData.Specializations,
                    Qualifications = teacherData.Qualifications,
                    ExperienceYears = teacherData.ExperienceYears,
                    ChineseProficiency = teacherData.ChineseProficiency,
                    TeachingLanguages = teacherData.TeachingLanguages,
                    MaxHoursPerWeek = teacherData.MaxHoursPerWeek,
                    HourlyRate = teacherData.HourlyRate,
                    HireDate = teacherData.HireDate,
                    ContractType = teacherData.ContractType,
                    EmploymentStatus = teacherData.EmploymentStatus,
                    Notes = teacherData.Notes,
                    ModifiedBy = teacherData.ModifiedBy
                };

                ViewBag.TeacherId = id;
                // Pass current user info for display (read-only)
                ViewBag.CurrentUser = new { 
                    UserName = teacherData.UserName, 
                    UserEmail = teacherData.UserEmail 
                };
                return PartialView("_editTeacher", teacherUpdateDTO);
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error retrieving Teacher: {ex.Message}");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditTeacher(UpdateTeacherDTO model)
        {
            try
            {
                if (!_authService.HasPermission("ManageTeacher"))
                {
                    return PartialView("_AccessDenied");
                }
                
                // Get original teacher data to verify user hasn't changed
                var originalTeacher = await _teacherService.GetTeacher(model.TeacherId);
                if (originalTeacher.UserId != model.UserId)
                {
                    return _returnHelper.ReturnNewResult(false, "Cannot change the user assigned to a teacher profile.");
                }
                
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.ToDictionary(kvp => kvp.Key, kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray());
                    return Json(new { validationErrors = errors });
                }
                
                var result = await _teacherService.UpdateTeacher(model);
                return _returnHelper.ReturnNewResult(result.Flag, result.Message ?? "Teacher updated successfully");
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error updating Teacher: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> DeleteTeacher(Guid id)
        {
            try
            {
                if (!_authService.HasPermission("DeleteTeacher"))
                {
                    return PartialView("_AccessDenied");
                }
                var teacher = await _teacherService.GetTeacher(id);
                if (teacher == null)
                {
                    return _returnHelper.ReturnNewResult(false, "Teacher not found.");
                }

                var teacherToDelete = new UpdateTeacherDTO
                {
                    TeacherId = teacher.TeacherId,
                    TeacherCode = teacher.TeacherCode,
                    ChineseProficiency = teacher.ChineseProficiency,
                    EmploymentStatus = teacher.EmploymentStatus
                };

                return PartialView("_deleteTeacher", teacherToDelete);
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error loading delete form: {ex.Message}");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteTeacherConfirmed(UpdateTeacherDTO model)
        {
            try
            {
                if (!_authService.HasPermission("DeleteTeacher"))
                {
                    return PartialView("_AccessDenied");
                }
                var result = await _teacherService.DeleteTeacher(model.TeacherId);
                return _returnHelper.ReturnNewResult(result.Flag, result.Message ?? "Teacher deleted successfully");
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error deleting teacher: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAvailableTeachers()
        {
            if (!_authService.HasPermission("ViewTeacher"))
            {
                return PartialView("_AccessDenied");
            }
            try
            {
                var teachers = await _teacherService.GetAvailableTeachers();
                var result = teachers.Select(t => new
                {
                    id = t.TeacherId,
                    text = $"{t.TeacherCode} - {t.UserName}",
                    code = t.TeacherCode,
                    name = t.UserName,
                    email = t.UserEmail,
                    chineseProficiency = t.ChineseProficiency,
                    experienceYears = t.ExperienceYears,
                    employmentStatus = t.EmploymentStatus
                }).ToList();
                return Json(result);
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error loading available teachers: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetTeacherWorkload(Guid id)
        {
            if (!_authService.HasPermission("ViewTeacher"))
            {
                return PartialView("_AccessDenied");
            }
            try
            {
                var workload = await _teacherService.GetTeacherWorkload(id);
                return Json(workload);
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error loading teacher workload: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetTeacherSchedule(Guid id)
        {
            if (!_authService.HasPermission("ViewTeacher"))
            {
                return PartialView("_AccessDenied");
            }
            try
            {
                var schedule = await _teacherService.GetTeacherSchedule(id);
                return Json(schedule);
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error loading teacher schedule: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> ViewTeacherSchedule(Guid id)
        {
            if (!_authService.HasPermission("ViewTeacher"))
            {
                return PartialView("_AccessDenied");
            }
            try
            {
                var teacher = await _teacherService.GetTeacher(id);
                var schedule = await _teacherService.GetTeacherSchedule(id);
                
                ViewBag.Teacher = teacher;
                ViewBag.Schedule = schedule;
                return PartialView("_viewTeacherSchedule");
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error loading teacher schedule: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> ViewTeacherWorkload(Guid id)
        {
            if (!_authService.HasPermission("ViewTeacher"))
            {
                return PartialView("_AccessDenied");
            }
            try
            {
                var teacher = await _teacherService.GetTeacher(id);
                var workload = await _teacherService.GetTeacherWorkload(id);
                
                ViewBag.Teacher = teacher;
                ViewBag.Workload = workload;
                return PartialView("_viewTeacherWorkload");
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error loading teacher workload: {ex.Message}");
            }
        }
    }
}
