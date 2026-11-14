using KASHOP.DAL.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace KASHOP.DAL.Dto
{
    public class ChangeOrderStatusRequest
    {
        [EnumDataType(typeof(OrderStatusEnum))]
        public OrderStatusEnum NewStatus { get; set; }

    }
}
