using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace projectMvc.Models.models
{
    public class Category: BaseModel
    {

        [Required(ErrorMessage = "Category name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Category name must be between 2 and 100 characters")]
        [Remote(action: "CheckCategoryNameExists", controller: "Admin", AdditionalFields = "Id", ErrorMessage = "This category name already exists.")]
        public string Name { get; set; }

        [StringLength(300, ErrorMessage = "Description cannot exceed 300 characters")]
        public string? Description { get; set; }
        [ValidateNever]
        public  List<ManuItem> ManuItems { get; set; }=null!;
    }
}
