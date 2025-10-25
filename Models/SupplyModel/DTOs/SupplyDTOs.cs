using System.ComponentModel.DataAnnotations;

namespace hongWenAPP.Models.SupplyModel.DTOs
{
    // Create Supply DTO
    public class CreateSupplyDTO
    {
        [Required(ErrorMessage = "Supply code is required")]
        [StringLength(20)]
        public string SupplyCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Supply name is required")]
        [StringLength(200)]
        public string SupplyName { get; set; } = string.Empty;

        public string? Description { get; set; }

        [StringLength(50)]
        public string? Category { get; set; }

        [Required(ErrorMessage = "Unit price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal UnitPrice { get; set; }

        public string? ApplicableLevels { get; set; }

        public string Status { get; set; } = "Active";

        public string? CreatedBy { get; set; }
    }

    // Update Supply DTO
    public class UpdateSupplyDTO
    {
        public Guid SupplyId { get; set; }

        [Required]
        [StringLength(20)]
        public string SupplyCode { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string SupplyName { get; set; } = string.Empty;

        public string? Description { get; set; }

        [StringLength(50)]
        public string? Category { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal UnitPrice { get; set; }

        public string? ApplicableLevels { get; set; }

        public string Status { get; set; } = "Active";

        public string? ModifiedBy { get; set; }
    }

    // Get Supply DTO
    public class GetSupplyDTO
    {
        public Guid SupplyId { get; set; }
        public string SupplyCode { get; set; } = string.Empty;
        public string SupplyName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Category { get; set; }
        public decimal UnitPrice { get; set; }
        public string? ApplicableLevels { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public string? CreatedBy { get; set; }
        public string? ModifiedBy { get; set; }
    }
}

