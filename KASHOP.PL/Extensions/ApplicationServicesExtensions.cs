using KASHOP.BLL.Common;
using KASHOP.BLL.Services;
using KASHOP.DAL.Repository;
using KASHOP.PL.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Stripe;


namespace KASHOP.PL.Extensions
{
    public static class ApplicationServicesExtensions
    {

        public static IServiceCollection AddServices (this IServiceCollection Services, IConfiguration Configuration)
        {
            Services.AddScoped<ICheckoutService, BLL.Services.CheckoutService>();
            Services.AddScoped<IUnitOfWork, UnitOfWork>();
            Services.AddExceptionHandler<GlobalExceptionHandler>();
            Services.AddScoped<IFileService, BLL.Services.FileService>();
            Services.AddScoped<IProductService, BLL.Services.ProductService>();
            Services.AddScoped<ICategoryService, CategoryService>();
            Services.AddScoped<ICartService, CartService>();

            Services.AddScoped<IAuthenticationService, AuthenticationService>();
            Services.AddScoped<ISeedData, RoleSeedData>();
            Services.AddTransient<IEmailSender, EmailSender>();

            Services.Configure<StripeSettings>(Configuration.GetSection("Stripe"));
            StripeConfiguration.ApiKey = Configuration["Stripe:SecretKey"];

            Services.AddProblemDetails();

            return Services;
        }
    }
}
