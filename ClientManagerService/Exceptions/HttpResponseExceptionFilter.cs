using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ClientManagerService.Exceptions
{
    /// <summary>
    /// Filter handles <see cref="HttpResponseException"/> and produces response with status code defined by excetpion.
    /// </summary>
    public class HttpResponseExceptionFilter : IActionFilter, IOrderedFilter
    {
        /// <summary>
        /// Filter order.
        /// </summary>
        public int Order { get; set; } = int.MaxValue - 1000;

        /// <summary>
        /// On Action executing.
        /// </summary>
        /// <param name="context"></param>
        public void OnActionExecuting(ActionExecutingContext context) 
        {
        
        }

        /// <summary>
        /// Exception handling.
        /// </summary>
        /// <param name="context"></param>
        public void OnActionExecuted(ActionExecutedContext context)
        {
            var httpResponseException = context.Exception as HttpResponseException;
            if (httpResponseException != null)
            {
                context.Result = new ObjectResult(httpResponseException.Message)
                {
                    StatusCode = httpResponseException.Status,
                };
                context.ExceptionHandled = true;
            }
        }
    }
}
