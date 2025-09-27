namespace hongWenAPP.Models.IdentityModel.DTOs
{
    public class UserAccountInfoResponse
    {
        public string CompanyName { get; set; }
        public List<AccountInfoResponse> Accounts { get; set; } = new List<AccountInfoResponse>();
    }

    public class AccountInfoResponse
    {
        public int AccID { get; set; }
        public string AccountNumber { get; set; }
        public bool IsActive { get; set; }
        public string Currency { get; set; }
    }
} 