using hongWenAPP.Models.AttendanceModel.DTOs;
using hongWenAPP.Models.Common;

namespace hongWenAPP.Services
{
    public interface IAttendanceService
    {
        Task<GetAttendanceDTO?> GetAttendance(Guid attendanceId);
        Task<List<GetAttendanceDTO>> GetAttendanceBySection(Guid sectionId, DateTime? startDate = null, DateTime? endDate = null);
        Task<List<GetAttendanceDTO>> GetAttendanceByStudent(Guid studentId, DateTime? startDate = null, DateTime? endDate = null);
        Task<List<GetAttendanceDTO>> GetAttendanceByDate(DateTime classDate);
        Task<Response> CreateAttendance(CreateAttendanceDTO attendanceDto);
        Task<Response> UpdateAttendance(UpdateAttendanceDTO attendanceDto);
        Task<Response> DeleteAttendance(Guid attendanceId);

        // Bulk Operations
        Task<Response> RecordBulkAttendance(BulkAttendanceDTO bulkAttendanceDto);
        Task<Response> UpdateBulkAttendance(BulkAttendanceDTO bulkAttendanceDto);

        // Reports
        Task<AttendanceReportDTO> GetAttendanceReport(Guid sectionId, DateTime? startDate = null, DateTime? endDate = null);
        Task<AttendanceSummaryDTO> GetAttendanceSummary(DateTime? date = null);
        Task<List<AttendanceAlertDTO>> GetAttendanceAlerts();
        Task<AttendanceStatisticsDTO> GetAttendanceStatistics(Guid sectionId, DateTime? startDate = null, DateTime? endDate = null);

        // Student-specific Operations
        Task<decimal> GetStudentAttendanceRate(Guid studentId, Guid sectionId);
        Task<List<AttendanceDetailDTO>> GetStudentAttendanceHistory(Guid studentId, DateTime? startDate = null, DateTime? endDate = null);
        Task<Response> ScheduleMakeupClass(Guid attendanceId, DateTime makeupDate, string? notes = null);

        // Validation and Business Rules
        Task<bool> ValidateAttendanceDate(DateTime classDate, Guid sectionId);
        Task<bool> CanModifyAttendance(Guid attendanceId);
        Task<bool> ValidateAttendanceStatus(string status);
        Task<Response> CheckConsecutiveAbsences(Guid studentId, Guid sectionId);

        // Dashboard and Analytics
        Task<List<SectionAttendanceDTO>> GetSectionAttendanceSummary(DateTime? date = null);
        Task<AttendanceFilterDTO> GetAttendanceFilterOptions();
        Task<Response> SendAttendanceAlerts();
    }

    public class AttendanceService : BaseApiService, IAttendanceService
    {
        public AttendanceService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
            : base(httpClientFactory, configuration)
        {
        }

        public async Task<GetAttendanceDTO?> GetAttendance(Guid attendanceId)
        {
            try
            {
                return await SendRequestAsync<GetAttendanceDTO>(
                    $"{_baseUrl}/Attendance/{attendanceId}",
                    HttpMethod.Get);
            }
            catch
            {
                return null;
            }
        }

        public async Task<List<GetAttendanceDTO>> GetAttendanceBySection(Guid sectionId, DateTime? startDate = null, DateTime? endDate = null)
        {
            var queryParams = new List<string>();
            if (startDate.HasValue) queryParams.Add($"startDate={startDate.Value:yyyy-MM-dd}");
            if (endDate.HasValue) queryParams.Add($"endDate={endDate.Value:yyyy-MM-dd}");

            var queryString = queryParams.Any() ? "?" + string.Join("&", queryParams) : "";
            return await SendRequestAsync<List<GetAttendanceDTO>>(
                $"{_baseUrl}/Attendance/section/{sectionId}{queryString}",
                HttpMethod.Get);
        }

        public async Task<List<GetAttendanceDTO>> GetAttendanceByStudent(Guid studentId, DateTime? startDate = null, DateTime? endDate = null)
        {
            var queryParams = new List<string>();
            if (startDate.HasValue) queryParams.Add($"startDate={startDate.Value:yyyy-MM-dd}");
            if (endDate.HasValue) queryParams.Add($"endDate={endDate.Value:yyyy-MM-dd}");

            var queryString = queryParams.Any() ? "?" + string.Join("&", queryParams) : "";
            return await SendRequestAsync<List<GetAttendanceDTO>>(
                $"{_baseUrl}/Attendance/student/{studentId}{queryString}",
                HttpMethod.Get);
        }

        public async Task<List<GetAttendanceDTO>> GetAttendanceByDate(DateTime classDate)
        {
            return await SendRequestAsync<List<GetAttendanceDTO>>(
                $"{_baseUrl}/Attendance/date/{classDate:yyyy-MM-dd}",
                HttpMethod.Get);
        }

        public async Task<Response> CreateAttendance(CreateAttendanceDTO attendanceDto)
        {
            return await SendRequestAsync<Response>(
                $"{_baseUrl}/Attendance",
                HttpMethod.Post,
                attendanceDto);
        }

        public async Task<Response> UpdateAttendance(UpdateAttendanceDTO attendanceDto)
        {
            return await SendRequestAsync<Response>(
                $"{_baseUrl}/Attendance",
                HttpMethod.Put,
                attendanceDto);
        }

        public async Task<Response> DeleteAttendance(Guid attendanceId)
        {
            return await SendRequestAsync<Response>(
                $"{_baseUrl}/Attendance/{attendanceId}",
                HttpMethod.Delete);
        }

        public async Task<Response> RecordBulkAttendance(BulkAttendanceDTO bulkAttendanceDto)
        {
            return await SendRequestAsync<Response>(
                $"{_baseUrl}/Attendance/bulk",
                HttpMethod.Post,
                bulkAttendanceDto);
        }

        public async Task<Response> UpdateBulkAttendance(BulkAttendanceDTO bulkAttendanceDto)
        {
            return await SendRequestAsync<Response>(
                $"{_baseUrl}/Attendance/bulk",
                HttpMethod.Put,
                bulkAttendanceDto);
        }

        public async Task<AttendanceReportDTO> GetAttendanceReport(Guid sectionId, DateTime? startDate = null, DateTime? endDate = null)
        {
            var queryParams = new List<string>();
            if (startDate.HasValue) queryParams.Add($"startDate={startDate.Value:yyyy-MM-dd}");
            if (endDate.HasValue) queryParams.Add($"endDate={endDate.Value:yyyy-MM-dd}");

            var queryString = queryParams.Any() ? "?" + string.Join("&", queryParams) : "";
            return await SendRequestAsync<AttendanceReportDTO>(
                $"{_baseUrl}/Attendance/reports/section/{sectionId}{queryString}",
                HttpMethod.Get);
        }

        public async Task<AttendanceSummaryDTO> GetAttendanceSummary(DateTime? date = null)
        {
            var queryString = date.HasValue ? $"?date={date.Value:yyyy-MM-dd}" : "";
            return await SendRequestAsync<AttendanceSummaryDTO>(
                $"{_baseUrl}/Attendance/summary{queryString}",
                HttpMethod.Get);
        }

        public async Task<List<AttendanceAlertDTO>> GetAttendanceAlerts()
        {
            return await SendRequestAsync<List<AttendanceAlertDTO>>(
                $"{_baseUrl}/Attendance/alerts",
                HttpMethod.Get);
        }

        public async Task<AttendanceStatisticsDTO> GetAttendanceStatistics(Guid sectionId, DateTime? startDate = null, DateTime? endDate = null)
        {
            var queryParams = new List<string>();
            if (startDate.HasValue) queryParams.Add($"startDate={startDate.Value:yyyy-MM-dd}");
            if (endDate.HasValue) queryParams.Add($"endDate={endDate.Value:yyyy-MM-dd}");

            var queryString = queryParams.Any() ? "?" + string.Join("&", queryParams) : "";
            return await SendRequestAsync<AttendanceStatisticsDTO>(
                $"{_baseUrl}/Attendance/statistics/section/{sectionId}{queryString}",
                HttpMethod.Get);
        }

        public async Task<decimal> GetStudentAttendanceRate(Guid studentId, Guid sectionId)
        {
            try
            {
                var result = await SendRequestAsync<dynamic>(
                    $"{_baseUrl}/Attendance/student/{studentId}/rate?sectionId={sectionId}",
                    HttpMethod.Get);
                return result?.AttendanceRate ?? 0;
            }
            catch
            {
                return 0;
            }
        }

        public async Task<List<AttendanceDetailDTO>> GetStudentAttendanceHistory(Guid studentId, DateTime? startDate = null, DateTime? endDate = null)
        {
            var queryParams = new List<string>();
            if (startDate.HasValue) queryParams.Add($"startDate={startDate.Value:yyyy-MM-dd}");
            if (endDate.HasValue) queryParams.Add($"endDate={endDate.Value:yyyy-MM-dd}");

            var queryString = queryParams.Any() ? "?" + string.Join("&", queryParams) : "";
            return await SendRequestAsync<List<AttendanceDetailDTO>>(
                $"{_baseUrl}/Attendance/student/{studentId}/history{queryString}",
                HttpMethod.Get);
        }

        public async Task<Response> ScheduleMakeupClass(Guid attendanceId, DateTime makeupDate, string? notes = null)
        {
            var request = new ScheduleMakeupRequest
            {
                MakeupDate = makeupDate,
                Notes = notes
            };
            return await SendRequestAsync<Response>(
                $"{_baseUrl}/Attendance/{attendanceId}/makeup",
                HttpMethod.Post,
                request);
        }

        public async Task<bool> ValidateAttendanceDate(DateTime classDate, Guid sectionId)
        {
            try
            {
                return await SendRequestAsync<bool>(
                    $"{_baseUrl}/Attendance/validate-date?classDate={classDate:yyyy-MM-dd}&sectionId={sectionId}",
                    HttpMethod.Get);
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> CanModifyAttendance(Guid attendanceId)
        {
            try
            {
                return await SendRequestAsync<bool>(
                    $"{_baseUrl}/Attendance/{attendanceId}/can-modify",
                    HttpMethod.Get);
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> ValidateAttendanceStatus(string status)
        {
            try
            {
                return await SendRequestAsync<bool>(
                    $"{_baseUrl}/Attendance/validate-status?status={status}",
                    HttpMethod.Get);
            }
            catch
            {
                return false;
            }
        }

        public async Task<Response> CheckConsecutiveAbsences(Guid studentId, Guid sectionId)
        {
            var request = new CheckAbsencesRequest
            {
                StudentId = studentId,
                SectionId = sectionId
            };
            return await SendRequestAsync<Response>(
                $"{_baseUrl}/Attendance/check-consecutive-absences",
                HttpMethod.Post,
                request);
        }

        public async Task<List<SectionAttendanceDTO>> GetSectionAttendanceSummary(DateTime? date = null)
        {
            var queryString = date.HasValue ? $"?date={date.Value:yyyy-MM-dd}" : "";
            return await SendRequestAsync<List<SectionAttendanceDTO>>(
                $"{_baseUrl}/Attendance/sections/summary{queryString}",
                HttpMethod.Get);
        }

        public async Task<AttendanceFilterDTO> GetAttendanceFilterOptions()
        {
            return await SendRequestAsync<AttendanceFilterDTO>(
                $"{_baseUrl}/Attendance/filter-options",
                HttpMethod.Get);
        }

        public async Task<Response> SendAttendanceAlerts()
        {
            return await SendRequestAsync<Response>(
                $"{_baseUrl}/Attendance/send-alerts",
                HttpMethod.Post);
        }
    }
}
