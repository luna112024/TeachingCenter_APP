using hongWenAPP.Services;
using hongWenAPP.Helpers;

namespace hongWenAPP.Middleware
{
    /// <summary>
    /// Custom authentication middleware that handles user authentication and session management
    /// Enhanced to work with API's GlobalExceptionMiddleware ProblemDetails responses
    /// </summary>
    public class CustomAuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<CustomAuthenticationMiddleware> _logger;

        public CustomAuthenticationMiddleware(RequestDelegate next, ILogger<CustomAuthenticationMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, AuthenticationService authService)
        {
            var path = context.Request.Path.Value?.ToLower();
            var isAjaxRequest = IsAjaxRequest(context);

            // Skip authentication check for public paths
            if (IsPublicPath(path))
            {
                await _next(context);
                return;
            }

            // Check if authentication failed during request processing (set by UnifiedRequestHandler)
            var authFailed = context.Items.ContainsKey("AuthenticationFailed");
            if (authFailed)
            {
                await HandleAuthenticationFailure(context, isAjaxRequest);
                return;
            }

            // Check if user is authenticated
            if (!authService.IsAuthenticated())
            {
                await HandleUnauthenticatedUser(context, authService, isAjaxRequest);
                return;
            }

            // User is authenticated, continue with the request
            await _next(context);

            // After request processing, handle API error responses only if response hasn't started
            if (!context.Response.HasStarted)
            {
                await HandlePostRequestErrors(context, isAjaxRequest);
            }
            else
            {
                // If response has started, just log the error context for debugging
                LogPostRequestErrorsForDebugging(context);
            }
        }

        /// <summary>
        /// Determines if the request is an AJAX request
        /// </summary>
        private static bool IsAjaxRequest(HttpContext context)
        {
            return context.Request.Headers.ContainsKey("X-Requested-With") && 
                   context.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
        }

        /// <summary>
        /// Checks if the path is public and doesn't require authentication
        /// </summary>
        private static bool IsPublicPath(string? path)
        {
            if (string.IsNullOrEmpty(path)) return false;

            var publicPaths = new[]
            {
                "/login",
                "/account/login",
                "/account/logout",
                "/account/register",
                "/home/error",
                "/css",
                "/js",
                "/lib",
                "/images",
                "/favicon.ico",
                "/robots.txt",
                "/sitemap.xml"
            };

            // Check for public paths
            bool isPublicPath = publicPaths.Any(p => path.StartsWith(p));
            
            // Check for static files (contains dot but not .cshtml)
            bool isStaticFile = path.Contains('.') && !path.EndsWith(".cshtml");

            return isPublicPath || isStaticFile;
        }

        /// <summary>
        /// Handles unauthenticated users
        /// </summary>
        private async Task HandleUnauthenticatedUser(HttpContext context, AuthenticationService authService, bool isAjaxRequest)
        {
            _logger.LogInformation("Unauthenticated access attempt to: {Path}", context.Request.Path);

            // Check for session expired message
            var sessionExpiredMessage = context.Session.GetString("SessionExpiredMessage");
            
            if (isAjaxRequest)
            {
                if (!context.Response.HasStarted)
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsJsonAsync(new 
                    { 
                        success = false,
                        message = sessionExpiredMessage ?? "Authentication required",
                        redirectUrl = "/Login/SessionExpired"
                    });
                }
                return;
            }

            // For regular requests, redirect to login
            if (!context.Response.HasStarted)
            {
                if (!string.IsNullOrEmpty(sessionExpiredMessage))
                {
                    // Clear the session message
                    context.Session.Remove("SessionExpiredMessage");
                    
                    // Redirect with expired flag and message
                    var encodedMessage = Uri.EscapeDataString(sessionExpiredMessage);
                    context.Response.Redirect($"/Login?expired=true&message={encodedMessage}");
                }
                else
                {
                    // Regular redirect to login
                    context.Response.Redirect("/Login");
                }
            }
        }

        /// <summary>
        /// Handles authentication failures that occurred during request processing
        /// </summary>
        private async Task HandleAuthenticationFailure(HttpContext context, bool isAjaxRequest)
        {
            _logger.LogInformation("Authentication failure detected during request processing");

            var errorMessage = context.Items["ErrorMessage"]?.ToString();
            var originalRequestUri = context.Items["OriginalRequestUri"]?.ToString();
            
            if (isAjaxRequest)
            {
                if (!context.Response.HasStarted)
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsJsonAsync(new 
                    { 
                        success = false,
                        message = errorMessage ?? "Your session has expired. Please log in again.",
                        redirectUrl = "/Login/SessionExpired",
                        originalRequest = originalRequestUri
                    });
                }
                return;
            }

            // For regular requests, redirect to session expired
            if (!context.Response.HasStarted)
            {
                context.Response.Redirect("/Login/SessionExpired");
            }
        }

        /// <summary>
        /// Handles various error responses after request processing
        /// </summary>
        private async Task HandlePostRequestErrors(HttpContext context, bool isAjaxRequest)
        {
            // Check for various error types set by UnifiedRequestHandler
            if (context.Items.ContainsKey("PermissionDenied"))
            {
                await HandlePermissionDenied(context, isAjaxRequest);
            }
            else if (context.Items.ContainsKey("ResourceNotFound"))
            {
                await HandleResourceNotFound(context, isAjaxRequest);
            }
            else if (context.Items.ContainsKey("RateLimitExceeded"))
            {
                await HandleRateLimitExceeded(context, isAjaxRequest);
            }
            else if (context.Items.ContainsKey("GenericError"))
            {
                await HandleGenericError(context, isAjaxRequest);
            }
        }

        /// <summary>
        /// Logs error information for debugging when response has already started
        /// </summary>
        private void LogPostRequestErrorsForDebugging(HttpContext context)
        {
            if (context.Items.ContainsKey("PermissionDenied"))
            {
                var errorMessage = context.Items["ErrorMessage"]?.ToString();
                var problemDetails = context.Items["ApiErrorResponse"] as ProblemDetailsResponse;
                _logger.LogWarning("Permission denied (response already started) for path: {Path}, Message: {Message}, TraceId: {TraceId}", 
                    context.Request.Path, errorMessage, problemDetails?.TraceId);
            }
            else if (context.Items.ContainsKey("GenericError"))
            {
                var errorMessage = context.Items["ErrorMessage"]?.ToString();
                var statusCode = context.Items["ErrorStatusCode"] as int? ?? 500;
                var problemDetails = context.Items["ApiErrorResponse"] as ProblemDetailsResponse;
                _logger.LogError("Generic error (response already started) for path: {Path}, Status: {StatusCode}, Message: {Message}, TraceId: {TraceId}", 
                    context.Request.Path, statusCode, errorMessage, problemDetails?.TraceId);
            }
        }

        /// <summary>
        /// Handles 403 Permission Denied responses
        /// </summary>
        private async Task HandlePermissionDenied(HttpContext context, bool isAjaxRequest)
        {
            var errorMessage = context.Items["ErrorMessage"]?.ToString() ?? "You don't have permission to access this resource.";
            var problemDetails = context.Items["ApiErrorResponse"] as ProblemDetailsResponse;
            
            _logger.LogWarning("Permission denied for path: {Path}, Message: {Message}, TraceId: {TraceId}", 
                context.Request.Path, errorMessage, problemDetails?.TraceId);

            if (isAjaxRequest)
            {
                if (!context.Response.HasStarted)
                {
                    context.Response.StatusCode = 403;
                    await context.Response.WriteAsJsonAsync(new 
                    { 
                        success = false,
                        message = errorMessage,
                        title = problemDetails?.Title ?? "Access Denied",
                        traceId = problemDetails?.TraceId
                    });
                }
                return;
            }

            // For regular requests, redirect to access denied page
            if (!context.Response.HasStarted)
            {
                context.Response.Redirect($"/Home/AccessDenied?message={Uri.EscapeDataString(errorMessage)}");
            }
        }

        /// <summary>
        /// Handles 404 Resource Not Found responses
        /// </summary>
        private async Task HandleResourceNotFound(HttpContext context, bool isAjaxRequest)
        {
            var errorMessage = context.Items["ErrorMessage"]?.ToString() ?? "The requested resource was not found.";
            var problemDetails = context.Items["ApiErrorResponse"] as ProblemDetailsResponse;
            
            _logger.LogWarning("Resource not found for path: {Path}, Message: {Message}, TraceId: {TraceId}", 
                context.Request.Path, errorMessage, problemDetails?.TraceId);

            if (isAjaxRequest)
            {
                if (!context.Response.HasStarted)
                {
                    context.Response.StatusCode = 404;
                    await context.Response.WriteAsJsonAsync(new 
                    { 
                        success = false,
                        message = errorMessage,
                        title = problemDetails?.Title ?? "Not Found",
                        traceId = problemDetails?.TraceId
                    });
                }
                return;
            }

            // For regular requests, you could redirect to a 404 page
            if (!context.Response.HasStarted)
            {
                context.Response.StatusCode = 404;
            }
        }

        /// <summary>
        /// Handles 429 Rate Limit Exceeded responses
        /// </summary>
        private async Task HandleRateLimitExceeded(HttpContext context, bool isAjaxRequest)
        {
            var errorMessage = context.Items["ErrorMessage"]?.ToString() ?? "Too many requests. Please try again later.";
            var problemDetails = context.Items["ApiErrorResponse"] as ProblemDetailsResponse;
            
            _logger.LogWarning("Rate limit exceeded for path: {Path}, Message: {Message}, TraceId: {TraceId}", 
                context.Request.Path, errorMessage, problemDetails?.TraceId);

            if (isAjaxRequest)
            {
                if (!context.Response.HasStarted)
                {
                    context.Response.StatusCode = 429;
                    await context.Response.WriteAsJsonAsync(new 
                    { 
                        success = false,
                        message = errorMessage,
                        title = problemDetails?.Title ?? "Rate Limit Exceeded",
                        traceId = problemDetails?.TraceId,
                        retryAfter = "60" // Suggest retry after 60 seconds
                    });
                }
                return;
            }

            // For regular requests, could redirect to a rate limit page
            if (!context.Response.HasStarted)
            {
                context.Response.StatusCode = 429;
            }
        }

        /// <summary>
        /// Handles generic errors (500, etc.)
        /// </summary>
        private async Task HandleGenericError(HttpContext context, bool isAjaxRequest)
        {
            var errorMessage = context.Items["ErrorMessage"]?.ToString() ?? "An unexpected error occurred.";
            var statusCode = context.Items["ErrorStatusCode"] as int? ?? 500;
            var problemDetails = context.Items["ApiErrorResponse"] as ProblemDetailsResponse;
            
            _logger.LogError("Generic error for path: {Path}, Status: {StatusCode}, Message: {Message}, TraceId: {TraceId}", 
                context.Request.Path, statusCode, errorMessage, problemDetails?.TraceId);

            if (isAjaxRequest)
            {
                if (!context.Response.HasStarted)
                {
                    context.Response.StatusCode = statusCode;
                    await context.Response.WriteAsJsonAsync(new 
                    { 
                        success = false,
                        message = errorMessage,
                        title = problemDetails?.Title ?? "Error",
                        traceId = problemDetails?.TraceId,
                        statusCode = statusCode
                    });
                }
                return;
            }

            // For regular requests, redirect to error page
            if (!context.Response.HasStarted)
            {
                context.Response.Redirect($"/Home/Error?message={Uri.EscapeDataString(errorMessage)}");
            }
        }
    }

    /// <summary>
    /// Extension method to register the middleware
    /// </summary>
    public static class CustomAuthenticationMiddlewareExtensions
    {
        public static IApplicationBuilder UseCustomAuthentication(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CustomAuthenticationMiddleware>();
        }
    }
} 