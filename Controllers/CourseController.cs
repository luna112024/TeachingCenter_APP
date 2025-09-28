using hongWenAPP.Helpers;
using hongWenAPP.Models;
using hongWenAPP.Models.CourseModel.DTOs;
using hongWenAPP.Services;
using Microsoft.AspNetCore.Mvc;

namespace hongWenAPP.Controllers
{
    public class CourseController : Controller
    {
        private readonly ICourseService _courseService;
        private readonly ReturnHelper _returnHelper;
        private readonly AuthenticationService _authService;

        public CourseController(ICourseService courseService, ReturnHelper returnHelper, AuthenticationService authService)
        {
            _courseService = courseService;
            _returnHelper = returnHelper;
            _authService = authService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(ListCourseDTOs model)
        {
            if (!_authService.HasPermission("ViewCourse"))
            {
                return PartialView("_AccessDenied");
            }
            var courses = await _courseService.GetAllCourses();
            var list = new ListCourseDTOs
            {
                course = PageList<GetCourseDTO>.Create(courses, model.Page, model.PageSize, "ListCourse")
            };

            return View(list);
        }

        [HttpGet]
        public async Task<IActionResult> ListCourse(ListCourseDTOs model)
        {
            if (!_authService.HasPermission("ViewCourse"))
            {
                return PartialView("_AccessDenied");
            }
            var courses = await _courseService.GetAllCourses(name: model.SearchText);
            var list = PageList<GetCourseDTO>.Create(courses, model.Page, model.PageSize, "ListCourse");
            return PartialView("_ListCourses", list);
        }

        [HttpGet]
        public async Task<IActionResult> FetchCourse(string i)
        {
            if (!_authService.HasPermission("ViewCourse"))
            {
                return PartialView("_AccessDenied");
            }
            var courses = await _courseService.GetAllCourses(name: i);
            var result = courses.Select(c => new
            {
                c.CourseId,
                c.CourseName,
                c.CourseCode,
                c.LevelName,
                c.AgeGroup,
                c.Status,
                c.BaseFee
            }).ToList();
            return Json(result);
        }

        [HttpGet]
        public IActionResult AddCourse()
        {
            if (!_authService.HasPermission("ManageCourse"))
            {
                return PartialView("_AccessDenied");
            }
            var model = new CreateCourseDTO();
            return PartialView("_addCourse", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCourse(CreateCourseDTO createCourseDTO)
        {
            try
            {
                if (!_authService.HasPermission("ManageCourse"))
                {
                    return PartialView("_AccessDenied");
                }
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.ToDictionary(kvp => kvp.Key, kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray());
                    return Json(new { validationErrors = errors });
                }
                var result = await _courseService.CreateCourse(createCourseDTO);
                return _returnHelper.ReturnNewResult(result.Flag, result.Message);
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditCourse(Guid id)
        {
            try
            {
                if (!_authService.HasPermission("ManageCourse"))
                {
                    return PartialView("_AccessDenied");
                }
                var courseData = await _courseService.GetCourse(id);

                var courseUpdateDTO = new UpdateCourseDTO
                {
                    CourseId = courseData.CourseId,
                    CourseCode = courseData.CourseCode,
                    CourseName = courseData.CourseName,
                    CourseNameChinese = courseData.CourseNameChinese,
                    Description = courseData.Description,
                    DescriptionChinese = courseData.DescriptionChinese,
                    LevelId = courseData.LevelId,
                    DurationWeeks = courseData.DurationWeeks,
                    HoursPerWeek = courseData.HoursPerWeek,
                    TotalHours = courseData.TotalHours,
                    MaxStudents = courseData.MaxStudents,
                    MinStudents = courseData.MinStudents,
                    AgeGroup = courseData.AgeGroup,
                    Prerequisites = courseData.Prerequisites,
                    LearningOutcomes = courseData.LearningOutcomes,
                    MaterialsIncluded = courseData.MaterialsIncluded,
                    BaseFee = courseData.BaseFee,
                    MaterialsFee = courseData.MaterialsFee,
                    Status = courseData.Status,
                    ModifiedBy = courseData.ModifiedBy
                };

                ViewBag.CourseId = id;
                return PartialView("_editCourse", courseUpdateDTO);
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error retrieving Course: {ex.Message}");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCourse(UpdateCourseDTO model)
        {
            try
            {
                if (!_authService.HasPermission("ManageCourse"))
                {
                    return PartialView("_AccessDenied");
                }
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.ToDictionary(kvp => kvp.Key, kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray());
                    return Json(new { validationErrors = errors });
                }
                var result = await _courseService.UpdateCourse(model);
                return _returnHelper.ReturnNewResult(result.Flag, result.Message ?? "Course updated successfully");
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error updating Course: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> DeleteCourse(Guid id)
        {
            try
            {
                if (!_authService.HasPermission("DeleteCourse"))
                {
                    return PartialView("_AccessDenied");
                }
                var course = await _courseService.GetCourse(id);
                if (course == null)
                {
                    return _returnHelper.ReturnNewResult(false, "Course not found.");
                }

                var courseToDelete = new UpdateCourseDTO
                {
                    CourseId = course.CourseId,
                    CourseName = course.CourseName,
                    CourseCode = course.CourseCode,
                    LevelId = course.LevelId
                };

                return PartialView("_deleteCourse", courseToDelete);
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error loading delete form: {ex.Message}");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCourseConfirmed(UpdateCourseDTO model)
        {
            try
            {
                if (!_authService.HasPermission("DeleteCourse"))
                {
                    return PartialView("_AccessDenied");
                }
                var result = await _courseService.DeleteCourse(model.CourseId);
                return _returnHelper.ReturnNewResult(result.Flag, result.Message ?? "Course deleted successfully");
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error deleting course: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> DuplicateCourse(Guid id)
        {
            try
            {
                if (!_authService.HasPermission("ManageCourse"))
                {
                    return PartialView("_AccessDenied");
                }
                var course = await _courseService.GetCourse(id);
                if (course == null)
                {
                    return _returnHelper.ReturnNewResult(false, "Course not found.");
                }

                var duplicateCourseDTO = new DuplicateCourseDTO
                {
                    NewCourseCode = $"{course.CourseCode}_COPY"
                };

                ViewBag.OriginalCourse = course;
                return PartialView("_duplicateCourse", duplicateCourseDTO);
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error loading duplicate form: {ex.Message}");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DuplicateCourseConfirmed(DuplicateCourseDTO model, Guid originalCourseId)
        {
            try
            {
                if (!_authService.HasPermission("ManageCourse"))
                {
                    return PartialView("_AccessDenied");
                }
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.ToDictionary(kvp => kvp.Key, kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray());
                    return Json(new { validationErrors = errors });
                }
                var result = await _courseService.DuplicateCourse(originalCourseId, model.NewCourseCode, null);
                return _returnHelper.ReturnNewResult(result.Flag, result.Message ?? "Course duplicated successfully");
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error duplicating course: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetCoursesByLevel(string level)
        {
            if (!_authService.HasPermission("ViewCourse"))
            {
                return PartialView("_AccessDenied");
            }
            var courses = await _courseService.GetCoursesByLevel(level);
            return Json(courses);
        }

        [HttpGet]
        public async Task<IActionResult> GetCoursesByLevelId(Guid levelId)
        {
            if (!_authService.HasPermission("ViewCourse"))
            {
                return PartialView("_AccessDenied");
            }
            var courses = await _courseService.GetCoursesByLevelId(levelId);
            return Json(courses);
        }
    }
}
