using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace SeminarHub.Data
{
    public class Seminar
    {
        [Key]
        [Comment("Seminar Identifier")]
        public int Id { get; set; }
        [Required]
        [MaxLength(DataConstants.SeminarTopicMaxLength)]
        [Comment("Seminar Topic")]
        public string Topic { get; set; } = string.Empty;
        [Required]
        [MaxLength(DataConstants.SeminarLecturerMaxLength)]
        [Comment("Seminar Lecturer")]
        public string Lecturer { get; set; } = string.Empty;
        [Required]
        [MaxLength(DataConstants.SeminarDetailsMaxLength)]
        [Comment("Seminar Details")]
        public string Details { get; set; } = string.Empty;
        [Required]
        [Comment("Seminar Organizer Identifier")]
        public string OrganizerId { get; set; } = string.Empty;
        [Required]
        [ForeignKey(nameof(OrganizerId))]
        public IdentityUser Organizer { get; set; } = null!;
        public DateTime DateAndTime { get; set; }
        [Range(DataConstants.SeminarDurationMinValue,DataConstants.SeminarDurationMaxValue)]
        [Comment("Seminar Duration")]
        public int Duration { get; set; }
        [Required]
        [Comment("Seminar Category Identifier")]
        public int CategoryId { get; set; }
        [Required]
        [ForeignKey(nameof(CategoryId))]
        public Category Category { get; set; } = null!;
        public ICollection<SeminarParticipant> SeminarParticipants { get; set; } = new List<SeminarParticipant>();

    }
}
