using KASHOP.DAL.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.DAL.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        public OrderStatusEnum Status { get; set; } = OrderStatusEnum.Pending;
        public DateTime? ShippedDate { get; set; }

        public PaymentStatusEnum? PaymentStatus { get; set; } = PaymentStatusEnum.Pending;
        public decimal? AmountPaid { get; set; }
        public PaymentMethodEnum? PaymentMethod { get; set; }

        public  decimal? TotalAmount { get; set; }
        public ApplicationUser User { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string PhoneNumber { get; set; }

        
        public List<OrderItem> OrderItems { get; set; } 


        public string? StripeSessionId { get; set; }
        public string? StripePaymentIntentId { get; set; }

        public DateTime? PaidAt { get; set; }
    }
}
    