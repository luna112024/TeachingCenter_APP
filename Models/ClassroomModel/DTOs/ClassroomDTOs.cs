using System.ComponentModel.DataAnnotations;

namespace hongWenAPP.Models.ClassroomModel.DTOs
{
    public abstract record class ClassroomBaseDTO
    {
        [Required]
        [StringLength(20)]
        public string RoomCode { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string RoomName { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Building { get; set; }

        [Range(0, int.MaxValue)]
        public int? FloorLevel { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Capacity { get; set; }

        public string? Equipment { get; set; }

        public string? Facilities { get; set; }

        public string? LocationNotes { get; set; }

        [StringLength(20)]
        public string Status { get; set; } = "Available";
    }

    public record class CreateClassroomDTO : ClassroomBaseDTO
    {
        public string? CreatedBy { get; set; }
    }

    public record class UpdateClassroomDTO : ClassroomBaseDTO
    {
        public Guid ClassroomId { get; set; }
        public string? ModifiedBy { get; set; }
    }

    public record class GetClassroomDTO : ClassroomBaseDTO
    {
        public Guid ClassroomId { get; set; }
        public string? CreatedBy { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
    }
}
