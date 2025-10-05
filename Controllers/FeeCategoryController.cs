using hongWenAPP.Helpers;
using hongWenAPP.Models;
using hongWenAPP.Models.FeeModel.DTOs;
using hongWenAPP.Services;
using Microsoft.AspNetCore.Mvc;

namespace hongWenAPP.Controllers
{
    public class FeeCategoryController : Controller
    {
        private readonly IFeeService _feeService;
        private readonly ReturnHelper _returnHelper;
        private readonly AuthenticationService _authService;

        public FeeCategoryController(IFeeService feeService, ReturnHelper returnHelper, AuthenticationService authService)
        {
            _feeService = feeService;
            _returnHelper = returnHelper;
            _authService = authService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(ListFeeCategoryDTOs model)
        {
            if (!_authService.HasPermission("ViewFee"))
            {
                return PartialView("_AccessDenied");
            }
            var categories = await _feeService.GetAllFeeCategories();
            var list = new ListFeeCategoryDTOs
            {
                feeCategory = PageList<GetFeeCategoryDTO>.Create(categories, model.Page, model.PageSize, "ListFeeCategory")
            };

            return View(list);
        }

        [HttpGet]
        public async Task<IActionResult> ListFeeCategory(ListFeeCategoryDTOs model)
        {
            if (!_authService.HasPermission("ViewFee"))
            {
                return PartialView("_AccessDenied");
            }
            var categories = await _feeService.GetAllFeeCategories();
            var list = PageList<GetFeeCategoryDTO>.Create(categories, model.Page, model.PageSize, "ListFeeCategory");
            return PartialView("_ListFeeCategories", list);
        }

        [HttpGet]
        public async Task<IActionResult> FetchFeeCategory(string i)
        {
            if (!_authService.HasPermission("ViewFee"))
            {
                return PartialView("_AccessDenied");
            }
            var categories = await _feeService.GetAllFeeCategories();
            var result = categories.Select(c => new
            {
                c.CategoryId,
                c.CategoryName,
                c.CategoryCode,
                c.Status,
                c.IsMandatory,
                c.DisplayOrder
            }).ToList();
            return Json(result);
        }

        [HttpGet]
        public async Task<IActionResult> AddFeeCategory()
        {
            if (!_authService.HasPermission("ManageFee"))
            {
                return PartialView("_AccessDenied");
            }
            
            var model = new CreateFeeCategoryDTO
            {
                Status = "Active",
                IsMandatory = true,
                DisplayOrder = 0
            };
            return PartialView("_addFeeCategory", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddFeeCategory(CreateFeeCategoryDTO createFeeCategoryDTO)
        {
            try
            {
                if (!_authService.HasPermission("ManageFee"))
                {
                    return PartialView("_AccessDenied");
                }
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.ToDictionary(kvp => kvp.Key, kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray());
                    return Json(new { validationErrors = errors });
                }  
                var result = await _feeService.CreateFeeCategory(createFeeCategoryDTO);
                return _returnHelper.ReturnNewResult(result.Flag, result.Message);
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditFeeCategory(Guid id)
        {
            try
            {
                if (!_authService.HasPermission("ManageFee"))
                {
                    return PartialView("_AccessDenied");
                }
                
                var categoryData = await _feeService.GetFeeCategory(id);

                var categoryUpdateDTO = new UpdateFeeCategoryDTO
                {
                    CategoryId = categoryData.CategoryId,
                    CategoryName = categoryData.CategoryName,
                    CategoryCode = categoryData.CategoryCode,
                    Description = categoryData.Description,
                    IsMandatory = categoryData.IsMandatory,
                    DisplayOrder = categoryData.DisplayOrder,
                    Status = categoryData.Status,
                    ModifiedBy = categoryData.CreatedBy
                };

                ViewBag.CategoryId = id;
                return PartialView("_editFeeCategory", categoryUpdateDTO);
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error retrieving Fee Category: {ex.Message}");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditFeeCategory(UpdateFeeCategoryDTO model)
        {
            try
            {
                if (!_authService.HasPermission("ManageFee"))
                {
                    return PartialView("_AccessDenied");
                }
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.ToDictionary(kvp => kvp.Key, kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray());
                    return Json(new { validationErrors = errors });
                }

                var result = await _feeService.UpdateFeeCategory(model);
                return _returnHelper.ReturnNewResult(result.Flag, result.Message ?? "Fee Category updated successfully");
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error updating Fee Category: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> DeleteFeeCategory(Guid id)
        {
            try
            {
                if (!_authService.HasPermission("DeleteFee"))
                {
                    return PartialView("_AccessDenied");
                }
                var category = await _feeService.GetFeeCategory(id);
                if (category == null)
                {
                    return _returnHelper.ReturnNewResult(false, "Fee Category not found.");
                }

                return PartialView("_deleteFeeCategory", category);
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error loading delete form: {ex.Message}");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteFeeCategoryConfirmed(Guid CategoryId)
        {
            try
            {
                if (!_authService.HasPermission("DeleteFee"))
                {
                    return PartialView("_AccessDenied");
                }
                var result = await _feeService.DeleteFeeCategory(CategoryId);
                return _returnHelper.ReturnNewResult(result.Flag, result.Message ?? "Fee Category deleted successfully");
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error deleting fee category: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> DetailsFeeCategory(Guid id)
        {
            try
            {
                if (!_authService.HasPermission("ViewFee"))
                {
                    return PartialView("_AccessDenied");
                }
                var category = await _feeService.GetFeeCategory(id);
                if (category == null)
                {
                    return _returnHelper.ReturnNewResult(false, "Fee Category not found.");
                }

                return PartialView("_detailsFeeCategory", category);
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error loading fee category details: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetActiveFeeCategories(string q = "")
        {
            try
            {
                var categories = await _feeService.GetAllFeeCategories();
                var activeCategories = categories.Where(c => c.Status == "Active").ToList();

                if (!string.IsNullOrEmpty(q))
                {
                    activeCategories = activeCategories.Where(c => 
                        c.CategoryName.Contains(q, StringComparison.OrdinalIgnoreCase) ||
                        c.CategoryCode.Contains(q, StringComparison.OrdinalIgnoreCase)
                    ).ToList();
                }

                var result = activeCategories.Select(c => new
                {
                    id = c.CategoryId,
                    text = c.CategoryName,
                    code = c.CategoryCode
                }).ToList();

                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new List<object>());
            }
        }
    }
}
