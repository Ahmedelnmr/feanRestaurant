

using Microsoft.AspNetCore.Mvc;
using projectMvc.Models.models;
using System.ComponentModel.DataAnnotations;

namespace projectMvc.Models.viewmodels
{
    public class Createmanuitemviewmodel
    {
        [Required(ErrorMessage = "menu item name is required")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "menu item name must be between 3 and 100 characters")]
        [Remote(action: "CheckNameExists", controller: "Admin", AdditionalFields = "id", ErrorMessage = "This name already exists.")]

        public string Name { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, 10000, ErrorMessage = "Price must be between 0.01 and 10,000")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Category is required")]
        public int CategoryId { get; set; }
        public bool UnAvailable { get; set; } = false;

        public IFormFile? ImageFile { get; set; }

        public string? ImageName { get; set; }
        public List<Category>? Categories { get; set; }

    }
}
