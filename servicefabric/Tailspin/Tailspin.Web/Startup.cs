using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Redis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using StackExchange.Redis;
using Tailspin.Web.Survey.Shared.Helpers;
using Tailspin.Web.Survey.Shared.Stores;

namespace Tailspin.Web
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            // Add framework services.

            // Persist data protection keys in Redis
            var redisConnectionString = ServiceFabricConfiguration.GetConfigurationSettingValue("ConnectionStrings", "RedisCacheConnectionString", "YourRedisCacheConnectionString");
            services.AddDataProtection()
              .PersistKeysToRedis(ConnectionMultiplexer.Connect(redisConnectionString), "DataProtection-Keys");

            // Add Redis-based distributed cache
            services.AddSingleton<IDistributedCache>(serviceProvider =>
                        new RedisCache(new RedisCacheOptions
                        {
                            Configuration= redisConnectionString 
                        }));
            services.AddSession();
            services.AddMvc();

            var account = ServiceFabricConfiguration.GetCloudStorageAccount();

            // Add Autofac
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterModule<ContainerBootstrapperModule>();
            containerBuilder.Populate(services);
            containerBuilder.RegisterInstance(account);
            var container = containerBuilder.Build();

            container.Resolve<ISurveyStore>().InitializeAsync().Wait();
            container.Resolve<ISurveyAnswerStore>().InitializeAsync().Wait();
            container.Resolve<ISurveyAnswersSummaryStore>().InitializeAsync().Wait();
            container.Resolve<ISurveyTransferStore>().InitializeAsync().Wait();
            container.Resolve<ITenantStore>().InitializeAsync().Wait();

            return new AutofacServiceProvider(container);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseSession();

            app.UseMvc(routes =>
            {
                AppRoutes.RegisterRoutes(routes);
            });
        }
    }
}
