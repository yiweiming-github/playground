using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace ConsoleApplication
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddDirectoryBrowser();            
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseStaticFiles();
            app.UseFileServer(enableDirectoryBrowsing: true);
            app.UseMvc();
        }
    }
}