using hongWenAPP.Models.GradeModel.DTOs;
using hongWenAPP.Models.Common;

namespace hongWenAPP.Services
{
    public interface IGradeService
    {
        Task<GetGradeDTO?> GetGrade(Guid gradeId);
        Task<List<GetGradeDTO>> GetGradesByAssessment(Guid assessmentId);
        Task<List<GetGradeDTO>> GetGradesByStudent(Guid studentId);
        Task<List<GetGradeDTO>> GetGradesBySection(Guid sectionId);
        Task<List<GetGradeDTO>> GetAllGrades(string? assessmentType = null, string? letterGrade = null);
        Task<List<StudentGradeDTO>> GetStudentGradeHistory(Guid studentId);
        Task<GradeReportDTO> GetGradeReport(Guid sectionId);
        Task<GradeStatisticsDTO> GetGradeStatistics(Guid sectionId);
        Task<Response> CreateGrade(CreateGradeDTO gradeDto);
        Task<Response> UpdateGrade(UpdateGradeDTO gradeDto);
        Task<Response> DeleteGrade(Guid gradeId);
        Task<Response> BulkGradeEntry(BulkGradeEntryDTO bulkGradeDto);
        Task<Response> CalculateFinalGrades(FinalGradeCalculationDTO calculationDto);
        Task<bool> ValidateGradeRange(decimal earnedScore, decimal maxScore);
        Task<bool> CanModifyGrade(Guid gradeId);
        Task<bool> ValidateAssessmentWeightTotal(Guid sectionId);
    }

    public class GradeService : BaseApiService, IGradeService
    {
        public GradeService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
            : base(httpClientFactory, configuration)
        {
        }

        public async Task<GetGradeDTO?> GetGrade(Guid gradeId) =>
            await SendRequestAsync<GetGradeDTO>(
                $"{_baseUrl}/Grade/{gradeId}",
                HttpMethod.Get);

        public async Task<List<GetGradeDTO>> GetGradesByAssessment(Guid assessmentId) =>
            await SendRequestAsync<List<GetGradeDTO>>(
                $"{_baseUrl}/Grade/assessment/{assessmentId}",
                HttpMethod.Get);

        public async Task<List<GetGradeDTO>> GetGradesByStudent(Guid studentId) =>
            await SendRequestAsync<List<GetGradeDTO>>(
                $"{_baseUrl}/Grade/student/{studentId}",
                HttpMethod.Get);

        public async Task<List<GetGradeDTO>> GetGradesBySection(Guid sectionId) =>
            await SendRequestAsync<List<GetGradeDTO>>(
                $"{_baseUrl}/Grade/section/{sectionId}",
                HttpMethod.Get);

        public async Task<List<GetGradeDTO>> GetAllGrades(string? assessmentType = null, string? letterGrade = null) =>
            await SendRequestAsync<List<GetGradeDTO>>(
                $"{_baseUrl}/Grade",
                HttpMethod.Get);

        public async Task<List<StudentGradeDTO>> GetStudentGradeHistory(Guid studentId) =>
            await SendRequestAsync<List<StudentGradeDTO>>(
                $"{_baseUrl}/Grade/student/{studentId}/history",
                HttpMethod.Get);

        public async Task<GradeReportDTO> GetGradeReport(Guid sectionId) =>
            await SendRequestAsync<GradeReportDTO>(
                $"{_baseUrl}/Grade/reports/section/{sectionId}",
                HttpMethod.Get);

        public async Task<GradeStatisticsDTO> GetGradeStatistics(Guid sectionId) =>
            await SendRequestAsync<GradeStatisticsDTO>(
                $"{_baseUrl}/Grade/statistics/section/{sectionId}",
                HttpMethod.Get);

        public async Task<Response> CreateGrade(CreateGradeDTO gradeDto) =>
            await SendRequestAsync<Response>(
                $"{_baseUrl}/Grade",
                HttpMethod.Post,
                gradeDto);

        public async Task<Response> UpdateGrade(UpdateGradeDTO gradeDto) =>
            await SendRequestAsync<Response>(
                $"{_baseUrl}/Grade",
                HttpMethod.Put,
                gradeDto);

        public async Task<Response> DeleteGrade(Guid gradeId) =>
            await SendRequestAsync<Response>(
                $"{_baseUrl}/Grade/{gradeId}",
                HttpMethod.Delete);

        public async Task<Response> BulkGradeEntry(BulkGradeEntryDTO bulkGradeDto) =>
            await SendRequestAsync<Response>(
                $"{_baseUrl}/Grade/bulk",
                HttpMethod.Post,
                bulkGradeDto);

        public async Task<Response> CalculateFinalGrades(FinalGradeCalculationDTO calculationDto) =>
            await SendRequestAsync<Response>(
                $"{_baseUrl}/Grade/calculate-final",
                HttpMethod.Post,
                calculationDto);

        public async Task<bool> ValidateGradeRange(decimal earnedScore, decimal maxScore)
        {
            // Client-side validation - check if earned score is within valid range
            return earnedScore >= 0 && earnedScore <= maxScore;
        }

        public async Task<bool> CanModifyGrade(Guid gradeId)
        {
            // For now, return true - in a real implementation, you might want to check
            // if the grade is within an editable time period
            return true;
        }

        public async Task<bool> ValidateAssessmentWeightTotal(Guid sectionId)
        {
            // For now, return true - in a real implementation, you might want to check
            // if assessment weights total 100% for the section
            return true;
        }
    }
}
