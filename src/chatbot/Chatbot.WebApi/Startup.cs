namespace Chatbot.WebApi
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Chatbot.Services;
    using Chatbot.WebApi.Middlewares;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

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
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMiddleware(typeof(ErrorHandlingMiddleware));
            app.UseMvc();

            DbSettingProviderManager
              .StartNewSession()
              .UseConnectionStrings(new Dictionary<string, string>
              {
                  { "CHATBOT_DB", Configuration["CHATBOT_DB"] }
              })
              .UseDefaultDbSettingProvider("~/App_Data")
              .Run();

        }
    }
}
