using hongWenAPP.Services;
using System.Net.Http.Headers;
using System.Text.Json;

namespace hongWenAPP.Helpers
{
    /// <summary>
    /// Unified request handler that combines authentication token attachment and response error handling
    /// Enhanced to work with API's GlobalExceptionMiddleware ProblemDetails responses
    /// </summary>
    public class UnifiedRequestHandler : DelegatingHandler
    {
        private readonly AuthenticationService _authService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<UnifiedRequestHandler> _logger;

        public UnifiedRequestHandler(
            AuthenticationService authService, 
            IHttpContextAccessor httpContextAccessor,
            ILogger<UnifiedRequestHandler> logger)
        {
            _authService = authService;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // STEP 1: Attach JWT token to outgoing requests
            await AttachAuthTokenAsync(request);

            HttpResponseMessage response;
            
            try
            {
                // STEP 2: Send the request
                response = await base.SendAsync(request, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending HTTP request to {RequestUri}", request.RequestUri);
                throw;
            }

            // STEP 3: Handle error responses from API GlobalExceptionMiddleware
            if (!response.IsSuccessStatusCode)
            {
                await HandleErrorResponseAsync(request, response);
            }

            return response;
        }

        /// <summary>
        /// Attaches JWT token to outgoing API requests
        /// </summary>
        private async Task AttachAuthTokenAsync(HttpRequestMessage request)
        {
            try
            {
                // Only attach token if user is authenticated and token exists
                if (_authService.IsAuthenticated())
                {
                    var token = _authService.GetToken();
                    if (!string.IsNullOrEmpty(token))
                    {
                        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                        _logger.LogDebug("JWT token attached to request: {RequestUri}", request.RequestUri);
                    }
                    else
                    {
                        _logger.LogWarning("User appears authenticated but no JWT token found");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error attaching auth token to request: {RequestUri}", request.RequestUri);
                // Continue without token rather than failing the request
            }
        }

        /// <summary>
        /// Handles error responses from API's GlobalExceptionMiddleware
        /// </summary>
        private async Task HandleErrorResponseAsync(HttpRequestMessage request, HttpResponseMessage response)
        {
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;
                if (httpContext == null)
                {
                    _logger.LogWarning("HttpContext not available during error response handling");
                    return;
                }

                // Parse ProblemDetails response from API
                var problemDetails = await ParseProblemDetailsAsync(response);
                var statusCode = (int)response.StatusCode;

                _logger.LogWarning("API error response received - Status: {StatusCode}, Title: {Title}, Detail: {Detail}, TraceId: {TraceId}", 
                    statusCode, problemDetails?.Title, problemDetails?.Detail, problemDetails?.TraceId);

                // Handle specific error types
                switch (statusCode)
                {
                    case 401: // Unauthorized
                        await HandleUnauthorizedResponseAsync(httpContext, problemDetails);
                        break;
                        
                    case 403: // Forbidden
                        await HandleForbiddenResponseAsync(httpContext, problemDetails);
                        break;
                        
                    case 404: // Not Found
                        await HandleNotFoundResponseAsync(httpContext, problemDetails);
                        break;
                        
                    case 429: // Rate Limit
                        await HandleRateLimitResponseAsync(httpContext, problemDetails);
                        break;
                        
                    default: // Other errors (500, etc.)
                        await HandleGenericErrorResponseAsync(httpContext, problemDetails, statusCode);
                        break;
                }

                // Set context information for middleware
                httpContext.Items["ApiErrorResponse"] = problemDetails;
                httpContext.Items["ApiErrorStatusCode"] = statusCode;
                httpContext.Items["OriginalRequestUri"] = request.RequestUri?.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling API error response for request: {RequestUri}", request.RequestUri);
            }
        }

        /// <summary>
        /// Parses ProblemDetails response from API
        /// </summary>
        private async Task<ProblemDetailsResponse?> ParseProblemDetailsAsync(HttpResponseMessage response)
        {
            try
            {
                var contentType = response.Content.Headers.ContentType?.MediaType;
                if (contentType == "application/problem+json" || contentType == "application/json")
                {
                    var content = await response.Content.ReadAsStringAsync();
                    if (!string.IsNullOrEmpty(content))
                    {
                        var options = new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true,
                            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                        };
                        
                        return JsonSerializer.Deserialize<ProblemDetailsResponse>(content, options);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error parsing ProblemDetails response");
            }

            // Fallback for non-JSON responses
            return new ProblemDetailsResponse
            {
                Status = (int)response.StatusCode,
                Title = response.ReasonPhrase ?? "Error",
                Detail = "An error occurred while processing your request",
                Instance = response.RequestMessage?.RequestUri?.ToString()
            };
        }

        /// <summary>
        /// Handles 401 Unauthorized responses
        /// </summary>
        private async Task HandleUnauthorizedResponseAsync(HttpContext httpContext, ProblemDetailsResponse? problemDetails)
        {
            // Set session expired message with API details
            var message = problemDetails?.Detail ?? "Your session has expired. Please log in again.";
            await SetSessionMessageAsync(httpContext, "SessionExpiredMessage", message);

            // Clear user authentication and session data
            _authService.ClearUserSession();

            // Set context flag for middleware
            httpContext.Items["AuthenticationFailed"] = true;
            httpContext.Items["ErrorMessage"] = message;

            _logger.LogInformation("User session cleared due to 401 unauthorized API response");
        }

        /// <summary>
        /// Handles 403 Forbidden responses
        /// </summary>
        private async Task HandleForbiddenResponseAsync(HttpContext httpContext, ProblemDetailsResponse? problemDetails)
        {
            var message = problemDetails?.Detail ?? "You don't have permission to perform this action.";
            await SetSessionMessageAsync(httpContext, "PermissionDeniedMessage", message);

            httpContext.Items["PermissionDenied"] = true;
            httpContext.Items["ErrorMessage"] = message;
        }

        /// <summary>
        /// Handles 404 Not Found responses
        /// </summary>
        private async Task HandleNotFoundResponseAsync(HttpContext httpContext, ProblemDetailsResponse? problemDetails)
        {
            var message = problemDetails?.Detail ?? "The requested resource was not found.";
            await SetSessionMessageAsync(httpContext, "NotFoundMessage", message);

            httpContext.Items["ResourceNotFound"] = true;
            httpContext.Items["ErrorMessage"] = message;
        }

        /// <summary>
        /// Handles 429 Rate Limit responses
        /// </summary>
        private async Task HandleRateLimitResponseAsync(HttpContext httpContext, ProblemDetailsResponse? problemDetails)
        {
            var message = problemDetails?.Detail ?? "Too many requests. Please try again later.";
            await SetSessionMessageAsync(httpContext, "RateLimitMessage", message);

            httpContext.Items["RateLimitExceeded"] = true;
            httpContext.Items["ErrorMessage"] = message;
        }

        /// <summary>
        /// Handles other error responses (500, etc.)
        /// </summary>
        private async Task HandleGenericErrorResponseAsync(HttpContext httpContext, ProblemDetailsResponse? problemDetails, int statusCode)
        {
            var message = problemDetails?.Detail ?? "An unexpected error occurred. Please try again.";
            await SetSessionMessageAsync(httpContext, "GenericErrorMessage", message);

            httpContext.Items["GenericError"] = true;
            httpContext.Items["ErrorMessage"] = message;
            httpContext.Items["ErrorStatusCode"] = statusCode;
        }

        /// <summary>
        /// Sets session message safely
        /// </summary>
        private async Task SetSessionMessageAsync(HttpContext httpContext, string key, string message)
        {
            try
            {
                if (httpContext.Session != null && httpContext.Session.IsAvailable)
                {
                    await httpContext.Session.LoadAsync();
                    httpContext.Session.SetString(key, message);
                    await httpContext.Session.CommitAsync();

                    _logger.LogDebug("Session message set: {Key} = {Message}", key, message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to set session message: {Key}", key);
            }
        }
    }

    /// <summary>
    /// Represents ProblemDetails response from API
    /// </summary>
    public class ProblemDetailsResponse
    {
        public int Status { get; set; }
        public string? Title { get; set; }
        public string? Detail { get; set; }
        public string? Instance { get; set; }
        public string? TraceId { get; set; }
    }
} 