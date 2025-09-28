using hongWenAPP.Models.Common;
using hongWenAPP.Models.RolesModel.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace hongWenAPP.Services
{
    public interface IRoleService
    {
        Task<List<GetRoleDTO>> GetRoles(string name = "");
        Task<GetRoleDTO> GetRoleById(Guid roleId);
        Task<Response> CreateRole(CreateRoleDTO createRoleDto);
        Task<Response> UpdateRole(UpdateRoleDTO updateRoleDto);
        Task<Response> DeleteRole(Guid roleId);
        Task<Response> AssignRoleToUser(Guid userId, Guid roleId);
        Task<Response> RemoveRoleFromUser(Guid userId, Guid roleId);
        Task<bool> HasTeacherProfile(Guid userId);
        Task<Response> ValidateTeacherRoleAssignment(Guid userId, List<Guid> newRoleIds);
    }
    public class RoleService : BaseApiService, IRoleService
    {
        public RoleService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
            : base(httpClientFactory, configuration)
        {
        }



        public async Task<List<GetRoleDTO>> GetRoles(string name = "") =>
            await SendRequestAsync<List<GetRoleDTO>>(
                $"{_baseUrl}/Role{(string.IsNullOrEmpty(name) ? "" : $"?name={name}")}",
                HttpMethod.Get);

        public async Task<Response> AssignRoleToUser(Guid userId, Guid roleId) =>
            await SendRequestAsync<Response>(
                $"{_baseUrl}/Role/assign",
                HttpMethod.Post,
                new UserRoleAssignDto { UserId = userId, RoleId = roleId });

        public async Task<Response> RemoveRoleFromUser(Guid userId, Guid roleId) =>
            await SendRequestAsync<Response>(
                $"{_baseUrl}/Role/remove",
                HttpMethod.Post,
                new UserRoleAssignDto { UserId = userId, RoleId = roleId });

        public async Task<GetRoleDTO> GetRoleById(Guid roleId) =>
            await SendRequestAsync<GetRoleDTO>(
                $"{_baseUrl}/Role/{roleId}",
                HttpMethod.Get);

        public async Task<Response> CreateRole(CreateRoleDTO createRoleDto) =>
            await SendRequestAsync<Response>(
                $"{_baseUrl}/Role",
                HttpMethod.Post,
                createRoleDto);

        public async Task<Response> UpdateRole(UpdateRoleDTO updateRoleDto) =>
            await SendRequestAsync<Response>(
                $"{_baseUrl}/Role",
                HttpMethod.Put,
                updateRoleDto);

        public async Task<Response> DeleteRole(Guid roleId) =>
            await SendRequestAsync<Response>(
                $"{_baseUrl}/Role/{roleId}",
                HttpMethod.Delete);

        public async Task<bool> HasTeacherProfile(Guid userId) =>
            await SendRequestAsync<bool>(
                $"{_baseUrl}/Identity/has-teacher-profile/{userId}",
                HttpMethod.Get);

        public async Task<Response> ValidateTeacherRoleAssignment(Guid userId, List<Guid> newRoleIds) =>
            await SendRequestAsync<Response>(
                $"{_baseUrl}/Identity/validate-teacher-role-assignment",
                HttpMethod.Post,
                new { UserId = userId, NewRoleIds = newRoleIds });
    }
}
