namespace Tailspin.Web
{
    using System.Collections.Generic;
    using Microsoft.WindowsAzure.Storage;
    using Autofac;
    using Tailspin.Web.Survey.Shared;
    using Tailspin.Web.Survey.Shared.Models;
    using Tailspin.Web.Survey.Shared.QueueMessages;
    using Tailspin.Web.Survey.Shared.Stores;
    using Tailspin.Web.Survey.Shared.Stores.AzureStorage;

    public class ContainerBootstrapperModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var initializationService = new InitializationStatusService { IsInitialized = false };
            builder.RegisterInstance(initializationService).As<IInitializationStatusService>();

            // registering IAzureTable types
            builder
                .Register(c => new AzureTable<SurveyRow>(c.Resolve<CloudStorageAccount>(), AzureConstants.Tables.Surveys))
                .As<IAzureTable<SurveyRow>>();
            builder
                .Register(c => new AzureTable<QuestionRow>(c.Resolve<CloudStorageAccount>(), AzureConstants.Tables.Questions))
                .As<IAzureTable<QuestionRow>>();

            // registering IAzureQueue types
            builder
                .Register(c => new AzureQueue<SurveyAnswerStoredMessage>(c.Resolve<CloudStorageAccount>(), AzureConstants.Queues.SurveyAnswerStoredStandard))
                .Named<IAzureQueue<SurveyAnswerStoredMessage>>(SubscriptionKind.Standard.ToString());
            builder
                .Register(c => new AzureQueue<SurveyAnswerStoredMessage>(c.Resolve<CloudStorageAccount>(), AzureConstants.Queues.SurveyAnswerStoredPremium))
                .Named<IAzureQueue<SurveyAnswerStoredMessage>>(SubscriptionKind.Premium.ToString());
            builder
                .Register(c => new AzureQueue<SurveyTransferMessage>(c.Resolve<CloudStorageAccount>(), AzureConstants.Queues.SurveyTransferRequest))
                .As<IAzureQueue<SurveyTransferMessage>>();

            // registering IAzureBlobContainer types
            builder
                .Register(c => new EntitiesBlobContainer<List<string>>(c.Resolve<CloudStorageAccount>(), AzureConstants.BlobContainers.SurveyAnswersLists))
                .As<IAzureBlobContainer<List<string>>>();
            builder
                .Register(c => new EntitiesBlobContainer<Tenant>(c.Resolve<CloudStorageAccount>(), AzureConstants.BlobContainers.Tenants))
                .As<IAzureBlobContainer<Tenant>>();
            builder
                .Register(c => new FilesBlobContainer(c.Resolve<CloudStorageAccount>(), AzureConstants.BlobContainers.Logos, "image/jpeg"))
                .As<IAzureBlobContainer<byte[]>>();
            builder
                .Register(c => new EntitiesBlobContainer<SurveyAnswersSummary>(c.Resolve<CloudStorageAccount>(), AzureConstants.BlobContainers.SurveyAnswersSummaries))
                .As<IAzureBlobContainer<SurveyAnswersSummary>>();

            // registering Store types
            builder
                .RegisterType<SurveyStore>().As<ISurveyStore>();
            builder
                .RegisterType<TenantStore>().As<ITenantStore>();
            builder
                .Register(c => new SurveyAnswerStore(
                    c.Resolve<ITenantStore>(),
                    c.Resolve<ISurveyAnswerContainerFactory>(),
                    c.ResolveNamed<IAzureQueue<SurveyAnswerStoredMessage>>(SubscriptionKind.Standard.ToString()),
                    c.ResolveNamed<IAzureQueue<SurveyAnswerStoredMessage>>(SubscriptionKind.Premium.ToString()),
                    c.Resolve<IAzureBlobContainer<List<string>>>()))
                    .As<ISurveyAnswerStore>();
            builder
                .RegisterType<SurveyTransferStore>().As<ISurveyTransferStore>();
            builder
                .RegisterType<SurveyAnswersSummaryStore>().As<ISurveyAnswersSummaryStore>();

            builder
                .RegisterType<EntitiesBlobContainer<SurveyAnswer>>()
                .As<IAzureBlobContainer<SurveyAnswer>>();

            builder.RegisterType<SurveyAnswerContainerFactory>()
                .As<ISurveyAnswerContainerFactory>();
        }
    }
}