namespace Chatbot.WebApi
{
    using System;
    using System.Linq;
    using System.Reflection;
    using Chatbot.Services;
    using Chatbot.WebApi.Middlewares;
    using Fanex.Caching;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using NJsonSchema;
    using NSwag.AspNetCore;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services.AddSingleton<MessengerFactory, MessengerFactoryImpl>();
            services.AddSingleton<SettingService, SettingServiceImpl>();
            services.AddSingleton<IDynamicRepository, DynamicRepository>();
            services.AddSingleton<ICacheService, CacheService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwaggerUi(typeof(Startup).GetTypeInfo().Assembly, settings =>
            {
                settings.GeneratorSettings.DefaultPropertyNameHandling = PropertyNameHandling.CamelCase;
            });

            app.UseMiddleware(typeof(ErrorHandlingMiddleware));
            app.UseMvc(
                routes =>
                {
                    routes.MapRoute(
                        name: "default",
                        template: "{controller=Home}/{action=Index}");
                });

            var connections = Configuration
               .GetSection("ConnectionStrings")
               .GetChildren()
               .ToDictionary(connection => connection.Key, connection => connection.Value, StringComparer.OrdinalIgnoreCase);

            DbSettingProviderManager
              .StartNewSession()
              .UseConnectionStrings(connections)
              .UseDefaultDbSettingProvider("~/App_Data")
              .Run();
        }
    }
}