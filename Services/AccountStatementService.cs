using hongWenAPP.Models.ViewStatement.DTOs;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace hongWenAPP.Services
{
    public interface IAccountStatementService
    {
        Task<AccountStatementResponse> GetAccountStatementDataAsync(AccountStatementRequest request);
    }

    public class AccountStatementService : IAccountStatementService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<AccountStatementService> _logger;
        private readonly AuthenticationService _authService;
        private readonly string _baseUrl;

        public AccountStatementService(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<AccountStatementService> logger, AuthenticationService authService)
        {
            _httpClient = httpClientFactory.CreateClient();
            _logger = logger;
            _authService = authService;
            _baseUrl = configuration.GetValue<string>("EStatementAPI:BaseUrl", "http://localhost:5002");
        }

        public async Task<AccountStatementResponse> GetAccountStatementDataAsync(AccountStatementRequest request)
        {
            try
            {
                _logger.LogInformation($"Calling EStatement API for account: {request.AccountNo}, Company: {request.CompanyName}, DateRange: {request.FromDate} to {request.ToDate}");

                var apiRequest = new
                {
                    accountNo = request.AccountNo,           // lowercase to match API
                    fromDate = request.FromDate,             // yyyyMMdd format
                    toDate = request.ToDate,                 // yyyyMMdd format
                    companyCode = request.CompanyName ?? "", 
                    userId = (int?)null                      // null as shown in example
                };

                var apiResponse = await SendRequestAsync<dynamic>(
                    $"{_baseUrl}/api/EStatement", 
                    HttpMethod.Post, 
                    apiRequest);

                if (apiResponse?.success == true)
                {
                    var accountStatements = ConvertApiDataToAccountStatements(apiResponse);
                    
                    _logger.LogInformation($"EStatement API response processed successfully. Records: {accountStatements.Count}");

                    return new AccountStatementResponse
                    {
                        Data = accountStatements,
                        Success = true,
                        Message = "Data retrieved successfully"
                    };
                }
                else
                {
                    var errorMessage = apiResponse?.message?.ToString() ?? "API returned unsuccessful response";
                    _logger.LogWarning($"EStatement API returned unsuccessful response: {errorMessage}");
                    
                    return new AccountStatementResponse
                    {
                        Success = false,
                        Message = errorMessage
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling EStatement API for account: {AccountNo}", request.AccountNo);
                return new AccountStatementResponse
                {
                    Success = false,
                    Message = $"Error calling EStatement API: {ex.Message}"
                };
            }
        }

        private List<AccountStatementModel> ConvertApiDataToAccountStatements(dynamic apiResponse)
        {
            var accountStatements = new List<AccountStatementModel>();
            
            try
            {
                string accountNo = apiResponse.accountNo?.ToString() ?? "";
                string nameLatin = apiResponse.nameLatin?.ToString() ?? "";
                string nameKhmer = apiResponse.nameKhmer?.ToString() ?? "";
                string address = apiResponse.address?.ToString() ?? "";
                string addressKhmer = apiResponse.addressKhmer?.ToString() ?? "";
                string phone = apiResponse.phone?.ToString() ?? "";
                string branch = apiResponse.branch?.ToString() ?? "";
                string currency = apiResponse.currency?.ToString() ?? "";

                decimal workingBalance = 0;
                if (apiResponse.workingBalance != null)
                {
                    decimal.TryParse(apiResponse.workingBalance.ToString(), out workingBalance);
                }

                var statements = apiResponse.statements;
                if (statements != null)
                {
                    int entryId = 1;
                    foreach (var stmt in statements)
                    {
                        var accountStatement = new AccountStatementModel
                        {
                            ID = entryId,
                            AccountNo = accountNo,
                            NameLatin = nameLatin,
                            NameKhmer = nameKhmer,
                            Address = address,
                            AddressKhmer = addressKhmer,
                            Phone = phone,
                            Branch = branch,
                            Currency = currency,
                            Date = stmt.date?.ToString() ?? "",
                            Time = stmt.time?.ToString() ?? "",
                            Code = stmt.code?.ToString() ?? "",
                            Description = stmt.description?.ToString() ?? "",
                            Deposit = stmt.deposit?.ToString() ?? "",
                            Withdraw = stmt.withdraw?.ToString() ?? "",
                            Balance = stmt.balance?.ToString() ?? "",
                            TransRefNo = stmt.transRefNo?.ToString() ?? "",
                            ActualTxnDate = stmt.tranDate?.ToString() ?? ""
                        };

                        accountStatements.Add(accountStatement);
                        entryId++;
                    }
                }

                return accountStatements;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error converting API data: {ex.Message}", ex);
            }
        }


        /// <summary>
        /// Custom SendRequestAsync method with JWT token attachment for EStatement API
        /// </summary>
        private async Task<T> SendRequestAsync<T>(string url, HttpMethod method, object data = null)
        {
            try
            {
                using var request = new HttpRequestMessage(method, url);

                await AttachAuthTokenAsync(request);

                if (method == HttpMethod.Post && data != null)
                {
                    var jsonContent = JsonConvert.SerializeObject(data);
                    request.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                    
                    _logger.LogInformation($"API Request to {url}: {jsonContent}");
                }

                var response = await _httpClient.SendAsync(request);
                var responseContent = await response.Content.ReadAsStringAsync();
                
                _logger.LogInformation($"API Response Status: {response.StatusCode}");
                _logger.LogInformation($"API Response Content: {responseContent}");

                if (response.IsSuccessStatusCode)
                {
                    if (typeof(T) == typeof(string))
                        return (T)(object)responseContent;

                    return JsonConvert.DeserializeObject<T>(responseContent);
                }
                else
                {
                    throw new HttpRequestException($"API call failed with status: {response.StatusCode}. Response: {responseContent}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in SendRequestAsync for URL: {Url}", url);
                throw;
            }
        }

        /// <summary>
        /// Attaches JWT token to outgoing API requests (similar to UnifiedRequestHandler)
        /// </summary>
        private async Task AttachAuthTokenAsync(HttpRequestMessage request)
        {
            try
            {
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
                else
                {
                    _logger.LogDebug("User not authenticated, no JWT token attached to request: {RequestUri}", request.RequestUri);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error attaching auth token to request: {RequestUri}", request.RequestUri);
                // Continue without token rather than failing the request
            }
        }
    }


} 