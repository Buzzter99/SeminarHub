using System.ComponentModel.DataAnnotations;
using SeminarHub.Data;

namespace SeminarHub.Models
{
    public class SeminarFormViewModel
    {
        public int Id { get; set; }
        public string OrganizerId { get; set; } = string.Empty;
        [Required]
        [MinLength(DataConstants.SeminarTopicMinLength)]
        [MaxLength(DataConstants.SeminarTopicMaxLength)]
        public string Topic { get; set; } = string.Empty;
        [Required]
        [MinLength(DataConstants.SeminarLecturerMinLength)]
        [MaxLength(DataConstants.SeminarLecturerMaxLength)]
        public string Lecturer { get; set; } = string.Empty;
        [Required]
        [MinLength(DataConstants.SeminarDetailsMinLength)]
        [MaxLength(DataConstants.SeminarDetailsMaxLength)]
        public string Details { get; set; } = string.Empty;
        [Required]
        public string DateAndTime { get; set; } = string.Empty;
        [Required]
        [Range(DataConstants.SeminarDurationMinValue, DataConstants.SeminarDurationMaxValue)]
        public int? Duration { get; set; }
        [Required]
        public int CategoryId { get; set; }
        [Required]
        public List<CategoryViewModel> Categories { get; set; } = new List<CategoryViewModel>();
        public string Category { get; set; } = string.Empty;
        public string Organizer = string.Empty;

    }
}
