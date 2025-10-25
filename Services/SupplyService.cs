using hongWenAPP.Models.Common;
using hongWenAPP.Models.SupplyModel.DTOs;

namespace hongWenAPP.Services
{
    public interface ISupplyService
    {
        Task<List<GetSupplyDTO>> GetAllSupplies(string? category = null, string? status = null, string? name = null);
        Task<GetSupplyDTO> GetSupplyById(Guid supplyId);
        Task<List<GetSupplyDTO>> GetSuppliesByCategory(string category);
        Task<Response> CreateSupply(CreateSupplyDTO dto);
        Task<Response> UpdateSupply(UpdateSupplyDTO dto);
        Task<Response> DeleteSupply(Guid supplyId);
    }

    public class SupplyService : BaseApiService, ISupplyService
    {
        public SupplyService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
            : base(httpClientFactory, configuration)
        {
        }

        public async Task<List<GetSupplyDTO>> GetAllSupplies(string? category = null, string? status = null, string? name = null)
        {
            var queryParams = new List<string>();
            if (!string.IsNullOrEmpty(category)) queryParams.Add($"category={category}");
            if (!string.IsNullOrEmpty(status)) queryParams.Add($"status={status}");
            if (!string.IsNullOrEmpty(name)) queryParams.Add($"name={name}");

            var queryString = queryParams.Any() ? "?" + string.Join("&", queryParams) : "";
            return await SendRequestAsync<List<GetSupplyDTO>>(
                $"{_baseUrl}/Supply{queryString}",
                HttpMethod.Get);
        }

        public async Task<GetSupplyDTO> GetSupplyById(Guid supplyId) =>
            await SendRequestAsync<GetSupplyDTO>(
                $"{_baseUrl}/Supply/{supplyId}",
                HttpMethod.Get);

        public async Task<List<GetSupplyDTO>> GetSuppliesByCategory(string category) =>
            await SendRequestAsync<List<GetSupplyDTO>>(
                $"{_baseUrl}/Supply/category/{category}",
                HttpMethod.Get);

        public async Task<Response> CreateSupply(CreateSupplyDTO dto) =>
            await SendRequestAsync<Response>(
                $"{_baseUrl}/Supply",
                HttpMethod.Post,
                dto);

        public async Task<Response> UpdateSupply(UpdateSupplyDTO dto) =>
            await SendRequestAsync<Response>(
                $"{_baseUrl}/Supply",
                HttpMethod.Put,
                dto);

        public async Task<Response> DeleteSupply(Guid supplyId) =>
            await SendRequestAsync<Response>(
                $"{_baseUrl}/Supply/{supplyId}",
                HttpMethod.Delete);
    }
}

