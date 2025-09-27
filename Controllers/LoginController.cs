using hongWenAPP.Models;
using hongWenAPP.Models.IdentityModel.DTOs;
using hongWenAPP.Services;
using Microsoft.AspNetCore.Mvc;

namespace hongWenAPP.Controllers
{
    /// <summary>
    /// Login controller - handles authentication-related actions
    /// Note: No authorization attributes needed - handled by CustomAuthenticationMiddleware
    /// </summary>
    public class LoginController : Controller
    {
        private readonly AuthenticationService _authService;
        private readonly IIdentityService _identityService;
        private readonly ILogger<LoginController> _logger;

        public LoginController(AuthenticationService authService, IIdentityService identityService, ILogger<LoginController> logger)
        {
            _authService = authService;
            _identityService = identityService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index(bool expired = false, string message = "")
        {
            // Handle session expired scenario
            if (expired)
            {
                ViewBag.IsSessionExpired = true;
                if (!string.IsNullOrEmpty(message))
                {
                    ViewBag.SessionExpiredMessage = Uri.UnescapeDataString(message);
                }
                else
                {
                    ViewBag.SessionExpiredMessage = "Your session has expired. Please log in again.";
                }
            }

            // If user is already authenticated and session is valid, redirect to home
            if (_authService.IsAuthenticated() && _authService.IsSessionValid())
            {
                _logger.LogInformation("User already authenticated, redirecting to home");
                return RedirectToAction("Index", "Home");
            }
            
            // Clear any invalid session data
            if (_authService.IsAuthenticated() && !_authService.IsSessionValid())
            {
                _authService.ClearUserSession();
                _logger.LogInformation("Invalid session cleared");
            }
            
            return View(new LoginModel());
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Login attempt with invalid model state for user: {Username}", model.Username);
                    return View("Index", model);
                }

                _logger.LogInformation("Login attempt for user: {Username}", model.Username);
                
                var response = await _authService.LoginAsync(model);
                
                if (response != null)
                {
                    // Check if login was successful
                    if (response.IsSuccess && !string.IsNullOrEmpty(response.AccessToken))
                    {
                        // Check if user needs to change password (first-time login)
                        if (response.User?.RequirePasswordChange == true)
                        {
                            _logger.LogInformation("User {Username} requires password change", model.Username);
                            
                            // Store temporary user info for password change
                            TempData["PasswordChangeUsername"] = model.Username;
                            TempData["PasswordChangeCurrentPassword"] = model.Password;
                            TempData["InfoMessage"] = "Welcome! For security reasons, you must set a new password.";
                            
                            return RedirectToAction("InitialPasswordChange");
                        }
                        
                        _logger.LogInformation("Successful login for user: {Username}", model.Username);
                        
                        // Set success message for user feedback
                        TempData["SuccessMessage"] = $"Welcome back, {response.User?.Username ?? model.Username}!";
                        
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        // Handle specific error scenarios
                        if (!string.IsNullOrEmpty(response.ErrorMessage))
                        {
                            // Check if account is locked due to multiple failed attempts
                            if (response.ErrorMessage.Contains("locked") || response.ErrorMessage.Contains("multiple failed"))
                            {
                                _logger.LogWarning("Account locked for user: {Username}", model.Username);
                                TempData["ErrorMessage"] = $"Account Locked: {response.ErrorMessage}";
                                return RedirectToAction("ForgotPassword");
                            }
                            
                            _logger.LogWarning("Login failed for user: {Username}, Reason: {ErrorMessage}", model.Username, response.ErrorMessage);
                            ModelState.AddModelError("", response.ErrorMessage);
                        }
                        else
                        {
                            _logger.LogWarning("Login failed for user: {Username}", model.Username);
                            ModelState.AddModelError("", "Invalid username or password. Please check your credentials and try again.");
                        }
                    }
                }
                else
                {
                    _logger.LogWarning("No response received for login attempt: {Username}", model.Username);
                    ModelState.AddModelError("", "Login service is currently unavailable. Please try again later.");
                }
                
                return View("Index", model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for user: {Username}", model.Username);
                ModelState.AddModelError("", "An error occurred during login. Please try again later.");
                return View("Index", model);
            }
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var currentUser = _authService.GetUserInfo()?.Username;
                await _authService.LogoutAsync();
                
                _logger.LogInformation("User logged out: {Username}", currentUser);
                TempData["InfoMessage"] = "You have been successfully logged out.";
                
                return RedirectToAction("Index", "Login");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout");
                return RedirectToAction("Index", "Login");
            }
        }

        [HttpGet]
        public async Task<IActionResult> SessionExpired()
        {
            try
            {
                // Clear any existing authentication
                await _authService.LogoutAsync();
                
                // Check if there's a session expired message
                var sessionMessage = HttpContext.Session.GetString("SessionExpiredMessage");
                string message = "Your session has expired. Please log in again.";
                
                if (!string.IsNullOrEmpty(sessionMessage))
                {
                    message = sessionMessage;
                    HttpContext.Session.Remove("SessionExpiredMessage");
                }
                
                _logger.LogInformation("Session expired, redirecting to login");
                
                // Redirect to login page with expired flag and message
                return RedirectToAction("Index", new { expired = true, message = Uri.EscapeDataString(message) });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling session expiration");
                return RedirectToAction("Index", new { expired = true });
            }
        }

        [HttpGet]
        public ActionResult ForgotPassword() 
        {
            return View(new ForgotPasswordDTO());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDTO model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                _logger.LogInformation("Forgot password request for email: {Email}", model.Email);
                
                var response = await _authService.ForgotPasswordAsync(model);
                
                if (response != null && response.Flag)
                {
                    TempData["SuccessMessage"] = response.Message ?? "Password reset instructions have been sent to your email.";
                    _logger.LogInformation("Forgot password email sent for: {Email}", model.Email);
                    return View(new ForgotPasswordDTO()); // Clear the form
                }

                // Handle error response
                string errorMessage = response?.Message ?? "An error occurred while processing your request. Please try again.";
                ModelState.AddModelError("", errorMessage);
                _logger.LogWarning("Forgot password failed for email: {Email}, Error: {Error}", model.Email, errorMessage);
                
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in forgot password for email: {Email}", model.Email);
                ModelState.AddModelError("", "An unexpected error occurred. Please try again later.");
                return View(model);
            }
        }

        [HttpGet]
        public ActionResult ResetPassword(string token = "", string email = "")
        {
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(email))
            {
                TempData["ErrorMessage"] = "Invalid or missing reset token. Please request a new password reset.";
                return RedirectToAction("ForgotPassword");
            }

            var model = new ResetPasswordDTO
            {
                Token = token,
                Email = email
            };
            
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordDTO model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                _logger.LogInformation("Password reset attempt for email: {Email}", model.Email);
                
                var response = await _authService.ResetPasswordAsync(model);
                
                if (response != null && response.Flag)
                {
                    TempData["SuccessMessage"] = response.Message ?? "Your password has been successfully reset. You can now log in with your new password.";
                    _logger.LogInformation("Password reset successful for email: {Email}", model.Email);
                    return RedirectToAction("Index", "Login");
                }

                // Handle error response
                string errorMessage = response?.Message ?? "An error occurred while resetting your password. Please try again.";
                ModelState.AddModelError("", errorMessage);
                _logger.LogWarning("Password reset failed for email: {Email}, Error: {Error}", model.Email, errorMessage);
                
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in password reset for email: {Email}", model.Email);
                ModelState.AddModelError("", "An unexpected error occurred. Please try again later.");
                return View(model);
            }
        }

        /// <summary>
        /// API endpoint to check authentication status (used by middleware for debugging)
        /// </summary>
        [HttpGet]
        public IActionResult CheckAuth()
        {
            var isAuth = _authService.IsAuthenticated();
            var userInfo = _authService.GetUserInfo();
            
            return Json(new { 
                isAuthenticated = isAuth,
                hasToken = !string.IsNullOrEmpty(_authService.GetToken()),
                userInfo = userInfo?.Username,
                timestamp = DateTime.UtcNow
            });
        }
        
        /// <summary>
        /// Display initial password change form for first-time login
        /// </summary>
        [HttpGet]
        public IActionResult InitialPasswordChange()
        {
            var username = TempData["PasswordChangeUsername"] as string;
            var currentPassword = TempData["PasswordChangeCurrentPassword"] as string;
            
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(currentPassword))
            {
                TempData["ErrorMessage"] = "Session expired. Please log in again.";
                return RedirectToAction("Index");
            }
            
            // Keep the data for the POST action
            TempData.Keep("PasswordChangeUsername");
            TempData.Keep("PasswordChangeCurrentPassword");
            
            var model = new ChangePasswordDTO
            {
                CurrentPassword = currentPassword
            };
            
            return View(model);
        }

        /// <summary>
        /// Handle initial password change submission
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> InitialPasswordChange(ChangePasswordDTO model)
        {
            try
            {


                var username = TempData["PasswordChangeUsername"] as string;
                var currentPassword = TempData["PasswordChangeCurrentPassword"] as string;
                
                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(currentPassword))
                {
                    TempData["ErrorMessage"] = "Session expired. Please log in again.";
                    return RedirectToAction("Index");
                }
                
                // Set the stored values
                model.CurrentPassword = currentPassword;

                _logger.LogInformation("Initial password change attempt for user: {Username}", username);
                
                var response = await _identityService.InitialPasswordChange(model);
                
                if (response != null && response.Flag)
                {
                    _logger.LogInformation("Successful initial password change for user: {Username}", username);
                    
                    // Clear any existing authentication/session data to force fresh login
                    _authService.ClearUserSession();
                    
                    TempData["SuccessMessage"] = "Password changed successfully! Please log in with your new password.";
                    return RedirectToAction("Index");
                }

                // Password change failed
                string errorMessage = response?.Message ?? "An error occurred while changing your password. Please try again.";
                ModelState.AddModelError("", errorMessage);
                _logger.LogWarning("Initial password change failed for user: {Username}, Error: {Error}", username, errorMessage);
                
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during initial password change for user: {Username}", model.CurrentPassword);
                ModelState.AddModelError("", "An unexpected error occurred. Please try again later.");
                return View(model);
            }
        }



        /// <summary>
        /// Debug endpoint to check session status
        /// </summary>
        [HttpGet]
        public IActionResult SessionDebug()
        {
            var session = HttpContext.Session;
            var cookies = HttpContext.Request.Cookies;
            
            return Json(new {
                sessionId = session.Id,
                sessionIsAvailable = session.IsAvailable,
                sessionKeys = session.Keys.ToList(),
                cookieCount = cookies.Count,
                cookieNames = cookies.Keys.ToList(),
                hasJwtToken = cookies.ContainsKey("jwt_token"),
                hasSessionCookie = cookies.Any(c => c.Key.Contains("Session")),
                scheme = HttpContext.Request.Scheme,
                isHttps = HttpContext.Request.IsHttps,
                host = HttpContext.Request.Host.ToString(),
                userAgent = HttpContext.Request.Headers["User-Agent"].ToString()
            });
        }
    }
}
