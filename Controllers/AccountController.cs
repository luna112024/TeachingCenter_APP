using hongWenAPP.Helpers;
using hongWenAPP.Models;
using hongWenAPP.Models.AccountModel.DTOs;
using hongWenAPP.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace hongWenAPP.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly ReturnHelper _returnHelper;
        private readonly ICompanyService _companyService;
        public AccountController(IAccountService accountService, ReturnHelper returnHelper,ICompanyService companyService)
        {
            _accountService = accountService;
            _returnHelper = returnHelper;
            _companyService = companyService;
        }
        [HttpGet]
        public async Task<IActionResult> Index(ListAccountDTOs model)
        {
            var account = await _accountService.GetAccounts();
            var list = new ListAccountDTOs
            {
                account = PageList<GetAccountDTO>.Create(account, model.Page, model.PageSize, "ListAccounts"),
            };

            return View(list);
        }

        [HttpGet]
        public async Task<IActionResult> ListAccounts(ListAccountDTOs model)
        {
            var account = await _accountService.GetAccounts(model.SearchText);
            var list = PageList<GetAccountDTO>.Create(account, model.Page, model.PageSize, "ListAccounts");
            return PartialView("_ListAccounts", list);
        }

        [HttpGet]
        public async Task<IActionResult> AddAccount()
        {
            var company = await _companyService.GetCompanies();
            var viewModel = new CreateAccountDTO
            {
                Companies = company
            };
            return PartialView("_addAccount",viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddAccount(CreateAccountDTO createAccount)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.ToDictionary(kvp => kvp.Key, kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray());
                    return Json(new { validationErrors = errors });
                }
                var result = await _accountService.CreateAccount(createAccount);
                return _returnHelper.ReturnNewResult(result.Flag, result.Message);
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error: {ex.Message}");
            }
        }
        [HttpGet]
        public async Task<IActionResult> EditAccount(Guid id)
        {
            try
            {
                var accountData = await _accountService.GetAccountById(id);
                var companies = await _companyService.GetCompanies();
                var compantUpdateDTO = new UpdateAccountDTO
                {
                    AccID = accountData.AccID,
                    CompanyId=accountData.CompanyId,
                    AccountNumber = accountData.AccountNumber,
                    Currency = accountData.Currency,
                    ModifyBy = accountData.ModifyBy,
                    Companies = companies,
                    IsActive = accountData.IsActive,
                };

                ViewBag.CompanyId = id;
                return PartialView("_editAccount", compantUpdateDTO);
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error retrieving Account: {ex.Message}");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAccount(UpdateAccountDTO updateAccountDTO)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.ToDictionary(kvp => kvp.Key, kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray());
                    return Json(new { validationErrors = errors });
                }
                var result = await _accountService.UpdateAccount(updateAccountDTO);
                return _returnHelper.ReturnNewResult(result.Flag, result.Message ?? "Account updated successfully");
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error updating account: {ex.Message}");
            }
        }
        [HttpGet]
        public async Task<IActionResult> DeleteAccount(Guid id)
        {
            try
            {
                var account = await _accountService.GetAccountById(id);
                if (account == null)
                {
                    return _returnHelper.ReturnNewResult(false, "Account not found.");
                }

                var accountToDelete = new UpdateAccountDTO
                {
                    AccID = account.AccID,
                    AccountNumber = account.AccountNumber,
                    Currency = account.Currency
                };

                return PartialView("_deleteAccount", accountToDelete);
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error loading delete form: {ex.Message}");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAccountConfirmed(UpdateAccountDTO model)
        {
            try
            {
                var result = await _accountService.DeleteAccount(model.AccID);
                return _returnHelper.ReturnNewResult(result.Flag, result.Message ?? "Account deleted successfully");
            }
            catch (Exception ex)
            {
                return _returnHelper.ReturnNewResult(false, $"Error deleting Account: {ex.Message}");
            }
        }
    }
}
