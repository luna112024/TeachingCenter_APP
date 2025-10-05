using hongWenAPP.Models.Common;
using hongWenAPP.Models.FeeModel.DTOs;

namespace hongWenAPP.Services
{
    public interface IFeeService
    {
        // Fee Categories
        Task<List<GetFeeCategoryDTO>> GetAllFeeCategories();
        Task<GetFeeCategoryDTO> GetFeeCategory(Guid categoryId);
        Task<Response> CreateFeeCategory(CreateFeeCategoryDTO categoryDto);
        Task<Response> UpdateFeeCategory(UpdateFeeCategoryDTO categoryDto);
        Task<Response> DeleteFeeCategory(Guid categoryId);

        // Fee Templates
        Task<List<GetFeeTemplateDTO>> GetAllFeeTemplates();
        Task<GetFeeTemplateDTO> GetFeeTemplate(Guid templateId);
        Task<List<GetFeeTemplateDTO>> GetFeeTemplatesByCategory(Guid categoryId);
        Task<List<GetFeeTemplateDTO>> GetFeeTemplatesByType(string feeType);
        Task<Response> CreateFeeTemplate(CreateFeeTemplateDTO templateDto);
        Task<Response> UpdateFeeTemplate(UpdateFeeTemplateDTO templateDto);
        Task<Response> DeleteFeeTemplate(Guid templateId);

        // Student Fee Assignments
        Task<List<GetStudentFeeDTO>> GetStudentFees(Guid studentId);
        Task<GetStudentFeeDTO> GetStudentFee(Guid studentFeeId);
        Task<List<GetStudentFeeDTO>> GetStudentFeesByEnrollment(Guid enrollmentId);
        Task<Response> AssignFeeToStudent(AssignFeeToStudentDTO assignmentDto);
        Task<Response> UpdateStudentFee(UpdateStudentFeeDTO studentFeeDto);
        Task<Response> WaiveStudentFee(WaiveStudentFeeDTO waiveDto);
        Task<Response> DeleteStudentFee(Guid studentFeeId);

        // Business Operations
        Task<List<GetStudentFeeDTO>> GetOverdueFees(DateTime? fromDate = null, DateTime? toDate = null);
        Task<List<GetStudentFeeDTO>> GetPendingFees(Guid? studentId = null);
        Task<Response> CalculateLateFees();
        Task<Response> GenerateInvoices(Guid? studentId = null, DateTime? dueDate = null);

        // Validation
        Task<bool> ValidateFeeTemplate(Guid templateId);
        Task<bool> CanDeleteFeeCategory(Guid categoryId);
        Task<bool> CanDeleteFeeTemplate(Guid templateId);
    }

    public class FeeService : BaseApiService, IFeeService
    {
        public FeeService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
            : base(httpClientFactory, configuration)
        {
        }

        // Fee Categories
        public async Task<List<GetFeeCategoryDTO>> GetAllFeeCategories() =>
            await SendRequestAsync<List<GetFeeCategoryDTO>>(
                $"{_baseUrl}/Fee/categories",
                HttpMethod.Get);

        public async Task<GetFeeCategoryDTO> GetFeeCategory(Guid categoryId) =>
            await SendRequestAsync<GetFeeCategoryDTO>(
                $"{_baseUrl}/Fee/categories/{categoryId}",
                HttpMethod.Get);

        public async Task<Response> CreateFeeCategory(CreateFeeCategoryDTO categoryDto) =>
            await SendRequestAsync<Response>(
                $"{_baseUrl}/Fee/categories",
                HttpMethod.Post,
                categoryDto);

        public async Task<Response> UpdateFeeCategory(UpdateFeeCategoryDTO categoryDto) =>
            await SendRequestAsync<Response>(
                $"{_baseUrl}/Fee/categories",
                HttpMethod.Put,
                categoryDto);

        public async Task<Response> DeleteFeeCategory(Guid categoryId) =>
            await SendRequestAsync<Response>(
                $"{_baseUrl}/Fee/categories/{categoryId}",
                HttpMethod.Delete);

        // Fee Templates
        public async Task<List<GetFeeTemplateDTO>> GetAllFeeTemplates() =>
            await SendRequestAsync<List<GetFeeTemplateDTO>>(
                $"{_baseUrl}/Fee/templates",
                HttpMethod.Get);

        public async Task<GetFeeTemplateDTO> GetFeeTemplate(Guid templateId) =>
            await SendRequestAsync<GetFeeTemplateDTO>(
                $"{_baseUrl}/Fee/templates/{templateId}",
                HttpMethod.Get);

        public async Task<List<GetFeeTemplateDTO>> GetFeeTemplatesByCategory(Guid categoryId) =>
            await SendRequestAsync<List<GetFeeTemplateDTO>>(
                $"{_baseUrl}/Fee/templates/category/{categoryId}",
                HttpMethod.Get);

        public async Task<List<GetFeeTemplateDTO>> GetFeeTemplatesByType(string feeType) =>
            await SendRequestAsync<List<GetFeeTemplateDTO>>(
                $"{_baseUrl}/Fee/templates/type/{feeType}",
                HttpMethod.Get);

        public async Task<Response> CreateFeeTemplate(CreateFeeTemplateDTO templateDto) =>
            await SendRequestAsync<Response>(
                $"{_baseUrl}/Fee/templates",
                HttpMethod.Post,
                templateDto);

        public async Task<Response> UpdateFeeTemplate(UpdateFeeTemplateDTO templateDto) =>
            await SendRequestAsync<Response>(
                $"{_baseUrl}/Fee/templates",
                HttpMethod.Put,
                templateDto);

        public async Task<Response> DeleteFeeTemplate(Guid templateId) =>
            await SendRequestAsync<Response>(
                $"{_baseUrl}/Fee/templates/{templateId}",
                HttpMethod.Delete);

        // Student Fee Assignments
        public async Task<List<GetStudentFeeDTO>> GetStudentFees(Guid studentId) =>
            await SendRequestAsync<List<GetStudentFeeDTO>>(
                $"{_baseUrl}/Fee/student-fees/{studentId}",
                HttpMethod.Get);

        public async Task<GetStudentFeeDTO> GetStudentFee(Guid studentFeeId) =>
            await SendRequestAsync<GetStudentFeeDTO>(
                $"{_baseUrl}/Fee/student-fees/{studentFeeId}/detail",
                HttpMethod.Get);

        public async Task<List<GetStudentFeeDTO>> GetStudentFeesByEnrollment(Guid enrollmentId) =>
            await SendRequestAsync<List<GetStudentFeeDTO>>(
                $"{_baseUrl}/Fee/student-fees/enrollment/{enrollmentId}",
                HttpMethod.Get);

        public async Task<Response> AssignFeeToStudent(AssignFeeToStudentDTO assignmentDto) =>
            await SendRequestAsync<Response>(
                $"{_baseUrl}/Fee/student-fees/assign",
                HttpMethod.Post,
                assignmentDto);

        public async Task<Response> UpdateStudentFee(UpdateStudentFeeDTO studentFeeDto) =>
            await SendRequestAsync<Response>(
                $"{_baseUrl}/Fee/student-fees",
                HttpMethod.Put,
                studentFeeDto);

        public async Task<Response> WaiveStudentFee(WaiveStudentFeeDTO waiveDto) =>
            await SendRequestAsync<Response>(
                $"{_baseUrl}/Fee/student-fees/{waiveDto.StudentFeeId}/waive",
                HttpMethod.Put,
                waiveDto);

        public async Task<Response> DeleteStudentFee(Guid studentFeeId) =>
            await SendRequestAsync<Response>(
                $"{_baseUrl}/Fee/student-fees/{studentFeeId}",
                HttpMethod.Delete);

        // Business Operations
        public async Task<List<GetStudentFeeDTO>> GetOverdueFees(DateTime? fromDate = null, DateTime? toDate = null)
        {
            var queryParams = new List<string>();
            if (fromDate.HasValue) queryParams.Add($"fromDate={fromDate.Value:yyyy-MM-dd}");
            if (toDate.HasValue) queryParams.Add($"toDate={toDate.Value:yyyy-MM-dd}");

            var queryString = queryParams.Any() ? "?" + string.Join("&", queryParams) : "";
            return await SendRequestAsync<List<GetStudentFeeDTO>>(
                $"{_baseUrl}/Fee/overdue{queryString}",
                HttpMethod.Get);
        }

        public async Task<List<GetStudentFeeDTO>> GetPendingFees(Guid? studentId = null)
        {
            var queryString = studentId.HasValue ? $"?studentId={studentId}" : "";
            return await SendRequestAsync<List<GetStudentFeeDTO>>(
                $"{_baseUrl}/Fee/pending{queryString}",
                HttpMethod.Get);
        }

        public async Task<Response> CalculateLateFees() =>
            await SendRequestAsync<Response>(
                $"{_baseUrl}/Fee/calculate-late-fees",
                HttpMethod.Post);

        public async Task<Response> GenerateInvoices(Guid? studentId = null, DateTime? dueDate = null)
        {
            var queryParams = new List<string>();
            if (studentId.HasValue) queryParams.Add($"studentId={studentId}");
            if (dueDate.HasValue) queryParams.Add($"dueDate={dueDate.Value:yyyy-MM-dd}");

            var queryString = queryParams.Any() ? "?" + string.Join("&", queryParams) : "";
            return await SendRequestAsync<Response>(
                $"{_baseUrl}/Fee/generate-invoices{queryString}",
                HttpMethod.Post);
        }

        // Validation
        public async Task<bool> ValidateFeeTemplate(Guid templateId)
        {
            var result = await SendRequestAsync<object>(
                $"{_baseUrl}/Fee/validate/template/{templateId}",
                HttpMethod.Get);
            return result != null;
        }

        public async Task<bool> CanDeleteFeeCategory(Guid categoryId)
        {
            var result = await SendRequestAsync<object>(
                $"{_baseUrl}/Fee/validate/category/{categoryId}",
                HttpMethod.Get);
            return result != null;
        }

        public async Task<bool> CanDeleteFeeTemplate(Guid templateId)
        {
            var result = await SendRequestAsync<object>(
                $"{_baseUrl}/Fee/validate/template-delete/{templateId}",
                HttpMethod.Get);
            return result != null;
        }
    }
}
