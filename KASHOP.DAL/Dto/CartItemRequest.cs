using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace KASHOP.DAL.Dto
{
    public class CartItemRequest
    {
        [Range(1, int.MaxValue)] public int ProductId { get; set; }
        [Range(1, int.MaxValue)] public int Count { get; set; }
    }
    }
