using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieApi.Filters
{
    public class MyExceptionFilter:ExceptionFilterAttribute
    {
        private readonly ILogger<MyExceptionFilter> logger;
        public MyExceptionFilter(ILogger<MyExceptionFilter> _logger)
        {
            this.logger = _logger;
        }

        public override void OnException(ExceptionContext context)
        {
            this.logger.LogError(context.Exception,context.Exception.Message+"HELLO EXCEPTION FROM VIVEK ARYA**********************************8");
            base.OnException(context);
        }
    }
}
