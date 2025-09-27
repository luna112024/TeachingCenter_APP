using System.ComponentModel.DataAnnotations;


namespace hongWenAPP.Models.IdentityModel.DTOs
{
    public record class ChangePasswordDTO
    {
        [Display(Name = "Current Password")]
        [Required(ErrorMessage = "Current password is required")]
        public string CurrentPassword { get; set; }
        
        [Display(Name = "New Password")]
        [Required(ErrorMessage = "New password is required")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$", 
            ErrorMessage = "Password must include at least one uppercase letter, one lowercase letter, one digit, and one special character")]
        public string NewPassword { get; set; }
        
        [Display(Name = "Confirm Password")]
        [Required(ErrorMessage = "Confirm password is required")]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }
    }
} 