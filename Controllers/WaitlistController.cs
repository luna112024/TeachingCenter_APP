using hongWenAPP.Helpers;
using hongWenAPP.Models;
using hongWenAPP.Models.WaitlistModel.DTOs;
using hongWenAPP.Services;
using Microsoft.AspNetCore.Mvc;

namespace hongWenAPP.Controllers
{
    public class WaitlistController : Controller
    {
        private readonly IWaitlistService _waitlistService;
        private readonly IStudentService _studentService;
        private readonly IClassSectionService _classSectionService;
        private readonly ReturnHelper _returnHelper;
        private readonly AuthenticationService _authService;

        public WaitlistController(IWaitlistService waitlistService, IStudentService studentService, IClassSectionService classSectionService, ReturnHelper returnHelper, AuthenticationService authService)
        {
            _waitlistService = waitlistService;
            _studentService = studentService;
            _classSectionService = classSectionService;
            _returnHelper = returnHelper;
            _authService = authService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(ListWaitlistDTOs model)
        {
            if (!_authService.HasPermission("ViewWaitlist"))
            {
                return PartialView("_AccessDenied");
            }
            var waitlists = await _waitlistService.GetAllWaitlists();
            var list = new ListWaitlistDTOs
            {
                waitlist = PageList<GetWaitlistDTO>.Create(waitlists, model.Page, model.PageSize, "ListWaitlist")
            };

            return View(list);
        }

        [HttpGet]
        public async Task<IActionResult> ListWaitlist(ListWaitlistDTOs model)
        {
            if (!_authService.HasPermission("ViewWaitlist"))
            {
                return PartialView("_AccessDenied");
            }
            var waitlists = await _waitlistService.GetAllWaitlists(status: model.SearchText);
            var list = PageList<GetWaitlistDTO>.Create(waitlists, model.Page, model.PageSize, "ListWaitlist");
            return PartialView("_ListWaitlists", list);
        }

        [HttpGet]
        public async Task<IActionResult> FetchWaitlist(string i)
        {
            if (!_authService.HasPermission("ViewWaitlist"))
            {
                return PartialView("_AccessDenied");
            }
            var waitlists = await _waitlistService.GetAllWaitlists(status: i);
            var result = waitlists.Select(w => new
            {
                w.WaitlistId,
                w.StudentCode,
                w.StudentName,
                w.CourseName,
                w.SectionName,
                w.Status,
                w.WaitlistDate,
                w.PriorityOrder,
                w.TeacherName
            }).ToList();
            return Json(result);
        }

        [HttpGet]
        public async Task<IActionResult> AddWaitlist()
        {
            if (!_authService.HasPermission("ManageWaitlist"))
            {
                return PartialView("_AccessDenied");
            }
            
            // Get active students and sections for dropdowns
            var students = await _studentService.GetAllStudents();
            var sections = await _classSectionService.GetAllClassSections();
            
            ViewBag.AvailableStudents = students;
            ViewBag.AvailableSections = sections;
            
            var model = new CreateWaitlistDTO
            {
                WaitlistDate = DateTime.Today,
                Status = "Waiting",
                PriorityOrder = 1
            };
            return PartialView("_addWaitlist", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddWaitlist(CreateWaitlistDTO createWaitlistDTO)
        {
            try
            {
                if (!_authService.HasPermission("ManageWaitlist"))
                {
                    return PartialView("_AccessDenied");
                }
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.ToDictionary(kvp => kvp.Key, kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray());
                    return Json(new { validationErrors = errors });
                }  
                var result = await _waitlistService.CreateWaitlist(createWaitlistDTO);
                return _returnHelper.ReturnNewResult(result.Flag, result.Message);
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditWaitlist(Guid id)
        {
            try
            {
                if (!_authService.HasPermission("ManageWaitlist"))
                {
                    return PartialView("_AccessDenied");
                }
                
                // Get active students and sections for dropdowns
                var students = await _studentService.GetAllStudents();
                var sections = await _classSectionService.GetAllClassSections();
                
                ViewBag.AvailableStudents = students;
                ViewBag.AvailableSections = sections;
                
                var waitlistData = await _waitlistService.GetWaitlist(id);

                var waitlistUpdateDTO = new UpdateWaitlistDTO
                {
                    WaitlistId = waitlistData.WaitlistId,
                    StudentId = waitlistData.StudentId,
                    SectionId = waitlistData.SectionId,
                    WaitlistDate = waitlistData.WaitlistDate,
                    PriorityOrder = waitlistData.PriorityOrder,
                    Status = waitlistData.Status,
                    Notes = waitlistData.Notes,
                    ModifiedBy = waitlistData.ModifiedBy
                };

                ViewBag.WaitlistId = id;
                return PartialView("_editWaitlist", waitlistUpdateDTO);
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error retrieving Waitlist: {ex.Message}");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditWaitlist(UpdateWaitlistDTO model)
        {
            try
            {
                if (!_authService.HasPermission("ManageWaitlist"))
                {
                    return PartialView("_AccessDenied");
                }
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.ToDictionary(kvp => kvp.Key, kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray());
                    return Json(new { validationErrors = errors });
                }

                
                var result = await _waitlistService.UpdateWaitlist(model);
                return _returnHelper.ReturnNewResult(result.Flag, result.Message ?? "Waitlist updated successfully");
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error updating Waitlist: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> DeleteWaitlist(Guid id)
        {
            try
            {
                if (!_authService.HasPermission("DeleteWaitlist"))
                {
                    return PartialView("_AccessDenied");
                }
                var waitlist = await _waitlistService.GetWaitlist(id);
                if (waitlist == null)
                {
                    return _returnHelper.ReturnNewResult(false, "Waitlist not found.");
                }

                return PartialView("_deleteWaitlist", waitlist);
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error loading delete form: {ex.Message}");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteWaitlistConfirmed(Guid WaitlistId)
        {
            try
            {
                if (!_authService.HasPermission("DeleteWaitlist"))
                {
                    return PartialView("_AccessDenied");
                }
                var result = await _waitlistService.DeleteWaitlist(WaitlistId);
                return _returnHelper.ReturnNewResult(result.Flag, result.Message ?? "Waitlist deleted successfully");
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error deleting waitlist: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> DetailsWaitlist(Guid id)
        {
            try
            {
                if (!_authService.HasPermission("ViewWaitlist"))
                {
                    return PartialView("_AccessDenied");
                }
                var waitlist = await _waitlistService.GetWaitlist(id);
                if (waitlist == null)
                {
                    return _returnHelper.ReturnNewResult(false, "Waitlist not found.");
                }

                return PartialView("_detailsWaitlist", waitlist);
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error loading waitlist details: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetWaitlistByStudent(Guid studentId)
        {
            if (!_authService.HasPermission("ViewWaitlist"))
            {
                return PartialView("_AccessDenied");
            }
            try
            {
                var waitlists = await _waitlistService.GetWaitlistByStudent(studentId);
                var result = waitlists.Select(w => new
                {
                    id = w.WaitlistId,
                    text = $"{w.CourseName} - {w.SectionName}",
                    courseName = w.CourseName,
                    sectionName = w.SectionName,
                    status = w.Status,
                    waitlistDate = w.WaitlistDate.ToString("yyyy-MM-dd"),
                    priorityOrder = w.PriorityOrder
                }).ToList();
                return Json(result);
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error loading waitlist by student: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetWaitlistBySection(Guid sectionId)
        {
            if (!_authService.HasPermission("ViewWaitlist"))
            {
                return PartialView("_AccessDenied");
            }
            try
            {
                var waitlists = await _waitlistService.GetWaitlistBySection(sectionId);
                var result = waitlists.Select(w => new
                {
                    id = w.WaitlistId,
                    text = $"{w.StudentName} ({w.StudentCode})",
                    studentName = w.StudentName,
                    studentCode = w.StudentCode,
                    status = w.Status,
                    waitlistDate = w.WaitlistDate.ToString("yyyy-MM-dd"),
                    priorityOrder = w.PriorityOrder
                }).ToList();
                return Json(result);
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error loading waitlist by section: {ex.Message}");
            }
        }

        [HttpGet]
        public IActionResult PromoteFromWaitlist(Guid id)
        {
            if (!_authService.HasPermission("ManageWaitlist"))
            {
                return PartialView("_AccessDenied");
            }
            try
            {
                ViewBag.WaitlistId = id;
                var model = new PromoteFromWaitlistDTO
                {
                    EnrollmentDate = DateTime.Today
                };
                return PartialView("_promoteFromWaitlist", model);
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error loading promote form: {ex.Message}");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PromoteFromWaitlist(Guid id, PromoteFromWaitlistDTO model)
        {
            try
            {
                if (!_authService.HasPermission("ManageWaitlist"))
                {
                    return PartialView("_AccessDenied");
                }
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.ToDictionary(kvp => kvp.Key, kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray());
                    return Json(new { validationErrors = errors });
                }
                
                
                
                var result = await _waitlistService.PromoteFromWaitlist(id, model);
                return _returnHelper.ReturnNewResult(result.Flag, result.Message ?? "Successfully promoted from waitlist to enrollment");
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error promoting from waitlist: {ex.Message}");
            }
        }

        [HttpGet]
        public IActionResult ReorderWaitlist(Guid sectionId)
        {
            if (!_authService.HasPermission("ManageWaitlist"))
            {
                return PartialView("_AccessDenied");
            }
            try
            {
                ViewBag.SectionId = sectionId;
                var model = new ReorderWaitlistDTO
                {
                    SectionId = sectionId,
                    WaitlistOrder = new List<WaitlistOrderItem>()
                };
                return PartialView("_reorderWaitlist", model);
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error loading reorder form: {ex.Message}");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReorderWaitlist(ReorderWaitlistDTO model)
        {
            try
            {
                if (!_authService.HasPermission("ManageWaitlist"))
                {
                    return PartialView("_AccessDenied");
                }
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.ToDictionary(kvp => kvp.Key, kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray());
                    return Json(new { validationErrors = errors });
                }
                
                
                
                var result = await _waitlistService.ReorderWaitlist(model);
                return _returnHelper.ReturnNewResult(result.Flag, result.Message ?? "Waitlist reordered successfully");
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error reordering waitlist: {ex.Message}");
            }
        }
    }
}
