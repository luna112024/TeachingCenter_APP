using hongWenAPP.Models.Common;
using hongWenAPP.Models.StudentCourseModel.DTOs;

namespace hongWenAPP.Services
{
    public interface IPromotionService
    {
        Task<Response> PromoteStudent(PromoteStudentDTO dto);
        Task<BulkPromotionResultDTO> BulkPromoteStudents(BulkPromoteDTO dto);
        Task<PromotionPreviewDTO> PreviewPromotion(PreviewPromotionDTO dto);
        Task<List<GetStudentCourseDTO>> GetPromotionHistory(Guid studentId);
    }

    public class PromotionService : BaseApiService, IPromotionService
    {
        public PromotionService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
            : base(httpClientFactory, configuration)
        {
        }

        public async Task<Response> PromoteStudent(PromoteStudentDTO dto) =>
            await SendRequestAsync<Response>(
                $"{_baseUrl}/Promotion/promote",
                HttpMethod.Post,
                dto);

        public async Task<BulkPromotionResultDTO> BulkPromoteStudents(BulkPromoteDTO dto) =>
            await SendRequestAsync<BulkPromotionResultDTO>(
                $"{_baseUrl}/Promotion/bulk-promote",
                HttpMethod.Post,
                dto);

        public async Task<PromotionPreviewDTO> PreviewPromotion(PreviewPromotionDTO dto) =>
            await SendRequestAsync<PromotionPreviewDTO>(
                $"{_baseUrl}/Promotion/preview",
                HttpMethod.Post,
                dto);

        public async Task<List<GetStudentCourseDTO>> GetPromotionHistory(Guid studentId) =>
            await SendRequestAsync<List<GetStudentCourseDTO>>(
                $"{_baseUrl}/Promotion/student/{studentId}/history",
                HttpMethod.Get);
    }
}

