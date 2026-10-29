using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.DAL.Dto
{
    public class CategoryTranslationRequest // This class represents the request for creating a category translation for one specific language
    {
        public string Name { get; set; }
        public string Language { get; set; }

       
    }
}
