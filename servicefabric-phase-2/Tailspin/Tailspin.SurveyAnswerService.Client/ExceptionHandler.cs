using Microsoft.ServiceFabric.Services.Communication.Client;

namespace Tailspin.SurveyAnswerService.Client
{
    class ExceptionHandler : IExceptionHandler
    {
        public bool TryHandleException(ExceptionInformation exceptionInformation, OperationRetrySettings retrySettings, out ExceptionHandlingResult result)
        {
            // if exceptionInformation.Exception is known and is transient (can be retried without re-resolving)
            //TODO: Add decision logic to determine whether exception is transient or not.
            result = new ExceptionHandlingRetryResult(exceptionInformation.Exception, true, retrySettings, retrySettings.DefaultMaxRetryCount);
            return true;

            //// if exceptionInformation.Exception is known and is not transient (indicates a new service endpoint address must be resolved)
            //result = new ExceptionHandlingRetryResult(exceptionInformation.Exception, false, retrySettings, retrySettings.DefaultMaxRetryCount);
            //return true;

            //// if exceptionInformation.Exception is unknown (let the next IExceptionHandler attempt to handle it)
            //result = null;
            //return false;
        }
    }
}
