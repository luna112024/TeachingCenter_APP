using hongWenAPP.Models.Common;
using hongWenAPP.Models.PaymentNewModel.DTOs;

namespace hongWenAPP.Services
{
    public interface IPaymentNewService
    {
        Task<Response> RecordPayment(CreatePaymentNewDTO dto);
        Task<Response> ConfirmPayment(Guid paymentId, ConfirmPaymentDTO dto);
        Task<GetPaymentNewDTO> GetPaymentById(Guid paymentId);
        Task<GetPaymentNewDTO> GetPaymentByReference(string paymentReference);
        Task<List<GetPaymentNewDTO>> GetStudentPaymentHistory(Guid studentId);
        Task<Response> AddNoteToPayment(Guid paymentId, AddPaymentNoteDTO dto);
        Task<List<PaymentAuditDTO>> GetPaymentAudit(Guid paymentId);
        Task<Response> CreatePaymentAdjustment(CreatePaymentAdjustmentDTO dto);
    }

    public class PaymentNewService : BaseApiService, IPaymentNewService
    {
        public PaymentNewService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
            : base(httpClientFactory, configuration)
        {
        }

        public async Task<Response> RecordPayment(CreatePaymentNewDTO dto) =>
            await SendRequestAsync<Response>(
                $"{_baseUrl}/PaymentNew",
                HttpMethod.Post,
                dto);

        public async Task<Response> ConfirmPayment(Guid paymentId, ConfirmPaymentDTO dto) =>
            await SendRequestAsync<Response>(
                $"{_baseUrl}/PaymentNew/{paymentId}/confirm",
                HttpMethod.Put,
                dto);

        public async Task<GetPaymentNewDTO> GetPaymentById(Guid paymentId) =>
            await SendRequestAsync<GetPaymentNewDTO>(
                $"{_baseUrl}/PaymentNew/{paymentId}",
                HttpMethod.Get);

        public async Task<GetPaymentNewDTO> GetPaymentByReference(string paymentReference) =>
            await SendRequestAsync<GetPaymentNewDTO>(
                $"{_baseUrl}/PaymentNew/reference/{paymentReference}",
                HttpMethod.Get);

        public async Task<List<GetPaymentNewDTO>> GetStudentPaymentHistory(Guid studentId) =>
            await SendRequestAsync<List<GetPaymentNewDTO>>(
                $"{_baseUrl}/PaymentNew/student/{studentId}/history",
                HttpMethod.Get);

        public async Task<Response> AddNoteToPayment(Guid paymentId, AddPaymentNoteDTO dto) =>
            await SendRequestAsync<Response>(
                $"{_baseUrl}/PaymentNew/{paymentId}/add-note",
                HttpMethod.Put,
                dto);

        public async Task<List<PaymentAuditDTO>> GetPaymentAudit(Guid paymentId) =>
            await SendRequestAsync<List<PaymentAuditDTO>>(
                $"{_baseUrl}/PaymentNew/{paymentId}/audit",
                HttpMethod.Get);

        public async Task<Response> CreatePaymentAdjustment(CreatePaymentAdjustmentDTO dto) =>
            await SendRequestAsync<Response>(
                $"{_baseUrl}/PaymentNew/adjustment",
                HttpMethod.Post,
                dto);
    }
}

