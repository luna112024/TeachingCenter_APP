using hongWenAPP.Models.Common;
using System.Net;
using System.Text.Json;

namespace hongWenAPP.Services
{
    /// <summary>
    /// Base service class providing common SendRequestAsync functionality for all API services
    /// Works optimally with UnifiedRequestHandler for consistent error handling
    /// </summary>
    public abstract class BaseApiService
    {
        protected readonly HttpClient _httpClient;
        protected readonly string _baseUrl;
        protected readonly JsonSerializerOptions _jsonOptions;

        protected BaseApiService(IHttpClientFactory httpClientFactory, IConfiguration configuration, string configKey = "ApiSettings:BaseUrl")
        {
            _httpClient = httpClientFactory.CreateClient("AuthJwtClient");
            _baseUrl = configuration[configKey];
            _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        /// <summary>
        /// Unified request method that works seamlessly with UnifiedRequestHandler
        /// The handler manages JWT tokens, authentication failures, and error responses automatically
        /// </summary>
        protected async Task<T> SendRequestAsync<T>(string url, HttpMethod method, object data = null)
        {
            try
            {
                HttpResponseMessage response = method switch
                {
                    var m when m == HttpMethod.Get => await _httpClient.GetAsync(url),
                    var m when m == HttpMethod.Post => await _httpClient.PostAsJsonAsync(url, data),
                    var m when m == HttpMethod.Put => await _httpClient.PutAsJsonAsync(url, data),
                    var m when m == HttpMethod.Delete => await _httpClient.DeleteAsync(url),
                    _ => throw new NotSupportedException($"HTTP method {method} not supported")
                };

                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    if (typeof(T) == typeof(string))
                        return (T)(object)content;

                    return JsonSerializer.Deserialize<T>(content, _jsonOptions);
                }
                else
                {
                    // Handle error responses
                    if (response.StatusCode == HttpStatusCode.BadRequest && typeof(T) == typeof(Response))
                    {
                        try
                        {
                            var errorResponse = JsonSerializer.Deserialize<Response>(content, _jsonOptions);
                            return (T)(object)errorResponse;
                        }
                        catch
                        {
                            // If can't deserialize as Response, try as ProblemDetails
                            var problemDetails = JsonSerializer.Deserialize<ProblemDetails>(content, _jsonOptions);
                            return (T)(object)new Response { Flag = false, Message = problemDetails.Detail ?? problemDetails.Title };
                        }
                    }
                    else
                    {
                        // Try to parse as ProblemDetails
                        try
                        {
                            var problemDetails = JsonSerializer.Deserialize<ProblemDetails>(content, _jsonOptions);
                            throw new HttpRequestException(problemDetails.Detail ?? problemDetails.Title, null, response.StatusCode);
                        }
                        catch (JsonException)
                        {
                            // If not valid JSON or not a ProblemDetails, use the content as error message
                            throw new HttpRequestException(content, null, response.StatusCode);
                        }
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                if (typeof(T) == typeof(Response))
                {
                    return (T)(object)new Response { Flag = false, Message = ex.Message };
                }
                throw;
            }
        }
    }

    /// <summary>
    /// Represents ProblemDetails response from API for error parsing
    /// </summary>
    public class ProblemDetails
    {
        public int Status { get; set; }
        public string? Title { get; set; }
        public string? Detail { get; set; }
        public string? Instance { get; set; }
        public string? TraceId { get; set; }
    }
} 