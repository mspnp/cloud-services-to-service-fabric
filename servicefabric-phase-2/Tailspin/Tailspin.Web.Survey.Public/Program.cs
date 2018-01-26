using System;
using System.Diagnostics;
using System.Net;
using System.Threading;
using Microsoft.ServiceFabric.Services.Runtime;

namespace Tailspin.Web.Survey.Public
{
    internal static class Program
    {
        /// <summary>
        /// This is the entry point of the service host process.
        /// </summary>
        private static void Main(string[] args)
        {
            try
            {
                ServiceRuntime.RegisterServiceAsync("Tailspin.Web.Survey.PublicType",
                        context => new WebSurveyPublic(context)).GetAwaiter().GetResult();

                ServiceEventSource.Current.ServiceTypeRegistered(Process.GetCurrentProcess().Id, typeof(WebSurveyPublic).Name);

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
