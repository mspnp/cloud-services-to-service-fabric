namespace Tailspin.Web
{
    using System;
    using Autofac;
    using Microsoft.ServiceFabric.Services.Remoting.Client;
    using Tailspin.SurveyAnalysisService.Client;
    using Tailspin.SurveyAnswerService.Client;
    using Tailspin.SurveyManagementService.Client;

    public class ContainerBootstrapperModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder
                .Register(c => ServiceProxy.Create<ISurveyManagementService>(new Uri("fabric:/Tailspin.SurveyManagementService.Application/SurveyManagementService")))
                .As<ISurveyManagementService>();
            builder.Register(c => ServiceProxy.Create<ISurveyAnswerService>(new Uri("fabric:/Tailspin.SurveyAnswerService.Application/SurveyAnswerService")))
                .As<ISurveyAnswerService>();
            builder.Register(c => ServiceProxy.Create<ISurveyAnalysisService>(new Uri("fabric:/Tailspin.SurveyAnalysisService.Application/SurveyAnalysisService")))
                .As<ISurveyAnalysisService>();
        }
    }
}