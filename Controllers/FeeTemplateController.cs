using hongWenAPP.Helpers;
using hongWenAPP.Models;
using hongWenAPP.Models.FeeModel.DTOs;
using hongWenAPP.Services;
using Microsoft.AspNetCore.Mvc;

namespace hongWenAPP.Controllers
{
    public class FeeTemplateController : Controller
    {
        private readonly IFeeService _feeService;
        private readonly ReturnHelper _returnHelper;
        private readonly AuthenticationService _authService;

        public FeeTemplateController(IFeeService feeService, ReturnHelper returnHelper, AuthenticationService authService)
        {
            _feeService = feeService;
            _returnHelper = returnHelper;
            _authService = authService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(ListFeeTemplateDTOs model)
        {
            if (!_authService.HasPermission("ViewFee"))
            {
                return PartialView("_AccessDenied");
            }
            var templates = await _feeService.GetAllFeeTemplates();
            var list = new ListFeeTemplateDTOs
            {
                feeTemplate = PageList<GetFeeTemplateDTO>.Create(templates, model.Page, model.PageSize, "ListFeeTemplate")
            };

            return View(list);
        }

        [HttpGet]
        public async Task<IActionResult> ListFeeTemplate(ListFeeTemplateDTOs model)
        {
            if (!_authService.HasPermission("ViewFee"))
            {
                return PartialView("_AccessDenied");
            }
            var templates = await _feeService.GetAllFeeTemplates();
            var list = PageList<GetFeeTemplateDTO>.Create(templates, model.Page, model.PageSize, "ListFeeTemplate");
            return PartialView("_ListFeeTemplates", list);
        }

        [HttpGet]
        public async Task<IActionResult> FetchFeeTemplate(string q = "")
        {
            try
            {
                if (!_authService.HasPermission("ViewFee"))
                {
                    Console.WriteLine("FetchFeeTemplate: User does not have ViewFee permission");
                    return Json(new List<object>());
                }
                
                Console.WriteLine($"FetchFeeTemplate: Fetching templates with search parameter: '{q}'");
                var templates = await _feeService.GetAllFeeTemplates();
                Console.WriteLine($"FetchFeeTemplate: Retrieved {templates.Count} templates");
                
                var result = templates.Select(t => new
                {
                    id = t.TemplateId,
                    templateId = t.TemplateId,
                    templateName = t.TemplateName,
                    text = t.TemplateName,
                    baseAmount = t.BaseAmount,
                    feeType = t.FeeType,
                    currency = t.Currency,
                    status = t.Status
                }).ToList();

                // Filter by search query if provided
                if (!string.IsNullOrEmpty(q))
                {
                    result = result.Where(t => 
                        t.templateName.Contains(q, StringComparison.OrdinalIgnoreCase) ||
                        t.feeType.Contains(q, StringComparison.OrdinalIgnoreCase)
                    ).ToList();
                }
                
                Console.WriteLine($"FetchFeeTemplate: Returning {result.Count} formatted templates");
                return Json(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"FetchFeeTemplate Error: {ex.Message}");
                return Json(new List<object>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> AddFeeTemplate()
        {
            if (!_authService.HasPermission("ManageFee"))
            {
                return PartialView("_AccessDenied");
            }
            
            // Fee categories are now loaded via AJAX endpoint
            
            var model = new CreateFeeTemplateDTO
            {
                Status = "Active",
                Currency = "USD",
                ApplicableTo = "All",
                EffectiveDate = DateTime.Today,
                DueDaysAfterEnrollment = 7,
                LateFeeDays = 7
            };
            return PartialView("_addFeeTemplate", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddFeeTemplate(CreateFeeTemplateDTO createFeeTemplateDTO)
        {
            try
            {
                if (!_authService.HasPermission("ManageFee"))
                {
                    return PartialView("_AccessDenied");
                }
                
                // Debug: Check if CategoryId is valid
                if (createFeeTemplateDTO.CategoryId == Guid.Empty)
                {
                    return _returnHelper.ReturnNewResult(false, "Please select a valid category.");
                }
                
                // Debug: Log the received CategoryId
                Console.WriteLine($"Received CategoryId: {createFeeTemplateDTO.CategoryId}");
                
                // Debug: Verify category exists
                var categories = await _feeService.GetAllFeeCategories();
                Console.WriteLine($"Total categories found: {categories.Count}");
                Console.WriteLine($"Categories: {string.Join(", ", categories.Select(c => $"{c.CategoryName} (ID: {c.CategoryId}, Status: {c.Status}"))}");
                
                var categoryExists = categories.Any(c => c.CategoryId == createFeeTemplateDTO.CategoryId && c.Status == "Active");
                if (!categoryExists)
                {
                    return _returnHelper.ReturnNewResult(false, $"Selected category is not valid or inactive. CategoryId: {createFeeTemplateDTO.CategoryId}, Available categories: {string.Join(", ", categories.Select(c => $"{c.CategoryName}({c.CategoryId})"))}");
                }
                
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.ToDictionary(kvp => kvp.Key, kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray());
                    return Json(new { validationErrors = errors });
                }  
                var result = await _feeService.CreateFeeTemplate(createFeeTemplateDTO);
                return _returnHelper.ReturnNewResult(result.Flag, result.Message);
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditFeeTemplate(Guid id)
        {
            try
            {
                if (!_authService.HasPermission("ManageFee"))
                {
                    return PartialView("_AccessDenied");
                }
                
                // Fee categories are now loaded via AJAX endpoint
                
                var templateData = await _feeService.GetFeeTemplate(id);

                var templateUpdateDTO = new UpdateFeeTemplateDTO
                {
                    TemplateId = templateData.TemplateId,
                    CategoryId = templateData.CategoryId,
                    TemplateName = templateData.TemplateName,
                    FeeType = templateData.FeeType,
                    BaseAmount = templateData.BaseAmount,
                    Currency = templateData.Currency,
                    ApplicableTo = templateData.ApplicableTo,
                    ApplicableLevels = templateData.ApplicableLevels,
                    DueDaysAfterEnrollment = templateData.DueDaysAfterEnrollment,
                    LateFeeDays = templateData.LateFeeDays,
                    LateFeeAmount = templateData.LateFeeAmount,
                    EarlyPaymentDiscountPercent = templateData.EarlyPaymentDiscountPercent,
                    EarlyPaymentDays = templateData.EarlyPaymentDays,
                    SiblingDiscountPercent = templateData.SiblingDiscountPercent,
                    EffectiveDate = templateData.EffectiveDate,
                    ExpiryDate = templateData.ExpiryDate,
                    Status = templateData.Status,
                    ModifiedBy = templateData.CreatedBy
                };

                ViewBag.TemplateId = id;
                return PartialView("_editFeeTemplate", templateUpdateDTO);
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error retrieving Fee Template: {ex.Message}");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditFeeTemplate(UpdateFeeTemplateDTO model)
        {
            try
            {
                if (!_authService.HasPermission("ManageFee"))
                {
                    return PartialView("_AccessDenied");
                }
                
                // Debug: Check if CategoryId is valid
                if (model.CategoryId == Guid.Empty)
                {
                    return _returnHelper.ReturnNewResult(false, "Please select a valid category.");
                }
                
                // Debug: Log the received CategoryId
                Console.WriteLine($"Received CategoryId: {model.CategoryId}");
                
                // Debug: Verify category exists
                var categories = await _feeService.GetAllFeeCategories();
                var categoryExists = categories.Any(c => c.CategoryId == model.CategoryId && c.Status == "Active");
                if (!categoryExists)
                {
                    return _returnHelper.ReturnNewResult(false, "Selected category is not valid or inactive.");
                }
                
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.ToDictionary(kvp => kvp.Key, kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray());
                    return Json(new { validationErrors = errors });
                }

                var result = await _feeService.UpdateFeeTemplate(model);
                return _returnHelper.ReturnNewResult(result.Flag, result.Message ?? "Fee Template updated successfully");
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error updating Fee Template: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> DeleteFeeTemplate(Guid id)
        {
            try
            {
                if (!_authService.HasPermission("DeleteFee"))
                {
                    return PartialView("_AccessDenied");
                }
                var template = await _feeService.GetFeeTemplate(id);
                if (template == null)
                {
                    return _returnHelper.ReturnNewResult(false, "Fee Template not found.");
                }

                return PartialView("_deleteFeeTemplate", template);
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error loading delete form: {ex.Message}");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteFeeTemplateConfirmed(Guid TemplateId)
        {
            try
            {
                if (!_authService.HasPermission("DeleteFee"))
                {
                    return PartialView("_AccessDenied");
                }
                var result = await _feeService.DeleteFeeTemplate(TemplateId);
                return _returnHelper.ReturnNewResult(result.Flag, result.Message ?? "Fee Template deleted successfully");
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error deleting fee template: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> DetailsFeeTemplate(Guid id)
        {
            try
            {
                if (!_authService.HasPermission("ViewFee"))
                {
                    return PartialView("_AccessDenied");
                }
                var template = await _feeService.GetFeeTemplate(id);
                if (template == null)
                {
                    return _returnHelper.ReturnNewResult(false, "Fee Template not found.");
                }

                return PartialView("_detailsFeeTemplate", template);
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error loading fee template details: {ex.Message}");
            }
        }
    }
}
