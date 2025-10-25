using hongWenAPP.Models.Common;
using hongWenAPP.Models.StudentCourseModel.DTOs;

namespace hongWenAPP.Services
{
    public interface IStudentCourseService
    {
        Task<Response> AssignStudentToCourse(AssignStudentToCourseDTO dto);
        Task<List<GetStudentCourseDTO>> GetStudentCourseHistory(Guid studentId);
        Task<GetStudentCourseDTO> GetCurrentCourseAssignment(Guid studentId);
        Task<List<GetStudentCourseDTO>> GetStudentsInCourse(Guid courseId, Guid termId);
    }

    public class StudentCourseService : BaseApiService, IStudentCourseService
    {
        public StudentCourseService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
            : base(httpClientFactory, configuration)
        {
        }

        public async Task<Response> AssignStudentToCourse(AssignStudentToCourseDTO dto) =>
            await SendRequestAsync<Response>(
                $"{_baseUrl}/StudentCourse/assign",
                HttpMethod.Post,
                dto);

        public async Task<List<GetStudentCourseDTO>> GetStudentCourseHistory(Guid studentId) =>
            await SendRequestAsync<List<GetStudentCourseDTO>>(
                $"{_baseUrl}/StudentCourse/student/{studentId}",
                HttpMethod.Get);

        public async Task<GetStudentCourseDTO> GetCurrentCourseAssignment(Guid studentId) =>
            await SendRequestAsync<GetStudentCourseDTO>(
                $"{_baseUrl}/StudentCourse/student/{studentId}/current",
                HttpMethod.Get);

        public async Task<List<GetStudentCourseDTO>> GetStudentsInCourse(Guid courseId, Guid termId) =>
            await SendRequestAsync<List<GetStudentCourseDTO>>(
                $"{_baseUrl}/StudentCourse/course/{courseId}?termId={termId}",
                HttpMethod.Get);
    }
}

