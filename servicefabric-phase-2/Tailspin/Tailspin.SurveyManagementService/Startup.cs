using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Autofac;
using Tailspin.SurveyManagementService.Configuration;
using Tailspin.SurveyManagementService.Store;
using Autofac.Extensions.DependencyInjection;
using Microsoft.WindowsAzure.Storage;
using Autofac.Core;

namespace Tailspin.SurveyManagementService
{
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
            var cosmosAccount = ServiceFabricConfiguration.GetCloudCosmosAccount();
            builder.Register(c => cloudStorageAccount).Named<CloudStorageAccount>("azure-storage");
            builder.Register(c => cosmosAccount).Named<CloudStorageAccount>("cosmos-storage");
            builder
                .RegisterGeneric(typeof(AzureBlobContainer<>))
                .WithParameter(
                    new ResolvedParameter(
                        (p, c) => p.Name == "account",
                        (p, c) => c.ResolveNamed<CloudStorageAccount>("azure-storage")))
                .As(typeof(IAzureBlobContainer<>));
            //builder.RegisterGeneric(typeof(AzureBlobContainer<>))
            //    .As(typeof(IAzureBlobContainer<>));
            builder
                .RegisterGeneric(typeof(AzureTable<>))
                .WithParameter(
                    new ResolvedParameter(
                        (p, c) => p.Name == "account",
                        (p, c) => c.ResolveNamed<CloudStorageAccount>("cosmos-storage")))
                .As(typeof(IAzureTable<>));
            //builder.RegisterGeneric(typeof(AzureTable<>))
            //    .As(typeof(IAzureTable<>));
            builder.Populate(services);
            return builder.Build();
        }
    }
}
