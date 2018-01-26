namespace Tailspin.Web
{
    using System;
    using Autofac;
    using Tailspin.SurveyAnalysisService.Client;
    using Tailspin.SurveyAnswerService.Client;
    using Tailspin.SurveyManagementService.Client;

    public class ContainerBootstrapperModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder
                .Register(c => new SurveyManagementService())
                .As<ISurveyManagementService>();
            builder.Register(c => new SurveyAnswerService())
                .As<ISurveyAnswerService>();
            builder.Register(c => new SurveyAnalysisService())
                .As<ISurveyAnalysisService>();
        }
    }
}