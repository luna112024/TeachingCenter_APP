using hongWenAPP.Helpers;
using hongWenAPP.Models;
using hongWenAPP.Models.ClassSectionModel.DTOs;
using hongWenAPP.Services;
using Microsoft.AspNetCore.Mvc;

namespace hongWenAPP.Controllers
{
    public class ClassSectionController : Controller
    {
        private readonly IClassSectionService _classSectionService;
        private readonly ICourseService _courseService;
        private readonly ITermService _termService;
        private readonly ITeacherService _teacherService;
        private readonly IClassroomService _classroomService;
        private readonly ReturnHelper _returnHelper;
        private readonly AuthenticationService _authService;

        public ClassSectionController(
            IClassSectionService classSectionService,
            ICourseService courseService,
            ITermService termService,
            ITeacherService teacherService,
            IClassroomService classroomService,
            ReturnHelper returnHelper,
            AuthenticationService authService)
        {
            _classSectionService = classSectionService;
            _courseService = courseService;
            _termService = termService;
            _teacherService = teacherService;
            _classroomService = classroomService;
            _returnHelper = returnHelper;
            _authService = authService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(ListClassSectionDTOs model)
        {
            if (!_authService.HasPermission("ViewClassSections"))
            {
                return PartialView("_AccessDenied");
            }

            var sections = await _classSectionService.GetAllClassSections(
                sectionCode: model.SearchText,
                sectionName: model.SearchText,
                status: model.StatusFilter,
                courseId: model.CourseFilter,
                termId: model.TermFilter,
                teacherId: model.TeacherFilter);

            var list = new ListClassSectionDTOs
            {
                classSection = PageList<GetClassSectionDTO>.Create(sections, model.Page, model.PageSize, "ListClassSection")
            };

            // Load filter data
            await LoadFilterData();
            return View(list);
        }

        [HttpGet]
        public async Task<IActionResult> ListClassSection(ListClassSectionDTOs model)
        {
            if (!_authService.HasPermission("ViewClassSections"))
            {
                return PartialView("_AccessDenied");
            }

            var sections = await _classSectionService.GetAllClassSections(
                sectionCode: model.SearchText,
                sectionName: model.SearchText,
                status: model.StatusFilter,
                courseId: model.CourseFilter,
                termId: model.TermFilter,
                teacherId: model.TeacherFilter);

            var list = PageList<GetClassSectionDTO>.Create(sections, model.Page, model.PageSize, "ListClassSection");
            return PartialView("_ListClassSections", list);
        }

        [HttpGet]
        public async Task<IActionResult> AddClassSection()
        {
            if (!_authService.HasPermission("ManageClassSections"))
            {
                return PartialView("_AccessDenied");
            }

            await LoadFilterData();
            var model = new CreateClassSectionDTO
            {
                Status = "Planning"
            };

            return PartialView("_addClassSection", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddClassSection(CreateClassSectionDTO createClassSectionDTO)
        {
            try
            {
                if (!_authService.HasPermission("ManageClassSections"))
                {
                    return PartialView("_AccessDenied");
                }

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.ToDictionary(kvp => kvp.Key, kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray());
                    return Json(new { validationErrors = errors });
                }

                var result = await _classSectionService.CreateClassSection(createClassSectionDTO);
                return _returnHelper.ReturnNewResult(result.Flag, result.Message);
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditClassSection(Guid id)
        {
            try
            {
                if (!_authService.HasPermission("ManageClassSections"))
                {
                    return PartialView("_AccessDenied");
                }

                var sectionData = await _classSectionService.GetClassSection(id);
                if (sectionData == null)
                {
                    return _returnHelper.ReturnNewResult(false, "Class Section not found");
                }

                var sectionUpdateDTO = new UpdateClassSectionDTO
                {
                    SectionId = sectionData.SectionId,
                    CourseId = sectionData.CourseId,
                    TermId = sectionData.TermId,
                    TeacherId = sectionData.TeacherId,
                    ClassroomId = sectionData.ClassroomId,
                    SectionCode = sectionData.SectionCode,
                    SectionName = sectionData.SectionName,
                    StartDate = sectionData.StartDate,
                    EndDate = sectionData.EndDate,
                    SchedulePattern = sectionData.SchedulePattern,
                    MaxEnrollment = sectionData.MaxEnrollment,
                    CurrentEnrollment = sectionData.CurrentEnrollment,
                    WaitlistCount = sectionData.WaitlistCount,
                    TuitionFee = sectionData.TuitionFee,
                    MaterialsFee = sectionData.MaterialsFee,
                    RegistrationFee = sectionData.RegistrationFee,
                    Status = sectionData.Status,
                    Notes = sectionData.Notes,
                    ModifiedBy = sectionData.ModifiedBy
                };

                await LoadFilterData();
                ViewBag.SectionId = id;
                ViewBag.SectionData = sectionData;
                return PartialView("_editClassSection", sectionUpdateDTO);
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error retrieving Class Section: {ex.Message}");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditClassSection(UpdateClassSectionDTO model)
        {
            try
            {
                if (!_authService.HasPermission("ManageClassSections"))
                {
                    return PartialView("_AccessDenied");
                }

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.ToDictionary(kvp => kvp.Key, kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray());
                    return Json(new { validationErrors = errors });
                }

                var result = await _classSectionService.UpdateClassSection(model);
                return _returnHelper.ReturnNewResult(result.Flag, result.Message ?? "Class Section updated successfully");
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error updating Class Section: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> DeleteClassSection(Guid id)
        {
            try
            {
                if (!_authService.HasPermission("DeleteClassSections"))
                {
                    return PartialView("_AccessDenied");
                }

                var section = await _classSectionService.GetClassSection(id);
                if (section == null)
                {
                    return _returnHelper.ReturnNewResult(false, "Class Section not found.");
                }

                var sectionToDelete = new UpdateClassSectionDTO
                {
                    SectionId = section.SectionId,
                    SectionCode = section.SectionCode,
                    SectionName = section.SectionName,
                    Status = section.Status
                };

                return PartialView("_deleteClassSection", sectionToDelete);
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error loading delete form: {ex.Message}");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteClassSectionConfirmed(UpdateClassSectionDTO model)
        {
            try
            {
                if (!_authService.HasPermission("DeleteClassSections"))
                {
                    return PartialView("_AccessDenied");
                }

                var result = await _classSectionService.DeleteClassSection(model.SectionId);
                return _returnHelper.ReturnNewResult(result.Flag, result.Message ?? "Class Section deleted successfully");
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error deleting class section: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> DuplicateClassSection(Guid id)
        {
            try
            {
                if (!_authService.HasPermission("ManageClassSections"))
                {
                    return PartialView("_AccessDenied");
                }

                var section = await _classSectionService.GetClassSection(id);
                if (section == null)
                {
                    return _returnHelper.ReturnNewResult(false, "Class Section not found.");
                }

                var terms = await _termService.GetAllTerms();
                var duplicateDto = new DuplicateClassSectionDTO
                {
                    NewSectionCode = $"{section.SectionCode}_COPY",
                    NewTermId = section.TermId
                };

                ViewBag.Section = section;
                ViewBag.Terms = terms;
                return PartialView("_duplicateClassSection", duplicateDto);
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error loading duplicate form: {ex.Message}");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DuplicateClassSection(Guid id, DuplicateClassSectionDTO model)
        {
            try
            {
                if (!_authService.HasPermission("ManageClassSections"))
                {
                    return PartialView("_AccessDenied");
                }

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.ToDictionary(kvp => kvp.Key, kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray());
                    return Json(new { validationErrors = errors });
                }

                var result = await _classSectionService.DuplicateClassSection(id, model);
                return _returnHelper.ReturnNewResult(result.Flag, result.Message ?? "Class Section duplicated successfully");
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error duplicating class section: {ex.Message}");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OpenForEnrollment(Guid id)
        {
            try
            {
                if (!_authService.HasPermission("ManageClassSections"))
                {
                    return PartialView("_AccessDenied");
                }

                var result = await _classSectionService.UpdateSectionStatus(id, "Open");
                return _returnHelper.ReturnNewResult(result.Flag, result.Message ?? "Section opened for enrollment");
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error opening section: {ex.Message}");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StartSection(Guid id)
        {
            try
            {
                if (!_authService.HasPermission("ManageClassSections"))
                {
                    return PartialView("_AccessDenied");
                }

                var result = await _classSectionService.UpdateSectionStatus(id, "Running");
                return _returnHelper.ReturnNewResult(result.Flag, result.Message ?? "Section started");
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error starting section: {ex.Message}");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CompleteSection(Guid id)
        {
            try
            {
                if (!_authService.HasPermission("ManageClassSections"))
                {
                    return PartialView("_AccessDenied");
                }

                var result = await _classSectionService.UpdateSectionStatus(id, "Completed");
                return _returnHelper.ReturnNewResult(result.Flag, result.Message ?? "Section completed");
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error completing section: {ex.Message}");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelSection(Guid id)
        {
            try
            {
                if (!_authService.HasPermission("ManageClassSections"))
                {
                    return PartialView("_AccessDenied");
                }

                var result = await _classSectionService.UpdateSectionStatus(id, "Cancelled");
                return _returnHelper.ReturnNewResult(result.Flag, result.Message ?? "Section cancelled");
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error cancelling section: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> ValidateScheduleConflicts(Guid teacherId, Guid classroomId, string schedulePattern, DateTime startDate, DateTime endDate, Guid? excludeSectionId = null)
        {
            try
            {
                if (!_authService.HasPermission("ViewClassSections"))
                {
                    return PartialView("_AccessDenied");
                }

                var hasConflicts = !await _classSectionService.ValidateScheduleConflicts(
                    teacherId, classroomId, schedulePattern, startDate, endDate, excludeSectionId);

                return Json(new { hasConflicts });
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error validating schedule: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetTermDates(Guid termId)
        {
            try
            {
                if (!_authService.HasPermission("ViewClassSections"))
                {
                    return Json(new { success = false, message = "Access denied" });
                }

                var terms = await _termService.GetAllTerms();
                var selectedTerm = terms.FirstOrDefault(t => t.TermId == termId);
                
                if (selectedTerm == null)
                {
                    return Json(new { success = false, message = $"Term not found for ID: {termId}" });
                }

                var result = new { 
                    success = true, 
                    startDate = selectedTerm.StartDate.ToString("yyyy-MM-dd"),
                    endDate = selectedTerm.EndDate.ToString("yyyy-MM-dd"),
                    debug = new {
                        termId = termId.ToString(),
                        termName = selectedTerm.TermName,
                        rawStartDate = selectedTerm.StartDate.ToString(),
                        rawEndDate = selectedTerm.EndDate.ToString()
                    }
                };

                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error retrieving term dates: {ex.Message}" });
            }
        }

        // Add these new methods for searchable selects
        [HttpGet]
        public async Task<IActionResult> GetActiveCourses(string q = "")
        {
            try
            {
                if (!_authService.HasPermission("ViewClassSections"))
                {
                    return Json(new { success = false, message = "Access denied" });
                }

                var courses = await _courseService.GetAllCourses(status: "Active");
                
                var filteredCourses = courses
                    .Where(c => string.IsNullOrEmpty(q) || 
                               (c.CourseName != null && c.CourseName.ToLower().Contains(q.ToLower())) || 
                               (c.CourseCode != null && c.CourseCode.ToLower().Contains(q.ToLower())))
                    .Select(c => new { id = c.CourseId, text = c.CourseName, code = c.CourseCode })
                    .ToList();

                return Json(filteredCourses);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error retrieving courses: {ex.Message}" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetActiveTerms(string q = "")
        {
            try
            {
                if (!_authService.HasPermission("ViewClassSections"))
                {
                    return Json(new { success = false, message = "Access denied" });
                }

                var terms = await _termService.GetActiveTerm();
                
                var filteredTerms = terms
                    .Where(t => string.IsNullOrEmpty(q) || 
                               (t.TermName != null && t.TermName.ToLower().Contains(q.ToLower())) || 
                               (t.TermCode != null && t.TermCode.ToLower().Contains(q.ToLower())))
                    .Select(t => new { id = t.TermId, text = t.TermName, code = t.TermCode })
                    .ToList();

                return Json(filteredTerms);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error retrieving terms: {ex.Message}" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetActiveTeachers(string q = "")
        {
            try
            {
                if (!_authService.HasPermission("ViewClassSections"))
                {
                    return Json(new { success = false, message = "Access denied" });
                }

                var teachers = await _teacherService.GetAllTeachers();
                
                var filteredTeachers = teachers
                    .Where(t => string.IsNullOrEmpty(q) || 
                               (t.UserName != null && t.UserName.ToLower().Contains(q.ToLower())) || 
                               (t.TeacherCode != null && t.TeacherCode.ToLower().Contains(q.ToLower())))
                    .Select(t => new { id = t.TeacherId, text = t.UserName, code = t.TeacherCode })
                    .ToList();

                return Json(filteredTeachers);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error retrieving teachers: {ex.Message}" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetActiveClassrooms(string q = "")
        {
            try
            {
                if (!_authService.HasPermission("ViewClassSections"))
                {
                    return Json(new { success = false, message = "Access denied" });
                }

                var classrooms = await _classroomService.GetAllClassrooms(status: "Available");
                
                var filteredClassrooms = classrooms
                    .Where(c => string.IsNullOrEmpty(q) || 
                               (c.RoomName != null && c.RoomName.ToLower().Contains(q.ToLower())) || 
                               (c.RoomCode != null && c.RoomCode.ToLower().Contains(q.ToLower())))
                    .Select(c => new { id = c.ClassroomId, text = c.RoomName, code = c.RoomCode })
                    .ToList();

                return Json(filteredClassrooms);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error retrieving classrooms: {ex.Message}" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> StatusUpdateModal(Guid id, string newStatus)
        {
            try
            {
                if (!_authService.HasPermission("ManageClassSections"))
                {
                    return PartialView("_AccessDenied");
                }

                var section = await _classSectionService.GetClassSection(id);
                if (section == null)
                {
                    return _returnHelper.ReturnNewResult(false, "Class Section not found.");
                }

                ViewBag.SectionId = id;
                ViewBag.NewStatus = newStatus;
                ViewBag.CurrentStatus = section.Status;
                ViewBag.SectionCode = section.SectionCode;

                return PartialView("_StatusUpdateModal", new { SectionId = id, NewStatus = newStatus, CurrentStatus = section.Status, SectionCode = section.SectionCode });
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error loading status update modal: {ex.Message}");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatusConfirmed(Guid id, string status)
        {
            try
            {
                if (!_authService.HasPermission("ManageClassSections"))
                {
                    return PartialView("_AccessDenied");
                }

                var result = await _classSectionService.UpdateSectionStatus(id, status);
                return _returnHelper.ReturnNewResult(result.Flag, result.Message ?? "Status updated successfully");
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error updating status: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetClassSection(Guid id)
        {
            if (!_authService.HasPermission("ViewClassSections"))
            {
                return Json(new { error = "Access denied" });
            }
            try
            {
                var section = await _classSectionService.GetClassSection(id);
                if (section == null)
                {
                    return Json(new { error = "Class section not found" });
                }
                
                // Get term details to include term start and end dates
                var terms = await _termService.GetAllTerms();
                var term = terms.FirstOrDefault(t => t.TermId == section.TermId);
                
                // Return section data with term information including dates
                var result = new
                {
                    sectionId = section.SectionId,
                    sectionName = section.SectionName,
                    sectionCode = section.SectionCode,
                    termName = section.TermName,
                    courseName = section.CourseName,
                    teacherName = section.TeacherName,
                    classroomName = section.ClassroomName,
                    sectionStartDate = section.StartDate,
                    sectionEndDate = section.EndDate,
                    termStartDate = term?.StartDate,
                    termEndDate = term?.EndDate
                };
                
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetClassSections(string q = "")
        {
            if (!_authService.HasPermission("ViewClassSections"))
            {
                return Json(new List<object>());
            }
            try
            {
                var sections = await _classSectionService.GetAllClassSections();
                System.Diagnostics.Debug.WriteLine($"Found {sections?.Count ?? 0} class sections");
                
                var result = sections.Select(s => new
                {
                    id = s.SectionId,
                    text = s.SectionName ?? "Unknown",
                    code = s.SectionCode
                }).ToList();

                // Filter by search query if provided
                if (!string.IsNullOrEmpty(q))
                {
                    result = result.Where(s => 
                        s.text.ToLower().Contains(q.ToLower()) || 
                        s.code.ToLower().Contains(q.ToLower())
                    ).ToList();
                }

                System.Diagnostics.Debug.WriteLine($"Returning {result.Count} filtered sections");
                return Json(result);
            }
            catch (Exception ex)
            {
                // Log the exception for debugging
                System.Diagnostics.Debug.WriteLine($"Error in GetClassSections: {ex.Message}");
                return Json(new List<object>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllClassSections(string q = "")
        {
            if (!_authService.HasPermission("ViewClassSections"))
            {
                return Json(new List<object>());
            }
            try
            {
                var sections = await _classSectionService.GetAllClassSections();
                System.Diagnostics.Debug.WriteLine($"Found {sections?.Count ?? 0} class sections");
                
                var result = sections.Select(s => new
                {
                    id = s.SectionId,
                    text = s.SectionName ?? "Unknown",
                    sectionCode = s.SectionCode
                }).ToList();

                // Filter by search query if provided
                if (!string.IsNullOrEmpty(q))
                {
                    result = result.Where(s => 
                        s.text.ToLower().Contains(q.ToLower()) || 
                        s.sectionCode.ToLower().Contains(q.ToLower())
                    ).ToList();
                }

                System.Diagnostics.Debug.WriteLine($"Returning {result.Count} filtered sections");
                return Json(result);
            }
            catch (Exception ex)
            {
                // Log the exception for debugging
               
                return Json(new List<object>());
            }
        }

        private async Task LoadFilterData()
        {
            try
            {
                var courses = await _courseService.GetAllCourses();
                var terms = await _termService.GetAllTerms();
                var teachers = await _teacherService.GetAllTeachers();
                var classrooms = await _classroomService.GetAllClassrooms();

                ViewBag.Courses = courses;
                ViewBag.Terms = terms;
                ViewBag.Teachers = teachers;
                ViewBag.Classrooms = classrooms;
                ViewBag.StatusOptions = new[] { "Planning", "Open", "Full", "Running", "Completed", "Cancelled" };
            }
            catch (Exception ex)
            {
                // Log error but don't break the page
                ViewBag.Courses = new List<object>();
                ViewBag.Terms = new List<object>();
                ViewBag.Teachers = new List<object>();
                ViewBag.Classrooms = new List<object>();
                ViewBag.StatusOptions = new[] { "Planning", "Open", "Full", "Running", "Completed", "Cancelled" };
            }
        }
    }
}
