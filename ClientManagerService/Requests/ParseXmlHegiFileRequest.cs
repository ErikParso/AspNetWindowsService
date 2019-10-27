using ClientManagerService.Model;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace ClientManagerService.Requests
{
    /// <summary>
    /// Parse xml .hegi file into <see cref="HegiDescriptor"/> request.
    /// </summary>
    public class ParseXmlHegiFileRequest : IRequest<HegiDescriptor>
    {
        /// <summary>
        /// .Hegi file name.
        /// </summary>
        [Required]
        public string HegiFileName { get; set; }
    }
}
