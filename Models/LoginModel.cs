using System.ComponentModel.DataAnnotations;

namespace hongWenAPP.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; }
        
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
        
        public bool RememberMe { get; set; } = true;
    }

    public class TokenResponse
    {
        public bool IsSuccess { get; set; } = true;
        public string ErrorMessage { get; set; }
        public string AccessToken { get; set; }
        public string TokenType { get; set; }
        public int ExpiresIn { get; set; }
        public UserInfo User { get; set; }
    }

    public class UserInfo
    {
        public Guid UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public bool RequirePasswordChange { get; set; } = false;
        public List<string> Roles { get; set; }
        public List<string> Permissions { get; set; }
        public CompanyInfo? Company { get; set; }
    }

    public class CompanyInfo
    {
        public Guid CompanyId { get; set; }
        public string CompanyName { get; set; }
    }
} 