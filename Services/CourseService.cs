using hongWenAPP.Models.Common;
using hongWenAPP.Models.CourseModel.DTOs;

namespace hongWenAPP.Services
{
    public interface ICourseService
    {
        Task<GetCourseDTO> GetCourse(Guid courseId);
        Task<List<GetCourseDTO>> GetAllCourses(string level = null, string ageGroup = null, string status = null, string name = null, string code = null);
        Task<List<GetCourseDTO>> GetCoursesByLevel(string level);
        Task<List<GetCourseDTO>> GetCoursesByLevelId(Guid levelId); // Add missing method
        Task<Response> CreateCourse(CreateCourseDTO courseDto);
        Task<Response> UpdateCourse(UpdateCourseDTO courseDto);
        Task<Response> DeleteCourse(Guid courseId);
        Task<Response> DuplicateCourse(Guid courseId, string newCourseCode, string? createdBy);
    }

    public class CourseService : BaseApiService, ICourseService
    {
        public CourseService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
            : base(httpClientFactory, configuration)
        {
        }

        public async Task<GetCourseDTO> GetCourse(Guid courseId) =>
            await SendRequestAsync<GetCourseDTO>(
                $"{_baseUrl}/Course/{courseId}",
                HttpMethod.Get);

        public async Task<List<GetCourseDTO>> GetAllCourses(string level = null, string ageGroup = null, string status = null, string name = null, string code = null)
        {
            var queryParams = new List<string>();
            if (!string.IsNullOrEmpty(level)) queryParams.Add($"level={level}");
            if (!string.IsNullOrEmpty(ageGroup)) queryParams.Add($"ageGroup={ageGroup}");
            if (!string.IsNullOrEmpty(status)) queryParams.Add($"status={status}");
            if (!string.IsNullOrEmpty(name)) queryParams.Add($"name={name}");
            if (!string.IsNullOrEmpty(code)) queryParams.Add($"code={code}");

            var queryString = queryParams.Any() ? "?" + string.Join("&", queryParams) : "";
            return await SendRequestAsync<List<GetCourseDTO>>(
                $"{_baseUrl}/Course{queryString}",
                HttpMethod.Get);
        }

        public async Task<List<GetCourseDTO>> GetCoursesByLevel(string level) =>
            await SendRequestAsync<List<GetCourseDTO>>(
                $"{_baseUrl}/Course/by-level/{level}", // Fix endpoint URL
                HttpMethod.Get);

        // Add missing method
        public async Task<List<GetCourseDTO>> GetCoursesByLevelId(Guid levelId) =>
            await SendRequestAsync<List<GetCourseDTO>>(
                $"{_baseUrl}/Course/by-level-id/{levelId}",
                HttpMethod.Get);

        public async Task<Response> CreateCourse(CreateCourseDTO courseDto) =>
            await SendRequestAsync<Response>(
                $"{_baseUrl}/Course",
                HttpMethod.Post,
                courseDto);

        public async Task<Response> UpdateCourse(UpdateCourseDTO courseDto) =>
            await SendRequestAsync<Response>(
                $"{_baseUrl}/Course",
                HttpMethod.Put,
                courseDto);

        public async Task<Response> DeleteCourse(Guid courseId) =>
            await SendRequestAsync<Response>(
                $"{_baseUrl}/Course/{courseId}",
                HttpMethod.Delete);

        public async Task<Response> DuplicateCourse(Guid courseId, string newCourseCode, string? createdBy) =>
            await SendRequestAsync<Response>(
                $"{_baseUrl}/Course/{courseId}/duplicate",
                HttpMethod.Post,
                new { newCourseCode, createdBy });
    }
}
