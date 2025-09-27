using hongWenAPP.Models.Common;
using hongWenAPP.Models.IdentityModel.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace hongWenAPP.Services
{
    public interface IIdentityService
    {
        Task<List<GetUsersDTOs>> GetUsers(string name = "");
        Task<Response> RegisterUser(RegisterDTO registerDTO);
        Task<GetUsersDTOs> GetUserProfile();
        Task<GetUsersDTOs> GetUserById(Guid userId);
        Task<Response> UpdateUser(UserUpdateDTO updateDto);
        Task<Response> UpdateProfileByAdmin(UserUpdateDTO updateDto);
        Task<Response> ChangePassword(ChangePasswordDTO changePasswordDto);
        Task<Response> DeleteUser(Guid userId);
        Task<Response> AdminResetPassword(AdminResetPasswordDTO adminResetPasswordDto);
        Task<UserAccountInfoResponse> GetUserAccountInfo();
        Task<Response> InitialPasswordChange(ChangePasswordDTO changePasswordDto);
    }

    public class IdentityService : BaseApiService, IIdentityService
    {
        public IdentityService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
            : base(httpClientFactory, configuration, "ApiSettings:IdentityBaseUrl")
        {
        }

        public async Task<List<GetUsersDTOs>> GetUsers(string name = "") =>
            await SendRequestAsync<List<GetUsersDTOs>>(
                $"{_baseUrl}/profiles{(string.IsNullOrEmpty(name) ? "" : $"?name={name}")}",
                HttpMethod.Get);

        public async Task<Response> RegisterUser(RegisterDTO registerDTO) =>
            await SendRequestAsync<Response>(
                $"{_baseUrl}/register", 
                HttpMethod.Post, 
                registerDTO);

        public async Task<GetUsersDTOs> GetUserProfile() =>
            await SendRequestAsync<GetUsersDTOs>(
                $"{_baseUrl}/profile",
                HttpMethod.Get);
                
        public async Task<GetUsersDTOs> GetUserById(Guid userId) =>
            await SendRequestAsync<GetUsersDTOs>(
                $"{_baseUrl}/{userId}",
                HttpMethod.Get);
                
        public async Task<Response> UpdateUser(UserUpdateDTO updateDto) =>
            await SendRequestAsync<Response>(
                $"{_baseUrl}/profile",
                HttpMethod.Put,
                updateDto);
        public async Task<Response> UpdateProfileByAdmin(UserUpdateDTO updateDto) =>
            await SendRequestAsync<Response>(
                $"{_baseUrl}/profile/admin",
                HttpMethod.Put,
                updateDto);

        public async Task<Response> ChangePassword(ChangePasswordDTO changePasswordDto) =>
            await SendRequestAsync<Response>(
                $"{_baseUrl}/change-password",
                HttpMethod.Post,
                changePasswordDto);

        public async Task<Response> DeleteUser(Guid userId) =>
            await SendRequestAsync<Response>(
                $"{_baseUrl}/soft-delete/{userId}",
                HttpMethod.Delete);

        public async Task<Response> AdminResetPassword(AdminResetPasswordDTO adminResetPasswordDto) =>
            await SendRequestAsync<Response>(
                $"{_baseUrl}/admin/reset-password",
                HttpMethod.Post,
                adminResetPasswordDto);

        public async Task<UserAccountInfoResponse> GetUserAccountInfo() =>
            await SendRequestAsync<UserAccountInfoResponse>(
                $"{_baseUrl}/account-info",
                HttpMethod.Get);

        public async Task<Response> InitialPasswordChange(ChangePasswordDTO changePasswordDto) =>
            await SendRequestAsync<Response>(
                $"{_baseUrl}/change-password",
                HttpMethod.Post,
                changePasswordDto);

    }
} 