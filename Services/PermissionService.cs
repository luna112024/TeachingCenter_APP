using hongWenAPP.Models.Common;
using hongWenAPP.Models.PermissionModel.DTOs;

namespace hongWenAPP.Services
{
    public interface IPermissionService
    {
        Task<List<GetPermissionDTO>> GetAllPermissions(string name = "");
        Task<List<GetPermissionDTO>> GetPermissionsByRole(Guid roleId);
        Task<Response> AssignPermissionToRole(Guid roleId, Guid permissionId);
        Task<Response> RemovePermissionFromRole(Guid roleId, Guid permissionId);
        Task<Response> CreatePermission(CreatePermissionDTO createPermissionDTO);
        Task<Response> UpdatePermission(UpdatePermissionDTO updatePermissionDTO);
        Task<Response> DelectPermission(Guid roleId);
        Task<GetPermissionDTO> GetPermissionbyId(Guid roleId);
    }

    public class PermissionService : BaseApiService, IPermissionService
    {
        public PermissionService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
            : base(httpClientFactory, configuration)
        {
        }

        public async Task<List<GetPermissionDTO>> GetAllPermissions(string name = "") =>
            await SendRequestAsync<List<GetPermissionDTO>>(
                $"{_baseUrl}/Permission{(string.IsNullOrEmpty(name) ? "" : $"?name={name}")}",
                HttpMethod.Get);

        public async Task<List<GetPermissionDTO>> GetPermissionsByRole(Guid roleId) =>
            await SendRequestAsync<List<GetPermissionDTO>>(
                $"{_baseUrl}/Permission/byrole/{roleId}",
                HttpMethod.Get);

        public async Task<Response> AssignPermissionToRole(Guid roleId, Guid permissionId) =>
            await SendRequestAsync<Response>(
                $"{_baseUrl}/Permission/assign",
                HttpMethod.Post,
                new { RoleId = roleId, PermissionId = permissionId });

        public async Task<Response> RemovePermissionFromRole(Guid roleId, Guid permissionId) =>
            await SendRequestAsync<Response>(
                $"{_baseUrl}/Permission/remove",
                HttpMethod.Post,
                new { RoleId = roleId, PermissionId = permissionId });

        public async Task<Response> CreatePermission(CreatePermissionDTO createPermissionDTO) =>
            await SendRequestAsync<Response>(
                $"{_baseUrl}/Permission",
                HttpMethod.Post,
                createPermissionDTO);

        public async Task<Response> UpdatePermission(UpdatePermissionDTO updatePermissionDTO) =>
            await SendRequestAsync<Response>(
                $"{_baseUrl}/Permission",
                HttpMethod.Put,
                updatePermissionDTO);

        public async Task<Response> DelectPermission(Guid roleId) =>
            await SendRequestAsync<Response>(
                $"{_baseUrl}/Permission/{roleId}",
                HttpMethod.Delete);

        public async Task<GetPermissionDTO> GetPermissionbyId(Guid roleId) =>
            await SendRequestAsync<GetPermissionDTO>(
                $"{_baseUrl}/Permission/{roleId}",
                HttpMethod.Get);
    }

}