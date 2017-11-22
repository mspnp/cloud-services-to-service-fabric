namespace Tailspin.Web.Survey.Public
{
    using Autofac;
    using Tailspin.SurveyAnswerService.Client;
    using Tailspin.SurveyManagementService.Client;
    using Tailspin.SurveyResponseService.Client;

    public class ContainerBootstrapperModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder
                .Register(c => new SurveyManagementService())
                .As<ISurveyManagementService>();
            builder
                .Register(c => new SurveyAnswerService())
                .As<ISurveyAnswerService>();
            builder
                .Register(c => new SurveyResponseService())
                .As<ISurveyResponseService>();
            
        }
    }
}