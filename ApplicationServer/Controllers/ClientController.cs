using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO.Compression;

namespace ApplicationServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        [HttpGet]
        public ActionResult<IEnumerable<string>> GetClientVersion()
        {
            return Ok(JsonConvert.DeserializeObject<dynamic>(System.IO.File.ReadAllText("Client/VersionInfo.json")).version);
        }

        [HttpGet("download")]
        public ActionResult<string> GetClient(string version)
        {
            //TODO: File by file zip method
            System.IO.File.Delete("HeliosGreen.zip");
            ZipFile.CreateFromDirectory("Client", "HeliosGreen.zip");
            byte[] fileBytes = System.IO.File.ReadAllBytes(@".\HeliosGreen.zip");
            return File(fileBytes, "application/force-download", "HeliosGreen.zip");
        }
    }
}
