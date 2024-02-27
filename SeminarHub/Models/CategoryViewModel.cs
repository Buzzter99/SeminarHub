using System.ComponentModel.DataAnnotations;
using SeminarHub.Data;

namespace SeminarHub.Models
{
    public class CategoryViewModel
    {
        public int Id { get; set; }
        [Required]
        [MinLength(DataConstants.CategoryNameMinLength)]
        [MaxLength(DataConstants.CategoryNameMaxLength)]
        public string Name { get; set; } = string.Empty;
    }
}
