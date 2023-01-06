using Microsoft.Extensions.DependencyInjection;
using ProjectsHub.Core;
using ProjectsHub.Model;

namespace ProjectsHub.Data
{
    public static class PostRepositpryConfiguration
    {
        public static void AddPostReopsitory(this IServiceCollection services)
        {
            services.AddSingleton<IPostRepository, PostRepository>();
        }
    }
}
