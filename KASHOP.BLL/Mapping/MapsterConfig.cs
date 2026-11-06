using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mapster;
using KASHOP.DAL.Models;
using KASHOP.DAL.Dto;
using System.Globalization;

namespace KASHOP.BLL.Mapping
{
    public static class MapsterConfig

    {
        public static void MappingConfigRegister()
        {
            TypeAdapterConfig<Category, CategoryResponse>.NewConfig()
                .Map(dest => dest.UserId, src => src.CreatedById)
                .Map(dest => dest.User, src => src.CreatedBy.UserName)
                .Map(dest => dest.Name, src => src.Translations.Where(t => t.Language == CultureInfo.CurrentUICulture.Name).Select(t => t.Name).FirstOrDefault());
        }
    }
}
