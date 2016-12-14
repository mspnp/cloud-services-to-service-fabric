namespace Tailspin.AnswerAnalysisService
{
    using System;
    using System.Collections.Generic;
    using System.Fabric;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Services.Communication.Runtime;
    using StatelessService = Microsoft.ServiceFabric.Services.Runtime.StatelessService;
    using Autofac;
    using Tailspin.AnswerAnalysisService.Commands;
    using Tailspin.AnswerAnalysisService.QueueHandlers;
    using Tailspin.Web.Survey.Shared.Helpers;
    using Tailspin.Web.Survey.Shared.Models;
    using Tailspin.Web.Survey.Shared.QueueMessages;
    using Tailspin.Web.Survey.Shared.Stores;
    using Tailspin.Web.Survey.Shared.Stores.AzureStorage;

    /// <summary>
    /// An instance of this class is created for each service instance by the Service Fabric runtime.
    /// </summary>
    internal sealed class AnswerAnalysisService : StatelessService, IDisposable
    {
        private readonly IContainer _container;

        public AnswerAnalysisService(StatelessServiceContext context)
            : base(context)
        {
            // The number of connections depends on the particular usage in each application
            ServicePointManager.DefaultConnectionLimit = 12;

            var account = ServiceFabricConfiguration.GetCloudStorageAccount();

            // TODO: Initialize DI container here or somewhere else since for each instance this constructor is invoked
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterModule<ContainerBootstrapperModule>();
            containerBuilder.RegisterInstance(account);
            _container = containerBuilder.Build();

            _container.Resolve<ISurveyAnswerStore>().InitializeAsync().Wait();
            _container.Resolve<ISurveyAnswersSummaryStore>().InitializeAsync().Wait();
            _container.Resolve<ITenantStore>().InitializeAsync().Wait();
            _container.Resolve<ISurveyStore>().InitializeAsync().Wait();
            _container.Resolve<ISurveyTransferStore>().InitializeAsync().Wait();
        }

        /// <summary>
        /// Optional override to create listeners (e.g., TCP, HTTP) for this service replica to handle client or user requests.
        /// </summary>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return new ServiceInstanceListener[0];
        }

        /// <summary>
        /// This is the main entry point for your service instance.
        /// </summary>
        /// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service instance.</param>
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            // The time interval for checking the queues have to be tuned depending on the scenario and the expected workload
            var standardQueue = _container.ResolveNamed<IAzureQueue<SurveyAnswerStoredMessage>>(SubscriptionKind.Standard.ToString());
            var premiumQueue = _container.ResolveNamed<IAzureQueue<SurveyAnswerStoredMessage>>(SubscriptionKind.Premium.ToString());

            BatchMultipleQueueHandler
                .For(premiumQueue, 8)
                .AndFor(standardQueue, 8)
                .Every(TimeSpan.FromSeconds(10))
                .Do(_container.Resolve<UpdatingSurveyResultsSummaryCommand>(), cancellationToken);

            QueueHandler
                .For(_container.Resolve<IAzureQueue<SurveyTransferMessage>>())
                .Every(TimeSpan.FromSeconds(5))
                .Do(_container.Resolve<TransferSurveysToSqlAzureCommand>(), cancellationToken);

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();
                ServiceEventSource.Current.ServiceMessage(this.Context, "Polling the queue...");
                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            }
        }

        public void Dispose()
        {
            _container.Dispose();
        }
    }
}
