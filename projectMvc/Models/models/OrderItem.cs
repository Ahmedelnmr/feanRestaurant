using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace projectMvc.Models.models
{
    public class OrderItem:BaseModel
    {
        public int OrderId { get; set; }

        [ForeignKey("OrderId")]
        public Order Order { get; set; }

        public int ManuItemId { get; set; }

        [ForeignKey("MenuItemId")]
        public ManuItem ManuItem { get; set; }

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }

        public decimal Subtotal => Price * Quantity;
    }
}
