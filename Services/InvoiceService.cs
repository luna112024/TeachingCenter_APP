using hongWenAPP.Models.Common;
using hongWenAPP.Models.InvoiceModel.DTOs;

namespace hongWenAPP.Services
{
    public interface IInvoiceService
    {
        Task<List<GetInvoiceDTO>> GetAllInvoices(Guid? studentId = null, string? status = null, string? invoiceType = null);
        Task<GetInvoiceDetailDTO> GetInvoiceById(Guid invoiceId);
        Task<GetInvoiceDetailDTO> GetInvoiceByNumber(string invoiceNumber);
        Task<List<GetInvoiceDTO>> GetStudentInvoices(Guid studentId, string? status = null, Guid? termId = null);
        Task<List<GetInvoiceDTO>> GetOutstandingInvoices(Guid? studentId = null);
        Task<List<GetInvoiceDTO>> GetOverdueInvoices();
        Task<OutstandingBalanceDTO> GetStudentOutstandingBalance(Guid studentId);
        Task<Response> AddLineItem(Guid invoiceId, AddInvoiceLineItemDTO dto);
        Task<Response> ApplyDiscount(Guid invoiceId, ApplyDiscountDTO dto);
    }

    public class InvoiceService : BaseApiService, IInvoiceService
    {
        public InvoiceService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
            : base(httpClientFactory, configuration)
        {
        }

        public async Task<List<GetInvoiceDTO>> GetAllInvoices(Guid? studentId = null, string? status = null, string? invoiceType = null)
        {
            var queryParams = new List<string>();
            if (studentId.HasValue) queryParams.Add($"studentId={studentId}");
            if (!string.IsNullOrEmpty(status)) queryParams.Add($"status={status}");
            if (!string.IsNullOrEmpty(invoiceType)) queryParams.Add($"invoiceType={invoiceType}");

            var queryString = queryParams.Any() ? "?" + string.Join("&", queryParams) : "";
            return await SendRequestAsync<List<GetInvoiceDTO>>(
                $"{_baseUrl}/Invoice{queryString}",
                HttpMethod.Get);
        }

        public async Task<GetInvoiceDetailDTO> GetInvoiceById(Guid invoiceId) =>
            await SendRequestAsync<GetInvoiceDetailDTO>(
                $"{_baseUrl}/Invoice/{invoiceId}",
                HttpMethod.Get);

        public async Task<GetInvoiceDetailDTO> GetInvoiceByNumber(string invoiceNumber) =>
            await SendRequestAsync<GetInvoiceDetailDTO>(
                $"{_baseUrl}/Invoice/number/{invoiceNumber}",
                HttpMethod.Get);

        public async Task<List<GetInvoiceDTO>> GetStudentInvoices(Guid studentId, string? status = null, Guid? termId = null)
        {
            var queryParams = new List<string>();
            if (!string.IsNullOrEmpty(status)) queryParams.Add($"status={status}");
            if (termId.HasValue) queryParams.Add($"termId={termId}");

            var queryString = queryParams.Any() ? "?" + string.Join("&", queryParams) : "";
            return await SendRequestAsync<List<GetInvoiceDTO>>(
                $"{_baseUrl}/Invoice/student/{studentId}{queryString}",
                HttpMethod.Get);
        }

        public async Task<List<GetInvoiceDTO>> GetOutstandingInvoices(Guid? studentId = null)
        {
            var queryString = studentId.HasValue ? $"?studentId={studentId}" : "";
            return await SendRequestAsync<List<GetInvoiceDTO>>(
                $"{_baseUrl}/Invoice/outstanding{queryString}",
                HttpMethod.Get);
        }

        public async Task<List<GetInvoiceDTO>> GetOverdueInvoices() =>
            await SendRequestAsync<List<GetInvoiceDTO>>(
                $"{_baseUrl}/Invoice/overdue",
                HttpMethod.Get);

        public async Task<OutstandingBalanceDTO> GetStudentOutstandingBalance(Guid studentId) =>
            await SendRequestAsync<OutstandingBalanceDTO>(
                $"{_baseUrl}/Invoice/student/{studentId}/outstanding-balance",
                HttpMethod.Get);

        public async Task<Response> AddLineItem(Guid invoiceId, AddInvoiceLineItemDTO dto) =>
            await SendRequestAsync<Response>(
                $"{_baseUrl}/Invoice/{invoiceId}/line-items",
                HttpMethod.Post,
                dto);

        public async Task<Response> ApplyDiscount(Guid invoiceId, ApplyDiscountDTO dto) =>
            await SendRequestAsync<Response>(
                $"{_baseUrl}/Invoice/{invoiceId}/discount",
                HttpMethod.Put,
                dto);
    }
}

