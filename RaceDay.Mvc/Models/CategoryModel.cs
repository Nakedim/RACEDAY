using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay.Mvc.Models
{
    public class CategoryModel
    {
        public int CategoryId { get; set; }
        [Required(ErrorMessage = "Please enter a name for the category.")]
        [StringLength(50, ErrorMessage = "Category name cannot exceed 50 characters.")]
        [Display(Name = "Category Name")]
        public string CategoryName { get; set; } = string.Empty;
        [NotMapped]
        public string? Description { get; set; }

    }
}
