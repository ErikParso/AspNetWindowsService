using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ApplicationServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VersionController : ControllerBase
    {
        private static string version = "1.0.1";

        [HttpGet]
        public ActionResult<IEnumerable<string>> GetLatestVersion()
        {
            return Ok(version);
        }

        [HttpGet("{version}")]
        public ActionResult<string> GetInstaller(string version)
        {
            return "MSI";
        }
    }
}
