using hongWenAPP.Models;
using hongWenAPP.Models.IdentityModel.DTOs;
using hongWenAPP.Models.Common;
using System.Text;
using System.Text.Json;

namespace hongWenAPP.Services
{
    public class AuthenticationService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        
        // Session keys for caching user data
        private const string USER_PERMISSIONS_KEY = "UserPermissions";
        private const string USER_INFO_KEY = "UserInfo";
        private const string JWT_TOKEN_COOKIE = "jwt_token";
        private const string AUTH_SESSION_KEY = "IsAuthenticated";
        
        public AuthenticationService(HttpClient httpClient, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }
        
        public async Task<TokenResponse?> LoginAsync(LoginModel loginModel)
        {
            try
            {
                var apiUrl = _configuration["ApiSettings:BaseUrl"];
                var requestContent = new StringContent(
                    JsonSerializer.Serialize(new { username = loginModel.Username, password = loginModel.Password }),
                    Encoding.UTF8,
                    "application/json");
                
                var response = await _httpClient.PostAsync($"{apiUrl}/Identity/login", requestContent);
                
                var responseContent = await response.Content.ReadAsStringAsync();
                
                // Handle different response scenarios
                if (response.IsSuccessStatusCode)
                {
                    // Successful login - parse as TokenResponse
                    var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(responseContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    
                    if (tokenResponse != null && !string.IsNullOrEmpty(tokenResponse.AccessToken))
                    {
                        StoreLoginResponse(tokenResponse);
                    }
                    return tokenResponse;
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    // Try to parse as TokenResponse first (for structured error responses)
                    try
                    {
                        var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(responseContent, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });
                        
                        if (tokenResponse != null && !string.IsNullOrEmpty(tokenResponse.ErrorMessage))
                        {
                            return tokenResponse;
                        }
                    }
                    catch
                    {
                        // Ignore parsing error, try alternative format
                    }
                    
                    // Try to parse as simple error message format (for locked accounts)
                    try
                    {
                        var errorResponse = JsonSerializer.Deserialize<Dictionary<string, object>>(responseContent, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });
                        
                        if (errorResponse != null && errorResponse.ContainsKey("message"))
                        {
                            var errorMessage = errorResponse["message"].ToString();
                            return new TokenResponse
                            {
                                IsSuccess = false,
                                ErrorMessage = errorMessage
                            };
                        }
                    }
                    catch
                    {
                        // Ignore parsing error, fall back to generic message
                    }
                    
                    // Fallback for generic unauthorized
                    return new TokenResponse
                    {
                        IsSuccess = false,
                        ErrorMessage = "Invalid username or password"
                    };
                }
                else
                {
                    // Other error responses
                    return new TokenResponse
                    {
                        IsSuccess = false,
                        ErrorMessage = "Login service is currently unavailable. Please try again later."
                    };
                }
            }
            catch (Exception ex)
            {
                // Log exception
                Console.WriteLine($"Login error: {ex.Message}");
                return new TokenResponse
                {
                    IsSuccess = false,
                    ErrorMessage = "An unexpected error occurred. Please try again."
                };
            }
        }
        
        /// <summary>
        /// Store login response securely - JWT in httpOnly cookie, user data in session
        /// </summary>
        private void StoreLoginResponse(TokenResponse tokenResponse)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var session = httpContext.Session;
            
            // Ensure session is available and loaded
            if (!session.IsAvailable)
            {
                // Force session to be loaded/created
                session.SetString("_init", "1");
                session.Remove("_init");
            }
            
            // Determine if we're using HTTPS or HTTP
            var isHttps = httpContext.Request.IsHttps;
            
            // Store JWT token as httpOnly cookie (most secure)
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = isHttps, // Only secure for HTTPS, false for HTTP
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn)
            };
            
            httpContext.Response.Cookies.Append(JWT_TOKEN_COOKIE, tokenResponse.AccessToken, cookieOptions);
            
            // Store user permissions in session for quick access (no need to decode JWT every time)
            if (tokenResponse.User.Permissions != null)
            {
                session.SetString(USER_PERMISSIONS_KEY, 
                    JsonSerializer.Serialize(tokenResponse.User.Permissions));
            }
            
            // Store user info in session for quick access
            session.SetString(USER_INFO_KEY, 
                JsonSerializer.Serialize(tokenResponse.User));
            
            // Mark user as authenticated in session
            session.SetString(AUTH_SESSION_KEY, "true");
        }
        
        /// <summary>
        /// Get user permissions from session (fast access)
        /// </summary>
        public List<string> GetUserPermissions()
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            if (session == null) return new List<string>();
            
            var permissionsJson = session.GetString(USER_PERMISSIONS_KEY);
            
            if (string.IsNullOrEmpty(permissionsJson))
                return new List<string>();
                
            try
            {
                return JsonSerializer.Deserialize<List<string>>(permissionsJson) ?? new List<string>();
            }
            catch
            {
                return new List<string>();
            }
        }
        
        /// <summary>
        /// Get user info from session (fast access)
        /// </summary>
        public UserInfo? GetUserInfo()
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            if (session == null) return null;
            
            var userJson = session.GetString(USER_INFO_KEY);
            
            if (string.IsNullOrEmpty(userJson))
                return null;
                
            try
            {
                return JsonSerializer.Deserialize<UserInfo>(userJson);
            }
            catch
            {
                return null;
            }
        }
        
        /// <summary>
        /// Check if user has a specific permission
        /// </summary>
        public bool HasPermission(string permission)
        {
            if (string.IsNullOrEmpty(permission)) return false;
            return GetUserPermissions().Contains(permission);
        }
        
        /// <summary>
        /// Check if user has any of the specified permissions (OR logic)
        /// </summary>
        public bool HasAnyPermission(params string[] permissions)
        {
            if (permissions == null || permissions.Length == 0) return false;
            var userPermissions = GetUserPermissions();
            return permissions.Any(p => userPermissions.Contains(p));
        }
        
        /// <summary>
        /// Check if user has all specified permissions (AND logic)
        /// </summary>
        public bool HasAllPermissions(params string[] permissions)
        {
            if (permissions == null || permissions.Length == 0) return false;
            var userPermissions = GetUserPermissions();
            return permissions.All(p => userPermissions.Contains(p));
        }
        
        /// <summary>
        /// Check if user is in specific role
        /// </summary>
        public bool HasRole(string role)
        {
            if (string.IsNullOrEmpty(role)) return false;
            var userInfo = GetUserInfo();
            return userInfo?.Roles?.Contains(role) ?? false;
        }
        
        /// <summary>
        /// Check if user has any of the specified roles (OR logic)
        /// </summary>
        public bool HasAnyRole(params string[] roles)
        {
            if (roles == null || roles.Length == 0) return false;
            var userInfo = GetUserInfo();
            if (userInfo?.Roles == null) return false;
            return roles.Any(r => userInfo.Roles.Contains(r));
        }
        
        public async Task LogoutAsync()
        {
            // Clear user session data - this handles everything
            ClearUserSession();
        }
        
        /// <summary>
        /// Clear user session data (logout)
        /// </summary>
        public void ClearUserSession()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var session = httpContext?.Session;
            
            if (session != null)
            {
                // Remove all session data
                session.Remove(USER_PERMISSIONS_KEY);
                session.Remove(USER_INFO_KEY);
                session.Remove(AUTH_SESSION_KEY);
                
                // Clear any session expired messages
                session.Remove("SessionExpiredMessage");
                
                // Clear the entire session to ensure no leftover data
                session.Clear();
            }
            
            // Remove JWT token cookie with same security settings as when it was created
            if (httpContext?.Response != null)
            {
                // Determine if we're using HTTPS or HTTP
                var isHttps = httpContext.Request.IsHttps;
                
                httpContext.Response.Cookies.Delete(JWT_TOKEN_COOKIE, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = isHttps, // Only secure for HTTPS, false for HTTP
                    SameSite = SameSiteMode.Strict
                });
            }
        }
        
        public string GetToken()
        {
            return _httpContextAccessor.HttpContext?.Request.Cookies[JWT_TOKEN_COOKIE] ?? string.Empty;
        }
        
        public bool IsAuthenticated()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext?.Session == null) return false;
            
            // Check session authentication flag, user data, and token presence
            var isSessionAuthenticated = httpContext.Session.GetString(AUTH_SESSION_KEY) == "true";
            var hasUserData = GetUserInfo() != null;
            var hasToken = !string.IsNullOrEmpty(GetToken());
            
            return isSessionAuthenticated && hasUserData && hasToken;
        }
        
        /// <summary>
        /// Get current user ID from session
        /// </summary>
        public Guid? GetCurrentUserId()
        {
            return GetUserInfo()?.UserId;
        }
        
        /// <summary>
        /// Get current user's company info
        /// </summary>
        public CompanyInfo? GetUserCompany()
        {
            return GetUserInfo()?.Company;
        }
        
        public async Task<Response?> ForgotPasswordAsync(ForgotPasswordDTO forgotPasswordDto)
        {
            try
            {
                var apiUrl = _configuration["ApiSettings:BaseUrl"];
                var requestContent = new StringContent(
                    JsonSerializer.Serialize(forgotPasswordDto),
                    Encoding.UTF8,
                    "application/json");

                var response = await _httpClient.PostAsync($"{apiUrl}/Identity/forgot-password", requestContent);
                var responseContent = await response.Content.ReadAsStringAsync();

                var result = JsonSerializer.Deserialize<Response>(responseContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return result;
            }
            catch (Exception ex)
            {
                // Log exception
                Console.WriteLine($"Forgot password error: {ex.Message}");
                return new Response
                {
                    Flag = false,
                    Message = "An error occurred while processing your request. Please try again later."
                };
            }
        }

        public async Task<Response?> ResetPasswordAsync(ResetPasswordDTO resetPasswordDto)
        {
            try
            {
                var apiUrl = _configuration["ApiSettings:BaseUrl"];
                var requestContent = new StringContent(
                    JsonSerializer.Serialize(resetPasswordDto),
                    Encoding.UTF8,
                    "application/json");

                var response = await _httpClient.PostAsync($"{apiUrl}/Identity/reset-password", requestContent);
                var responseContent = await response.Content.ReadAsStringAsync();

                var result = JsonSerializer.Deserialize<Response>(responseContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return result;
            }
            catch (Exception ex)
            {
                // Log exception
                Console.WriteLine($"Reset password error: {ex.Message}");
                return new Response
                {
                    Flag = false,
                    Message = "An error occurred while processing your request. Please try again later."
                };
            }
        }
        
    
        
        /// <summary>
        /// Check if the current session is valid and not expired
        /// </summary>
        public bool IsSessionValid()
        {
            if (!IsAuthenticated()) return false;
            
            var token = GetToken();
            if (string.IsNullOrEmpty(token)) return false;
            
            // Additional validation can be added here if needed
            // For example, checking token expiry from JWT payload
            
            return true;
        }
    }
} 