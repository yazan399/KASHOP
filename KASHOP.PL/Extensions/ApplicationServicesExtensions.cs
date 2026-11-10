using KASHOP.BLL.Common;
using KASHOP.BLL.Services;
using KASHOP.DAL.Repository;
using KASHOP.PL.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;


namespace KASHOP.PL.Extensions
{
    public static class ApplicationServicesExtensions
    {

        public static IServiceCollection AddServices (this IServiceCollection Services, IConfiguration Configuration)
        {
            Services.AddScoped<ICheckoutService, CheckoutService>();
            Services.AddScoped<IUnitOfWork, UnitOfWork>();
            Services.AddExceptionHandler<GlobalExceptionHandler>();
            Services.AddScoped<IFileService, FileService>();
            Services.AddScoped<IProductService, ProductService>();
            Services.AddScoped<ICategoryService, CategoryService>();
            Services.AddScoped<ICartService, CartService>();

            Services.AddScoped<IAuthenticationService, AuthenticationService>();
            Services.AddScoped<ISeedData, RoleSeedData>();
            Services.AddTransient<IEmailSender, EmailSender>();

            Services.AddProblemDetails();

            return Services;
        }
    }
}
