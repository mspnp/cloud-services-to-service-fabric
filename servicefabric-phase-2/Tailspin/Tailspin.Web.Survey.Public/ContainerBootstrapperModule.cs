namespace Tailspin.Web.Survey.Public
{
    using System;
    using Autofac;
    using Microsoft.ServiceFabric.Services.Remoting.Client;
    using SurveyAnswerService.Client;
    using SurveyManagementService.Client;

    public class ContainerBootstrapperModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder
                .Register(c => ServiceProxy.Create<ISurveyManagementService>(new Uri("fabric:/Tailspin.SurveyManagementService.Application/SurveyManagementService")))
                .As<ISurveyManagementService>();
            builder
                .Register(c => ServiceProxy.Create<ISurveyAnswerService>(new Uri("fabric:/Tailspin.SurveyAnswerService.Application/SurveyAnswerService")))
                .As<ISurveyAnswerService>();
        }
    }
}