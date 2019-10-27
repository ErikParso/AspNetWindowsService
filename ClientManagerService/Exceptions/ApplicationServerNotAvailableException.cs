namespace ClientManagerService.Exceptions
{
    /// <summary>
    /// Throw this exception when application server is unavailable.
    /// </summary>
    public class ApplicationServerNotAvailableException : HttpResponseException
    {
        /// <summary>
        /// Initializes <see cref="ApplicationServerNotAvailableException"/>
        /// </summary>
        /// <param name="message">Exception message.</param>
        public ApplicationServerNotAvailableException(string message) 
            : base(message)
        {

        }
    }
}
