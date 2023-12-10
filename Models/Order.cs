using Pronia.Utilities.Enums;

namespace Pronia.Models
{
    public class Order
    {
        public int Id { get; set; }

        public DateTime CreatedAt { get; set; }

        public OrderStatus Status { get; set; }

        public decimal TotalPrice { get; set; }

        public string Address { get; set; }
        public List<OrderItem> OrderItems { get; set; }

        public string AppUserId { get; set; }

        public AppUser AppUser { get; set; }


    }
}
