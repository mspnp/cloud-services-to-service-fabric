namespace Tailspin.Web
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Security.Permissions;
    using Microsoft.Practices.Unity;
    using Microsoft.WindowsAzure.ServiceRuntime;
    using Survey.Shared.Stores;
    using Tailspin.Web.Survey.Shared.Helpers;

    public class WebRole : RoleEntryPoint
    {
        [SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands", Justification = "This is the default code from the project template for the Windows Azure SDK.")]
        public override bool OnStart()
        {
            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.
            RoleEnvironment.Changing += RoleEnvironmentChanging;
            RoleEnvironment.Changed += RoleEnvironmentChanged;
            
            using (var container = new UnityContainer())
            {
                ContainerBootstraper.RegisterTypes(container, true);

                container.Resolve<ISurveyStore>().InitializeAsync().Wait();
                container.Resolve<ISurveyAnswerStore>().InitializeAsync().Wait();
                container.Resolve<ISurveyAnswersSummaryStore>().InitializeAsync().Wait();
                container.Resolve<ISurveyTransferStore>().InitializeAsync().Wait();

                var tenantStore = container.Resolve<ITenantStore>();
                tenantStore.InitializeAsync().Wait();
            }

            return base.OnStart();
            }

        private static void RoleEnvironmentChanging(object sender, RoleEnvironmentChangingEventArgs e)
        {
            // for any configuration setting change except TraceEventTypeFilter
            if (e.Changes.OfType<RoleEnvironmentConfigurationSettingChange>().Any(change => change.ConfigurationSettingName != "TraceEventTypeFilter"))
            {
                // Set e.Cancel to true to restart this role instance
                e.Cancel = true;
            }
        }

        private static void RoleEnvironmentChanged(object sender, RoleEnvironmentChangedEventArgs e)
        {
            // configure trace listener for any changes to EnableTableStorageTraceListener 
            if (e.Changes.OfType<RoleEnvironmentConfigurationSettingChange>().Any(change => change.ConfigurationSettingName == "TraceEventTypeFilter"))
            {
                ConfigureTraceListener(RoleEnvironment.GetConfigurationSettingValue("TraceEventTypeFilter"));
            }
        }

        [EnvironmentPermission(SecurityAction.LinkDemand, Unrestricted = true)]
        private static void ConfigureTraceListener(string traceEventTypeFilter)
        {
            SourceLevels sourceLevels;
            if (Enum.TryParse(traceEventTypeFilter, true, out sourceLevels))
            {
                TraceHelper.Configure(sourceLevels);
            }
        }
    }
}
