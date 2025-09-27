using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace hongWenAPP.Models.IdentityModel.DTOs
{
    public record class RegisterDTO
    {
        [Display(Name = "Username")]
        [Required(ErrorMessage = "Username is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters")]
        [RegularExpression(@"^[a-zA-Z0-9]+(?: [a-zA-Z0-9]+)*$", ErrorMessage = "Username can only contain letters, numbers, and spaces in the middle")]
        public string Username { get; set; }

        [Display(Name = "Email")]
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
        public string Email { get; set; }

        [Display(Name = "Password")]
        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$",
            ErrorMessage = "Password must include at least one uppercase letter, one lowercase letter, one digit, and one special character")]
        public string Password { get; set; }

        [Display(Name = "ConfirmPassword")]
        [Required(ErrorMessage = "Confirm password is required")]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "Status")]
        public bool Status { get; set; } = true; // Default to active


        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [Display(Name = "CreateBy")]
        public string? CreateBy { get; set; } = "";

    }
}
