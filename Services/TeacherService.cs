using hongWenAPP.Models.Common;
using hongWenAPP.Models.TeacherModel.DTOs;

namespace hongWenAPP.Services
{
    public interface ITeacherService
    {
        Task<GetTeacherDTO> GetTeacher(Guid teacherId);
        Task<GetTeacherDTO> GetTeacherByUserId(Guid userId);
        Task<List<GetTeacherDTO>> GetAllTeachers(string? name = null);
        Task<List<GetTeacherDTO>> GetAvailableTeachers(DateTime? from = null, DateTime? to = null);
        Task<TeacherWorkloadDTO> GetTeacherWorkload(Guid teacherId);
        Task<List<TeacherScheduleItemDTO>> GetTeacherSchedule(Guid teacherId);
        Task<Response> CreateTeacher(CreateTeacherDTO teacherDto);
        Task<Response> UpdateTeacher(UpdateTeacherDTO teacherDto);
        Task<Response> DeleteTeacher(Guid teacherId);
    }

    public class TeacherService : BaseApiService, ITeacherService
    {
        public TeacherService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
            : base(httpClientFactory, configuration)
        {
        }

        public async Task<GetTeacherDTO> GetTeacher(Guid teacherId) =>
            await SendRequestAsync<GetTeacherDTO>(
                $"{_baseUrl}/Teacher/{teacherId}",
                HttpMethod.Get);

        public async Task<GetTeacherDTO> GetTeacherByUserId(Guid userId) =>
            await SendRequestAsync<GetTeacherDTO>(
                $"{_baseUrl}/Teacher/by-user/{userId}",
                HttpMethod.Get);

        public async Task<List<GetTeacherDTO>> GetAllTeachers(string? name = null)
        {
            var queryParams = new List<string>();
            if (!string.IsNullOrEmpty(name)) queryParams.Add($"name={name}");

            var queryString = queryParams.Any() ? "?" + string.Join("&", queryParams) : "";
            return await SendRequestAsync<List<GetTeacherDTO>>(
                $"{_baseUrl}/Teacher{queryString}",
                HttpMethod.Get);
        }

        public async Task<List<GetTeacherDTO>> GetAvailableTeachers(DateTime? from = null, DateTime? to = null)
        {
            var queryParams = new List<string>();
            if (from.HasValue) queryParams.Add($"from={from.Value:yyyy-MM-dd}");
            if (to.HasValue) queryParams.Add($"to={to.Value:yyyy-MM-dd}");

            var queryString = queryParams.Any() ? "?" + string.Join("&", queryParams) : "";
            return await SendRequestAsync<List<GetTeacherDTO>>(
                $"{_baseUrl}/Teacher/available{queryString}",
                HttpMethod.Get);
        }

        public async Task<TeacherWorkloadDTO> GetTeacherWorkload(Guid teacherId) =>
            await SendRequestAsync<TeacherWorkloadDTO>(
                $"{_baseUrl}/Teacher/{teacherId}/workload",
                HttpMethod.Get);

        public async Task<List<TeacherScheduleItemDTO>> GetTeacherSchedule(Guid teacherId) =>
            await SendRequestAsync<List<TeacherScheduleItemDTO>>(
                $"{_baseUrl}/Teacher/{teacherId}/schedule",
                HttpMethod.Get);

        public async Task<Response> CreateTeacher(CreateTeacherDTO teacherDto) =>
            await SendRequestAsync<Response>(
                $"{_baseUrl}/Teacher",
                HttpMethod.Post,
                teacherDto);

        public async Task<Response> UpdateTeacher(UpdateTeacherDTO teacherDto) =>
            await SendRequestAsync<Response>(
                $"{_baseUrl}/Teacher",
                HttpMethod.Put,
                teacherDto);

        public async Task<Response> DeleteTeacher(Guid teacherId) =>
            await SendRequestAsync<Response>(
                $"{_baseUrl}/Teacher/{teacherId}",
                HttpMethod.Delete);
    }
}
