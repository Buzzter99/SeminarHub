using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace SeminarHub.Data
{
    public class Category
    {
        [Required]
        [Comment("Category Identifier")]
        public int Id { get; set; }
        [Required]
        [MaxLength(DataConstants.CategoryNameMaxLength)]
        [Comment("Category Name")]
        public string Name { get; set; } = string.Empty;
        public ICollection<Seminar> Seminars { get; set; } = new HashSet<Seminar>();
    }
}
