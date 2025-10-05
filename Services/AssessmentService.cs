using hongWenAPP.Models.AssessmentModel.DTOs;
using hongWenAPP.Models.Common;

namespace hongWenAPP.Services
{
    public interface IAssessmentService
    {
        Task<GetAssessmentDTO?> GetAssessment(Guid assessmentId);
        Task<List<GetAssessmentDTO>> GetAssessmentsBySection(Guid sectionId);
        Task<List<GetAssessmentDTO>> GetAllAssessments(string? assessmentType = null, string? status = null);
        Task<List<GetAssessmentDTO>> GetUpcomingAssessments(DateTime? fromDate = null, DateTime? toDate = null);
        Task<List<GetAssessmentDTO>> GetAssessmentsByDateRange(DateTime startDate, DateTime endDate);
        Task<Response> CreateAssessment(CreateAssessmentDTO assessmentDto);
        Task<Response> UpdateAssessment(UpdateAssessmentDTO assessmentDto);
        Task<Response> DeleteAssessment(Guid assessmentId);
        Task<Response> DuplicateAssessment(Guid assessmentId, DuplicateAssessmentDTO duplicateDto);
        Task<bool> ValidateAssessmentWeight(Guid sectionId, Guid? excludeAssessmentId = null);
        Task<bool> CanDeleteAssessment(Guid assessmentId);
        Task<bool> ValidateAssessmentDates(CreateAssessmentDTO assessmentDto);
    }

    public class AssessmentService : BaseApiService, IAssessmentService
    {
        public AssessmentService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
            : base(httpClientFactory, configuration)
        {
        }

        public async Task<GetAssessmentDTO?> GetAssessment(Guid assessmentId)
        {
            try
            {
                return await SendRequestAsync<GetAssessmentDTO>(
                    $"{_baseUrl}/Assessment/{assessmentId}",
                    HttpMethod.Get);
            }
            catch
            {
                return null;
            }
        }

        public async Task<List<GetAssessmentDTO>> GetAssessmentsBySection(Guid sectionId)
        {
            return await SendRequestAsync<List<GetAssessmentDTO>>(
                $"{_baseUrl}/Assessment/section/{sectionId}",
                HttpMethod.Get);
        }

        public async Task<List<GetAssessmentDTO>> GetAllAssessments(string? assessmentType = null, string? status = null)
        {
            var queryParams = new List<string>();
            if (!string.IsNullOrEmpty(assessmentType)) queryParams.Add($"assessmentType={assessmentType}");
            if (!string.IsNullOrEmpty(status)) queryParams.Add($"status={status}");

            var queryString = queryParams.Any() ? "?" + string.Join("&", queryParams) : "";
            return await SendRequestAsync<List<GetAssessmentDTO>>(
                $"{_baseUrl}/Assessment{queryString}",
                HttpMethod.Get);
        }

        public async Task<List<GetAssessmentDTO>> GetUpcomingAssessments(DateTime? fromDate = null, DateTime? toDate = null)
        {
            var queryParams = new List<string>();
            if (fromDate.HasValue) queryParams.Add($"fromDate={fromDate.Value:yyyy-MM-dd}");
            if (toDate.HasValue) queryParams.Add($"toDate={toDate.Value:yyyy-MM-dd}");

            var queryString = queryParams.Any() ? "?" + string.Join("&", queryParams) : "";
            return await SendRequestAsync<List<GetAssessmentDTO>>(
                $"{_baseUrl}/Assessment/upcoming{queryString}",
                HttpMethod.Get);
        }

        public async Task<List<GetAssessmentDTO>> GetAssessmentsByDateRange(DateTime startDate, DateTime endDate)
        {
            return await SendRequestAsync<List<GetAssessmentDTO>>(
                $"{_baseUrl}/Assessment/date-range?startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}",
                HttpMethod.Get);
        }

        public async Task<Response> CreateAssessment(CreateAssessmentDTO assessmentDto)
        {
            return await SendRequestAsync<Response>(
                $"{_baseUrl}/Assessment",
                HttpMethod.Post,
                assessmentDto);
        }

        public async Task<Response> UpdateAssessment(UpdateAssessmentDTO assessmentDto)
        {
            return await SendRequestAsync<Response>(
                $"{_baseUrl}/Assessment",
                HttpMethod.Put,
                assessmentDto);
        }

        public async Task<Response> DeleteAssessment(Guid assessmentId)
        {
            return await SendRequestAsync<Response>(
                $"{_baseUrl}/Assessment/{assessmentId}",
                HttpMethod.Delete);
        }

        public async Task<Response> DuplicateAssessment(Guid assessmentId, DuplicateAssessmentDTO duplicateDto)
        {
            return await SendRequestAsync<Response>(
                $"{_baseUrl}/Assessment/{assessmentId}/duplicate",
                HttpMethod.Post,
                duplicateDto);
        }

        public async Task<bool> ValidateAssessmentWeight(Guid sectionId, Guid? excludeAssessmentId = null)
        {
            try
            {
                var queryParams = new List<string>();
                if (excludeAssessmentId.HasValue) queryParams.Add($"excludeAssessmentId={excludeAssessmentId.Value}");

                var queryString = queryParams.Any() ? "?" + string.Join("&", queryParams) : "";
                return await SendRequestAsync<bool>(
                    $"{_baseUrl}/Assessment/validate-weight/{sectionId}{queryString}",
                    HttpMethod.Get);
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> CanDeleteAssessment(Guid assessmentId)
        {
            try
            {
                return await SendRequestAsync<bool>(
                    $"{_baseUrl}/Assessment/{assessmentId}/can-delete",
                    HttpMethod.Get);
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> ValidateAssessmentDates(CreateAssessmentDTO assessmentDto)
        {
            try
            {
                return await SendRequestAsync<bool>(
                    $"{_baseUrl}/Assessment/validate-dates",
                    HttpMethod.Post,
                    assessmentDto);
            }
            catch
            {
                return false;
            }
        }
    }
}
