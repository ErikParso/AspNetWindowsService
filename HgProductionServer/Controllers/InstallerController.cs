using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ProductionServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InstallerController : ControllerBase
    {
        /// <summary>
        /// Gets current version of Client Manager.
        /// </summary>
        /// <returns>
        /// Current version of Client Manager.
        /// </returns>
        [HttpGet]
        public IActionResult GetClientManagerVersion()
        {
            string version = JsonConvert.DeserializeObject<dynamic>(System.IO.File.ReadAllText(@"Installer\VersionInfo.json")).version;
            return Ok(version);
        }

        /// <summary>
        /// Downloads the current Client Manager installer.
        /// </summary>
        /// <returns>.msi package file.</returns>
        [HttpGet("download")]
        public ActionResult<string> GetInstaller()
        {
            byte[] fileBytes = System.IO.File.ReadAllBytes("Installer/Helios Green Client Manager.msi");
            return File(fileBytes, "application/force-download", "Helios Green Client Manager.msi");
        }

    }
}
