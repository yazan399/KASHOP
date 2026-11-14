using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace KASHOP.DAL.Dto
{
    public   class CategoryRequest // This class represents the request for creating a category with its all translations
    {
        [Required, MinLength(1)]
        public List<CategoryTranslationRequest> Translations { get; set; }
    }
}
