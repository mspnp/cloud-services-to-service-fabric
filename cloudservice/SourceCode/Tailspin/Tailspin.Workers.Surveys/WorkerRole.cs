namespace Tailspin.Workers.Surveys
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Net;
    using System.Security.Permissions;
    using System.Threading;
    using Microsoft.Practices.Unity;
    using Microsoft.WindowsAzure.ServiceRuntime;
    using Tailspin.Web.Survey.Shared.Helpers;
    using Tailspin.Web.Survey.Shared.Models;
    using Tailspin.Web.Survey.Shared.Stores;
    using Tailspin.Workers.Surveys.Commands;
    using Tailspin.Workers.Surveys.QueueHandlers;
    using Web.Survey.Shared.QueueMessages;
    using Web.Survey.Shared.Stores.AzureStorage;

    public sealed class WorkerRole : RoleEntryPoint, IDisposable
    {
        private IUnityContainer container;

        [SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands", Justification = "This is the default code from the project template for the Windows Azure SDK.")]
        public override bool OnStart()
        {            
            // The number of connections depends on the particular usage in each application
            ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.
            RoleEnvironment.Changing += RoleEnvironmentChanging;
            RoleEnvironment.Changed += RoleEnvironmentChanged;

            this.container = new UnityContainer();
            ContainerBootstraper.RegisterTypes(this.container, true);
            
            this.container.Resolve<ISurveyAnswerStore>().InitializeAsync().Wait();
            this.container.Resolve<ISurveyAnswersSummaryStore>().InitializeAsync().Wait();
            this.container.Resolve<ITenantStore>().InitializeAsync().Wait();
            this.container.Resolve<ISurveyStore>().InitializeAsync().Wait();
            this.container.Resolve<ISurveyTransferStore>().InitializeAsync().Wait();

            return base.OnStart();
        }

        public override void Run()
        {
            //// The time interval for checking the queues have to be tuned depending on the scenario and the expected workload
            var standardQueue = this.container.Resolve<IAzureQueue<SurveyAnswerStoredMessage>>(SubscriptionKind.Standard.ToString());
            var premiumQueue = this.container.Resolve<IAzureQueue<SurveyAnswerStoredMessage>>(SubscriptionKind.Premium.ToString());
            
            BatchMultipleQueueHandler
                .For(premiumQueue, 8)
                .AndFor(standardQueue, 8)
                .Every(TimeSpan.FromSeconds(10))
                .Do(this.container.Resolve<UpdatingSurveyResultsSummaryCommand>());

            QueueHandler
                .For(this.container.Resolve<IAzureQueue<SurveyTransferMessage>>())
                .Every(TimeSpan.FromSeconds(5))
                .Do(this.container.Resolve<TransferSurveysToSqlAzureCommand>());

            while (true)
            {
                Thread.Sleep(TimeSpan.FromSeconds(5));
            }
        }

        public void Dispose()
        {
            this.container.Dispose();
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

        [EnvironmentPermissionAttribute(SecurityAction.LinkDemand, Unrestricted = true)]
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
