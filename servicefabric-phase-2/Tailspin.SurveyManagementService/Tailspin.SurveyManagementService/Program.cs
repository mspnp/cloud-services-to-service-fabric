namespace Tailspin.SurveyManagementService
{
    using System;
    using System.Diagnostics;
    using System.Net;
    using System.Threading;
    using Microsoft.ServiceFabric.Services.Runtime;
    using Microsoft.WindowsAzure.Storage.Table;
    using Autofac;
    using Tailspin.SurveyManagementService.Configuration;
    using Tailspin.SurveyManagementService.Models;
    using Tailspin.SurveyManagementService.Store;

    public delegate IAzureBlobContainer<T> AzureBlobContainerFactory<T>(string containerName);
    public delegate IAzureTable<T> AzureTableFactory<T>(string tableName)
        where T : TableEntity, new();

    internal static class Program
    {
        private static IContainer SetupContainer()
        {
            ContainerBuilder builder = new ContainerBuilder();
            var cloudStorageAccount = ServiceFabricConfiguration.GetCloudStorageAccount();
            builder.RegisterInstance(cloudStorageAccount);
            builder.RegisterGeneric(typeof(AzureBlobContainer<>))
                .As(typeof(IAzureBlobContainer<>));
            builder.RegisterGeneric(typeof(AzureTable<>))
                .As(typeof(IAzureTable<>));
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

                ServiceRuntime.RegisterServiceAsync("SurveyManagementServiceType",
                    context => new SurveyManagementService(context,
                                    container.Resolve<AzureTableFactory<SurveyInformationRow>>(),
                                    container.Resolve<AzureBlobContainerFactory<Models.Survey>>())).GetAwaiter().GetResult();

                ServiceEventSource.Current.ServiceTypeRegistered(Process.GetCurrentProcess().Id, typeof(SurveyManagementService).Name);

                // Prevents this host process from terminating so services keep running.
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
