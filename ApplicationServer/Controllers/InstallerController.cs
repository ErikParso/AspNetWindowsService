using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ApplicationServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InstallerController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetInstallerVersion()
        {
            string version = JsonConvert.DeserializeObject<dynamic>(System.IO.File.ReadAllText(@"Installer\VersionInfo.json")).version;
            return Ok(version);
        }

        [HttpGet("download")]
        public ActionResult<string> GetInstaller()
        {
            byte[] fileBytes = System.IO.File.ReadAllBytes("Installer/MyProduct.msi");
            return File(fileBytes, "application/force-download", "MyProduct.msi");
        }

    }
}
