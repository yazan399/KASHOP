using Microsoft.AspNetCore.Localization;
using System.Globalization;

namespace KASHOP.PL.Extensions
{
    public static class LocalizationExtensions
    {
        public static IServiceCollection AddLocalizationServices(this IServiceCollection Services,IConfiguration Configuration)
        {
            Services.AddLocalization(options =>
            {
                options.ResourcesPath = "";
            });


            var supportedCultures = new[]
{
    new CultureInfo("en"),
    new CultureInfo("ar")
};

            Services.Configure<RequestLocalizationOptions>(options =>
            {
                options.DefaultRequestCulture = new RequestCulture("en");

                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
                options.RequestCultureProviders.Clear();
                options.RequestCultureProviders.Add(new AcceptLanguageHeaderRequestCultureProvider());
            });
            return Services;
        }
    }
}
