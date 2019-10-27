using Microsoft.AspNetCore.Http;
using System;

namespace ClientManagerService.Exceptions
{
    /// <summary>
    /// Http response exception.
    /// Exception is handled by filter and converted to response with status code <see cref="Status"/>.
    /// </summary>
    public class HttpResponseException : Exception
    {
        /// <summary>
        /// Response status code.
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// Initializes <see cref="HttpResponseException"/>.
        /// </summary>
        /// <param name="message">Exception message.</param>
        /// <param name="statusCode">Status code.</param>
        public HttpResponseException(string message, int statusCode = StatusCodes.Status500InternalServerError)
            :base(message)
        {
            Status = statusCode;
        }
    }
}
