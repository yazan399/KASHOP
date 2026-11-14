using KASHOP.DAL.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace KASHOP.DAL.Dto
{
    public class CheckoutRequest
    {
        [EnumDataType(typeof(PaymentMethodEnum))]
        public PaymentMethodEnum PaymentMethod { get; set; }

        [StringLength(100)] public string? City { get; set; }
        [StringLength(200)] public string? Street { get; set; }
        [Phone, StringLength(30)] public string? PhoneNumber { get; set; }
    }
}
