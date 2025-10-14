using hongWenAPP.Models.Common;
using hongWenAPP.Models.PaymentModel.DTOs;

namespace hongWenAPP.Services
{
    /// <summary>
    /// Simple Payment Service - Cambodia Standard Practice
    /// - Immutable payments (NO editing after creation)
    /// - Void only for mistakes (original stays in system)
    /// - Support: Cash, Bank Transfer, ABA, Wing, TrueMoney
    /// </summary>
    public interface IPaymentService
    {
        // ========================================
        // PAYMENT RECORDING (IMMUTABLE - NO EDIT)
        // ========================================
        Task<Response> CreatePayment(CreatePaymentDTO paymentDto);
        Task<GetPaymentDTO> GetPayment(Guid paymentId);
        Task<GetPaymentDTO> GetPaymentByReference(string paymentReference);

        // ========================================
        // STUDENT PAYMENT HISTORY
        // ========================================
        Task<StudentPaymentHistoryDTO> GetStudentPaymentHistory(Guid studentId);

        // ========================================
        // VOID PAYMENT (For Mistakes Only)
        // ========================================
        Task<Response> VoidPayment(VoidPaymentDTO voidDto);

        // ========================================
        // PAYMENT REPORTS
        // ========================================
        Task<DailyPaymentReportDTO> GetDailyReport(DateTime reportDate);
        Task<List<GetPaymentDTO>> GetPaymentsByDateRange(DateTime startDate, DateTime endDate);
        Task<List<GetPaymentDTO>> GetPaymentsByTerm(string term);
        Task<decimal> GetTotalRevenue(DateTime? startDate = null, DateTime? endDate = null);
    }

    public class PaymentService : BaseApiService, IPaymentService
    {
        private readonly string _baseUrl;

        public PaymentService(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
            : base(httpClientFactory, configuration)
        {
            _baseUrl = $"{configuration["ApiSettings:BaseUrl"]}/Payment";
        }

        // ========================================
        // PAYMENT RECORDING (IMMUTABLE)
        // ========================================
        
        public async Task<Response> CreatePayment(CreatePaymentDTO paymentDto) =>
            await SendRequestAsync<Response>(
                $"{_baseUrl}",
                HttpMethod.Post,
                paymentDto);

        public async Task<GetPaymentDTO> GetPayment(Guid paymentId) =>
            await SendRequestAsync<GetPaymentDTO>(
                $"{_baseUrl}/{paymentId}",
                HttpMethod.Get);

        public async Task<GetPaymentDTO> GetPaymentByReference(string paymentReference) =>
            await SendRequestAsync<GetPaymentDTO>(
                $"{_baseUrl}/reference/{paymentReference}",
                HttpMethod.Get);

        // ========================================
        // STUDENT PAYMENT HISTORY
        // ========================================
        
        public async Task<StudentPaymentHistoryDTO> GetStudentPaymentHistory(Guid studentId) =>
            await SendRequestAsync<StudentPaymentHistoryDTO>(
                $"{_baseUrl}/student/{studentId}/history",
                HttpMethod.Get);

        // ========================================
        // VOID PAYMENT (For Mistakes Only)
        // ========================================
        
        public async Task<Response> VoidPayment(VoidPaymentDTO voidDto) =>
            await SendRequestAsync<Response>(
                $"{_baseUrl}/{voidDto.PaymentId}/void",
                HttpMethod.Post,
                voidDto);

        // ========================================
        // PAYMENT REPORTS
        // ========================================
        
        public async Task<DailyPaymentReportDTO> GetDailyReport(DateTime reportDate) =>
            await SendRequestAsync<DailyPaymentReportDTO>(
                $"{_baseUrl}/reports/daily/{reportDate:yyyy-MM-dd}",
                HttpMethod.Get);

        public async Task<List<GetPaymentDTO>> GetPaymentsByDateRange(DateTime startDate, DateTime endDate) =>
            await SendRequestAsync<List<GetPaymentDTO>>(
                $"{_baseUrl}/reports/date-range?startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}",
                HttpMethod.Get);

        public async Task<List<GetPaymentDTO>> GetPaymentsByTerm(string term) =>
            await SendRequestAsync<List<GetPaymentDTO>>(
                $"{_baseUrl}/reports/term/{term}",
                HttpMethod.Get);

        public async Task<decimal> GetTotalRevenue(DateTime? startDate = null, DateTime? endDate = null)
        {
            var url = $"{_baseUrl}/reports/revenue";
            if (startDate.HasValue && endDate.HasValue)
            {
                url += $"?startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}";
            }
            var result = await SendRequestAsync<dynamic>(url, HttpMethod.Get);
            return result?.TotalRevenueUSD ?? 0;
        }
    }
}
