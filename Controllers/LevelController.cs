using hongWenAPP.Helpers;
using hongWenAPP.Models;
using hongWenAPP.Models.LevelModel.DTOs;
using hongWenAPP.Services;
using Microsoft.AspNetCore.Mvc;

namespace hongWenAPP.Controllers
{
    public class LevelController : Controller
    {
        private readonly ILevelService _levelService;
        private readonly ReturnHelper _returnHelper;
        private readonly AuthenticationService _authService;

        public LevelController(ILevelService levelService, ReturnHelper returnHelper, AuthenticationService authService)
        {
            _levelService = levelService;
            _returnHelper = returnHelper;
            _authService = authService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(ListLevelDTOs model)
        {
            if (!_authService.HasPermission("ViewLevel"))
            {
                return PartialView("_AccessDenied");
            }
            var levels = await _levelService.GetAllLevels();
            var list = new ListLevelDTOs
            {
                level = PageList<GetLevelDTO>.Create(levels, model.Page, model.PageSize, "ListLevel")
            };

            return View(list);
        }

        [HttpGet]
        public async Task<IActionResult> ListLevel(ListLevelDTOs model)
        {
            if (!_authService.HasPermission("ViewLevel"))
            {
                return PartialView("_AccessDenied");
            }
            var levels = await _levelService.GetAllLevels(levelName: model.SearchText);
            var list = PageList<GetLevelDTO>.Create(levels, model.Page, model.PageSize, "ListLevel");
            return PartialView("_ListLevels", list);
        }

        [HttpGet]
        public async Task<IActionResult> FetchLevel(string i)
        {
            if (!_authService.HasPermission("ViewLevel"))
            {
                return PartialView("_AccessDenied");
            }
            var levels = await _levelService.GetAllLevels(levelName: i);
            var result = levels.Select(l => new
            {
                l.LevelId,
                l.LevelName,
                l.LevelCode,
                l.HskLevel,
                l.CefrEquivalent,
                l.IsActive
            }).ToList();
            return Json(result);
        }

        [HttpGet]
        public IActionResult AddLevel()
        {
            if (!_authService.HasPermission("ManageLevel"))
            {
                return PartialView("_AccessDenied");
            }
            var model = new CreateLevelDTO();
            return PartialView("_addLevel", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddLevel(CreateLevelDTO createLevelDTO)
        {
            try
            {
                if (!_authService.HasPermission("ManageLevel"))
                {
                    return PartialView("_AccessDenied");
                }
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.ToDictionary(kvp => kvp.Key, kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray());
                    return Json(new { validationErrors = errors });
                }
                var result = await _levelService.CreateLevel(createLevelDTO);
                return _returnHelper.ReturnNewResult(result.Flag, result.Message);
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditLevel(Guid id)
        {
            try
            {
                if (!_authService.HasPermission("ManageLevel"))
                {
                    return PartialView("_AccessDenied");
                }
                var levelData = await _levelService.GetLevel(id);

                var levelUpdateDTO = new UpdateLevelDTO
                {
                    LevelId = levelData.LevelId,
                    LevelCode = levelData.LevelCode,
                    LevelName = levelData.LevelName,
                    LevelNameChinese = levelData.LevelNameChinese,
                    LevelNameKhmer = levelData.LevelNameKhmer,
                    LevelOrder = levelData.LevelOrder,
                    ParentLevelId = levelData.ParentLevelId,
                    Description = levelData.Description,
                    DescriptionChinese = levelData.DescriptionChinese,
                    Prerequisites = levelData.Prerequisites,
                    LearningObjectives = levelData.LearningObjectives,
                    ExpectedHours = levelData.ExpectedHours,
                    HskLevel = levelData.HskLevel,
                    CefrEquivalent = levelData.CefrEquivalent,
                    MinAgeGroup = levelData.MinAgeGroup,
                    MaxAgeGroup = levelData.MaxAgeGroup,
                    IsActive = levelData.IsActive,
                    IsForBeginner = levelData.IsForBeginner,
                    IsForPlacement = levelData.IsForPlacement,
                    ModifiedBy = levelData.ModifiedBy
                };

                ViewBag.LevelId = id;
                return PartialView("_editLevel", levelUpdateDTO);
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error retrieving Level: {ex.Message}");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditLevel(UpdateLevelDTO model)
        {
            try
            {
                if (!_authService.HasPermission("ManageLevel"))
                {
                    return PartialView("_AccessDenied");
                }
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.ToDictionary(kvp => kvp.Key, kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray());
                    return Json(new { validationErrors = errors });
                }
                var result = await _levelService.UpdateLevel(model);
                return _returnHelper.ReturnNewResult(result.Flag, result.Message ?? "Level updated successfully");
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error updating Level: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> DeleteLevel(Guid id)
        {
            try
            {
                if (!_authService.HasPermission("DeleteLevel"))
                {
                    return PartialView("_AccessDenied");
                }
                var level = await _levelService.GetLevel(id);
                if (level == null)
                {
                    return _returnHelper.ReturnNewResult(false, "Level not found.");
                }

                var levelToDelete = new UpdateLevelDTO
                {
                    LevelId = level.LevelId,
                    LevelName = level.LevelName,
                    LevelCode = level.LevelCode,
                    HskLevel = level.HskLevel
                };

                return PartialView("_deleteLevel", levelToDelete);
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error loading delete form: {ex.Message}");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteLevelConfirmed(UpdateLevelDTO model)
        {
            try
            {
                if (!_authService.HasPermission("DeleteLevel"))
                {
                    return PartialView("_AccessDenied");
                }
                var result = await _levelService.DeleteLevel(model.LevelId);
                return _returnHelper.ReturnNewResult(result.Flag, result.Message ?? "Level deleted successfully");
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error deleting level: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetActiveLevels()
        {
            if (!_authService.HasPermission("ViewLevel"))
            {
                return PartialView("_AccessDenied");
            }
            try
            {
                var levels = await _levelService.GetActiveLevels();
                var result = levels.Select(l => new
                {
                    id = l.LevelId,
                    text = l.LevelName,
                    code = l.LevelCode,
                    hskLevel = l.HskLevel,
                    cefrEquivalent = l.CefrEquivalent
                }).ToList();
                return Json(result);
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error loading levels: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetLevelsList()
        {
            if (!_authService.HasPermission("ViewLevel"))
            {
                return PartialView("_AccessDenied");
            }
            try
            {
                var levels = await _levelService.GetLevelsList(isActive: true);
                var result = levels.Select(l => new
                {
                    id = l.LevelId,
                    text = l.LevelName,
                    code = l.LevelCode,
                    hskLevel = l.HskLevel,
                    cefrEquivalent = l.CefrEquivalent
                }).ToList();
                return Json(result);
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error loading levels: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetLevel(Guid id)
        {
            if (!_authService.HasPermission("ViewLevel"))
            {
                return PartialView("_AccessDenied");
            }
            try
            {
                var level = await _levelService.GetLevel(id);
                if (level == null)
                {
                    return _returnHelper.ReturnNewResult(false, "Level not found.");
                }
                return Json(level);
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error loading level: {ex.Message}");
            }
        }
    }
}
