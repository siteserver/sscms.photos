using Microsoft.Extensions.DependencyInjection;
using SSCMS.Photos.Abstractions;
using SSCMS.Photos.Core;
using SSCMS.Plugins;

namespace SSCMS.Photos
{
    public class Startup : IPluginConfigureServices
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IPhotoRepository, PhotoRepository>();
            services.AddScoped<IPhotoManager, PhotoManager>();
        }
    }
}
