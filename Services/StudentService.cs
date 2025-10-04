using hongWenAPP.Models.Common;
using hongWenAPP.Models.StudentModel.DTOs;

namespace hongWenAPP.Services
{
    public interface IStudentService
    {
        Task<GetStudentDTO> GetStudent(Guid studentId);
        Task<GetStudentDTO> GetStudentByUserId(Guid userId);
        Task<List<GetStudentDTO>> GetAllStudents(string? search = null, string? status = null);
        Task<List<GetStudentDTO>> GetStudentsByStatus(string status);
        Task<object> GetAcademicHistory(Guid studentId);
        Task<object> GetPaymentHistory(Guid studentId);
        Task<Response> CreateStudent(CreateStudentDTO studentDto);
        Task<Response> UpdateStudent(UpdateStudentDTO studentDto);
        Task<Response> DeleteStudent(Guid studentId);
        Task<Response> UpdateEmergencyContact(Guid studentId, string name, string phone, string relationship);
    }

    public class StudentService : BaseApiService, IStudentService
    {
        public StudentService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
            : base(httpClientFactory, configuration)
        {
        }

        public async Task<GetStudentDTO> GetStudent(Guid studentId) =>
            await SendRequestAsync<GetStudentDTO>(
                $"{_baseUrl}/Student/{studentId}",
                HttpMethod.Get);

        public async Task<GetStudentDTO> GetStudentByUserId(Guid userId) =>
            await SendRequestAsync<GetStudentDTO>(
                $"{_baseUrl}/Student/user/{userId}",
                HttpMethod.Get);

        public async Task<List<GetStudentDTO>> GetAllStudents(string? search = null, string? status = null)
        {
            var queryParams = new List<string>();
            if (!string.IsNullOrEmpty(search)) queryParams.Add($"search={search}");
            if (!string.IsNullOrEmpty(status)) queryParams.Add($"status={status}");

            var queryString = queryParams.Any() ? "?" + string.Join("&", queryParams) : "";
            return await SendRequestAsync<List<GetStudentDTO>>(
                $"{_baseUrl}/Student{queryString}",
                HttpMethod.Get);
        }

        public async Task<List<GetStudentDTO>> GetStudentsByStatus(string status) =>
            await SendRequestAsync<List<GetStudentDTO>>(
                $"{_baseUrl}/Student/status/{status}",
                HttpMethod.Get);

        public async Task<object> GetAcademicHistory(Guid studentId) =>
            await SendRequestAsync<object>(
                $"{_baseUrl}/Student/{studentId}/academic-history",
                HttpMethod.Get);

        public async Task<object> GetPaymentHistory(Guid studentId) =>
            await SendRequestAsync<object>(
                $"{_baseUrl}/Student/{studentId}/payment-history",
                HttpMethod.Get);

        public async Task<Response> CreateStudent(CreateStudentDTO studentDto) =>
            await SendRequestAsync<Response>(
                $"{_baseUrl}/Student",
                HttpMethod.Post,
                studentDto);

        public async Task<Response> UpdateStudent(UpdateStudentDTO studentDto) =>
            await SendRequestAsync<Response>(
                $"{_baseUrl}/Student",
                HttpMethod.Put,
                studentDto);

        public async Task<Response> DeleteStudent(Guid studentId) =>
            await SendRequestAsync<Response>(
                $"{_baseUrl}/Student/{studentId}",
                HttpMethod.Delete);

        public async Task<Response> UpdateEmergencyContact(Guid studentId, string name, string phone, string relationship)
        {
            var data = new { name, phone, relationship };
            return await SendRequestAsync<Response>(
                $"{_baseUrl}/Student/{studentId}/emergency-contact",
                HttpMethod.Put,
                data);
        }
    }
}
