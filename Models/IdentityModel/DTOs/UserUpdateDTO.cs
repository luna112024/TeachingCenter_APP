using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace hongWenAPP.Models.IdentityModel.DTOs
{
    public record class UserUpdateDTO
    {
        public Guid UserId { get; set; }

        [Display(Name = "Email")]
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
        public string Email { get; set; }

        [Display(Name = "Username")]
        [Required(ErrorMessage = "Username is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters")]
        [RegularExpression(@"^[a-zA-Z0-9]+(?: [a-zA-Z0-9]+)*$", ErrorMessage = "Username can only contain letters, numbers, and spaces in the middle")]
        public string UserName { get; set; }

        [Display(Name = "Status")]
        public bool Status { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [Display(Name = "CompanyId")]
        public Guid? CompanyId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? KhmerName { get; set; }
        public string? ChineseName { get; set; }
        public string? Gender { get; set; }
        public DateOnly? DateofBirth { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
    }
}
