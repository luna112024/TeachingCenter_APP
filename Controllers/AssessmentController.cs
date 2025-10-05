using hongWenAPP.Helpers;
using hongWenAPP.Models;
using hongWenAPP.Models.AssessmentModel.DTOs;
using hongWenAPP.Services;
using Microsoft.AspNetCore.Mvc;

namespace hongWenAPP.Controllers
{
    public class AssessmentController : Controller
    {
        private readonly IAssessmentService _assessmentService;
        private readonly IClassSectionService _classSectionService;
        private readonly ReturnHelper _returnHelper;
        private readonly AuthenticationService _authService;

        public AssessmentController(IAssessmentService assessmentService, IClassSectionService classSectionService, ReturnHelper returnHelper, AuthenticationService authService)
        {
            _assessmentService = assessmentService;
            _classSectionService = classSectionService;
            _returnHelper = returnHelper;
            _authService = authService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(ListAssessmentDTOs model)
        {
            if (!_authService.HasPermission("ViewAssessment"))
            {
                return PartialView("_AccessDenied");
            }
            var assessments = await _assessmentService.GetAllAssessments();
            var list = new ListAssessmentDTOs
            {
                assessment = PageList<GetAssessmentDTO>.Create(assessments, model.Page, model.PageSize, "ListAssessment")
            };

            return View(list);
        }

        [HttpGet]
        public async Task<IActionResult> ListAssessment(ListAssessmentDTOs model)
        {
            if (!_authService.HasPermission("ViewAssessment"))
            {
                return PartialView("_AccessDenied");
            }
            var assessments = await _assessmentService.GetAllAssessments();
            var list = PageList<GetAssessmentDTO>.Create(assessments, model.Page, model.PageSize, "ListAssessment");
            return PartialView("_ListAssessments", list);
        }

        [HttpGet]
        public async Task<IActionResult> GetAssessmentsBySection(Guid sectionId)
        {
            if (!_authService.HasPermission("ViewAssessment"))
            {
                return PartialView("_AccessDenied");
            }
            var assessments = await _assessmentService.GetAssessmentsBySection(sectionId);
            return Json(assessments);
        }

        [HttpGet]
        public async Task<IActionResult> GetUpcomingAssessments(DateTime? fromDate = null, DateTime? toDate = null)
        {
            if (!_authService.HasPermission("ViewAssessment"))
            {
                return PartialView("_AccessDenied");
            }
            var assessments = await _assessmentService.GetUpcomingAssessments(fromDate, toDate);
            return Json(assessments);
        }

        [HttpGet]
        public async Task<IActionResult> GetAssessmentsByDateRange(DateTime startDate, DateTime endDate)
        {
            if (!_authService.HasPermission("ViewAssessment"))
            {
                return PartialView("_AccessDenied");
            }
            var assessments = await _assessmentService.GetAssessmentsByDateRange(startDate, endDate);
            return Json(assessments);
        }

        [HttpGet]
        public IActionResult AddAssessment()
        {
            if (!_authService.HasPermission("ManageAssessment"))
            {
                return PartialView("_AccessDenied");
            }
            var model = new CreateAssessmentDTO();
            return PartialView("_addAssessment", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddAssessment(CreateAssessmentDTO createAssessmentDTO)
        {
            try
            {
                if (!_authService.HasPermission("ManageAssessment"))
                {
                    return PartialView("_AccessDenied");
                }
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.ToDictionary(kvp => kvp.Key, kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray());
                    return Json(new { validationErrors = errors });
                }

                // Set CreatedBy from current user
                createAssessmentDTO.CreatedBy = _authService.GetCurrentUserId()?.ToString() ?? Guid.Empty.ToString();

                var result = await _assessmentService.CreateAssessment(createAssessmentDTO);
                return _returnHelper.ReturnNewResult(result.Flag, result.Message);
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error creating assessment: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditAssessment(Guid id)
        {
            if (!_authService.HasPermission("ManageAssessment"))
            {
                return PartialView("_AccessDenied");
            }
            var assessment = await _assessmentService.GetAssessment(id);
            if (assessment == null)
            {
                return PartialView("_NotFound");
            }

            var updateDto = new UpdateAssessmentDTO
            {
                AssessmentId = assessment.AssessmentId,
                SectionId = assessment.SectionId,
                AssessmentName = assessment.AssessmentName,
                AssessmentType = assessment.AssessmentType,
                Description = assessment.Description,
                MaxScore = assessment.MaxScore,
                WeightPercentage = assessment.WeightPercentage,
                AssessmentDate = assessment.AssessmentDate,
                DueDate = assessment.DueDate,
                MaterialsNeeded = assessment.MaterialsNeeded
            };

            return PartialView("_editAssessment", updateDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAssessment(UpdateAssessmentDTO updateAssessmentDTO)
        {
            try
            {
                if (!_authService.HasPermission("ManageAssessment"))
                {
                    return PartialView("_AccessDenied");
                }
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.ToDictionary(kvp => kvp.Key, kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray());
                    return Json(new { validationErrors = errors });
                }

                // Set ModifiedBy from current user
                updateAssessmentDTO.ModifiedBy = _authService.GetCurrentUserId()?.ToString() ?? Guid.Empty.ToString();

                var result = await _assessmentService.UpdateAssessment(updateAssessmentDTO);
                return _returnHelper.ReturnNewResult(result.Flag, result.Message);
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error updating assessment: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> DetailsAssessment(Guid id)
        {
            if (!_authService.HasPermission("ViewAssessment"))
            {
                return PartialView("_AccessDenied");
            }
            var assessment = await _assessmentService.GetAssessment(id);
            if (assessment == null)
            {
                return PartialView("_NotFound");
            }
            return PartialView("_detailsAssessment", assessment);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteAssessment(Guid id)
        {
            try
            {
                if (!_authService.HasPermission("DeleteAssessment"))
                {
                    return PartialView("_AccessDenied");
                }

                var assessment = await _assessmentService.GetAssessment(id);
                if (assessment == null)
                {
                    return _returnHelper.ReturnNewResult(false, "Assessment not found");
                }

                return PartialView("_deleteAssessment", assessment);
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error loading assessment: {ex.Message}");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAssessmentConfirmed(GetAssessmentDTO dto)
        {
            try
            {
                if (!_authService.HasPermission("DeleteAssessment"))
                {
                    return PartialView("_AccessDenied");
                }

                // Check if assessment can be deleted
                var canDelete = await _assessmentService.CanDeleteAssessment(dto.AssessmentId);
                if (!canDelete)
                {
                    return _returnHelper.ReturnNewResult(false, "Assessment cannot be deleted as it has recorded grades");
                }

                var result = await _assessmentService.DeleteAssessment(dto.AssessmentId);
                return _returnHelper.ReturnNewResult(result.Flag, result.Message);
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error deleting assessment: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> DuplicateAssessment(Guid id)
        {
            if (!_authService.HasPermission("ManageAssessment"))
            {
                return PartialView("_AccessDenied");
            }
            var assessment = await _assessmentService.GetAssessment(id);
            if (assessment == null)
            {
                return PartialView("_NotFound");
            }

            var duplicateDto = new DuplicateAssessmentDTO
            {
                NewAssessmentName = $"{assessment.AssessmentName} (Copy)",
                NewWeightPercentage = assessment.WeightPercentage,
                NewAssessmentDate = assessment.AssessmentDate.AddDays(7),
                NewDueDate = assessment.DueDate?.AddDays(7)
            };

            ViewBag.SourceAssessment = assessment;
            return PartialView("_duplicateAssessment", duplicateDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DuplicateAssessment(Guid id, DuplicateAssessmentDTO duplicateDto)
        {
            try
            {
                if (!_authService.HasPermission("ManageAssessment"))
                {
                    return PartialView("_AccessDenied");
                }
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.ToDictionary(kvp => kvp.Key, kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray());
                    return Json(new { validationErrors = errors });
                }

                // Set CreatedBy from current user
                duplicateDto.CreatedBy = _authService.GetCurrentUserId()?.ToString() ?? Guid.Empty.ToString();

                var result = await _assessmentService.DuplicateAssessment(id, duplicateDto);
                return _returnHelper.ReturnNewResult(result.Flag, result.Message);
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error duplicating assessment: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> ValidateAssessmentWeight(Guid sectionId, Guid? excludeAssessmentId = null)
        {
            if (!_authService.HasPermission("ViewAssessment"))
            {
                return PartialView("_AccessDenied");
            }
            var isValid = await _assessmentService.ValidateAssessmentWeight(sectionId, excludeAssessmentId);
            return Json(new { isValid });
        }
    }
}
