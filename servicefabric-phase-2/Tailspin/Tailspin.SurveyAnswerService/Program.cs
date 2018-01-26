using Microsoft.ServiceFabric.Services.Runtime;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Tailspin.SurveyAnswerService.Configuration;
using Tailspin.SurveyAnswerService.Store;
using Tailspin.SurveyAnalysisService.Client;

namespace Tailspin.SurveyAnswerService
{
    internal static class Program
    {
        private static IContainer SetupContainer()
        {
            ContainerBuilder builder = new ContainerBuilder();
            var cloudStorageAccount = ServiceFabricConfiguration.GetCloudStorageAccount();
            builder.RegisterInstance(cloudStorageAccount);
            builder.RegisterGeneric(typeof(AzureBlobContainer<>))
                .As(typeof(IAzureBlobContainer<>));
            builder.Register(c => new Tailspin.SurveyAnalysisService.Client.SurveyAnalysisService())
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

                ServiceRuntime.RegisterServiceAsync("SurveyAnswerServiceType",
                    context => new SurveyAnswerService(context)).GetAwaiter().GetResult();

                ServiceEventSource.Current.ServiceTypeRegistered(Process.GetCurrentProcess().Id, typeof(SurveyAnswerService).Name);

                // Prevents this host process from terminating so services keeps running. 
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
