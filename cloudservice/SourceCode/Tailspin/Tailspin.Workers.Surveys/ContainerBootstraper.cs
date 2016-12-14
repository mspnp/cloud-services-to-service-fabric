namespace Tailspin.Workers.Surveys
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.Practices.Unity;
    using Tailspin.Web.Survey.Shared.DataExtensibility;
    using Tailspin.Web.Survey.Shared.Helpers;
    using Tailspin.Web.Survey.Shared.QueueMessages;
    using Tailspin.Web.Survey.Shared.Stores.Azure;
    using Tailspin.Workers.Surveys.Commands;
    using Web.Survey.Shared;
    using Web.Survey.Shared.Models;
    using Web.Survey.Shared.Stores;
    using Web.Survey.Shared.Stores.AzureStorage;

    public static class ContainerBootstraper
    {
        public static void RegisterTypes(IUnityContainer container)
        {
            RegisterTypes(container, true);
        }

        [SuppressMessage("Microsoft.Reliability", "CA2000:Microsoft.DisposeObjectsBeforeLosingScope", Justification = "This container is used by the main container and cannot be disposed.")]
        public static void RegisterTypes(IUnityContainer container, bool roleInitialization)
        {
            var account = CloudConfiguration.GetStorageAccount("DataConnectionString");

            container.RegisterInstance(account);

            // http://msdn.microsoft.com/en-us/library/hh680900(v=pandp.50).aspx
            container.RegisterInstance<IRetryPolicyFactory>(roleInitialization
                ? new DefaultRetryPolicyFactory() as IRetryPolicyFactory
                : new ConfiguredRetryPolicyFactory() as IRetryPolicyFactory);

            container.RegisterType<IDictionary<string, TenantSurveyProcessingInfo>, Dictionary<string, TenantSurveyProcessingInfo>>(new InjectionConstructor());

            var cloudStorageAccountType = typeof(Microsoft.WindowsAzure.Storage.CloudStorageAccount);
            var retryPolicyFactoryProperty = new InjectionProperty("RetryPolicyFactory", typeof(IRetryPolicyFactory));

            // registering IAzureTable types
            container
                .RegisterType<IAzureTable<SurveyRow>, AzureTable<SurveyRow>>(
                    new InjectionConstructor(cloudStorageAccountType, AzureConstants.Tables.Surveys))
                .RegisterType<IAzureTable<QuestionRow>, AzureTable<QuestionRow>>(
                    new InjectionConstructor(cloudStorageAccountType, AzureConstants.Tables.Questions))
                .RegisterType<IUDFAzureTable, UDFAzureTable>(
                    new InjectionConstructor(cloudStorageAccountType, AzureConstants.Tables.SurveyExtensions));

            // registering IAzureQueue types
            container
                .RegisterType<IAzureQueue<SurveyAnswerStoredMessage>, AzureQueue<SurveyAnswerStoredMessage>>(
                    SubscriptionKind.Standard.ToString(),
                    new InjectionConstructor(cloudStorageAccountType, AzureConstants.Queues.SurveyAnswerStoredStandard))
                .RegisterType<IAzureQueue<SurveyAnswerStoredMessage>, AzureQueue<SurveyAnswerStoredMessage>>(
                    SubscriptionKind.Premium.ToString(),
                    new InjectionConstructor(cloudStorageAccountType, AzureConstants.Queues.SurveyAnswerStoredPremium))
                .RegisterType<IAzureQueue<SurveyTransferMessage>, AzureQueue<SurveyTransferMessage>>(
                    new InjectionConstructor(cloudStorageAccountType, AzureConstants.Queues.SurveyTransferRequest, TimeSpan.FromMinutes(1)));

            // registering IAzureBlobContainer types
            container
                .RegisterType<IAzureBlobContainer<byte[]>, FilesBlobContainer>(
                    new InjectionConstructor(cloudStorageAccountType, AzureConstants.BlobContainers.Logos, "image/jpeg"))
                .RegisterType<IAzureBlobContainer<SurveyAnswersSummary>, EntitiesBlobContainer<SurveyAnswersSummary>>(
                    new InjectionConstructor(cloudStorageAccountType, AzureConstants.BlobContainers.SurveyAnswersSummaries))
                .RegisterType<IAzureBlobContainer<List<string>>, EntitiesBlobContainer<List<string>>>(
                    new InjectionConstructor(cloudStorageAccountType, AzureConstants.BlobContainers.SurveyAnswersLists))
                .RegisterType<IAzureBlobContainer<Tenant>, EntitiesBlobContainer<Tenant>>(
                    new InjectionConstructor(cloudStorageAccountType, AzureConstants.BlobContainers.Tenants));

            var cacheEnabledProperty = new InjectionProperty("CacheEnabled", !roleInitialization);

            // registering Store types
            container
                .RegisterType<ISurveyStore, SurveyStore>(cacheEnabledProperty)
                .RegisterType<ITenantStore, TenantStore>(cacheEnabledProperty)
                .RegisterType<ISurveyAnswerStore, SurveyAnswerStore>(new InjectionFactory((c, t, s) => new SurveyAnswerStore(
                    container.Resolve<ITenantStore>(),
                    container.Resolve<ISurveyAnswerContainerFactory>(),
                    container.Resolve<IAzureQueue<SurveyAnswerStoredMessage>>(SubscriptionKind.Standard.ToString()),
                    container.Resolve<IAzureQueue<SurveyAnswerStoredMessage>>(SubscriptionKind.Premium.ToString()),
                    container.Resolve<IAzureBlobContainer<List<string>>>())))
                .RegisterType<ISurveyAnswersSummaryStore, SurveyAnswersSummaryStore>()
                .RegisterType<ISurveySqlStore, SurveySqlStore>()
                .RegisterType<ISurveyTransferStore, SurveyTransferStore>();

            // Container for resolving the survey answer containers
            var surveyAnswerBlobContainerResolver = new UnityContainer();

            surveyAnswerBlobContainerResolver.RegisterInstance(account);

            surveyAnswerBlobContainerResolver.RegisterType<IAzureBlobContainer<SurveyAnswer>, EntitiesBlobContainer<SurveyAnswer>>(
                new InjectionConstructor(cloudStorageAccountType, typeof(string)));

            container.RegisterType<ISurveyAnswerContainerFactory, SurveyAnswerContainerFactory>(
                new InjectionConstructor(surveyAnswerBlobContainerResolver));
        }
    }
}