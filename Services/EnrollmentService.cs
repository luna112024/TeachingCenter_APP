using hongWenAPP.Models.Common;
using hongWenAPP.Models.EnrollmentModel.DTOs;

namespace hongWenAPP.Services
{
    public interface IEnrollmentService
    {
        Task<GetEnrollmentDTO> GetEnrollment(Guid enrollmentId);
        Task<List<GetEnrollmentDTO>> GetAllEnrollments(string? status = null, string? enrollmentType = null);
        Task<List<GetEnrollmentDTO>> GetEnrollmentsByStudent(Guid studentId);
        Task<List<GetEnrollmentDTO>> GetEnrollmentsBySection(Guid sectionId);
        Task<Response> CreateEnrollment(CreateEnrollmentDTO enrollmentDto);
        Task<Response> UpdateEnrollment(UpdateEnrollmentDTO enrollmentDto);
        Task<Response> DeleteEnrollment(Guid enrollmentId);
        Task<Response> TransferEnrollment(Guid enrollmentId, TransferEnrollmentDTO transferDto);
        Task<bool> ValidateSectionCapacity(Guid sectionId);
        Task<bool> ValidateStudentPrerequisites(Guid studentId, Guid courseId);
        Task<bool> ValidateEnrollmentDate(Guid sectionId, DateTime enrollmentDate);
        Task<bool> ValidateDuplicateEnrollment(Guid studentId, Guid sectionId);
    }

    public class EnrollmentService : BaseApiService, IEnrollmentService
    {
        public EnrollmentService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
            : base(httpClientFactory, configuration)
        {
        }

        public async Task<GetEnrollmentDTO> GetEnrollment(Guid enrollmentId) =>
            await SendRequestAsync<GetEnrollmentDTO>(
                $"{_baseUrl}/Enrollment/{enrollmentId}",
                HttpMethod.Get);

        public async Task<List<GetEnrollmentDTO>> GetAllEnrollments(string? status = null, string? enrollmentType = null)
        {
            var queryParams = new List<string>();
            if (!string.IsNullOrEmpty(status)) queryParams.Add($"status={status}");
            if (!string.IsNullOrEmpty(enrollmentType)) queryParams.Add($"enrollmentType={enrollmentType}");

            var queryString = queryParams.Any() ? "?" + string.Join("&", queryParams) : "";
            return await SendRequestAsync<List<GetEnrollmentDTO>>(
                $"{_baseUrl}/Enrollment{queryString}",
                HttpMethod.Get);
        }

        public async Task<List<GetEnrollmentDTO>> GetEnrollmentsByStudent(Guid studentId) =>
            await SendRequestAsync<List<GetEnrollmentDTO>>(
                $"{_baseUrl}/Enrollment/student/{studentId}",
                HttpMethod.Get);

        public async Task<List<GetEnrollmentDTO>> GetEnrollmentsBySection(Guid sectionId) =>
            await SendRequestAsync<List<GetEnrollmentDTO>>(
                $"{_baseUrl}/Enrollment/section/{sectionId}",
                HttpMethod.Get);

        public async Task<Response> CreateEnrollment(CreateEnrollmentDTO enrollmentDto) =>
            await SendRequestAsync<Response>(
                $"{_baseUrl}/Enrollment",
                HttpMethod.Post,
                enrollmentDto);

        public async Task<Response> UpdateEnrollment(UpdateEnrollmentDTO enrollmentDto) =>
            await SendRequestAsync<Response>(
                $"{_baseUrl}/Enrollment",
                HttpMethod.Put,
                enrollmentDto);

        public async Task<Response> DeleteEnrollment(Guid enrollmentId) =>
            await SendRequestAsync<Response>(
                $"{_baseUrl}/Enrollment/{enrollmentId}",
                HttpMethod.Delete);

        public async Task<Response> TransferEnrollment(Guid enrollmentId, TransferEnrollmentDTO transferDto) =>
            await SendRequestAsync<Response>(
                $"{_baseUrl}/Enrollment/{enrollmentId}/transfer",
                HttpMethod.Post,
                transferDto);

        public async Task<bool> ValidateSectionCapacity(Guid sectionId)
        {
            var result = await SendRequestAsync<object>(
                $"{_baseUrl}/Enrollment/validate/capacity/{sectionId}",
                HttpMethod.Get);
            return result != null;
        }

        public async Task<bool> ValidateStudentPrerequisites(Guid studentId, Guid courseId)
        {
            var result = await SendRequestAsync<object>(
                $"{_baseUrl}/Enrollment/validate/prerequisites/{studentId}/{courseId}",
                HttpMethod.Get);
            return result != null;
        }

        public async Task<bool> ValidateEnrollmentDate(Guid sectionId, DateTime enrollmentDate)
        {
            var result = await SendRequestAsync<object>(
                $"{_baseUrl}/Enrollment/validate/date/{sectionId}?enrollmentDate={enrollmentDate:yyyy-MM-dd}",
                HttpMethod.Get);
            return result != null;
        }

        public async Task<bool> ValidateDuplicateEnrollment(Guid studentId, Guid sectionId)
        {
            var result = await SendRequestAsync<object>(
                $"{_baseUrl}/Enrollment/validate/duplicate/{studentId}/{sectionId}",
                HttpMethod.Get);
            return result != null;
        }
    }
}
