using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace KASHOP.DAL.Dto
{
    public class ProductTranslationRequest
    {
        [Required, StringLength(150)] public string Name { get; set; }
        [Required, StringLength(2000)] public string Description { get; set; }
        [Required, StringLength(10)] public string Language { get; set; }
    }
}
