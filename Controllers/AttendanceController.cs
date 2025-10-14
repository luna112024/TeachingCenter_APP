using hongWenAPP.Helpers;
using hongWenAPP.Models;
using hongWenAPP.Models.AttendanceModel.DTOs;
using hongWenAPP.Services;
using Microsoft.AspNetCore.Mvc;

namespace hongWenAPP.Controllers
{
    public class AttendanceController : Controller
    {
        private readonly IAttendanceService _attendanceService;
        private readonly IClassSectionService _classSectionService;
        private readonly ReturnHelper _returnHelper;
        private readonly AuthenticationService _authService;

        public AttendanceController(IAttendanceService attendanceService, IClassSectionService classSectionService, ReturnHelper returnHelper, AuthenticationService authService)
        {
            _attendanceService = attendanceService;
            _classSectionService = classSectionService;
            _returnHelper = returnHelper;
            _authService = authService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(ListAttendanceDTOs model)
        {
            if (!_authService.HasPermission("ViewAttendance"))
            {
                return PartialView("_AccessDenied");
            }
            try
            {
                List<GetAttendanceDTO> attendances;
                
                if (model.Id1 != Guid.Empty)
                {
                    attendances = await _attendanceService.GetAttendanceBySection(model.Id1, null, null);
                }
                else
                {
                    attendances = await _attendanceService.GetAttendanceByDate(DateTime.Today);
                }

                var list = new ListAttendanceDTOs
                {
                    attendance = PageList<GetAttendanceDTO>.Create(attendances, model.Page, model.PageSize, "ListAttendance")
                };

                return View(list);
            }
            catch (Exception ex)
            {
                // Return empty list on error
                var list = new ListAttendanceDTOs
                {
                    attendance = PageList<GetAttendanceDTO>.Create(new List<GetAttendanceDTO>(), model.Page, model.PageSize, "ListAttendance")
                };
                return View(list);
            }
        }

        [HttpGet]
        public async Task<IActionResult> ListAttendance(ListAttendanceDTOs model)
        {
            if (!_authService.HasPermission("ViewAttendance"))
            {
                return PartialView("_AccessDenied");
            }

            // Initialize model if null
            if (model == null)
            {
                model = new ListAttendanceDTOs();
            }

            try
            {
                List<GetAttendanceDTO> attendances;
                
                if (model.Id1 != Guid.Empty)
                {
                    attendances = await _attendanceService.GetAttendanceBySection(model.Id1, null, null);
                }
                else
                {
                    // Get all attendances for today if no section specified
                    attendances = await _attendanceService.GetAttendanceByDate(DateTime.Today);
                }

                var list = PageList<GetAttendanceDTO>.Create(attendances, model.Page, model.PageSize, "ListAttendance");
                return PartialView("_ListAttendance", list);
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error loading attendance: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAttendanceBySection(Guid sectionId, DateTime? startDate = null, DateTime? endDate = null)
        {
            if (!_authService.HasPermission("ViewAttendance"))
            {
                return PartialView("_AccessDenied");
            }

            try
            {
                var attendances = await _attendanceService.GetAttendanceBySection(sectionId, startDate, endDate);
                return Json(attendances);
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error loading section attendance: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAttendanceByStudent(Guid studentId, DateTime? startDate = null, DateTime? endDate = null)
        {
            if (!_authService.HasPermission("ViewAttendance"))
            {
                return PartialView("_AccessDenied");
            }

            try
            {
                var attendances = await _attendanceService.GetAttendanceByStudent(studentId, startDate, endDate);
                return Json(attendances);
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error loading student attendance: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAttendanceByDate(DateTime classDate)
        {
            if (!_authService.HasPermission("ViewAttendance"))
            {
                return PartialView("_AccessDenied");
            }

            try
            {
                var attendances = await _attendanceService.GetAttendanceByDate(classDate);
                return Json(attendances);
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error loading daily attendance: {ex.Message}");
            }
        }

        [HttpGet]
        public IActionResult AddAttendance()
        {
            if (!_authService.HasPermission("ManageAttendance"))
            {
                return PartialView("_AccessDenied");
            }

            var model = new CreateAttendanceDTO();
            return PartialView("_addAttendance", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddAttendance(CreateAttendanceDTO createAttendanceDTO)
        {
            try
            {
                if (!_authService.HasPermission("ManageAttendance"))
                {
                    return PartialView("_AccessDenied");
                }

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.ToDictionary(kvp => kvp.Key, kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray());
                    return Json(new { validationErrors = errors });
                }

                // Set RecordedBy from current user
                var currentUserId = _authService.GetCurrentUserId();
                createAttendanceDTO.RecordedBy = currentUserId;
                
                // Log the values being sent for debugging
                System.Diagnostics.Debug.WriteLine($"Creating attendance - EnrollmentId: {createAttendanceDTO.EnrollmentId}, RecordedBy: {currentUserId}, ClassDate: {createAttendanceDTO.ClassDate}, Status: {createAttendanceDTO.Status}");

                var result = await _attendanceService.CreateAttendance(createAttendanceDTO);
                return _returnHelper.ReturnNewResult(result.Flag, result.Message);
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error creating attendance: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditAttendance(Guid id)
        {
            if (!_authService.HasPermission("ManageAttendance"))
            {
                return PartialView("_AccessDenied");
            }

            try
            {
                var attendance = await _attendanceService.GetAttendance(id);
                if (attendance == null)
                {
                    return PartialView("_NotFound");
                }

                var updateDto = new UpdateAttendanceDTO
                {
                    AttendanceId = attendance.AttendanceId,
                    ClassDate = attendance.ClassDate,
                    ScheduledStartTime = attendance.ScheduledStartTime,
                    ScheduledEndTime = attendance.ScheduledEndTime,
                    Status = attendance.Status,
                    CheckInTime = attendance.CheckInTime,
                    CheckOutTime = attendance.CheckOutTime,
                    MinutesLate = attendance.MinutesLate,
                    MakeupRequired = attendance.MakeupRequired,
                    MakeupScheduledDate = attendance.MakeupScheduledDate,
                    HomeworkSubmitted = attendance.HomeworkSubmitted,
                    ParticipationScore = attendance.ParticipationScore,
                    Notes = attendance.Notes
                };

                return PartialView("_editAttendance", updateDto);
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error loading attendance: {ex.Message}");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAttendance(UpdateAttendanceDTO updateAttendanceDTO)
        {
            try
            {
                if (!_authService.HasPermission("ManageAttendance"))
                {
                    return PartialView("_AccessDenied");
                }

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.ToDictionary(kvp => kvp.Key, kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray());
                    return Json(new { validationErrors = errors });
                }

                // Set ModifiedBy from current user
                updateAttendanceDTO.ModifiedBy = _authService.GetCurrentUserId()?.ToString() ?? Guid.Empty.ToString();

                var result = await _attendanceService.UpdateAttendance(updateAttendanceDTO);
                return _returnHelper.ReturnNewResult(result.Flag, result.Message);
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error updating attendance: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> DetailsAttendance(Guid id)
        {
            if (!_authService.HasPermission("ViewAttendance"))
            {
                return PartialView("_AccessDenied");
            }

            try
            {
                var attendance = await _attendanceService.GetAttendance(id);
                if (attendance == null)
                {
                    return PartialView("_NotFound");
                }

                return PartialView("_detailsAttendance", attendance);
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error loading attendance details: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> DeleteAttendance(Guid id)
        {
            try
            {
                if (!_authService.HasPermission("DeleteAttendance"))
                {
                    return PartialView("_AccessDenied");
                }

                var attendance = await _attendanceService.GetAttendance(id);
                if (attendance == null)
                {
                    return _returnHelper.ReturnNewResult(false, "Attendance not found");
                }

                return PartialView("_deleteAttendance", attendance);
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error loading attendance: {ex.Message}");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAttendanceConfirmed(GetAttendanceDTO dto)
        {
            try
            {
                if (!_authService.HasPermission("DeleteAttendance"))
                {
                    return PartialView("_AccessDenied");
                }

                var result = await _attendanceService.DeleteAttendance(dto.AttendanceId);
                return _returnHelper.ReturnNewResult(result.Flag, result.Message);
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error deleting attendance: {ex.Message}");
            }
        }

        [HttpGet]
        public IActionResult BulkAttendance()
        {
            if (!_authService.HasPermission("ManageAttendance"))
            {
                return PartialView("_AccessDenied");
            }

            var model = new BulkAttendanceDTO();
            return PartialView("_bulkAttendance", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RecordBulkAttendance(BulkAttendanceDTO bulkAttendanceDTO)
        {
            try
            {
                if (!_authService.HasPermission("ManageAttendance"))
                {
                    return PartialView("_AccessDenied");
                }

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.ToDictionary(kvp => kvp.Key, kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray());
                    return Json(new { validationErrors = errors });
                }

                // Set CreatedBy from current user
                bulkAttendanceDTO.CreatedBy = _authService.GetCurrentUserId()?.ToString() ?? Guid.Empty.ToString();

                var result = await _attendanceService.RecordBulkAttendance(bulkAttendanceDTO);
                return _returnHelper.ReturnNewResult(result.Flag, result.Message);
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error recording bulk attendance: {ex.Message}");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateBulkAttendance(BulkAttendanceDTO bulkAttendanceDTO)
        {
            try
            {
                if (!_authService.HasPermission("ManageAttendance"))
                {
                    return PartialView("_AccessDenied");
                }

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.ToDictionary(kvp => kvp.Key, kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray());
                    return Json(new { validationErrors = errors });
                }

                // Set CreatedBy from current user
                bulkAttendanceDTO.CreatedBy = _authService.GetCurrentUserId()?.ToString() ?? Guid.Empty.ToString();

                var result = await _attendanceService.UpdateBulkAttendance(bulkAttendanceDTO);
                return _returnHelper.ReturnNewResult(result.Flag, result.Message);
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error updating bulk attendance: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> AttendanceReport(Guid sectionId, DateTime? startDate = null, DateTime? endDate = null)
        {
            if (!_authService.HasPermission("ViewAttendance"))
            {
                return PartialView("_AccessDenied");
            }

            try
            {
                var report = await _attendanceService.GetAttendanceReport(sectionId, startDate, endDate);
                return PartialView("_attendanceReport", report);
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error generating attendance report: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> AttendanceAlerts()
        {
            if (!_authService.HasPermission("ViewAttendance"))
            {
                return PartialView("_AccessDenied");
            }

            try
            {
                var alerts = await _attendanceService.GetAttendanceAlerts();
                return PartialView("_attendanceAlerts", alerts);
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error loading attendance alerts: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetStudentAttendanceRate(Guid studentId, Guid sectionId)
        {
            if (!_authService.HasPermission("ViewAttendance"))
            {
                return PartialView("_AccessDenied");
            }

            try
            {
                var rate = await _attendanceService.GetStudentAttendanceRate(studentId, sectionId);
                return Json(new { AttendanceRate = rate });
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error getting attendance rate: {ex.Message}");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ScheduleMakeupClass(Guid attendanceId, ScheduleMakeupRequest request)
        {
            try
            {
                if (!_authService.HasPermission("ManageAttendance"))
                {
                    return PartialView("_AccessDenied");
                }

                var result = await _attendanceService.ScheduleMakeupClass(attendanceId, request.MakeupDate, request.Notes);
                return _returnHelper.ReturnNewResult(result.Flag, result.Message);
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error scheduling makeup class: {ex.Message}");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckConsecutiveAbsences(CheckAbsencesRequest request)
        {
            try
            {
                if (!_authService.HasPermission("ManageAttendance"))
                {
                    return PartialView("_AccessDenied");
                }

                var result = await _attendanceService.CheckConsecutiveAbsences(request.StudentId, request.SectionId);
                return Json(result);
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error checking consecutive absences: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAttendanceFilterOptions()
        {
            if (!_authService.HasPermission("ViewAttendance"))
            {
                return PartialView("_AccessDenied");
            }

            try
            {
                var options = await _attendanceService.GetAttendanceFilterOptions();
                return Json(options);
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error loading filter options: {ex.Message}");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendAttendanceAlerts()
        {
            try
            {
                if (!_authService.HasPermission("ManageAttendance"))
                {
                    return PartialView("_AccessDenied");
                }

                var result = await _attendanceService.SendAttendanceAlerts();
                return _returnHelper.ReturnNewResult(result.Flag, result.Message);
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error sending attendance alerts: {ex.Message}");
            }
        }
    }
}
