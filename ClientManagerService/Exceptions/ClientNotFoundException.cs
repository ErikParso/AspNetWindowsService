using Microsoft.AspNetCore.Http;

namespace ClientManagerService.Exceptions
{
    /// <summary>
    /// Throw this exception when Client with id is not found.
    /// </summary>
    public class ClientNotFoundException : HttpResponseException
    {
        /// <summary>
        /// Initiallizes <see cref="ClientNotFoundException"/>
        /// </summary>
        /// <param name="clientId">Client id.</param>
        public ClientNotFoundException(string clientId) 
            : base($"Client with id '{clientId}' was not found.", StatusCodes.Status404NotFound)
        {

        }
    }
}
