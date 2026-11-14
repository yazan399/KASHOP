using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace KASHOP.DAL.Dto
{
    public class ConfirmEmailRequest
    {
        [Required] public string UserId  { get; set; }
        [Required] public string Token { get; set; }
    }
}
