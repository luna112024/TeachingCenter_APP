using Microsoft.AspNetCore.Mvc;
using hongWenAPP.Models;
using hongWenAPP.Models.IdentityModel.DTOs;
using hongWenAPP.Helpers;
using hongWenAPP.Services;
using hongWenAPP.Models.RolesModel.DTOs;
using hongWenAPP.Models.PermissionModel.DTOs;
using hongWenAPP.Models.CompanyModel.DTOs;
using System.Data;

namespace hongWenAPP.Controllers
{
    public class IdentityController : Controller
    {
        private readonly IIdentityService _identityService;
        private readonly ReturnHelper _returnHelper;
        private readonly IRoleService _roleService;
        private readonly IPermissionService _permissionService;
        private readonly AuthenticationService _authService;
        private readonly ICompanyService _companyService;
        private readonly ILogger<IdentityController> _logger;

        public IdentityController(IIdentityService identityService, ReturnHelper returnHelper, IRoleService roleService, IPermissionService permissionService, AuthenticationService authService, ICompanyService companyService, ILogger<IdentityController> logger)
        {
            _identityService = identityService;
            _returnHelper = returnHelper;
            _roleService = roleService;
            _permissionService = permissionService;
            _authService = authService;
            _companyService = companyService;
            _logger = logger;
        }
        [HttpGet]
        public async Task<IActionResult> Index(ViewModelUserRole viewModel)
        {
            if (!_authService.HasPermission("ViewUsers"))
            {
                return View("AccessDenied");
            }
            var users = await _identityService.GetUsers();
            var roles = await _roleService.GetRoles();

            viewModel = new ViewModelUserRole
            {
                Users = PageList<GetUsersDTOs>.Create(users, viewModel.Page, viewModel.PageSize, "ListUser"),
                Roles = PageList<GetRoleDTO>.Create(roles, viewModel.Page, viewModel.PageSize, "ListRoles"),
            };
            return View(viewModel);
        }
        [HttpGet]
        public async Task<IActionResult> ListUser(ListGetUsersDTOs model)
        {
            if (!_authService.HasPermission("ViewUsers"))
            {
                return View("AccessDenied");
            }
            var users = await _identityService.GetUsers(model.SearchText);
            var list = PageList<GetUsersDTOs>.Create(users, model.Page, model.PageSize, "ListUser");
            return PartialView("_ListUser", list);

        }

        [HttpGet]
        public IActionResult AddUser()
        {
            if (!_authService.HasPermission("ManageUsers"))
            {
                return PartialView("_AccessDenied");
            }
            return PartialView("_addUser");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddUser(RegisterDTO registerDTO)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.ToDictionary(kvp => kvp.Key, kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray());
                    return Json(new { validationErrors = errors });
                }
                var result = await _identityService.RegisterUser(registerDTO);
                return _returnHelper.ReturnNewResult(result.Flag, result.Message ?? "One or more validation errors occurred.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user with email: {Email}", registerDTO.Email);
                return _returnHelper.ReturnNewResult(false, "An error occurred while creating the user. Please try again.");
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditUser(Guid id)
        {
            try
            {
                if (!_authService.HasPermission("ManageUsers"))
                {
                    return PartialView("_AccessDenied");
                }
                var userData = await _identityService.GetUserById(id);

                var userUpdateDTO = new UserUpdateDTO
                {
                    UserId = userData.UserId,
                    Email = userData.Email,
                    UserName = userData.Username,
                    Status = userData.Status,
                    FirstName= userData.FirstName,
                    LastName=userData.LastName,
                    KhmerName=userData.KhmerName,
                    ChineseName=userData.ChineseName,
                    Gender=userData.Gender,
                    Phone=userData.Phone,
                    Address=userData.Address,
                    DateofBirth=userData.DateofBirth
                };

                ViewBag.UserId = id;
                return PartialView("_editUser", userUpdateDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user with ID: {UserId}", id);
                return _returnHelper.ReturnNewResult(false, "An error occurred while retrieving user information. Please try again.");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(UserUpdateDTO model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.ToDictionary(kvp => kvp.Key, kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray());
                    return Json(new { validationErrors = errors });
                }

                var result = await _identityService.UpdateProfileByAdmin(model);
                return _returnHelper.ReturnNewResult(result.Flag, result.Message ?? "One or more validation errors occurred.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user with ID: {UserId}", model.UserId);
                return _returnHelper.ReturnNewResult(false, "An error occurred while updating user information. Please try again.");
            }
        }

        [HttpGet]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            try
            {
                if (!_authService.HasPermission("DeleteUsers"))
                {
                    return PartialView("_AccessDenied");
                }
                var userData = await _identityService.GetUserById(id);
                if (userData == null)
                {
                    return _returnHelper.ReturnNewResult(false, "User not found.");
                }

                var userDelete = new UserUpdateDTO
                {
                    UserId = id,
                    UserName = userData.Username,
                    Email = userData.Email,
                    Status = userData.Status,
                };

                return PartialView("_deleteUser", userDelete);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading delete form for user with ID: {UserId}", id);
                return _returnHelper.ReturnNewResult(false, "An error occurred while loading the delete form. Please try again.");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUserConfirmed(UserUpdateDTO user)
        {
            try
            {
                var result = await _identityService.DeleteUser(user.UserId);
                return _returnHelper.ReturnNewResult(result.Flag, result.Message ?? "User account deactivated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deactivating user with ID: {UserId}", user.UserId);
                return _returnHelper.ReturnNewResult(false, "An error occurred while deactivating the user. Please try again.");
            }
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            //if (!_authService.HasPermission("ManageUsers"))
            //{
            //    return PartialView("_AccessDenied");
            //}
            return PartialView("_changePassword");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordDTO changePasswordDTO)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.ToDictionary(kvp => kvp.Key, kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray());
                    return Json(new { validationErrors = errors });
                }

                var result = await _identityService.ChangePassword(changePasswordDTO);
                return _returnHelper.ReturnNewResult(result.Flag, result.Message ?? "Password changed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing password for user");
                return _returnHelper.ReturnNewResult(false, "An error occurred while changing the password. Please try again.");
            }
        }

        public async Task<IActionResult> ListRoles(ListRoles model)
        {
            if (!_authService.HasPermission("ViewRoles"))
            {
                return View("AccessDenied");
            }
            var roles = await _roleService.GetRoles(model.SearchText);
            var list = PageList<GetRoleDTO>.Create(roles, model.Page, model.PageSize, "ListRoles");
            return PartialView("_ListRoles", list);
        }

        [HttpGet]
        public async Task<IActionResult> ManageRole(Guid? roleId)
        {
            try
            {
                if (!_authService.HasPermission("ManageUserRoles"))
                {
                    return PartialView("_AccessDenied");
                }
                var roles = await _roleService.GetRoles();
                var allPermissions = await _permissionService.GetAllPermissions();

                var model = new ManageRolePermissionDTO();

                if (roleId.HasValue && roleId.Value != Guid.Empty)
                {
                    var selectedRole = roles.FirstOrDefault(r => r.RoleId == roleId.Value);
                    if (selectedRole != null)
                    {
                        model.RoleId = selectedRole.RoleId;
                        model.RoleName = selectedRole.RoleName;
                        model.AssignedPermissions = await _permissionService.GetPermissionsByRole(roleId.Value);
                        model.AvailablePermissions = allPermissions
                            .Where(p => !model.AssignedPermissions.Any(ap => ap.PermissionId == p.PermissionId))
                            .ToList();
                    }
                }
                else
                {
                    model.AvailablePermissions = allPermissions;
                }

                ViewBag.Roles = roles;
                return PartialView("_manageRole", model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading role permissions for role ID: {RoleId}", roleId);
                return _returnHelper.ReturnNewResult(false, "An error occurred while loading role permissions. Please try again.");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignPermissions(RolePermissionAssignmentDTO model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.ToDictionary(kvp => kvp.Key, kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray());
                    return Json(new { validationErrors = errors });
                }

                // Get current permissions for the role
                var currentPermissions = await _permissionService.GetPermissionsByRole(model.RoleId);
                var currentPermissionIds = currentPermissions.Select(p => p.PermissionId).ToList();

                // Determine permissions to add and remove
                var permissionsToAdd = model.PermissionIds.Except(currentPermissionIds).ToList();
                var permissionsToRemove = currentPermissionIds.Except(model.PermissionIds).ToList();

                // Check if no changes needed
                if (!permissionsToAdd.Any() && !permissionsToRemove.Any())
                {
                    return _returnHelper.ReturnNewResult(true, "No changes detected. Permissions are already up to date.");
                }

                var successCount = 0;
                var totalOperations = permissionsToAdd.Count + permissionsToRemove.Count;
                var messages = new List<string>();

                // Add new permissions
                foreach (var permissionId in permissionsToAdd)
                {
                    var result = await _permissionService.AssignPermissionToRole(model.RoleId, permissionId);
                    if (result.Flag)
                    {
                        successCount++;
                    }
                    else
                    {
                        return _returnHelper.ReturnNewResult(false, $"Failed to assign permission: {result.Message}");
                    }
                }

                // Remove unselected permissions
                foreach (var permissionId in permissionsToRemove)
                {
                    var result = await _permissionService.RemovePermissionFromRole(model.RoleId, permissionId);
                    if (result.Flag)
                    {
                        successCount++;
                    }
                    else
                    {
                        return _returnHelper.ReturnNewResult(false, $"Failed to remove permission: {result.Message}");
                    }
                }

                // Generate smart success message
                var message = GenerateSmartMessage(permissionsToAdd.Count, permissionsToRemove.Count, successCount);

                return _returnHelper.ReturnNewResult(true, message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating role permissions for role ID: {RoleId}", model.RoleId);
                return _returnHelper.ReturnNewResult(false, "An error occurred while updating role permissions. Please try again.");
            }
        }

        private string GenerateSmartMessage(int addedCount, int removedCount, int successCount)
        {
            var messages = new List<string>();

            if (addedCount > 0)
            {
                messages.Add($"{addedCount} permission{(addedCount > 1 ? "s" : "")} assigned");
            }

            if (removedCount > 0)
            {
                messages.Add($"{removedCount} permission{(removedCount > 1 ? "s" : "")} removed");
            }

            var result = string.Join(" and ", messages);

            // Add completion confirmation
            return $"✓ {result} successfully.";
        }

        [HttpGet]
        public async Task<IActionResult> GetRolePermissions(Guid roleId)
        {
            try
            {
                var allPermissions = await _permissionService.GetAllPermissions();
                var assignedPermissions = await _permissionService.GetPermissionsByRole(roleId);

                var availablePermissions = allPermissions
                    .Where(p => !assignedPermissions.Any(ap => ap.PermissionId == p.PermissionId))
                    .ToList();

                return Json(new
                {
                    success = true,
                    availablePermissions = availablePermissions,
                    assignedPermissions = assignedPermissions
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving role permissions for role ID: {RoleId}", roleId);
                return Json(new { success = false, message = "An error occurred while retrieving role permissions." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> AssignRole(Guid id)
        {
            try
            {
                if (!_authService.HasPermission("ManageUserRoles"))
                {
                    return PartialView("_AccessDenied");
                }
                var user = await _identityService.GetUserById(id);
                var allRoles = await _roleService.GetRoles();

                // Check if user has Teacher profile
                var hasTeacherProfile = await _roleService.HasTeacherProfile(id);

                // Map user role names to role objects
                var userRoles = allRoles.Where(r => user.Roles.Contains(r.RoleName)).ToList();

                var model = new ManageUserRoleDTO
                {
                    UserId = user.UserId,
                    UserName = user.Username,
                    Email = user.Email,
                    AssignedRoles = userRoles,
                    AvailableRoles = allRoles
                        .Where(r => !userRoles.Any(ur => ur.RoleId == r.RoleId))
                        .ToList()
                };

                ViewBag.AllRoles = allRoles;
                ViewBag.HasTeacherProfile = hasTeacherProfile;
                ViewBag.TeacherRole = allRoles.FirstOrDefault(r => r.RoleName == "Teacher");
                return PartialView("_assignRole", model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading user roles for user ID: {UserId}", id);
                return _returnHelper.ReturnNewResult(false, "An error occurred while loading user roles. Please try again.");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignUserRoles(UserRoleAssignmentDTO model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.ToDictionary(kvp => kvp.Key, kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray());
                    return Json(new { validationErrors = errors });
                }

                // Validate Teacher role assignment for users with Teacher profiles
                var validationResult = await _roleService.ValidateTeacherRoleAssignment(model.UserId, model.RoleIds);
                if (!validationResult.Flag)
                {
                    return _returnHelper.ReturnNewResult(false, validationResult.Message);
                }

                // Get current roles for the user
                var user = await _identityService.GetUserById(model.UserId);
                var allRoles = await _roleService.GetRoles();
                var currentRoles = allRoles.Where(r => user.Roles.Contains(r.RoleName)).ToList();
                var currentRoleIds = currentRoles.Select(r => r.RoleId).ToList();

                // Determine roles to add and remove
                var rolesToAdd = model.RoleIds.Except(currentRoleIds).ToList();
                var rolesToRemove = currentRoleIds.Except(model.RoleIds).ToList();

                // Check if no changes needed
                if (!rolesToAdd.Any() && !rolesToRemove.Any())
                {
                    return _returnHelper.ReturnNewResult(true, "No changes detected. User roles are already up to date.");
                }

                var successCount = 0;
                var totalOperations = rolesToAdd.Count + rolesToRemove.Count;

                // Add new roles
                foreach (var roleId in rolesToAdd)
                {
                    var result = await _roleService.AssignRoleToUser(model.UserId, roleId);
                    if (result.Flag)
                    {
                        successCount++;
                    }
                    else
                    {
                        return _returnHelper.ReturnNewResult(false, $"Failed to assign role: {result.Message}");
                    }
                }

                // Remove unselected roles (with Teacher role protection)
                foreach (var roleId in rolesToRemove)
                {
                    // Check if this is a Teacher role removal attempt
                    var roleToRemove = allRoles.FirstOrDefault(r => r.RoleId == roleId);
                    if (roleToRemove?.RoleName == "Teacher")
                    {
                        // Skip Teacher role removal - it's protected
                        _logger.LogWarning("Attempted to remove Teacher role from user {UserId} with Teacher profile - blocked", model.UserId);
                        continue;
                    }

                    var result = await _roleService.RemoveRoleFromUser(model.UserId, roleId);
                    if (result.Flag)
                    {
                        successCount++;
                    }
                    else
                    {
                        return _returnHelper.ReturnNewResult(false, $"Failed to remove role: {result.Message}");
                    }
                }

                // Generate smart success message
                var message = GenerateSmartRoleMessage(rolesToAdd.Count, rolesToRemove.Count, successCount);

                return _returnHelper.ReturnNewResult(true, message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user roles for user ID: {UserId}", model.UserId);
                return _returnHelper.ReturnNewResult(false, "An error occurred while updating user roles. Please try again.");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetUserRoles(Guid userId)
        {
            try
            {
                var user = await _identityService.GetUserById(userId);
                var allRoles = await _roleService.GetRoles();
                var assignedRoles = allRoles.Where(r => user.Roles.Contains(r.RoleName)).ToList();

                var availableRoles = allRoles
                    .Where(r => !assignedRoles.Any(ar => ar.RoleId == r.RoleId))
                    .ToList();

                return Json(new
                {
                    success = true,
                    availableRoles = availableRoles,
                    assignedRoles = assignedRoles
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user roles for user ID: {UserId}", userId);
                return Json(new { success = false, message = "An error occurred while retrieving user roles." });
            }
        }

        private string GenerateSmartRoleMessage(int addedCount, int removedCount, int successCount)
        {
            var messages = new List<string>();

            if (addedCount > 0)
            {
                messages.Add($"{addedCount} role{(addedCount > 1 ? "s" : "")} assigned");
            }

            if (removedCount > 0)
            {
                messages.Add($"{removedCount} role{(removedCount > 1 ? "s" : "")} removed");
            }

            var result = string.Join(" and ", messages);

            // Add completion confirmation
            return $"✓ {result} successfully.";
        }
        [HttpGet]
        public async Task<IActionResult> Permission(ListPermissions model)
        {
            if (!_authService.HasPermission("ViewPermissions"))
            {
                return View("AccessDenied");
            }
            var permission = await _permissionService.GetAllPermissions();
            var list = new ListPermissions
            {
                permissions = PageList<GetPermissionDTO>.Create(permission, model.Page, model.PageSize, "ListPermission"),
            };
            return View(list);
        }
        [HttpGet]
        public async Task<IActionResult> ListPermission(ListPermissions model)
        {
            if (!_authService.HasPermission("ViewPermissions"))
            {
                return View("AccessDenied");
            }
            var permission = await _permissionService.GetAllPermissions(model.SearchText);
            var list = PageList<GetPermissionDTO>.Create(permission, model.Page, model.PageSize, "ListPermission");
            return PartialView("_ListPermission", list);
        }

        [HttpGet]
        public IActionResult AddPermission()
        {
            if (!_authService.HasPermission("ManagePermissions"))
            {
                return PartialView("_AccessDenied");
            }
            return PartialView("_addPermission");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePermission(CreatePermissionDTO createPermissionDTO)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.ToDictionary(kvp => kvp.Key, kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray());
                    return Json(new { validationErrors = errors });
                }
                var result = await _permissionService.CreatePermission(createPermissionDTO);
                return _returnHelper.ReturnNewResult(result.Flag, result.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating permission: {PermissionName}", createPermissionDTO.PermissionName);
                return _returnHelper.ReturnNewResult(false, "An error occurred while creating the permission. Please try again.");
            }
        }
        [HttpGet]
        public async Task<IActionResult> EditPermission(Guid id)
        {
            try
            {
                if (!_authService.HasPermission("ManagePermissions"))
                {
                    return PartialView("_AccessDenied");
                }

                var permissionData = await _permissionService.GetPermissionbyId(id);

                var permissionUpdateDTO = new UpdatePermissionDTO
                {
                    PermissionId = permissionData.PermissionId,
                    PermissionCode=permissionData.PermissionCode,
                    PermissionName = permissionData.PermissionName,
                    Description = permissionData.Description,
                    Module=permissionData.Module,
                    ModifyBy = permissionData.ModifyBy,
                };

                ViewBag.PermissionId = id;
                return PartialView("_editPermission", permissionUpdateDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving permission with ID: {PermissionId}", id);
                return _returnHelper.ReturnNewResult(false, "An error occurred while retrieving permission information. Please try again.");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPermission(UpdatePermissionDTO model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.ToDictionary(kvp => kvp.Key, kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray());
                    return Json(new { validationErrors = errors });
                }
                var result = await _permissionService.UpdatePermission(model);
                return _returnHelper.ReturnNewResult(result.Flag, result.Message ?? "Permission updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating permission with ID: {PermissionId}", model.PermissionId);
                return _returnHelper.ReturnNewResult(false, "An error occurred while updating the permission. Please try again.");
            }
        }
        [HttpGet]
        public async Task<IActionResult> DeletePermission(Guid id)
        {
            try
            {
                if (!_authService.HasPermission("DeletePermissions"))
                {
                    return PartialView("_AccessDenied");
                }
                var permissionData = await _permissionService.GetPermissionbyId(id);
                if (permissionData == null)
                {
                    return _returnHelper.ReturnNewResult(false, "Company not found.");
                }

                var permissionDelete = new UpdatePermissionDTO
                {
                    PermissionId = id,
                    PermissionName = permissionData.PermissionName,
                    Description = permissionData.Description,
                };

                return PartialView("_deletePermission", permissionDelete);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading delete form for permission with ID: {PermissionId}", id);
                return _returnHelper.ReturnNewResult(false, "An error occurred while loading the delete form. Please try again.");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePermissionConfirmed(UpdatePermissionDTO permission)
        {
            try
            {
                var result = await _permissionService.DelectPermission(permission.PermissionId);
                return _returnHelper.ReturnNewResult(result.Flag, result.Message ?? "Permission deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting permission with ID: {PermissionId}", permission.PermissionId);
                return _returnHelper.ReturnNewResult(false, "An error occurred while deleting the permission. Please try again.");
            }
        }


        // Role CRUD Operations
        [HttpGet]
        public IActionResult AddRole()
        {
            if (!_authService.HasPermission("ManageRoles"))
            {
                return PartialView("_AccessDenied");
            }
            return PartialView("_addRole");
        }

        [HttpGet]
        public async Task<IActionResult> Role(ListRoles model)
        {
            if (!_authService.HasPermission("ManageUserRoles"))
            {
                return View("AccessDenied");
            }
            var roles = await _roleService.GetRoles();
            var list = new ListRoles
            {
                roles = PageList<GetRoleDTO>.Create(roles, model.Page, model.PageSize, "ListRole"),
            };
            return View(list);
        }

        [HttpGet]
        public async Task<IActionResult> ListRole(ListRoles model)
        {
            if (!_authService.HasPermission("ManageUserRoles"))
            {
                return View("AccessDenied");
            }
            var roles = await _roleService.GetRoles(model.SearchText);
            var list = PageList<GetRoleDTO>.Create(roles, model.Page, model.PageSize, "ListRole");
            return PartialView("_ListRole", list);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRole(CreateRoleDTO createRoleDTO)
        {
            try
            {

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.ToDictionary(kvp => kvp.Key, kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray());
                    return Json(new { validationErrors = errors });
                }
                var result = await _roleService.CreateRole(createRoleDTO);
                return _returnHelper.ReturnNewResult(result.Flag, result.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating role: {RoleName}", createRoleDTO.RoleName);
                return _returnHelper.ReturnNewResult(false, "An error occurred while creating the role. Please try again.");
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditRole(Guid id)
        {
            try
            {
                if (!_authService.HasPermission("ManageUserRoles"))
                {
                    return PartialView("_AccessDenied");
                }
                var roleData = await _roleService.GetRoleById(id);

                var roleUpdateDTO = new UpdateRoleDTO
                {
                    RoleId = roleData.RoleId,
                    RoleName = roleData.RoleName,
                    Description = roleData.Description,
                    ModifyBy = roleData.ModifyBy,
                };

                ViewBag.RoleId = id;
                return PartialView("_editRole", roleUpdateDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving role with ID: {RoleId}", id);
                return _returnHelper.ReturnNewResult(false, "An error occurred while retrieving role information. Please try again.");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRole(UpdateRoleDTO model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.ToDictionary(kvp => kvp.Key, kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray());
                    return Json(new { validationErrors = errors });
                }
                var result = await _roleService.UpdateRole(model);
                return _returnHelper.ReturnNewResult(result.Flag, result.Message ?? "Role updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating role with ID: {RoleId}", model.RoleId);
                return _returnHelper.ReturnNewResult(false, "An error occurred while updating the role. Please try again.");
            }
        }

        [HttpGet]
        public async Task<IActionResult> DeleteRole(Guid id)
        {
            try
            {
                if (!_authService.HasPermission("DeleteRoles"))
                {
                    return PartialView("_AccessDenied");
                }
                var roleData = await _roleService.GetRoleById(id);
                if (roleData == null)
                {
                    return _returnHelper.ReturnNewResult(false, "Role not found.");
                }

                var roleDelete = new UpdateRoleDTO
                {
                    RoleId = id,
                    RoleName = roleData.RoleName,
                    Description = roleData.Description,
                };

                return PartialView("_deleteRole", roleDelete);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading delete form for role with ID: {RoleId}", id);
                return _returnHelper.ReturnNewResult(false, "An error occurred while loading the delete form. Please try again.");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteRoleConfirmed(UpdateRoleDTO role)
        {
            try
            {
                var result = await _roleService.DeleteRole(role.RoleId);
                return _returnHelper.ReturnNewResult(result.Flag, result.Message ?? "Role deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting role with ID: {RoleId}", role.RoleId);
                return _returnHelper.ReturnNewResult(false, "An error occurred while deleting the role. Please try again.");
            }
        }


        // Admin Reset Force Password Operations
        [HttpGet]
        public async Task<IActionResult> ResetForcePassword(Guid id)
        {
            try
            {
                if (!_authService.HasPermission("ManageUsers"))
                {
                    return PartialView("_AccessDenied");
                }
                var userData = await _identityService.GetUserById(id);
                if (userData == null)
                {
                    return _returnHelper.ReturnNewResult(false, "User not found.");
                }

                var adminResetDTO = new AdminResetPasswordDTO
                {
                    UserId = id
                };

                ViewBag.UserId = id;
                ViewBag.Username = userData.Username;
                ViewBag.Email = userData.Email;
                return PartialView("_resetForcePassword", adminResetDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading reset password form for user with ID: {UserId}", id);
                return _returnHelper.ReturnNewResult(false, "An error occurred while loading the reset password form. Please try again.");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetForcePassword(AdminResetPasswordDTO model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.ToDictionary(kvp => kvp.Key, kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray());
                    return Json(new { validationErrors = errors });
                }

                var result = await _identityService.AdminResetPassword(model);
                return _returnHelper.ReturnNewResult(result.Flag, result.Message ?? "Password reset successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resetting password for user with ID: {UserId}", model.UserId);
                return _returnHelper.ReturnNewResult(false, "An error occurred while resetting the password. Please try again.");
            }
        }

        // Add AssignCompany action
        [HttpGet]
        public async Task<IActionResult> AssignCompany(Guid id)
        {
            try
            {
                var user = await _identityService.GetUserById(id);
                var allCompanies = await _companyService.GetActiveCompanies();

                var userCompanies = allCompanies
                    .Where(c => c.CompanyId == user.CompanyId)
                    .ToList();
                var availableCompanies = allCompanies
                    .Where(c => c.CompanyId != user.CompanyId)
                    .ToList();
                var model = new ManageUserCompanyDTO
                {
                    UserId = user.UserId,
                    UserName = user.Username,
                    Email = user.Email,
                    AssignedCompanies = userCompanies,
                    AvailableCompanies = availableCompanies
                };
                ViewBag.AllCompanies = allCompanies;
                return PartialView("_assignCompany", model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading user companies for user with ID: {UserId}", id);
                return _returnHelper.ReturnNewResult(false, "An error occurred while loading user companies. Please try again.");
            }
        }

        // Add AssignUserCompanies action
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignUserCompanies(UserCompanyAssignmentDTO model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.ToDictionary(kvp => kvp.Key, kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray());
                    return Json(new { validationErrors = errors });
                }

                // Get current companies for the user
                var user = await _identityService.GetUserById(model.UserId);
                var allCompanies = await _companyService.GetCompanies();
                var currentCompanies = allCompanies.Where(c => user.Companies.Contains(c.CompanyName)).ToList();
                var currentCompanyIds = currentCompanies.Select(c => c.CompanyId).ToList();

                // Determine companies to add and remove
                var companiesToAdd = model.CompanyIds.Except(currentCompanyIds).ToList();
                var companiesToRemove = currentCompanyIds.Except(model.CompanyIds).ToList();

                // Check if no changes needed
                if (!companiesToAdd.Any() && !companiesToRemove.Any())
                {
                    return _returnHelper.ReturnNewResult(true, "No changes detected. User companies are already up to date.");
                }

                var successCount = 0;
                var totalOperations = companiesToAdd.Count + companiesToRemove.Count;

                // Add new companies
                foreach (var companyId in companiesToAdd)
                {
                    var result = await _companyService.AssignCompanyToUser(model.UserId, companyId);
                    if (result.Flag)
                    {
                        successCount++;
                    }
                    else
                    {
                        return _returnHelper.ReturnNewResult(false, $"Failed to assign company: {result.Message}");
                    }
                }

                // Remove unselected companies
                foreach (var companyId in companiesToRemove)
                {
                    var result = await _companyService.RemoveCompanyFromUser(model.UserId, companyId);
                    if (result.Flag)
                    {
                        successCount++;
                    }
                    else
                    {
                        return _returnHelper.ReturnNewResult(false, $"Failed to remove company: {result.Message}");
                    }
                }

                // Generate smart success message
                var message = GenerateSmartCompanyMessage(companiesToAdd.Count, companiesToRemove.Count, successCount);

                return _returnHelper.ReturnNewResult(true, message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user companies for user with ID: {UserId}", model.UserId);
                return _returnHelper.ReturnNewResult(false, "An error occurred while updating user companies. Please try again.");
            }
        }

        // Add GetUserCompanies action
        [HttpGet]
        public async Task<IActionResult> GetUserCompanies(Guid userId)
        {
            try
            {
                var user = await _identityService.GetUserById(userId);
                var allCompanies = await _companyService.GetCompanies();
                var assignedCompanies = allCompanies.Where(c => user.Companies.Contains(c.CompanyName)).ToList();

                var availableCompanies = allCompanies
                    .Where(c => !assignedCompanies.Any(ac => ac.CompanyId == c.CompanyId))
                    .ToList();

                return Json(new
                {
                    success = true,
                    availableCompanies = availableCompanies,
                    assignedCompanies = assignedCompanies
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user companies for user with ID: {UserId}", userId);
                return Json(new { success = false, message = "An error occurred while retrieving user companies." });
            }
        }

        // Add GenerateSmartCompanyMessage helper method
        private string GenerateSmartCompanyMessage(int addedCount, int removedCount, int successCount)
        {
            var messages = new List<string>();

            if (addedCount > 0)
            {
                messages.Add($"{addedCount} company{(addedCount > 1 ? "s" : "")} assigned");
            }

            if (removedCount > 0)
            {
                messages.Add($"{removedCount} company{(removedCount > 1 ? "s" : "")} removed");
            }

            var result = string.Join(" and ", messages);

            // Add completion confirmation
            return $"✓ {result} successfully.";
        }
    }
}
