using System.ComponentModel.DataAnnotations;

namespace hongWenAPP.Models.IdentityModel.DTOs
{
    public record class AdminResetPasswordDTO
    {
        [Required(ErrorMessage = "User ID is required")]
        public Guid UserId { get; set; }
        
        [Required(ErrorMessage = "New password is required")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$", 
            ErrorMessage = "Password must include at least one uppercase letter, one lowercase letter, one digit, and one special character")]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; }
        
        [Required(ErrorMessage = "Confirm password is required")]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }
        
        [Display(Name = "Reason (Optional)")]
        [StringLength(500, ErrorMessage = "Reason cannot exceed 500 characters")]
        public string? Reason { get; set; } // Optional reason for the password reset for audit purposes
    }
} 