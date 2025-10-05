using hongWenAPP.Models.Common;
using hongWenAPP.Models.WaitlistModel.DTOs;

namespace hongWenAPP.Services
{
    public interface IWaitlistService
    {
        Task<GetWaitlistDTO> GetWaitlist(Guid waitlistId);
        Task<List<GetWaitlistDTO>> GetAllWaitlists(string? status = null);
        Task<List<GetWaitlistDTO>> GetWaitlistBySection(Guid sectionId);
        Task<List<GetWaitlistDTO>> GetWaitlistByStudent(Guid studentId);
        Task<Response> CreateWaitlist(CreateWaitlistDTO waitlistDto);
        Task<Response> UpdateWaitlist(UpdateWaitlistDTO waitlistDto);
        Task<Response> DeleteWaitlist(Guid waitlistId);
        Task<Response> PromoteFromWaitlist(Guid waitlistId, PromoteFromWaitlistDTO promoteDto);
        Task<Response> ReorderWaitlist(ReorderWaitlistDTO reorderDto);
        Task<bool> ValidateDuplicateWaitlist(Guid studentId, Guid sectionId);
        Task<bool> ValidateSectionCapacity(Guid sectionId);
        Task<bool> ValidateStudentExists(Guid studentId);
        Task<bool> ValidateSectionExists(Guid sectionId);
    }

    public class WaitlistService : BaseApiService, IWaitlistService
    {
        public WaitlistService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
            : base(httpClientFactory, configuration)
        {
        }

        public async Task<GetWaitlistDTO> GetWaitlist(Guid waitlistId) =>
            await SendRequestAsync<GetWaitlistDTO>(
                $"{_baseUrl}/Waitlist/{waitlistId}",
                HttpMethod.Get);

        public async Task<List<GetWaitlistDTO>> GetAllWaitlists(string? status = null)
        {
            var queryString = !string.IsNullOrEmpty(status) ? $"?status={status}" : "";
            return await SendRequestAsync<List<GetWaitlistDTO>>(
                $"{_baseUrl}/Waitlist{queryString}",
                HttpMethod.Get);
        }

        public async Task<List<GetWaitlistDTO>> GetWaitlistBySection(Guid sectionId) =>
            await SendRequestAsync<List<GetWaitlistDTO>>(
                $"{_baseUrl}/Waitlist/section/{sectionId}",
                HttpMethod.Get);

        public async Task<List<GetWaitlistDTO>> GetWaitlistByStudent(Guid studentId) =>
            await SendRequestAsync<List<GetWaitlistDTO>>(
                $"{_baseUrl}/Waitlist/student/{studentId}",
                HttpMethod.Get);

        public async Task<Response> CreateWaitlist(CreateWaitlistDTO waitlistDto) =>
            await SendRequestAsync<Response>(
                $"{_baseUrl}/Waitlist",
                HttpMethod.Post,
                waitlistDto);

        public async Task<Response> UpdateWaitlist(UpdateWaitlistDTO waitlistDto) =>
            await SendRequestAsync<Response>(
                $"{_baseUrl}/Waitlist",
                HttpMethod.Put,
                waitlistDto);

        public async Task<Response> DeleteWaitlist(Guid waitlistId) =>
            await SendRequestAsync<Response>(
                $"{_baseUrl}/Waitlist/{waitlistId}",
                HttpMethod.Delete);

        public async Task<Response> PromoteFromWaitlist(Guid waitlistId, PromoteFromWaitlistDTO promoteDto) =>
            await SendRequestAsync<Response>(
                $"{_baseUrl}/Waitlist/{waitlistId}/promote",
                HttpMethod.Post,
                promoteDto);

        public async Task<Response> ReorderWaitlist(ReorderWaitlistDTO reorderDto) =>
            await SendRequestAsync<Response>(
                $"{_baseUrl}/Waitlist/reorder",
                HttpMethod.Put,
                reorderDto);

        public async Task<bool> ValidateDuplicateWaitlist(Guid studentId, Guid sectionId)
        {
            var result = await SendRequestAsync<object>(
                $"{_baseUrl}/Waitlist/validate/duplicate/{studentId}/{sectionId}",
                HttpMethod.Get);
            return result != null;
        }

        public async Task<bool> ValidateSectionCapacity(Guid sectionId)
        {
            var result = await SendRequestAsync<object>(
                $"{_baseUrl}/Waitlist/validate/capacity/{sectionId}",
                HttpMethod.Get);
            return result != null;
        }

        public async Task<bool> ValidateStudentExists(Guid studentId)
        {
            var result = await SendRequestAsync<object>(
                $"{_baseUrl}/Waitlist/validate/student/{studentId}",
                HttpMethod.Get);
            return result != null;
        }

        public async Task<bool> ValidateSectionExists(Guid sectionId)
        {
            var result = await SendRequestAsync<object>(
                $"{_baseUrl}/Waitlist/validate/section/{sectionId}",
                HttpMethod.Get);
            return result != null;
        }
    }
}
