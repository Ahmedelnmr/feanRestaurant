using projectMvc.Models.models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace projectMvc.Models
{
    public class ManuItem : BaseModel
    {
        [Required(ErrorMessage = "Product name is required")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Product name must be between 3 and 100 characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, 10000, ErrorMessage = "Price must be between 0.01 and 10,000")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Image URL is required")]
        [Url(ErrorMessage = "Please enter a valid image URL")]
        public string Image { get; set; }

        [Required(ErrorMessage = "Preparation time is required")]
        [Range(1, 240, ErrorMessage = "Preparation time must be between 1 and 240 minutes")]
        public bool Unavailable { get; set; } = false;

        public int PreparationTimeInMinutes { get; set; }

        [Required(ErrorMessage = "Category is required")]
        public int CategoryId { get; set; }
        public  Category Category { get; set; }

        public ICollection<DailyMenuItemStats> DailyStats { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
