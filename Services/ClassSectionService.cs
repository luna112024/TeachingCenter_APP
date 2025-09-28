using hongWenAPP.Models.Common;
using hongWenAPP.Models.ClassSectionModel.DTOs;

namespace hongWenAPP.Services
{
    public interface IClassSectionService
    {
        Task<GetClassSectionDTO> GetClassSection(Guid sectionId);
        Task<List<GetClassSectionDTO>> GetAllClassSections(
            string? sectionCode = null, 
            string? sectionName = null,
            string? status = null,
            Guid? courseId = null,
            Guid? termId = null,
            Guid? teacherId = null);
        Task<Response> CreateClassSection(CreateClassSectionDTO sectionDto);
        Task<Response> UpdateClassSection(UpdateClassSectionDTO sectionDto);
        Task<Response> DeleteClassSection(Guid sectionId);
        Task<Response> DuplicateClassSection(Guid sectionId, DuplicateClassSectionDTO duplicateDto);
        Task<Response> UpdateSectionStatus(Guid sectionId, string status);
        Task<bool> ValidateScheduleConflicts(Guid teacherId, Guid classroomId, string schedulePattern, DateTime startDate, DateTime endDate, Guid? excludeSectionId = null);
    }

    public class ClassSectionService : BaseApiService, IClassSectionService
    {
        public ClassSectionService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
            : base(httpClientFactory, configuration)
        {
        }

        public async Task<GetClassSectionDTO> GetClassSection(Guid sectionId) =>
            await SendRequestAsync<GetClassSectionDTO>(
                $"{_baseUrl}/sections/{sectionId}",
                HttpMethod.Get);

        public async Task<List<GetClassSectionDTO>> GetAllClassSections(
            string? sectionCode = null,
            string? sectionName = null,
            string? status = null,
            Guid? courseId = null,
            Guid? termId = null,
            Guid? teacherId = null)
        {
            var queryParams = new List<string>();
            if (!string.IsNullOrEmpty(sectionCode)) queryParams.Add($"sectionCode={sectionCode}");
            if (!string.IsNullOrEmpty(sectionName)) queryParams.Add($"sectionName={sectionName}");
            if (!string.IsNullOrEmpty(status)) queryParams.Add($"status={status}");
            if (courseId.HasValue) queryParams.Add($"courseId={courseId.Value}");
            if (termId.HasValue) queryParams.Add($"termId={termId.Value}");
            if (teacherId.HasValue) queryParams.Add($"teacherId={teacherId.Value}");

            var queryString = queryParams.Any() ? "?" + string.Join("&", queryParams) : "";
            return await SendRequestAsync<List<GetClassSectionDTO>>(
                $"{_baseUrl}/sections{queryString}",
                HttpMethod.Get);
        }

        public async Task<Response> CreateClassSection(CreateClassSectionDTO sectionDto) =>
            await SendRequestAsync<Response>(
                $"{_baseUrl}/sections",
                HttpMethod.Post,
                sectionDto);

        public async Task<Response> UpdateClassSection(UpdateClassSectionDTO sectionDto) =>
            await SendRequestAsync<Response>(
                $"{_baseUrl}/sections",
                HttpMethod.Put,
                sectionDto);

        public async Task<Response> DeleteClassSection(Guid sectionId) =>
            await SendRequestAsync<Response>(
                $"{_baseUrl}/sections/{sectionId}",
                HttpMethod.Delete);

        public async Task<Response> DuplicateClassSection(Guid sectionId, DuplicateClassSectionDTO duplicateDto) =>
            await SendRequestAsync<Response>(
                $"{_baseUrl}/sections/{sectionId}/duplicate",
                HttpMethod.Post,
                duplicateDto);

        public async Task<Response> UpdateSectionStatus(Guid sectionId, string status)
        {
            var dto = new UpdateSectionStatusDTO { Status = status };
            return await SendRequestAsync<Response>(
                $"{_baseUrl}/sections/{sectionId}/status",
                HttpMethod.Put,
                dto);
        }

        public async Task<bool> ValidateScheduleConflicts(Guid teacherId, Guid classroomId, string schedulePattern, DateTime startDate, DateTime endDate, Guid? excludeSectionId = null)
        {
            var queryParams = new List<string>
            {
                $"teacherId={teacherId}",
                $"classroomId={classroomId}",
                $"schedulePattern={schedulePattern}",
                $"startDate={startDate:yyyy-MM-dd}",
                $"endDate={endDate:yyyy-MM-dd}"
            };
            
            if (excludeSectionId.HasValue)
                queryParams.Add($"excludeSectionId={excludeSectionId.Value}");

            var queryString = "?" + string.Join("&", queryParams);
            return await SendRequestAsync<bool>(
                $"{_baseUrl}/sections/validate-conflicts{queryString}",
                HttpMethod.Get);
        }
    }
}
