using MediatR;

namespace ClientManagerService.Requests
{
    /// <summary>
    /// Download Client Manager installer from Production Server request.
    /// </summary>
    public class DownloadClientManagerRequest : IRequest<string>
    {

    }
}
