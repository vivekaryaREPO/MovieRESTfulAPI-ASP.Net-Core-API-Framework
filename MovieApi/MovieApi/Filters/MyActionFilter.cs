using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieApi.Filters
{
    public class MyActionFilter : IActionFilter

    {
        private readonly ILogger<MyActionFilter> logger;
        public MyActionFilter(ILogger<MyActionFilter> _logger)
        {
            this.logger = _logger;
        }
        void IActionFilter.OnActionExecuted(ActionExecutedContext context)
        {
            //this.logger.LogInformation("Hello from OnActionExecuted filter:");
        }

        void IActionFilter.OnActionExecuting(ActionExecutingContext context)
        {
            //this.logger.LogInformation("Hello from OnActionExecuting filter:");
        }
    }
}
