using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace KASHOP.DAL.Dto
{
    public class CategoryTranslationRequest // This class represents the request for creating a category translation for one specific language
    {
        [Required, StringLength(150)] public string Name { get; set; }
        [Required, StringLength(10)] public string Language { get; set; }

       
    }
}
