using hongWenAPP.Models.AccountModel.DTOs;
using hongWenAPP.Models.Common;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace hongWenAPP.Services
{
    public interface IAccountService
    {
        Task<GetAccountDTO> GetAccountById(Guid id);
        Task<List<GetAccountDTO>> GetAccounts(string name="");
        Task<Response> CreateAccount(CreateAccountDTO createAccountDTO);
        Task<Response> UpdateAccount(UpdateAccountDTO updateAccountDTO);
        Task<Response> DeleteAccount(Guid id);
    }
    public class AccountService : BaseApiService, IAccountService
    {
        public AccountService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
            : base(httpClientFactory, configuration)
        {
        }

        public async Task<GetAccountDTO> GetAccountById(Guid id)
        {
            return await SendRequestAsync<GetAccountDTO>($"{_baseUrl}/Account/{id}", HttpMethod.Get);
        }

        public async Task<List<GetAccountDTO>> GetAccounts(string name="")
        {
            return await SendRequestAsync<List<GetAccountDTO>>($"{_baseUrl}/Account{(string.IsNullOrEmpty(name) ? "" : $"?name={name}")}", HttpMethod.Get);
        }
        public async Task<Response> CreateAccount(CreateAccountDTO createAccountDTO)
        {
            return await SendRequestAsync<Response>($"{_baseUrl}/Account", HttpMethod.Post, createAccountDTO);
        }

        public async Task<Response> UpdateAccount(UpdateAccountDTO updateAccountDTO)
        {
            return await SendRequestAsync<Response>($"{_baseUrl}/Account", HttpMethod.Put, updateAccountDTO);
        }
        public async Task<Response> DeleteAccount(Guid id)
        {
            return await SendRequestAsync<Response>($"{_baseUrl}/Account/{id}", HttpMethod.Delete);
        }
    }
}
