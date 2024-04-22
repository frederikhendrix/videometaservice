using VideoMetaService.Interfaces;
using VideoMetaService.Services;

namespace VideoMetaService.Extensions
{
    public static class ServiceExtension
    {
        public static IServiceCollection AddService(this IServiceCollection services)
        {
            services.AddScoped<IVideoMetadataService, VideoMetadataService>();
            return services;
        }
    }
}
