using Microsoft.Extensions.DependencyInjection;
using ProjectsHub.Core;

namespace ProjectsHub.Data
{
    public static class RepositpryConfiguration
    {
        public static void AddPostReopsitory(this IServiceCollection services)
        {
            services.AddTransient<IPostRepository, PostRepository>();
        }
        public static void AddUserRepository(this IServiceCollection services)
        {
            services.AddTransient<IUserRepository, UserRepository>();
        }
    }
}
