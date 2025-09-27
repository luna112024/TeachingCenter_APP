using System.ComponentModel.DataAnnotations;

namespace hongWenAPP.Models.IdentityModel.DTOs
{
    public record class ForgotPasswordDTO
    {
        [Display(Name = "Email")]
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
        public string Email { get; set; }
    }
} 