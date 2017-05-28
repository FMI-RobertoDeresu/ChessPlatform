using ChessPlatform.Logging;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ChessPlatform.Filters
{
    public class RequestHistoryLogFilterAttribute : ActionFilterAttribute
    {
        private readonly IRequestHistoryLogger _logger;

        public RequestHistoryLogFilterAttribute(IRequestHistoryLogger logger)
        {
            _logger = logger;
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            _logger.LogRequest();
        }
    }
}