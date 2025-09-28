using hongWenAPP.Models.Common;
using hongWenAPP.Models.ClassroomModel.DTOs;

namespace hongWenAPP.Services
{
    public interface IClassroomService
    {
        Task<GetClassroomDTO> GetClassroom(Guid classroomId);
        Task<List<GetClassroomDTO>> GetAllClassrooms(string? status = null, int? minCapacity = null, string? name = null);
        Task<List<GetClassroomDTO>> GetAvailableClassrooms(string? name = null, DateTime? startDate = null, DateTime? endDate = null);
        Task<List<object>> GetClassroomSchedule(Guid classroomId);
        Task<Response> CreateClassroom(CreateClassroomDTO classroomDto);
        Task<Response> UpdateClassroom(UpdateClassroomDTO classroomDto);
        Task<Response> DeleteClassroom(Guid classroomId);
    }

    public class ClassroomService : BaseApiService, IClassroomService
    {
        public ClassroomService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
            : base(httpClientFactory, configuration)
        {
        }

        public async Task<GetClassroomDTO> GetClassroom(Guid classroomId) =>
            await SendRequestAsync<GetClassroomDTO>(
                $"{_baseUrl}/Classroom/{classroomId}",
                HttpMethod.Get);

        public async Task<List<GetClassroomDTO>> GetAllClassrooms(string? status = null, int? minCapacity = null, string? name = null)
        {
            var queryParams = new List<string>();
            if (!string.IsNullOrEmpty(status)) queryParams.Add($"status={status}");
            if (minCapacity.HasValue) queryParams.Add($"minCapacity={minCapacity.Value}");
            if (!string.IsNullOrEmpty(name)) queryParams.Add($"name={name}");

            var queryString = queryParams.Any() ? "?" + string.Join("&", queryParams) : "";
            return await SendRequestAsync<List<GetClassroomDTO>>(
                $"{_baseUrl}/Classroom{queryString}",
                HttpMethod.Get);
        }

        public async Task<List<GetClassroomDTO>> GetAvailableClassrooms(string? name = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            var queryParams = new List<string>();
            if (!string.IsNullOrEmpty(name)) queryParams.Add($"name={name}");
            if (startDate.HasValue) queryParams.Add($"startDate={startDate.Value:yyyy-MM-dd}");
            if (endDate.HasValue) queryParams.Add($"endDate={endDate.Value:yyyy-MM-dd}");

            var queryString = queryParams.Any() ? "?" + string.Join("&", queryParams) : "";
            return await SendRequestAsync<List<GetClassroomDTO>>(
                $"{_baseUrl}/Classroom/available{queryString}",
                HttpMethod.Get);
        }

        public async Task<List<object>> GetClassroomSchedule(Guid classroomId) =>
            await SendRequestAsync<List<object>>(
                $"{_baseUrl}/Classroom/{classroomId}/schedule",
                HttpMethod.Get);

        public async Task<Response> CreateClassroom(CreateClassroomDTO classroomDto) =>
            await SendRequestAsync<Response>(
                $"{_baseUrl}/Classroom",
                HttpMethod.Post,
                classroomDto);

        public async Task<Response> UpdateClassroom(UpdateClassroomDTO classroomDto) =>
            await SendRequestAsync<Response>(
                $"{_baseUrl}/Classroom",
                HttpMethod.Put,
                classroomDto);

        public async Task<Response> DeleteClassroom(Guid classroomId) =>
            await SendRequestAsync<Response>(
                $"{_baseUrl}/Classroom/{classroomId}",
                HttpMethod.Delete);
    }
}
