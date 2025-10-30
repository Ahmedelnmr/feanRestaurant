using projectMvc.Models.models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace projectMvc.Models.models
{
    public enum OrderType { DineIn, Takeout, Delivery }
    public enum OrderStatus { Pending, Preparing, Ready, Delivered, Cancelled }

    public class Order : BaseModel
    {
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        public OrderType OrderType { get; set; }

        [StringLength(300)]
        public string? DeliveryAddress { get; set; }

        public decimal Subtotal { get; set; }
        public decimal Tax { get; set; }
        public decimal Discount { get; set; }
        public decimal Total { get; set; }

        public OrderStatus Status { get; set; } = OrderStatus.Pending;

      
        public DateTime? EstimatedDeliveryTime { get; set; }

        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
