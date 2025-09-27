using System.ComponentModel.DataAnnotations;

namespace hongWenAPP.Models.IdentityModel.DTOs
{
    public record class GetUsersDTOs
    {
        public Guid UserId { get; set; }

        [Required]
        public string Username { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        public bool Status { get; set; }

        public List<string> Roles { get; set; } = new List<string>();
        
        // Company information
        public Guid? CompanyId { get; set; }
        public string CompanyName { get; set; }
        public List<string> Companies { get; set; } = new List<string>();
    }
}
