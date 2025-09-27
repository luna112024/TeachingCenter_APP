using hongWenAPP.Helpers;
using hongWenAPP.Models;
using hongWenAPP.Models.CompanyModel.DTOs;
using hongWenAPP.Services;
using Microsoft.AspNetCore.Mvc;

namespace hongWenAPP.Controllers
{
    public class CompanyController : Controller
    {
        private readonly ICompanyService _companyService;
        private readonly ReturnHelper _returnHelper;
        private readonly AuthenticationService _authService;

        public CompanyController(ICompanyService companyService, ReturnHelper returnHelper, AuthenticationService authService)
        {
            _companyService = companyService;
            _returnHelper = returnHelper;
            _authService = authService;
        }
        [HttpGet]
        public async Task<IActionResult> Index(ListCompanyDTOs model)
        {

            var company = await _companyService.GetCompanies();
            var list = new ListCompanyDTOs
            {
                company = PageList<GetCompanyDTO>.Create(company, model.Page, model.PageSize, "ListCompany")
            };
            
            return View(list);

        }
        [HttpGet]
        public async Task<IActionResult> ListCompany(ListCompanyDTOs model)
        {
            var company = await _companyService.GetCompanies(model.SearchText);
            var list = PageList<GetCompanyDTO>.Create(company, model.Page, model.PageSize, "ListCompany");
            return PartialView("_ListCompanies", list);

        }

        [HttpGet]
        public async Task<IActionResult> FetchCompany(string i)
        {
            var company = await _companyService.GetCompanies(i);
            var result = company.Select(c => new
            {
                c.CompanyId,
                c.CompanyName,
                c.Description,
                c.IsActive
            }).ToList();
            return Json(result);
        }
        [HttpGet]
        public IActionResult AddCompany()
        {
            // Check permission for viewing account statements
            if (!_authService.HasPermission("ManageCompanies"))
            {
                return PartialView("_AccessDenied");
            }
            return PartialView("_addCompany");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCompany(CreateCompanyDTO createCompanyDTOs)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.ToDictionary(kvp => kvp.Key, kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray());
                    return Json(new { validationErrors = errors });
                }
                var result = await _companyService.CreateCompany(createCompanyDTOs);
                return _returnHelper.ReturnNewResult(result.Flag, result.Message);
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error: {ex.Message}");
            }
        }
        [HttpGet]
        public async Task<IActionResult> EditCompany(Guid id)
        {
            try
            {
                var companyData = await _companyService.GetCompanybyId(id);

                var companyUpdateDTO = new UpdateCompanyDTO
                {
                    CompanyId = companyData.CompanyId,
                    CompanyName = companyData.CompanyName, 
                    ModifyBy = companyData.ModifyBy,
                    Description = companyData.Description,
                    IsActive = companyData.IsActive
                };

                ViewBag.CompanyId = id;
                return PartialView("_editCompany", companyUpdateDTO);
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error retrieving Company: {ex.Message}");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCompany(UpdateCompanyDTO model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.ToDictionary(kvp => kvp.Key, kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray());
                    return Json(new { validationErrors = errors });
                }
                var result = await _companyService.UpdateCompanies(model);
                return _returnHelper.ReturnNewResult(result.Flag, result.Message ?? "Company updated successfully");
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error updating Company: {ex.Message}");
            }
        }
        [HttpGet]
        public async Task<IActionResult> DeleteCompany(Guid id)
        {
            try
            {
                var company = await _companyService.GetCompanybyId(id);
                if (company == null)
                {
                    return _returnHelper.ReturnNewResult(false, "Company not found.");
                }

                var companyToDelete = new UpdateCompanyDTO
                {
                    CompanyId = company.CompanyId,
                    CompanyName = company.CompanyName,
                    Description = company.Description
                };

                return PartialView("_deleteCompany", companyToDelete);
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error loading delete form: {ex.Message}");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCompanyConfirmed(UpdateCompanyDTO model)
        {
            try
            {
                var result = await _companyService.DeleteCompany(model.CompanyId);
                return _returnHelper.ReturnNewResult(result.Flag, result.Message ?? "Company deleted successfully");
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error deleting company: {ex.Message}");
            }
        }
    }
}
