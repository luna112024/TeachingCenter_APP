using hongWenAPP.Models.Common;
using hongWenAPP.Models.LevelModel.DTOs;

namespace hongWenAPP.Services
{
    public interface ILevelService
    {
        Task<GetLevelDTO> GetLevel(Guid levelId);
        Task<List<GetLevelDTO>> GetAllLevels(string? levelCode = null, string? levelName = null, bool? isActive = null, bool? isForBeginner = null, bool? isForPlacement = null, string? hskLevel = null, string? cefrEquivalent = null);
        Task<List<LevelListDTO>> GetLevelsList(string? levelCode = null, string? levelName = null, bool? isActive = null, string? hskLevel = null);
        Task<List<LevelHierarchyDTO>> GetLevelsHierarchy();
        Task<List<GetLevelDTO>> GetLevelsByParent(Guid? parentLevelId);
        Task<List<GetLevelDTO>> GetActiveLevels();
        Task<List<GetLevelDTO>> GetLevelsForPlacement();
        Task<List<GetLevelDTO>> GetBeginnerLevels();
        Task<Response> CreateLevel(CreateLevelDTO levelDto);
        Task<Response> UpdateLevel(UpdateLevelDTO levelDto);
        Task<Response> DeleteLevel(Guid levelId);
        Task<Response> ReorderLevels(List<Guid> levelIds);
        Task<bool> IsLevelCodeUnique(string levelCode, Guid? excludeLevelId = null);
        Task<bool> IsLevelNameUnique(string levelName, Guid? excludeLevelId = null);
    }

    public class LevelService : BaseApiService, ILevelService
    {
        public LevelService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
            : base(httpClientFactory, configuration)
        {
        }

        public async Task<GetLevelDTO> GetLevel(Guid levelId) =>
            await SendRequestAsync<GetLevelDTO>(
                $"{_baseUrl}/Level/{levelId}",
                HttpMethod.Get);

        public async Task<List<GetLevelDTO>> GetAllLevels(string? levelCode = null, string? levelName = null, bool? isActive = null, bool? isForBeginner = null, bool? isForPlacement = null, string? hskLevel = null, string? cefrEquivalent = null)
        {
            var queryParams = new List<string>();
            if (!string.IsNullOrEmpty(levelCode)) queryParams.Add($"levelCode={levelCode}");
            if (!string.IsNullOrEmpty(levelName)) queryParams.Add($"levelName={levelName}");
            if (isActive.HasValue) queryParams.Add($"isActive={isActive.Value}");
            if (isForBeginner.HasValue) queryParams.Add($"isForBeginner={isForBeginner.Value}");
            if (isForPlacement.HasValue) queryParams.Add($"isForPlacement={isForPlacement.Value}");
            if (!string.IsNullOrEmpty(hskLevel)) queryParams.Add($"hskLevel={hskLevel}");
            if (!string.IsNullOrEmpty(cefrEquivalent)) queryParams.Add($"cefrEquivalent={cefrEquivalent}");

            var queryString = queryParams.Any() ? "?" + string.Join("&", queryParams) : "";
            return await SendRequestAsync<List<GetLevelDTO>>(
                $"{_baseUrl}/Level{queryString}",
                HttpMethod.Get);
        }

        public async Task<List<LevelListDTO>> GetLevelsList(string? levelCode = null, string? levelName = null, bool? isActive = null, string? hskLevel = null)
        {
            var queryParams = new List<string>();
            if (!string.IsNullOrEmpty(levelCode)) queryParams.Add($"levelCode={levelCode}");
            if (!string.IsNullOrEmpty(levelName)) queryParams.Add($"levelName={levelName}");
            if (isActive.HasValue) queryParams.Add($"isActive={isActive.Value}");
            if (!string.IsNullOrEmpty(hskLevel)) queryParams.Add($"hskLevel={hskLevel}");

            var queryString = queryParams.Any() ? "?" + string.Join("&", queryParams) : "";
            return await SendRequestAsync<List<LevelListDTO>>(
                $"{_baseUrl}/Level/list{queryString}",
                HttpMethod.Get);
        }

        public async Task<List<LevelHierarchyDTO>> GetLevelsHierarchy() =>
            await SendRequestAsync<List<LevelHierarchyDTO>>(
                $"{_baseUrl}/Level/hierarchy",
                HttpMethod.Get);

        public async Task<List<GetLevelDTO>> GetLevelsByParent(Guid? parentLevelId)
        {
            var url = parentLevelId.HasValue 
                ? $"{_baseUrl}/Level/by-parent/{parentLevelId.Value}"
                : $"{_baseUrl}/Level/by-parent";
            return await SendRequestAsync<List<GetLevelDTO>>(url, HttpMethod.Get);
        }

        public async Task<List<GetLevelDTO>> GetActiveLevels() =>
            await SendRequestAsync<List<GetLevelDTO>>(
                $"{_baseUrl}/Level/active",
                HttpMethod.Get);

        public async Task<List<GetLevelDTO>> GetLevelsForPlacement() =>
            await SendRequestAsync<List<GetLevelDTO>>(
                $"{_baseUrl}/Level/placement",
                HttpMethod.Get);

        public async Task<List<GetLevelDTO>> GetBeginnerLevels() =>
            await SendRequestAsync<List<GetLevelDTO>>(
                $"{_baseUrl}/Level/beginner",
                HttpMethod.Get);

        public async Task<Response> CreateLevel(CreateLevelDTO levelDto) =>
            await SendRequestAsync<Response>(
                $"{_baseUrl}/Level",
                HttpMethod.Post,
                levelDto);

        public async Task<Response> UpdateLevel(UpdateLevelDTO levelDto) =>
            await SendRequestAsync<Response>(
                $"{_baseUrl}/Level",
                HttpMethod.Put,
                levelDto);

        public async Task<Response> DeleteLevel(Guid levelId) =>
            await SendRequestAsync<Response>(
                $"{_baseUrl}/Level/{levelId}",
                HttpMethod.Delete);

        public async Task<Response> ReorderLevels(List<Guid> levelIds) =>
            await SendRequestAsync<Response>(
                $"{_baseUrl}/Level/reorder",
                HttpMethod.Put,
                levelIds);

        public async Task<bool> IsLevelCodeUnique(string levelCode, Guid? excludeLevelId = null)
        {
            var queryParams = new List<string>();
            if (excludeLevelId.HasValue) queryParams.Add($"excludeLevelId={excludeLevelId.Value}");
            var queryString = queryParams.Any() ? "?" + string.Join("&", queryParams) : "";
            
            var result = await SendRequestAsync<dynamic>(
                $"{_baseUrl}/Level/validate/code/{levelCode}{queryString}",
                HttpMethod.Get);
            
            return result?.IsUnique ?? false;
        }

        public async Task<bool> IsLevelNameUnique(string levelName, Guid? excludeLevelId = null)
        {
            var queryParams = new List<string>();
            if (excludeLevelId.HasValue) queryParams.Add($"excludeLevelId={excludeLevelId.Value}");
            var queryString = queryParams.Any() ? "?" + string.Join("&", queryParams) : "";
            
            var result = await SendRequestAsync<dynamic>(
                $"{_baseUrl}/Level/validate/name/{levelName}{queryString}",
                HttpMethod.Get);
            
            return result?.IsUnique ?? false;
        }
    }
}
