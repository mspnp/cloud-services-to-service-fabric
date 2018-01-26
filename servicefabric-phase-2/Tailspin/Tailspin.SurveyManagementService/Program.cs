using Microsoft.ServiceFabric.Services.Runtime;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Tailspin.SurveyManagementService.Configuration;
using Tailspin.SurveyManagementService.Store;

public delegate IAzureBlobContainer<T> AzureBlobContainerFactory<T>(string containerName);
public delegate IAzureTable<T> AzureTableFactory<T>(string tableName)
    where T : TableEntity, new();

namespace Tailspin.SurveyManagementService
{
    internal static class Program
    {
        

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

                ServiceRuntime.RegisterServiceAsync("SurveyManagementServiceType",
                    context => new SurveyManagementService(context)).GetAwaiter().GetResult();

                ServiceEventSource.Current.ServiceTypeRegistered(Process.GetCurrentProcess().Id, typeof(SurveyManagementService).Name);

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
