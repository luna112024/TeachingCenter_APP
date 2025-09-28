using hongWenAPP.Helpers;
using hongWenAPP.Models;
using hongWenAPP.Models.TermModel.DTOs;
using hongWenAPP.Services;
using Microsoft.AspNetCore.Mvc;

namespace hongWenAPP.Controllers
{
    public class TermController : Controller
    {
        private readonly ITermService _termService;
        private readonly ReturnHelper _returnHelper;
        private readonly AuthenticationService _authService;

        public TermController(ITermService termService, ReturnHelper returnHelper, AuthenticationService authService)
        {
            _termService = termService;
            _returnHelper = returnHelper;
            _authService = authService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(ListTermDTOs model)
        {
            if (!_authService.HasPermission("ViewTerm"))
            {
                return PartialView("_AccessDenied");
            }
            // Use SearchText as termName filter if provided
            var terms = await _termService.GetAllTerms(termName: model.SearchText);
            var list = new ListTermDTOs
            {
                term = PageList<GetTermDTO>.Create(terms, model.Page, model.PageSize, "ListTerm")
            };
            
            return View(list);
        }

        [HttpGet]
        public async Task<IActionResult> ListTerm(ListTermDTOs model)
        {
            if (!_authService.HasPermission("ViewTerm"))
            {
                return PartialView("_AccessDenied");
            }
            // Fix: Pass SearchText as termName parameter, not academicYear
            var terms = await _termService.GetAllTerms(termName: model.SearchText);
            var list = PageList<GetTermDTO>.Create(terms, model.Page, model.PageSize, "ListTerm");
            return PartialView("_ListTerms", list);
        }

        [HttpGet]
        public async Task<IActionResult> FetchTerm(string i)
        {
            if (!_authService.HasPermission("ViewTerm"))
            {
                return PartialView("_AccessDenied");
            }
            var terms = await _termService.GetAllTerms(termName: i);
            var result = terms.Select(t => new
            {
                t.TermId,
                t.TermName,
                t.TermCode,
                t.AcademicYear,
                t.Status,
                t.Iscurrent
            }).ToList();
            return Json(result);
        }

        [HttpGet]
        public IActionResult AddTerm()
        {
            if (!_authService.HasPermission("ManageTerm"))
            {
                return PartialView("_AccessDenied");
            }
            return PartialView("_addTerm");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddTerm(CreateTermDTO createTermDTO)
        {
            try
            {
                if (!_authService.HasPermission("ManageTerm"))
                {
                    return PartialView("_AccessDenied");
                }
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.ToDictionary(kvp => kvp.Key, kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray());
                    return Json(new { validationErrors = errors });
                }
                var result = await _termService.CreateTerm(createTermDTO);
                return _returnHelper.ReturnNewResult(result.Flag, result.Message);
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditTerm(Guid id)
        {
            try
            {
                if (!_authService.HasPermission("ManageTerm"))
                {
                    return PartialView("_AccessDenied");
                }
                var termData = await _termService.GetTerm(id);

                var termUpdateDTO = new UpdateTermDTO
                {
                    TermId = termData.TermId,
                    TermName = termData.TermName,
                    TermCode = termData.TermCode,
                    StartDate = termData.StartDate,
                    EndDate = termData.EndDate,
                    RegistrationStart = termData.RegistrationStart,
                    RegistrationEnd = termData.RegistrationEnd,
                    Status = termData.Status,
                    Iscurrent = termData.Iscurrent,
                    AcademicYear = termData.AcademicYear,
                    ModifiedBy = termData.ModifiedBy
                };

                ViewBag.TermId = id;
                return PartialView("_editTerm", termUpdateDTO);
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error retrieving Term: {ex.Message}");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditTerm(UpdateTermDTO model)
        {
            try
            {
                if (!_authService.HasPermission("ManageTerm"))
                {
                    return PartialView("_AccessDenied");
                }
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.ToDictionary(kvp => kvp.Key, kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray());
                    return Json(new { validationErrors = errors });
                }
                var result = await _termService.UpdateTerm(model);
                return _returnHelper.ReturnNewResult(result.Flag, result.Message ?? "Term updated successfully");
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error updating Term: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> DeleteTerm(Guid id)
        {
            try
            {
                if (!_authService.HasPermission("DeleteTerm"))
                {
                    return PartialView("_AccessDenied");
                }
                var term = await _termService.GetTerm(id);
                if (term == null)
                {
                    return _returnHelper.ReturnNewResult(false, "Term not found.");
                }

                var termToDelete = new UpdateTermDTO
                {
                    TermId = term.TermId,
                    TermName = term.TermName,
                    TermCode = term.TermCode,
                    AcademicYear = term.AcademicYear
                };

                return PartialView("_deleteTerm", termToDelete);
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error loading delete form: {ex.Message}");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteTermConfirmed(UpdateTermDTO model)
        {
            try
            {
                if (!_authService.HasPermission("DeleteTerm"))
                {
                    return PartialView("_AccessDenied");
                }
                var result = await _termService.DeleteTerm(model.TermId);
                return _returnHelper.ReturnNewResult(result.Flag, result.Message ?? "Term deleted successfully");
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error deleting term: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> SetCurrentTerm(Guid id)
        {
            try
            {
                if (!_authService.HasPermission("ManageTerm"))
                {
                    return PartialView("_AccessDenied");
                }
                var term = await _termService.GetTerm(id);
                if (term == null)
                {
                    return _returnHelper.ReturnNewResult(false, "Term not found.");
                }

                var termToSetCurrent = new UpdateTermDTO
                {
                    TermId = term.TermId,
                    TermName = term.TermName,
                    TermCode = term.TermCode,
                    AcademicYear = term.AcademicYear
                };

                return PartialView("_setCurrentTerm", termToSetCurrent);
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error loading set current form: {ex.Message}");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetCurrentTermConfirmed(UpdateTermDTO model)
        {
            try
            {
                if (!_authService.HasPermission("ManageTerm"))
                {
                    return PartialView("_AccessDenied");
                }
                var result = await _termService.SetCurrentTerm(model.TermId);
                return _returnHelper.ReturnNewResult(result.Flag, result.Message ?? "Current term set successfully");
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error setting current term: {ex.Message}");
            }
        }
    }
}
