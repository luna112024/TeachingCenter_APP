using hongWenAPP.Helpers;
using hongWenAPP.Models;
using hongWenAPP.Models.ClassroomModel.DTOs;
using hongWenAPP.Services;
using Microsoft.AspNetCore.Mvc;

namespace hongWenAPP.Controllers
{
    public class ClassroomController : Controller
    {
        private readonly IClassroomService _classroomService;
        private readonly ReturnHelper _returnHelper;
        private readonly AuthenticationService _authService;

        public ClassroomController(IClassroomService classroomService, ReturnHelper returnHelper, AuthenticationService authService)
        {
            _classroomService = classroomService;
            _returnHelper = returnHelper;
            _authService = authService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(ListClassroomDTOs model)
        {
            if (!_authService.HasPermission("ViewClassroom"))
            {
                return PartialView("_AccessDenied");
            }
            var classrooms = await _classroomService.GetAllClassrooms();
            var list = new ListClassroomDTOs
            {
                classroom = PageList<GetClassroomDTO>.Create(classrooms, model.Page, model.PageSize, "ListClassroom")
            };

            return View(list);
        }

        [HttpGet]
        public async Task<IActionResult> ListClassroom(ListClassroomDTOs model)
        {
            if (!_authService.HasPermission("ViewClassroom"))
            {
                return PartialView("_AccessDenied");
            }
            var classrooms = await _classroomService.GetAllClassrooms(name: model.SearchText);
            var list = PageList<GetClassroomDTO>.Create(classrooms, model.Page, model.PageSize, "ListClassroom");
            return PartialView("_ListClassrooms", list);
        }

        [HttpGet]
        public async Task<IActionResult> FetchClassroom(string i)
        {
            if (!_authService.HasPermission("ViewClassroom"))
            {
                return PartialView("_AccessDenied");
            }
            var classrooms = await _classroomService.GetAllClassrooms(name: i);
            var result = classrooms.Select(c => new
            {
                c.ClassroomId,
                c.RoomCode,
                c.RoomName,
                c.Building,
                c.FloorLevel,
                c.Capacity,
                c.Status
            }).ToList();
            return Json(result);
        }

        [HttpGet]
        public IActionResult AddClassroom()
        {
            if (!_authService.HasPermission("ManageClassroom"))
            {
                return PartialView("_AccessDenied");
            }
            var model = new CreateClassroomDTO();
            return PartialView("_addClassroom", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddClassroom(CreateClassroomDTO createClassroomDTO)
        {
            try
            {
                if (!_authService.HasPermission("ManageClassroom"))
                {
                    return PartialView("_AccessDenied");
                }
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.ToDictionary(kvp => kvp.Key, kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray());
                    return Json(new { validationErrors = errors });
                }
                var result = await _classroomService.CreateClassroom(createClassroomDTO);
                return _returnHelper.ReturnNewResult(result.Flag, result.Message);
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditClassroom(Guid id)
        {
            try
            {
                if (!_authService.HasPermission("ManageClassroom"))
                {
                    return PartialView("_AccessDenied");
                }
                var classroomData = await _classroomService.GetClassroom(id);

                var classroomUpdateDTO = new UpdateClassroomDTO
                {
                    ClassroomId = classroomData.ClassroomId,
                    RoomCode = classroomData.RoomCode,
                    RoomName = classroomData.RoomName,
                    Building = classroomData.Building,
                    FloorLevel = classroomData.FloorLevel,
                    Capacity = classroomData.Capacity,
                    Equipment = classroomData.Equipment,
                    Facilities = classroomData.Facilities,
                    LocationNotes = classroomData.LocationNotes,
                    Status = classroomData.Status,
                    ModifiedBy = classroomData.ModifiedBy
                };

                ViewBag.ClassroomId = id;
                return PartialView("_editClassroom", classroomUpdateDTO);
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error retrieving Classroom: {ex.Message}");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditClassroom(UpdateClassroomDTO model)
        {
            try
            {
                if (!_authService.HasPermission("ManageClassroom"))
                {
                    return PartialView("_AccessDenied");
                }
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.ToDictionary(kvp => kvp.Key, kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray());
                    return Json(new { validationErrors = errors });
                }
                var result = await _classroomService.UpdateClassroom(model);
                return _returnHelper.ReturnNewResult(result.Flag, result.Message ?? "Classroom updated successfully");
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error updating Classroom: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> DeleteClassroom(Guid id)
        {
            try
            {
                if (!_authService.HasPermission("DeleteClassroom"))
                {
                    return PartialView("_AccessDenied");
                }
                var classroom = await _classroomService.GetClassroom(id);
                if (classroom == null)
                {
                    return _returnHelper.ReturnNewResult(false, "Classroom not found.");
                }

                var classroomToDelete = new UpdateClassroomDTO
                {
                    ClassroomId = classroom.ClassroomId,
                    RoomCode = classroom.RoomCode,
                    RoomName = classroom.RoomName,
                    Building = classroom.Building,
                    Capacity = classroom.Capacity,
                    Status = classroom.Status
                };

                return PartialView("_deleteClassroom", classroomToDelete);
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error loading delete form: {ex.Message}");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteClassroomConfirmed(UpdateClassroomDTO model)
        {
            try
            {
                if (!_authService.HasPermission("DeleteClassroom"))
                {
                    return PartialView("_AccessDenied");
                }
                var result = await _classroomService.DeleteClassroom(model.ClassroomId);
                return _returnHelper.ReturnNewResult(result.Flag, result.Message ?? "Classroom deleted successfully");
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error deleting classroom: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAvailableClassrooms()
        {
            if (!_authService.HasPermission("ViewClassroom"))
            {
                return PartialView("_AccessDenied");
            }
            try
            {
                var classrooms = await _classroomService.GetAvailableClassrooms();
                var result = classrooms.Select(c => new
                {
                    id = c.ClassroomId,
                    text = $"{c.RoomCode} - {c.RoomName}",
                    code = c.RoomCode,
                    name = c.RoomName,
                    building = c.Building,
                    capacity = c.Capacity,
                    status = c.Status
                }).ToList();
                return Json(result);
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error loading available classrooms: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetClassroomSchedule(Guid id)
        {
            if (!_authService.HasPermission("ViewClassroom"))
            {
                return PartialView("_AccessDenied");
            }
            try
            {
                var schedule = await _classroomService.GetClassroomSchedule(id);
                return Json(schedule);
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error loading classroom schedule: {ex.Message}");
            }
        }
    }
}
