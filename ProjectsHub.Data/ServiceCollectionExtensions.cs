using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectsHub.Data
{
    public static class ServiceCollectionExtensions
    {
        public static void AddData(this IServiceCollection services)
        {
            //services.AddDbContext<BooksDbContexst>(options =>
              //  options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
            //services.AddScoped<IUserRepo, RelationalUserRepo>();
        }

    }
}
