using hongWenAPP.Models.Common;
using hongWenAPP.Models.TermModel.DTOs;

namespace hongWenAPP.Services
{
    public interface ITermService
    {
        Task<GetTermDTO> GetTerm(Guid termId);
        Task<List<GetTermDTO>> GetAllTerms(string academicYear = null, string termName = null, string termCode = null);
        Task<List<GetTermDTO>> GetActiveTerm(string academicYear = null, string termName = null, string termCode = null);
        Task<GetTermDTO> GetCurrentTerm();
        Task<Response> CreateTerm(CreateTermDTO termDto);
        Task<Response> UpdateTerm(UpdateTermDTO termDto);
        Task<Response> DeleteTerm(Guid termId);
        Task<Response> SetCurrentTerm(Guid termId);
    }
    
    public class TermService : BaseApiService, ITermService
    {
        public TermService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
            : base(httpClientFactory, configuration)
        {
        }

        public async Task<GetTermDTO> GetTerm(Guid termId) =>
            await SendRequestAsync<GetTermDTO>(
                $"{_baseUrl}/Term/{termId}",
                HttpMethod.Get);

        public async Task<List<GetTermDTO>> GetAllTerms(string academicYear = null, string termName = null, string termCode = null)
        {
            var queryParams = new List<string>();
            if (!string.IsNullOrEmpty(academicYear)) queryParams.Add($"academicYear={academicYear}");
            if (!string.IsNullOrEmpty(termName)) queryParams.Add($"termName={termName}");
            if (!string.IsNullOrEmpty(termCode)) queryParams.Add($"termCode={termCode}");
            
            var queryString = queryParams.Any() ? "?" + string.Join("&", queryParams) : "";
            return await SendRequestAsync<List<GetTermDTO>>(
                $"{_baseUrl}/Term{queryString}",
                HttpMethod.Get);
        }

        public async Task<List<GetTermDTO>> GetActiveTerm(string academicYear = null, string termName = null, string termCode = null)
        {
            var queryParams = new List<string>();
            if (!string.IsNullOrEmpty(academicYear)) queryParams.Add($"academicYear={academicYear}");
            if (!string.IsNullOrEmpty(termName)) queryParams.Add($"termName={termName}");
            if (!string.IsNullOrEmpty(termCode)) queryParams.Add($"termCode={termCode}");
            
            var queryString = queryParams.Any() ? "?" + string.Join("&", queryParams) : "";
            return await SendRequestAsync<List<GetTermDTO>>(
                $"{_baseUrl}/Term/active{queryString}",
                HttpMethod.Get);
        }

        public async Task<GetTermDTO> GetCurrentTerm() =>
            await SendRequestAsync<GetTermDTO>(
                $"{_baseUrl}/Term/current",
                HttpMethod.Get);

        public async Task<Response> CreateTerm(CreateTermDTO termDto) =>
            await SendRequestAsync<Response>(
                $"{_baseUrl}/Term",
                HttpMethod.Post,
                termDto);

        public async Task<Response> UpdateTerm(UpdateTermDTO termDto) =>
            await SendRequestAsync<Response>(
                $"{_baseUrl}/Term",
                HttpMethod.Put,
                termDto);

        public async Task<Response> DeleteTerm(Guid termId) =>
            await SendRequestAsync<Response>(
                $"{_baseUrl}/Term/{termId}",
                HttpMethod.Delete);

        public async Task<Response> SetCurrentTerm(Guid termId) =>
     await SendRequestAsync<Response>(
         $"{_baseUrl}/Term/{termId}/set-current",
         HttpMethod.Post);

    }
}
