using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading;
using Microsoft.ServiceFabric.Services.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using Autofac;
using Tailspin.SurveyAnswerService.Configuration;
using Tailspin.SurveyAnswerService.Store;
using Tailspin.SurveyAnalysisService.Client;

namespace Tailspin.SurveyAnswerService
{
    public delegate IAzureBlobContainer<T> AzureBlobContainerFactory<T>(string containerName);

    internal static class Program
    {
        private static IContainer SetupContainer()
        {
            ContainerBuilder builder = new ContainerBuilder();
            var cloudStorageAccount = ServiceFabricConfiguration.GetCloudStorageAccount();
            builder.RegisterInstance(cloudStorageAccount);
            builder.RegisterGeneric(typeof(AzureBlobContainer<>))
                .As(typeof(IAzureBlobContainer<>));
            builder.Register(c => ServiceProxy.Create<ISurveyAnalysisService>(new Uri("fabric:/Tailspin.SurveyAnalysisService.Application/SurveyAnalysisService")))
                .As<ISurveyAnalysisService>();
            return builder.Build();
        }

        /// <summary>
        /// This is the entry point of the service host process.
        /// </summary>
        private static void Main()
        {
            try
            {
                // The ServiceManifest.XML file defines one or more service type names.
                // Registering a service maps a service type name to a .NET type.
                // When Service Fabric creates an instance of this service type,
                // an instance of the class is created in this host process.

                ServicePointManager.DefaultConnectionLimit = int.MaxValue;
                ServicePointManager.UseNagleAlgorithm = false;
                ServicePointManager.Expect100Continue = false;

                var container = SetupContainer();
                ServiceRuntime.RegisterServiceAsync("SurveyAnswerServiceType",
                    context => new SurveyAnswerService(context,
                    container.Resolve<AzureBlobContainerFactory<Models.SurveyAnswer>>(),
                    container.Resolve<AzureBlobContainerFactory<List<string>>>(),
                    container.Resolve<ISurveyAnalysisService>())).GetAwaiter().GetResult();

                ServiceEventSource.Current.ServiceTypeRegistered(Process.GetCurrentProcess().Id, typeof(SurveyAnswerService).Name);

                Thread.Sleep(Timeout.Infinite);
            }
            catch (Exception e)
            {
                ServiceEventSource.Current.ServiceHostInitializationFailed(e.ToString());
                throw;
            }
        }
    }
}
