using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;

namespace AspWinService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VersionController : ControllerBase
    {
        private const string version = "1.0.0";

        // GET api/values
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var latest = await GetLatestVersion();
            return Ok(new { Local = version, Latest = latest });
        }

        private async Task<string> GetLatestVersion()
        {
            var httpClient = new HttpClient();
            return await httpClient.GetStringAsync("http://localhost:5100/api/version");
        }
    }
}
