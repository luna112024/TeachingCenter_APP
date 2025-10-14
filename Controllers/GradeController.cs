using hongWenAPP.Helpers;
using hongWenAPP.Models;
using hongWenAPP.Models.GradeModel.DTOs;
using hongWenAPP.Services;
using Microsoft.AspNetCore.Mvc;

namespace hongWenAPP.Controllers
{
    public class GradeController : Controller
    {
        private readonly IGradeService _gradeService;
        private readonly IAssessmentService _assessmentService;
        private readonly IClassSectionService _classSectionService;
        private readonly IStudentService _studentService;
        private readonly ReturnHelper _returnHelper;
        private readonly AuthenticationService _authService;

        public GradeController(IGradeService gradeService, IAssessmentService assessmentService, 
            IClassSectionService classSectionService, IStudentService studentService, 
            ReturnHelper returnHelper, AuthenticationService authService)
        {
            _gradeService = gradeService;
            _assessmentService = assessmentService;
            _classSectionService = classSectionService;
            _studentService = studentService;
            _returnHelper = returnHelper;
            _authService = authService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(ListGradeDTOs model)
        {
            if (!_authService.HasPermission("ViewGrade"))
            {
                return PartialView("_AccessDenied");
            }
            var grades = await _gradeService.GetAllGrades();
            var list = new ListGradeDTOs
            {
                grade = PageList<GetGradeDTO>.Create(grades, model.Page, model.PageSize, "ListGrade")
            };

            return View(list);
        }

        [HttpGet]
        public async Task<IActionResult> ListGrade(ListGradeDTOs model)
        {
            if (!_authService.HasPermission("ViewGrade"))
            {
                return PartialView("_AccessDenied");
            }
            var grades = await _gradeService.GetAllGrades();
            var list = PageList<GetGradeDTO>.Create(grades, model.Page, model.PageSize, "ListGrade");
            return PartialView("_ListGrades", list);
        }

        [HttpGet]
        public async Task<IActionResult> FetchGrade(string i)
        {
            if (!_authService.HasPermission("ViewGrade"))
            {
                return PartialView("_AccessDenied");
            }
            var grades = await _gradeService.GetAllGrades();
            var result = grades.Select(g => new
            {
                g.GradeId,
                g.StudentCode,
                StudentName = g.StudentName,
                g.AssessmentName,
                g.AssessmentType,
                g.SectionName,
                g.CourseName,
                g.EarnedScore,
                g.MaxScore,
                g.Percentage,
                g.LetterGrade,
                g.GradedDate
            }).ToList();
            return Json(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetGradesByAssessment(Guid assessmentId)
        {
            if (!_authService.HasPermission("ViewGrade"))
            {
                return PartialView("_AccessDenied");
            }
            var grades = await _gradeService.GetGradesByAssessment(assessmentId);
            return Json(grades);
        }

        [HttpGet]
        public async Task<IActionResult> GetGradesByStudent(Guid studentId)
        {
            if (!_authService.HasPermission("ViewGrade"))
            {
                return PartialView("_AccessDenied");
            }
            var grades = await _gradeService.GetGradesByStudent(studentId);
            return Json(grades);
        }

        [HttpGet]
        public async Task<IActionResult> GetGradesBySection(Guid sectionId)
        {
            if (!_authService.HasPermission("ViewGrade"))
            {
                return PartialView("_AccessDenied");
            }
            var grades = await _gradeService.GetGradesBySection(sectionId);
            return Json(grades);
        }

        [HttpGet]
        public async Task<IActionResult> GetStudentGradeHistory(Guid studentId)
        {
            if (!_authService.HasPermission("ViewGrade"))
            {
                return PartialView("_AccessDenied");
            }
            var gradeHistory = await _gradeService.GetStudentGradeHistory(studentId);
            return Json(gradeHistory);
        }

        [HttpGet]
        public async Task<IActionResult> GetGradeReport(Guid sectionId)
        {
            if (!_authService.HasPermission("ViewGrade"))
            {
                return PartialView("_AccessDenied");
            }
            var report = await _gradeService.GetGradeReport(sectionId);
            return Json(report);
        }

        [HttpGet]
        public async Task<IActionResult> GetGradeStatistics(Guid sectionId)
        {
            if (!_authService.HasPermission("ViewGrade"))
            {
                return PartialView("_AccessDenied");
            }
            var statistics = await _gradeService.GetGradeStatistics(sectionId);
            return Json(statistics);
        }

        [HttpGet]
        public IActionResult AddGrade()
        {
            if (!_authService.HasPermission("ManageGrade"))
            {
                return PartialView("_AccessDenied");
            }
            var model = new CreateGradeDTO();
            return PartialView("_addGrade", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddGrade(CreateGradeDTO createGradeDTO)
        {
            if (!_authService.HasPermission("ManageGrade"))
            {
                return PartialView("_AccessDenied");
            }
            if (!ModelState.IsValid)
            {
                var errors = ModelState.ToDictionary(kvp => kvp.Key, kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray());
                return Json(new { validationErrors = errors });
            }

            // Set GradedBy from current user
            createGradeDTO.GradedBy = _authService.GetCurrentUserId()?.ToString() ?? Guid.Empty.ToString();

            var result = await _gradeService.CreateGrade(createGradeDTO);
            return _returnHelper.ReturnNewResult(result.Flag, result.Message);
        }

        [HttpGet]
        public async Task<IActionResult> EditGrade(Guid id)
        {
            if (!_authService.HasPermission("ManageGrade"))
            {
                return PartialView("_AccessDenied");
            }
            var grade = await _gradeService.GetGrade(id);
            if (grade == null)
            {
                return PartialView("_NotFound");
            }

            var updateDto = new UpdateGradeDTO
            {
                GradeId = grade.GradeId,
                EarnedScore = grade.EarnedScore,
                Percentage = grade.Percentage,
                LetterGrade = grade.LetterGrade,
                Strengths = grade.Strengths,
                AreasForImprovement = grade.AreasForImprovement,
                TeacherComments = grade.TeacherComments,
                GradedDate = grade.GradedDate,
                LateSubmission = grade.LateSubmission,
                ResubmissionAllowed = grade.ResubmissionAllowed,
                GradedBy = grade.GradedBy
            };

            return PartialView("_editGrade", updateDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditGrade(UpdateGradeDTO updateGradeDTO)
        {
            if (!_authService.HasPermission("ManageGrade"))
            {
                return PartialView("_AccessDenied");
            }
            if (!ModelState.IsValid)
            {
                var errors = ModelState.ToDictionary(kvp => kvp.Key, kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray());
                return Json(new { validationErrors = errors });
            }

            // Set ModifiedBy from current user
            updateGradeDTO.ModifiedBy = _authService.GetCurrentUserId()?.ToString() ?? Guid.Empty.ToString();

            var result = await _gradeService.UpdateGrade(updateGradeDTO);
            return _returnHelper.ReturnNewResult(result.Flag, result.Message);
        }

        [HttpGet]
        public async Task<IActionResult> DetailsGrade(Guid id)
        {
            if (!_authService.HasPermission("ViewGrade"))
            {
                return PartialView("_AccessDenied");
            }
            var grade = await _gradeService.GetGrade(id);
            if (grade == null)
            {
                return PartialView("_NotFound");
            }
            return PartialView("_detailsGrade", grade);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteGrade(Guid id)
        {
            if (!_authService.HasPermission("DeleteGrade"))
            {
                return PartialView("_AccessDenied");
            }

            var grade = await _gradeService.GetGrade(id);
            if (grade == null)
            {
                return _returnHelper.ReturnNewResult(false, "Grade not found");
            }

            return PartialView("_deleteGrade", grade);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteGradeConfirmed(GetGradeDTO dto)
        {
            if (!_authService.HasPermission("DeleteGrade"))
            {
                return PartialView("_AccessDenied");
            }

            // Check if grade can be modified (deleted)
            var canModify = await _gradeService.CanModifyGrade(dto.GradeId);
            if (!canModify)
            {
                return _returnHelper.ReturnNewResult(false, "Grade cannot be deleted as it's outside the editable period");
            }

            var result = await _gradeService.DeleteGrade(dto.GradeId);
            return _returnHelper.ReturnNewResult(result.Flag, result.Message);
        }

        [HttpGet]
        public IActionResult BulkGradeEntry(Guid assessmentId)
        {
            if (!_authService.HasPermission("ManageGrade"))
            {
                return PartialView("_AccessDenied");
            }
            var model = new BulkGradeEntryDTO
            {
                AssessmentId = assessmentId
            };
            return PartialView("_bulkGradeEntry", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BulkGradeEntry(BulkGradeEntryDTO bulkGradeDTO)
        {
            if (!_authService.HasPermission("ManageGrade"))
            {
                return PartialView("_AccessDenied");
            }
            if (!ModelState.IsValid)
            {
                var errors = ModelState.ToDictionary(kvp => kvp.Key, kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray());
                return Json(new { validationErrors = errors });
            }

            // Set GradedBy from current user
            bulkGradeDTO.GradedBy = _authService.GetCurrentUserId()?.ToString() ?? Guid.Empty.ToString();

            var result = await _gradeService.BulkGradeEntry(bulkGradeDTO);
            return _returnHelper.ReturnNewResult(result.Flag, result.Message);
        }

        [HttpGet]
        public IActionResult CalculateFinalGrades(Guid sectionId)
        {
            if (!_authService.HasPermission("ManageGrade"))
            {
                return PartialView("_AccessDenied");
            }
            var model = new FinalGradeCalculationDTO
            {
                SectionId = sectionId
            };
            return PartialView("_calculateFinalGrades", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CalculateFinalGrades(FinalGradeCalculationDTO calculationDTO)
        {
            if (!_authService.HasPermission("ManageGrade"))
            {
                return PartialView("_AccessDenied");
            }

            // Set CalculatedBy from current user
            calculationDTO.CalculatedBy = _authService.GetCurrentUserId()?.ToString() ?? Guid.Empty.ToString();

            var result = await _gradeService.CalculateFinalGrades(calculationDTO);
            return _returnHelper.ReturnNewResult(result.Flag, result.Message);
        }

        [HttpGet]
        public async Task<IActionResult> ValidateGradeRange(decimal earnedScore, decimal maxScore)
        {
            if (!_authService.HasPermission("ViewGrade"))
            {
                return PartialView("_AccessDenied");
            }
            var isValid = await _gradeService.ValidateGradeRange(earnedScore, maxScore);
            return Json(new { isValid });
        }

        [HttpGet]
        public async Task<IActionResult> ValidateAssessmentWeightTotal(Guid sectionId)
        {
            if (!_authService.HasPermission("ViewGrade"))
            {
                return PartialView("_AccessDenied");
            }
            var isValid = await _gradeService.ValidateAssessmentWeightTotal(sectionId);
            return Json(new { isValid });
        }

        [HttpGet]
        public async Task<IActionResult> GetAllGrades(string q = "")
        {
            if (!_authService.HasPermission("ViewGrade"))
            {
                return Json(new List<object>());
            }
            var grades = await _gradeService.GetAllGrades();
            var result = grades.Select(g => new
            {
                id = g.GradeId,
                text = $"{g.StudentName} - {g.AssessmentName}" ?? "Unknown",
                studentCode = g.StudentCode,
                assessmentName = g.AssessmentName,
                earnedScore = g.EarnedScore,
                maxScore = g.MaxScore,
                percentage = g.Percentage,
                letterGrade = g.LetterGrade
            }).ToList();

            // Filter by search query if provided
            if (!string.IsNullOrEmpty(q))
            {
                result = result.Where(g => 
                    g.text.ToLower().Contains(q.ToLower()) || 
                    g.studentCode.ToLower().Contains(q.ToLower()) ||
                    g.assessmentName.ToLower().Contains(q.ToLower())
                ).ToList();
            }

            return Json(result);
        }
    }
}
