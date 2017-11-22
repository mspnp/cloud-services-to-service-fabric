using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Autofac;
using Tailspin.SurveyResponseService.Configuration;
using Tailspin.SurveyResponseService.Store;
using Tailspin.SurveyAnalysisService.Client;
using Autofac.Extensions.DependencyInjection;

namespace Tailspin.SurveyResponseService
{
    public delegate IAzureBlobContainer<T> AzureBlobContainerFactory<T>(string containerName);

    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();

            var container = SetupContainer(services);
            return new AutofacServiceProvider(container);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseMvc();
        }

        private static IContainer SetupContainer(IServiceCollection services)
        {
            ContainerBuilder builder = new ContainerBuilder();
            var cloudStorageAccount = ServiceFabricConfiguration.GetCloudStorageAccount();
            builder.RegisterInstance(cloudStorageAccount);
            builder.RegisterGeneric(typeof(AzureBlobContainer<>))
                .As(typeof(IAzureBlobContainer<>));
            builder.Register(c => new Tailspin.SurveyAnalysisService.Client.SurveyAnalysisService())
                .As<ISurveyAnalysisService>();
            builder.Populate(services);
            return builder.Build();
        }
    }
}
