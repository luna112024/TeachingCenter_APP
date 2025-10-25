using hongWenAPP.Helpers;
using hongWenAPP.Models;
using hongWenAPP.Models.SupplyModel.DTOs;
using hongWenAPP.Services;
using Microsoft.AspNetCore.Mvc;

namespace hongWenAPP.Controllers
{
    public class SupplyController : Controller
    {
        private readonly ISupplyService _supplyService;
        private readonly ReturnHelper _returnHelper;
        private readonly AuthenticationService _authService;

        public SupplyController(
            ISupplyService supplyService,
            ReturnHelper returnHelper,
            AuthenticationService authService)
        {
            _supplyService = supplyService;
            _returnHelper = returnHelper;
            _authService = authService;
        }

        // GET: Supply/Index
        [HttpGet]
        public IActionResult Index()
        {
            if (!_authService.HasPermission("ViewSupply"))
            {
                return PartialView("_AccessDenied");
            }

            return View();
        }

        // GET: Supply/ListSupplies
        [HttpGet]
        public async Task<IActionResult> ListSupplies(string? category, string? status, string? search)
        {
            if (!_authService.HasPermission("ViewSupply"))
            {
                return PartialView("_AccessDenied");
            }

            var supplies = await _supplyService.GetAllSupplies(category, status, search);
            return PartialView("_ListSupplies", supplies);
        }

        // GET: Supply/AddSupply
        [HttpGet]
        public IActionResult AddSupply()
        {
            if (!_authService.HasPermission("ManageSupply"))
            {
                return PartialView("_AccessDenied");
            }

            ViewBag.Categories = new List<string> 
            { 
                "Textbook", 
                "Workbook", 
                "Materials", 
                "Equipment", 
                "Stationery", 
                "Other" 
            };

            var model = new CreateSupplyDTO
            {
                Status = "Active"
            };

            return PartialView("_AddSupply", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddSupply(CreateSupplyDTO dto)
        {
            try
            {
                if (!_authService.HasPermission("ManageSupply"))
                {
                    return _returnHelper.ReturnNewResult(false, "You don't have permission to create supplies.");
                }

                if (!ModelState.IsValid)
                {
                    return _returnHelper.ReturnNewResult(false, "Please fill all required fields.");
                }

                dto.CreatedBy = _authService.GetUserInfo()?.Username ?? "Unknown";

                var result = await _supplyService.CreateSupply(dto);

                if (result.Flag)
                {
                    return _returnHelper.ReturnNewResult(true, "Supply created successfully.");
                }

                return _returnHelper.ReturnNewResult(false, result.Message);
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error: {ex.Message}");
            }
        }

        // GET: Supply/EditSupply/{id}
        [HttpGet]
        public async Task<IActionResult> EditSupply(Guid id)
        {
            if (!_authService.HasPermission("ManageSupply"))
            {
                return PartialView("_AccessDenied");
            }

            var supply = await _supplyService.GetSupplyById(id);
            if (supply == null)
            {
                TempData["ErrorMessage"] = "Supply not found";
                return RedirectToAction("Index");
            }

            ViewBag.Categories = new List<string>
            {
                "Textbook",
                "Workbook",
                "Materials",
                "Equipment",
                "Stationery",
                "Other"
            };

            var model = new UpdateSupplyDTO
            {
                SupplyId = supply.SupplyId,
                SupplyCode = supply.SupplyCode,
                SupplyName = supply.SupplyName,
                Description = supply.Description,
                Category = supply.Category,
                UnitPrice = supply.UnitPrice,
                ApplicableLevels = supply.ApplicableLevels,
                Status = supply.Status
            };

            return PartialView("_EditSupply", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditSupply(UpdateSupplyDTO dto)
        {
            try
            {
                if (!_authService.HasPermission("ManageSupply"))
                {
                    return _returnHelper.ReturnNewResult(false, "You don't have permission to edit supplies.");
                }

                if (!ModelState.IsValid)
                {
                    return _returnHelper.ReturnNewResult(false, "Please fill all required fields.");
                }

                dto.ModifiedBy = _authService.GetUserInfo()?.Username ?? "Unknown";

                var result = await _supplyService.UpdateSupply(dto);

                if (result.Flag)
                {
                    return _returnHelper.ReturnNewResult(true, "Supply updated successfully.");
                }

                return _returnHelper.ReturnNewResult(false, result.Message);
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error: {ex.Message}");
            }
        }

        // POST: Supply/DeleteSupply/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSupply(Guid id)
        {
            try
            {
                if (!_authService.HasPermission("DeleteSupply"))
                {
                    return _returnHelper.ReturnNewResult(false, "You don't have permission to delete supplies.");
                }

                var result = await _supplyService.DeleteSupply(id);

                if (result.Flag)
                {
                    return _returnHelper.ReturnNewResult(true, "Supply deleted successfully.");
                }

                return _returnHelper.ReturnNewResult(false, result.Message);
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error: {ex.Message}");
            }
        }

        // API: Get Supplies by Category
        [HttpGet]
        public async Task<IActionResult> GetSuppliesByCategory(string category)
        {
            if (!_authService.HasPermission("ViewSupply"))
            {
                return Json(new { success = false, message = "Access denied" });
            }

            try
            {
                var supplies = await _supplyService.GetSuppliesByCategory(category);
                return Json(new { success = true, data = supplies });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}

