using hongWenAPP.Models.Common;
using hongWenAPP.Models.CompanyModel.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace hongWenAPP.Services
{
    public interface ICompanyService
    {
        Task<List<GetCompanyDTO>> GetCompanies(string name = "");
        Task<List<GetCompanyDTO>> GetActiveCompanies(string name = "");

        Task<GetCompanyDTO> GetCompanybyId(Guid id);
        Task<Response> CreateCompany(CreateCompanyDTO createCompnayDTO);
        Task<Response> UpdateCompanies(UpdateCompanyDTO updateCompany);
        Task<Response> DeleteCompany(Guid id);
        Task<Response> AssignCompanyToUser(Guid userId, Guid companyId);
        Task<Response> RemoveCompanyFromUser(Guid userId, Guid companyId);
        Task<List<GetCompanyDTO>> GetCompaniesByUser(Guid userId);
    } 
    public class CompanyService : BaseApiService, ICompanyService
    {
        public CompanyService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
            : base(httpClientFactory, configuration)
        {
        }

        public async Task<List<GetCompanyDTO>> GetCompanies(string name = "") =>
            await SendRequestAsync<List<GetCompanyDTO>>(
                $"{_baseUrl}/Company{(string.IsNullOrEmpty(name) ? "" : $"?name={name}")}",
                HttpMethod.Get);
        public async Task<List<GetCompanyDTO>> GetActiveCompanies(string name = "") =>
            await SendRequestAsync<List<GetCompanyDTO>>(
                $"{_baseUrl}/Company/active{(string.IsNullOrEmpty(name) ? "" : $"?name={name}")}",
                HttpMethod.Get);

        public async Task<GetCompanyDTO> GetCompanybyId(Guid id) =>
            await SendRequestAsync<GetCompanyDTO>(
                $"{_baseUrl}/Company/{id}",
                HttpMethod.Get);
        public async Task<Response> DeleteCompany(Guid id) =>
           await SendRequestAsync<Response>(
               $"{_baseUrl}/Company/{id}",
               HttpMethod.Delete);
        public async Task<Response> CreateCompany(CreateCompanyDTO createCompanyDTO) =>
            await SendRequestAsync<Response>($"{_baseUrl}/Company", HttpMethod.Post, createCompanyDTO);

        public async Task<Response> UpdateCompanies(UpdateCompanyDTO updateCompany) =>
          await SendRequestAsync<Response>($"{_baseUrl}/Company", HttpMethod.Put, updateCompany);

        public async Task<Response> AssignCompanyToUser(Guid userId, Guid companyId) =>
            await SendRequestAsync<Response>(
                $"{_baseUrl}/Company/assign",
                HttpMethod.Post,
                new Models.CompanyModel.DTOs.UserCompanyAssignDto { UserId = userId, CompanyId = companyId });

        public async Task<Response> RemoveCompanyFromUser(Guid userId, Guid companyId) =>
            await SendRequestAsync<Response>(
                $"{_baseUrl}/Company/remove",
                HttpMethod.Delete,
                new Models.CompanyModel.DTOs.UserCompanyAssignDto { UserId = userId, CompanyId = companyId });

        public async Task<List<GetCompanyDTO>> GetCompaniesByUser(Guid userId) =>
            await SendRequestAsync<List<GetCompanyDTO>>(
                $"{_baseUrl}/Company/user/{userId}",
                HttpMethod.Get);
    }
}