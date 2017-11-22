using System;
using System.Collections.Generic;
using System.Fabric;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.ServiceFabric.Services.Communication.AspNetCore;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;
using ClientModels = Tailspin.Shared.Models.Client;
using ApiModels = Tailspin.Shared.Models.Api;
using Tailspin.SurveyResponseService.Models;
using Tailspin.SurveyResponseService.Store;
using Tailspin.SurveyResponseService.Configuration;
using Tailspin.SurveyAnalysisService.Client;
using Tailspin.SurveyResponseService.Helpers;
using Microsoft.WindowsAzure.Storage;

namespace Tailspin.SurveyResponseService
{
    /// <summary>
    /// The FabricRuntime creates an instance of this class for each service type instance. 
    /// </summary>
    internal sealed class SurveyResponseService : StatefulService
    {
        private IReliableConcurrentQueue<ClientModels.SurveyAnswer> surveyQueue = null;

        public SurveyResponseService(StatefulServiceContext context)
            : base(context)
        { }

        /// <summary>
        /// Optional override to create listeners (like tcp, http) for this service instance.
        /// </summary>
        /// <returns>The collection of listeners.</returns>
        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return new ServiceReplicaListener[]
            {
                new ServiceReplicaListener(serviceContext =>
                    new KestrelCommunicationListener(serviceContext, (url, listener) =>
                    {
                        ServiceEventSource.Current.ServiceMessage(serviceContext, $"Starting Kestrel on {url}");

                        return new WebHostBuilder()
                                    .UseKestrel()
                                    .ConfigureServices(
                                        services => services
                                            .AddSingleton<StatefulServiceContext>(serviceContext)
                                            .AddSingleton<IReliableStateManager>(this.StateManager))
                                    .UseContentRoot(Directory.GetCurrentDirectory())
                                    .UseStartup<Startup>()
                                    .UseApplicationInsights()
                                    .UseServiceFabricIntegration(listener, ServiceFabricIntegrationOptions.UseUniqueServiceUrl)
                                    .UseUrls(url)
                                    .Build();
                    }))
            };
        }

        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            int batchSize = 100;
            long delayMs = 20;

            surveyQueue = await this.StateManager.GetOrAddAsync<IReliableConcurrentQueue<ClientModels.SurveyAnswer>>("surveyQueue");
            ISurveyAnalysisService surveyAnalysisService = new Tailspin.SurveyAnalysisService.Client.SurveyAnalysisService();

            List<ClientModels.SurveyAnswer> processItems = new List<ClientModels.SurveyAnswer>();

            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    using (var tx = this.StateManager.CreateTransaction())
                    {
                        ConditionalValue<ClientModels.SurveyAnswer> ret;
                        
                        for (int i = 0; i < batchSize; ++i)
                        {
                            ret = await surveyQueue.TryDequeueAsync(tx, cancellationToken);

                            if (ret.HasValue)
                            {
                                processItems.Add(ret.Value.DeepCopy());
                            }
                            else
                                break;
                        }

                        if (processItems.Count > 0)
                        {
                            foreach (var sa in processItems)
                            {
                                var model = sa.ToSurveyAnswer();
                                model.CreatedOn = DateTime.UtcNow;

                                var container = new AzureBlobContainer<ApiModels.SurveyAnswer>(
                                    ServiceFabricConfiguration.GetCloudStorageAccount(),
                                    $"{model.SlugName}-answers");

                                try
                                {
                                    await container.SaveAsync(model.Id, model);
                                }
                                catch (StorageException ex)
                                {
                                    if (ex.Message.Contains("404"))
                                    {
                                        await container.EnsureExistsAsync();
                                        await container.SaveAsync(model.Id, model);
                                    }
                                    else
                                    {
                                        throw ex;
                                    }
                                }

                                await this.AppendSurveyAnswerIdToSurveyAnswerListAsync(model.SlugName, model.Id);

                                await surveyAnalysisService.MergeSurveyAnswerToAnalysisAsync(model.ToAnalysisServiceSurveyAnswer());
                            }

                            processItems.Clear();
                        }

                        await tx.CommitAsync();
                    }

                    await Task.Delay(TimeSpan.FromMilliseconds(delayMs), cancellationToken);
                }
            }
            catch (Exception ex)
            {
                ServiceEventSource.Current.ServiceRequestFailed(ex.ToString());
                throw;
            }
        }
        private async Task AppendSurveyAnswerIdToSurveyAnswerListAsync(string slugName, string surveyAnswerId)
        {
            var SurveyAnswersListContainerName = "surveyanswerslist";

            if (string.IsNullOrWhiteSpace(slugName))
            {
                throw new ArgumentException($"{nameof(slugName)} cannot be null, empty, or only whitespace");
            }

            if (string.IsNullOrWhiteSpace(surveyAnswerId))
            {
                throw new ArgumentException($"{nameof(surveyAnswerId)} cannot be null, empty, or only whitespace");
            }

            var answerListContainer = new AzureBlobContainer<List<string>>(
                                ServiceFabricConfiguration.GetCloudStorageAccount(), SurveyAnswersListContainerName);
            try
            {
                await SaveAsync(answerListContainer, slugName, surveyAnswerId);
            }
            catch (StorageException ex)
            {
                if (ex.Message.Contains("404"))
                {
                    await answerListContainer.EnsureExistsAsync();
                    await SaveAsync(answerListContainer, slugName, surveyAnswerId);
                }
                else
                {
                    throw ex;
                }
            }
        }

        private async Task SaveAsync(AzureBlobContainer<List<string>> answerListContainer, string slugName, string surveyAnswerId)
        {
            // NOTE: appending ID's into a blob doesn't scale well under heavy load. Other solution is left
            //       for the reader to implement.
            var answerIdList = await answerListContainer.GetAsync(slugName)
                .ConfigureAwait(false) ?? new List<string>();
            answerIdList.Add(surveyAnswerId);
            await answerListContainer.SaveAsync(slugName, answerIdList)
                .ConfigureAwait(false);
        }
    }
}
