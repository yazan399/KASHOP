namespace KASHOP.PL.Extensions
{
    public static class ServicesExtensions
    {
        public static IServiceCollection AddApplicationServices (this IServiceCollection Services, IConfiguration Configuration)
        {
            Services.AddControllers();
            Services.AddOpenApi();
           Services.AddDatabaseServices(Configuration);
           Services.AddIdentityServices();
            Services.AddJwtAuthentication(Configuration);
         Services.AddLocalizationServices(Configuration);
          Services.addApplicationServices(Configuration);

            return Services;
        }
    }
}
