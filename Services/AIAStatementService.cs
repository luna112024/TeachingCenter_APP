using hongWenAPP.Models.ViewStatement.DTOs;
using hongWenAPP.Models.Common;

namespace hongWenAPP.Services
{
    public interface IAIAStatementService
    {
        Task<AIAStatementResponse> GetAIAStatementDataAsync(AIAStatementModel request);
    }

    public class AIAStatementService : BaseApiService, IAIAStatementService
    {
        private readonly ILogger<AIAStatementService> _logger;

        public AIAStatementService(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<AIAStatementService> logger)
            : base(httpClientFactory, configuration, "AIAApi:BaseUrl")
        {
            _logger = logger;
        }

        public async Task<AIAStatementResponse> GetAIAStatementDataAsync(AIAStatementModel request)
        {
            try
            {
                _logger.LogInformation("Calling AIA API for statement data. Type: {TransactionType}, Period: {PeriodDate}", 
                    request.TransactionType, request.PeriodDate);

                var apiRequest = new
                {
                    AccountNo = request.AccountNo,
                    PeriodDate = request.PeriodDate,
                    TransactionType = request.TransactionType
                };

                var apiResponse = await SendRequestAsync<AIAApiResponse>(
                    $"{_baseUrl}/api/AIAStatement", 
                    HttpMethod.Post, 
                    apiRequest);

                // Convert the data array to DynamicAIAData objects
                var dynamicData = apiResponse?.Data?.Select(item => new DynamicAIAData(item)).ToList() ?? new List<DynamicAIAData>();

                _logger.LogInformation("AIA API response processed successfully. Records: {RecordCount}", dynamicData.Count);

                return new AIAStatementResponse
                {
                    Data = dynamicData,
                    GeneratedAt = apiResponse?.GeneratedAt ?? DateTime.UtcNow,
                    Success = true,
                    Message = "Data retrieved successfully"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling AIA API");
                return new AIAStatementResponse
                {
                    Success = false,
                    Message = $"Error calling AIA API: {ex.Message}"
                };
            }
        }
    }

    // Internal class to match the AIA API response structure
    internal class AIAApiResponse
    {
        public List<Dictionary<string, object>> Data { get; set; } = new();
        public DateTime GeneratedAt { get; set; }
    }
} 