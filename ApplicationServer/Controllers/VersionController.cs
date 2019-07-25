using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace ApplicationServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VersionController : ControllerBase
    {
        private static string version = "6.0.9";

        [HttpGet]
        public ActionResult<IEnumerable<string>> GetLatestVersion()
        {
            return Ok(version);
        }

        [HttpGet("{version}")]
        public ActionResult<string> GetInstaller(string version)
        {
            byte[] fileBytes = System.IO.File.ReadAllBytes("Installer/MyProduct.msi");
            return File(fileBytes, "application/force-download", "MyProduct.msi");
        }
    }
}
