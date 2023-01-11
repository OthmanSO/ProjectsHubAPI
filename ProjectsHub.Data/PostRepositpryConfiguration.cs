using Microsoft.Extensions.DependencyInjection;
using ProjectsHub.Core;

namespace ProjectsHub.Data
{
    public static class PostRepositpryConfiguration
    {
        public static void AddPostReopsitory(this IServiceCollection services)
        {
            services.AddScoped<IPostRepository, PostRepository>();
        }
    }
}
