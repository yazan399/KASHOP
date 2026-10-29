using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.DAL.Dto
{
    public   class CategoryRequest // This class represents the request for creating a category with its all translations
    {
        public List<CategoryTranslationRequest> Translations { get; set; }
    }
}
