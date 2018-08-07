namespace Chatbot.WebApi
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Chatbot.Configurations;
    using Chatbot.Services;
    using Chatbot.WebApi.Middlewares;
    using Fanex.Caching;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Swashbuckle.AspNetCore.Swagger;

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

            var chatbotConfigs = new ChatbotConfigs(Configuration);

            services.AddSingleton<MessengerFactory, MessengerFactoryImpl>();
            services.AddSingleton<SettingService, SettingServiceImpl>();
            services.AddSingleton<IDynamicRepository, DynamicRepository>();
            services.AddSingleton<ICacheService, CacheService>();
            services.AddSingleton<IMessageBuilder, MessageBuilder>();
            services.AddSingleton<IMessageLogger, MessageLogger>();
            services.AddSingleton<IMessageSender, MessageSender>();
            services.AddSingleton<CommandService, CommandServiceImpl>();
            services.AddSingleton(chatbotConfigs);
            services.AddSingleton<IChatbotConfigs>(new ChatbotConfigs(Configuration));
            services.AddSingleton(new Dictionary<int, ICommandHandler>
            {
                { chatbotConfigs.OutstandingFunctionId, new OutstandingCommandHandler() },
                { chatbotConfigs.WinlossFunctionId, new WinLossCommandHandler() },
                { chatbotConfigs.BalanceFunctionId, new BalanceInfoCommandHandler() }
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Chatbot API", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                var basePath = Environment.GetEnvironmentVariable("ASPNETCORE_APPL_PATH");
                if (basePath == null)
                {
                    basePath = "/";
                }

                c.SwaggerEndpoint($"{basePath}swagger/v1/swagger.json", "Chatbot API Docs");
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